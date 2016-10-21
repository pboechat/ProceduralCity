using UnityEngine;
using System;
using System.Collections.Generic;
using ModelRepository;
using Pattern;

public class CombinedBuildingsMeshGenerator : BuildingsMeshGenerator
{
    enum GroundFloorOpts
    {
        NONE,
        ITEMS,
        FLOOR,
        ITEMS_AND_FLOOR
    }

	private static readonly ICollection<Direction> ALL_FACADE_DIRECTIONS = new Direction[] { Direction.BACK, Direction.LEFT, Direction.RIGHT, Direction.FRONT };
	[SerializeField]
	private Color _colorMask;
    [SerializeField]
    private GroundFloorOpts _groundFloorsOpts = GroundFloorOpts.ITEMS_AND_FLOOR;
    [SerializeField]
    private bool _performFacadeOperations = true;
    [SerializeField]
    private bool _addFacadeItems = true;
    [SerializeField]
    private bool _addHeaders = true;
    [SerializeField]
    private bool _addRailings = true;
    [SerializeField]
    private bool _addRooftopItems = true;
	
	public Matrix4x4 GetBuildingSideTransform (Building building, Direction direction)
	{
		switch (direction) {
		case Direction.BACK:
			return Matrix4x4.TRS (new Vector3 (building.width, 0.0f, building.depth), Quaternion.Euler (0.0f, -180.0f, 0.0f), Vector3.one);
		case Direction.LEFT:
			return Matrix4x4.TRS (new Vector3 (0.0f, 0.0f, building.depth), Quaternion.Euler (0.0f, 90.0f, 0.0f), Vector3.one);
		case Direction.FRONT:
			return Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3.one);
		case Direction.RIGHT:
			return Matrix4x4.TRS (new Vector3 (building.width, 0.0f, 0.0f), Quaternion.Euler (0.0f, -90.0f, 0.0f), Vector3.one);
		default:
			throw new Exception ("invalid building side direction");
		}
	}
	
	Color[] ApplyHue (Color[] colorBuffer, Color hue)
	{
		Color[] color = new Color[colorBuffer.Length];
		Array.Copy (colorBuffer, color, colorBuffer.Length);
		for (int i = 0; i < color.Length; i++) {
			if (color [i].r != _colorMask.r || color [i].g != _colorMask.g || color [i].b != _colorMask.b || color [i].a != _colorMask.a) {
				color [i] = hue;
			} else {
				color [i] = Color.white;
			}
		}
		
		return color;
	}
	
	void InstantiateRandomDoor (Facade facade, Vector3 localPosition, Dictionary<IModel, List<Matrix4x4>> modelTransforms)
	{
		IModel model = Repository.List ("facade_item").Filter ("style", facade.architectureStyle.name). Filter ("symbol", "d").Random ();
		
		if (model == null) {
			throw new Exception ("no door moder for architecture style (style: " + facade.architectureStyle.name + ")");
		}
		
		Matrix4x4 localTransform = Matrix4x4.TRS (localPosition, Quaternion.identity, Vector3.one);
		
		List<Matrix4x4> transforms;
		if (!modelTransforms.TryGetValue (model, out transforms)) {
			transforms = new List<Matrix4x4> ();
			modelTransforms.Add (model, transforms);
		}
		transforms.Add (localTransform);
	}
	
	void InstantiateFacadeItem (Facade facade, FacadeItem facadeItem, Vector3 localPosition, Dictionary<IModel, List<Matrix4x4>> modelTransforms)
	{
		if (facadeItem.invalid) {
			return;
		}
		
		Matrix4x4 localTransform = Matrix4x4.TRS (localPosition, Quaternion.identity, Vector3.one);
		
		List<Matrix4x4> transforms;
		if (!modelTransforms.TryGetValue (facadeItem.model, out transforms)) {
			transforms = new List<Matrix4x4> ();
			modelTransforms.Add (facadeItem.model, transforms);
		}
		transforms.Add (localTransform);
	}

	void AddFacadeItems (Facade facade, Color hue, float facadeYOffset, Matrix4x4 worldTransform, List<CombineInstance> combineInstances)
	{
		ArchitectureStyle architectureStyle = facade.architectureStyle;
		
		float tileWidth = architectureStyle.tileWidth;
		float tileHeight = architectureStyle.tileHeight;

		int doorPosition;
		switch (architectureStyle.doorPosition) {
		case DoorPosition.RANDOM_POSITION:
			doorPosition = UnityEngine.Random.Range (0, facade.widthInTiles);
			break;
		case DoorPosition.CENTERED:
			doorPosition = facade.widthInTiles / 2;
			break;
		case DoorPosition.NONE:
			doorPosition = -1;
			break;
		default:
			// FIXME: checking invariants
			throw new Exception ("unknown door position");
		}
		
		Dictionary<IModel, List<Matrix4x4>> localTransforms = new Dictionary<IModel, List<Matrix4x4>> ();
		for (int y = 0; y < facade.heightInTiles; y++) {
			for (int x = 0; x < facade.widthInTiles; x++) {
				float worldZ = 0;
				if (architectureStyle.usesOperations) {
					FacadeOperation operation = facade.operationsPattern.GetElement (x, y);

					if (operation.extrude) {
						worldZ = -operation.level * facade.architectureStyle.extrusionDepth;
					} else if (operation.caveIn) {
						worldZ = operation.level * facade.architectureStyle.extrusionDepth;
					}
				}
				
				float worldX = (x * tileWidth) + tileWidth * 0.5f;
				float worldY = (facade.heightInTiles - y) * tileHeight - tileHeight * 0.5f;
			
				if (x == doorPosition && y == facade.heightInTiles - 1) {
					InstantiateRandomDoor (facade, new Vector3 (worldX, 0, worldZ), localTransforms);
				} else {
					InstantiateFacadeItem (facade, facade.elementsPattern.GetElement (x, y), new Vector3 (worldX, worldY + facadeYOffset, worldZ), localTransforms);
				}
			
				if (y < facade.heightInTiles - 1 || x != doorPosition) {
					InstantiateFacadeItem (facade, facade.detailsPattern.GetElement (x, y), new Vector3 (worldX, worldY + facadeYOffset, worldZ), localTransforms);
				}
			}
		}
		
		foreach (KeyValuePair<IModel, List<Matrix4x4>> entry in localTransforms) {
			IModel model = entry.Key;
			
			Mesh mesh;
			Matrix4x4 modelTransform;
			CloneModelMesh (model, hue, out mesh, out modelTransform);
			
			List<Matrix4x4> transforms = entry.Value;
			for (int i = 0; i < transforms.Count; i++) {
				transforms [i] = worldTransform * transforms [i] * modelTransform;
			}
			
			CreateCombineInstances (mesh, transforms, combineInstances);
		}
	}

	Mesh CreateMeshForSymbolGroup (Facade facade, Rect uvRect1, Rect uvRect2, Rect uvRect3, Rect uvRect4, SymbolicOperations.SymbolGroup<FacadeOperation> symbolGroup, Color hue, float facadeYOffset)
	{
		ArchitectureStyle architectureStyle = facade.architectureStyle;

		float tileWidth = architectureStyle.tileWidth;
		float tileHeight = architectureStyle.tileHeight;

		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<int> indices = new List<int> ();
		List<Vector2> uvs1 = new List<Vector2> ();
		List<Vector2> uvs2 = new List<Vector2> ();
		List<Color> colors = new List<Color> ();
		
		for (int i = 0; i < symbolGroup.positions.Count; i++) {
			Vector2 position = symbolGroup.positions [i];
	
			float z = 0, z1 = 0, z2 = 0;
			if (symbolGroup.symbol.extrude) {
				z = z1 = -symbolGroup.symbol.level * architectureStyle.extrusionDepth;
			} else if (symbolGroup.symbol.caveIn) {
				z = z1 = symbolGroup.symbol.level * architectureStyle.extrusionDepth;
			}
	
			float x1 = position.x * tileWidth;
			float x2 = (position.x + 1) * tileWidth;
			float y1 = (facade.heightInTiles - position.y) * tileHeight + facadeYOffset;
			float y2 = (facade.heightInTiles - position.y - 1) * tileHeight + facadeYOffset;
	
			MeshUtils.CreateFrontFace (x1, x2, y1, y2, z, 1, 1, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
	
			if (symbolGroup.symbol.none) {
				continue;
			}
	
			if (!symbolGroup.HasNeighbourAtLeft (position)) {
				MeshUtils.CreateLeftFace (y1, y2, z1, z2, x1, 1, 1, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
			} 
	
			if (!symbolGroup.HasNeighbourAtRight (position)) {
				MeshUtils.CreateRightFace (y1, y2, z1, z2, x2, 1, 1, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
			} 
	
			if (symbolGroup.symbol.extrude && !symbolGroup.HasNeighbourAbove (position)) {
				if (position.y == 0) {
					MeshUtils.CreateTopFace (x1, x2, z1, z2, y1, 1, 1, uvRect3, uvRect4, vertices, normals, uvs1, uvs2, indices, colors, Color.white);
				} else {
					MeshUtils.CreateTopFace (x1, x2, z1, z2, y1, 1, 1, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
				}
			}
			
			if (symbolGroup.symbol.caveIn && !symbolGroup.HasNeighbourBelow (position)) {
				MeshUtils.CreateTopFace (x1, x2, z2, z1, y2, 1, 1, uvRect1, uvRect2/*uvRect3, uvRect4*/, vertices, normals, uvs1, uvs2, indices, colors, hue/*Color.white*/);
			}
		}

		Mesh mesh = new Mesh ();

		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.triangles = indices.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.uv2 = uvs2.ToArray ();
		mesh.colors = colors.ToArray ();
		MeshUtils.CalculateTangents (mesh);
		mesh.RecalculateBounds ();

		return mesh;
	}
	
	void CloneModelMesh (IModel model, Color hue, out Mesh mesh, out Matrix4x4 modelTransform)
	{
		GameObject clone = model.Clone ();
		MeshFilter meshFilter = clone.GetComponent<MeshFilter> ();
		if (meshFilter == null) {
			throw new Exception ("model doesn't have single mesh: " + model.Uid ());
		}
		mesh = meshFilter.mesh;
		mesh.colors = ApplyHue (mesh.colors, hue);
		modelTransform = Matrix4x4.TRS (clone.transform.position, clone.transform.rotation, clone.transform.localScale);
		DestroyImmediate (clone);
	}
	
	void CloneModelMesh (IModel model, out Mesh mesh, out Matrix4x4 modelTransform)
	{
		GameObject clone = model.Clone ();
		MeshFilter meshFilter = clone.GetComponent<MeshFilter> ();
		if (meshFilter == null) {
			throw new Exception ("model doesn't have single mesh: " + model.Uid ());
		}
		mesh = meshFilter.mesh;
		modelTransform = Matrix4x4.TRS (clone.transform.position, clone.transform.rotation, clone.transform.localScale);
		DestroyImmediate (clone);
	}
	
	void CreateCombineInstances (Mesh mesh, List<Matrix4x4> transforms, List<CombineInstance> combineInstances)
	{
		CombineInstance[] newCombineInstances = new CombineInstance[transforms.Count];
		for (int i = 0; i < transforms.Count; i++) {
			newCombineInstances [i].mesh = mesh;
			newCombineInstances [i].transform = transforms [i];
		}
		
		combineInstances.AddRange (newCombineInstances);
	}
	
	void AddGroundFloorItems (Building building, IModel groundFloorItem, Color hue, List<CombineInstance> combineInstances)
	{
		Mesh groundFloorItemMesh;
		Matrix4x4 groundFloorItemModelTransform;
		CloneModelMesh (groundFloorItem, hue, out groundFloorItemMesh, out groundFloorItemModelTransform);
		
		ArchitectureStyle architectureStyle = building.architectureStyle;
		int tileWidth = architectureStyle.tileWidth;
		
		string corner = groundFloorItem.Metadata ("corner");
		IModel groundFloorItemCorner = null;
		Mesh groundFloorItemCornerMesh = null;
		Matrix4x4 groundFloorItemCornerModelTransform = new Matrix4x4 ();
		if (corner != "none") {
			groundFloorItemCorner = Repository.Get (corner);
			if (groundFloorItemCorner == null) {
				throw new Exception ("invalid ground floor item corner");
			}
			CloneModelMesh (groundFloorItemCorner, hue, out groundFloorItemCornerMesh, out groundFloorItemCornerModelTransform);
		}
		
		ICollection<Direction> directions;
		if (building.architectureStyle.spacing > 0) {
			directions = ALL_FACADE_DIRECTIONS;
		} else {
			directions = building.facadeDirections;
		}
		
		List<Matrix4x4> groundFloorItemsTransforms = new List<Matrix4x4> ();
		List<Matrix4x4> groundFloorItemCornersTransforms = new List<Matrix4x4> ();
		foreach (Direction direction in directions) {
			Matrix4x4 worldTransform = GetBuildingSideTransform (building, direction);
			
			int widthInTiles;
			if (direction == Direction.BACK || direction == Direction.FRONT) {
				widthInTiles = building.widthInTiles;
			} else {
				widthInTiles = building.depthInTiles;
			}
			
			Facade facade = null;
			if (architectureStyle.usesOperations && building.HasFacade (direction)) {
				facade = building.GetFacade (direction);
			}
			
			float halfTileWidth = tileWidth * 0.5f;
			for (int tileX = 0; tileX < widthInTiles; tileX++) {
				float x = (tileX * tileWidth) + halfTileWidth;
				float z = 0.0f;
				
				Matrix4x4 localTransform;
				
				if (facade != null) {
					int tileY = facade.heightInTiles - 1;
					
					FacadeOperation operation = facade.operationsPattern.GetElement (tileX, tileY);
					
					if (!operation.none) {
						if (operation.extrude) {
							z = -operation.level * architectureStyle.extrusionDepth;
						} else if (operation.caveIn) {
							z = operation.level * architectureStyle.extrusionDepth;
						} else {
							// FIXME: checking invariants
							throw new Exception ("invalid operation");
						}
						
						FacadeOperation leftSideOperation = null;
						if (tileX > 0) {
							leftSideOperation = facade.operationsPattern.GetElement (tileX - 1, tileY);
						}
						
						bool hasLeftRepetition = (leftSideOperation == null || leftSideOperation != operation);
						
						FacadeOperation rightSideOperation = null;
						if (tileX < widthInTiles - 1) {
							rightSideOperation = facade.operationsPattern.GetElement (tileX + 1, tileY);
						}
						
						bool hasRightRepetition = (rightSideOperation == null || rightSideOperation != operation);
						
						int repetitions = Mathf.FloorToInt (Mathf.Abs (z / (float)tileWidth));
						
						float x1 = tileX * tileWidth;
						float x2 = (tileX + 1) * tileWidth;
						Quaternion leftRepetitionRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
						Quaternion rightRepetitionRotation = Quaternion.Euler (0.0f, -90.0f, 0.0f);
						
						for (int i = 0; i < repetitions; i++) {
							float z1;
							if (operation.extrude) {
								z1 = z + (tileWidth * i) + halfTileWidth;
							} else if (operation.caveIn) {
								z1 = z - ((tileWidth * i) + halfTileWidth);
							} else {
								// FIXME: checking invariants
								throw new Exception ("invalid operation");
							}
							
							if (hasLeftRepetition) {
								localTransform = Matrix4x4.TRS (new Vector3 (x1, 0.0f, z1), leftRepetitionRotation, Vector3.one);
								groundFloorItemsTransforms.Add (worldTransform * localTransform * groundFloorItemModelTransform);
							}
							
							if (hasRightRepetition) {
								localTransform = Matrix4x4.TRS (new Vector3 (x2, 0.0f, z1), rightRepetitionRotation, Vector3.one);
								groundFloorItemsTransforms.Add (worldTransform * localTransform * groundFloorItemModelTransform);
							}
						}
					}
				}
				
				localTransform = Matrix4x4.TRS (new Vector3 (x, 0.0f, z), Quaternion.identity, Vector3.one);
				groundFloorItemsTransforms.Add (worldTransform * localTransform * groundFloorItemModelTransform);
				
				if (tileX == 0 && groundFloorItemCorner != null) {
					groundFloorItemCornersTransforms.Add (worldTransform * localTransform * groundFloorItemCornerModelTransform);
				}
			}
		}
		
		CreateCombineInstances (groundFloorItemMesh, groundFloorItemsTransforms, combineInstances);
		CreateCombineInstances (groundFloorItemCornerMesh, groundFloorItemCornersTransforms, combineInstances);
	}
	
	void CreateHeader (Building building, IModel headerItem, Color hue, float facadeYOffset, List<CombineInstance> combineInstances)
	{
		Mesh headerItemMesh;
		Matrix4x4 headerItemModelTransform;
		CloneModelMesh (headerItem, hue, out headerItemMesh, out headerItemModelTransform);
		
		ArchitectureStyle architectureStyle = building.architectureStyle;
		int tileWidth = architectureStyle.tileWidth;
		int tileHeight = architectureStyle.tileHeight;
		Header header = architectureStyle.header;
		
		float y = (building.heightInTiles - 0.5f) * tileHeight + facadeYOffset;
		float terraceExtension = architectureStyle.terraceExtension;
		
		string corner = headerItem.Metadata ("corner");
		IModel headerItemCorner = null;
		Mesh headerItemCornerMesh = null;
		Matrix4x4 headerItemCornerModelTransform = new Matrix4x4 ();
		if (corner != "none") {
			headerItemCorner = Repository.Get (corner);
			if (headerItemCorner == null) {
				throw new Exception ("invalid header item corner");
			}
			CloneModelMesh (headerItemCorner, hue, out headerItemCornerMesh, out headerItemCornerModelTransform);
		}
		
		ICollection<Direction> directions;
		if (header == Header.OVER_FACADES) {
			directions = building.facadeDirections;
		} else if (header == Header.ALL_AROUND) {
			directions = ALL_FACADE_DIRECTIONS;
		} else {
			// FIXME: checking invariants
			throw new Exception ("invalid header: " + header);
		}
		
		List<Matrix4x4> headerItemsTransforms = new List<Matrix4x4> ();
		List<Matrix4x4> headerItemCornersTransforms = new List<Matrix4x4> ();
		foreach (Direction direction in directions) {
			Matrix4x4 worldTransform = GetBuildingSideTransform (building, direction);
			
			int widthInTiles;
			if (direction == Direction.BACK || direction == Direction.FRONT) {
				widthInTiles = building.widthInTiles;
			} else {
				widthInTiles = building.depthInTiles;
			}
			
			Facade facade = null;
			if (building.HasFacade (direction)) {
				facade = building.GetFacade (direction);
			}
			
			float halfTileWidth = tileWidth * 0.5f;
			for (int tileX = 0; tileX < widthInTiles; tileX++) {
				float x = (tileX * tileWidth) + halfTileWidth;
				float z = -terraceExtension;
				
				Matrix4x4 localTransform;
				
				bool valid = true;
				if (facade != null) {
					if (architectureStyle.usesOperations) {
						FacadeOperation operation = facade.operationsPattern.GetElement (tileX, 0);
						
						if (!operation.none) {
							if (operation.extrude) {
								z = -(operation.level * architectureStyle.extrusionDepth + terraceExtension);
							} else if (operation.caveIn) {
								z = operation.level * architectureStyle.extrusionDepth - terraceExtension;
							} else {
								// FIXME: checking invariants
								throw new Exception ("invalid operation");
							}
							
							FacadeOperation leftSideOperation = null;
							if (tileX > 0) {
								leftSideOperation = facade.operationsPattern.GetElement (tileX - 1, 0);
							}
							
							bool hasLeftRepetition = (leftSideOperation == null || leftSideOperation != operation);
							
							FacadeOperation rightSideOperation = null;
							if (tileX < widthInTiles - 1) {
								rightSideOperation = facade.operationsPattern.GetElement (tileX + 1, 0);
							}
							
							bool hasRightRepetition = (rightSideOperation == null || rightSideOperation != operation);
							
							int repetitions = Mathf.FloorToInt (Mathf.Abs (z / (float)tileWidth));
							
							float x1 = tileX * tileWidth;
							float x2 = (tileX + 1) * tileWidth;
							Quaternion leftRepetitionRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
							Quaternion rightRepetitionRotation = Quaternion.Euler (0.0f, -90.0f, 0.0f);
							
							for (int i = 0; i < repetitions; i++) {
								float z1;
								if (operation.extrude) {
									z1 = z + (tileWidth * i) + halfTileWidth;
								} else if (operation.caveIn) {
									z1 = z - ((tileWidth * i) + halfTileWidth);
								} else {
									// FIXME: checking invariants
									throw new Exception ("invalid operation");
								}
								
								if (hasLeftRepetition) {
									localTransform = Matrix4x4.TRS (new Vector3 (x1, y, z1), leftRepetitionRotation, Vector3.one);
									headerItemsTransforms.Add (worldTransform * localTransform * headerItemModelTransform);
								}
								
								if (hasRightRepetition) {
									localTransform = Matrix4x4.TRS (new Vector3 (x2, y, z1), rightRepetitionRotation, Vector3.one);
									headerItemsTransforms.Add (worldTransform * localTransform * headerItemModelTransform);
								}
							}
						}
					}
					
					valid = facade.elementsPattern.GetElement (tileX, 0).allowsHeader;
				}
				
				if (valid) {
					localTransform = Matrix4x4.TRS (new Vector3 (x, y, z), Quaternion.identity, Vector3.one);
					headerItemsTransforms.Add (worldTransform * localTransform * headerItemModelTransform);
					
					if (tileX == 0 && 
						headerItemCorner != null && 
						(header == Header.ALL_AROUND || (header == Header.OVER_FACADES && DirectionHelper.HasLeft (direction, building.facadeDirections)))) {
						headerItemCornersTransforms.Add (worldTransform * localTransform * headerItemCornerModelTransform);	
					}
				}
				
			}
		}
		
		CreateCombineInstances (headerItemMesh, headerItemsTransforms, combineInstances);
		CreateCombineInstances (headerItemCornerMesh, headerItemCornersTransforms, combineInstances);
	}
	
	void CreateRailing (Building building, IModel railingItem, Color hue, float facadeYOffset, List<CombineInstance> combineInstances)
	{
		Mesh railingItemMesh;
		Matrix4x4 railingItemModelTransform;
		CloneModelMesh (railingItem, hue, out railingItemMesh, out railingItemModelTransform);
		
		ArchitectureStyle architectureStyle = building.architectureStyle;
		int tileWidth = architectureStyle.tileWidth;
		int tileHeight = architectureStyle.tileHeight;
		Header header = architectureStyle.header;
		
		float y = (building.heightInTiles - 0.5f) * tileHeight + facadeYOffset;
		float terraceExtension = architectureStyle.terraceExtension;
		
		ICollection<Direction> directions;
		if (header == Header.OVER_FACADES) {
			directions = CollectionUtils.Disjoint (ALL_FACADE_DIRECTIONS, building.facadeDirections);
		} else if (header == Header.NONE) {
			directions = ALL_FACADE_DIRECTIONS;
		} else {
			// FIXME: checking invariants
			throw new Exception ("invalid header: " + header);
		}
		
		List<Matrix4x4> railingItemsTransforms = new List<Matrix4x4> ();
		foreach (Direction direction in directions) {
			Matrix4x4 worldTransform = GetBuildingSideTransform (building, direction);
			
			int widthInTiles;
			if (direction == Direction.BACK || direction == Direction.FRONT) {
				widthInTiles = building.widthInTiles;
			} else {
				widthInTiles = building.depthInTiles;
			}
			
			Facade facade = null;
			if (building.HasFacade (direction)) {
				facade = building.GetFacade (direction);
			}
			
			float halfTileWidth = tileWidth * 0.5f;
			for (int tileX = 0; tileX < widthInTiles; tileX++) {
				float x = (tileX * tileWidth) + halfTileWidth;
				float z = -terraceExtension;
				
				Matrix4x4 localTransform;
				
				bool valid = true;
				if (facade != null) {
					if (architectureStyle.usesOperations) {
						FacadeOperation operation = facade.operationsPattern.GetElement (tileX, 0);
						
						if (!operation.none) {
							if (operation.extrude) {
								z = -(operation.level * architectureStyle.extrusionDepth + terraceExtension);
							} else if (operation.caveIn) {
								z = operation.level * architectureStyle.extrusionDepth - terraceExtension;
							} else {
								// FIXME: checking invariants
								throw new Exception ("invalid operation");
							}
							
							FacadeOperation leftSideOperation = null;
							if (tileX > 0) {
								leftSideOperation = facade.operationsPattern.GetElement (tileX - 1, 0);
							}
							
							bool hasLeftRepetition = (leftSideOperation == null || leftSideOperation != operation);
							
							FacadeOperation rightSideOperation = null;
							if (tileX < widthInTiles - 1) {
								rightSideOperation = facade.operationsPattern.GetElement (tileX + 1, 0);
							}
							
							bool hasRightRepetition = (rightSideOperation == null || rightSideOperation != operation);
							
							int repetitions = Mathf.FloorToInt (Mathf.Abs (z / (float)tileWidth));
							
							float x1 = tileX * tileWidth;
							float x2 = (tileX + 1) * tileWidth;
							Quaternion leftRepetitionRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
							Quaternion rightRepetitionRotation = Quaternion.Euler (0.0f, -90.0f, 0.0f);
							
							for (int i = 0; i < repetitions; i++) {
								float z1;
								if (operation.extrude) {
									z1 = z + (tileWidth * i) + halfTileWidth;
								} else if (operation.caveIn) {
									z1 = z - ((tileWidth * i) + halfTileWidth);
								} else {
									// FIXME: checking invariants
									throw new Exception ("invalid operation");
								}
								
								if (hasLeftRepetition) {
									localTransform = Matrix4x4.TRS (new Vector3 (x1, y, z1), leftRepetitionRotation, Vector3.one);
									railingItemsTransforms.Add (worldTransform * localTransform * railingItemModelTransform);
								}
								
								if (hasRightRepetition) {
									localTransform = Matrix4x4.TRS (new Vector3 (x2, y, z1), rightRepetitionRotation, Vector3.one);
									railingItemsTransforms.Add (worldTransform * localTransform * railingItemModelTransform);
								}
							}
						}
					}
					
					valid = facade.elementsPattern.GetElement (tileX, 0).allowsHeader;
				}
				
				if (valid) {
					localTransform = Matrix4x4.TRS (new Vector3 (x, y, z), Quaternion.identity, Vector3.one);
					railingItemsTransforms.Add (worldTransform * localTransform * railingItemModelTransform);
				}
				
			}
		}
		
		CreateCombineInstances (railingItemMesh, railingItemsTransforms, combineInstances);
	}
	
	void CreateFacades (Building building, Rect wallTextureRect, Rect wallBumpTextureRect, Rect terraceTextureRect, Rect terraceBumpTextureRect, Color hue, float facadeYOffset, List<CombineInstance> combineInstances)
	{
		ArchitectureStyle architectureStyle = building.architectureStyle;
		float textureAtlasSize = (float)architectureStyle.textureAtlas.size;
		
		Rect uvRect1 = new Rect (wallTextureRect.x / textureAtlasSize, wallTextureRect.y / textureAtlasSize, wallTextureRect.width / textureAtlasSize, wallTextureRect.height / textureAtlasSize);
		Rect uvRect2 = new Rect (wallBumpTextureRect.x / textureAtlasSize, wallBumpTextureRect.y / textureAtlasSize, wallBumpTextureRect.width / textureAtlasSize, wallBumpTextureRect.height / textureAtlasSize);
		Rect uvRect3 = new Rect (terraceTextureRect.x / textureAtlasSize, terraceTextureRect.y / textureAtlasSize, terraceTextureRect.width / textureAtlasSize, terraceTextureRect.height / textureAtlasSize);
		Rect uvRect4 = new Rect (terraceBumpTextureRect.x / textureAtlasSize, terraceBumpTextureRect.y / textureAtlasSize, terraceBumpTextureRect.width / textureAtlasSize, terraceBumpTextureRect.height / textureAtlasSize);
		
		foreach (Facade facade in building.facades) {
			Matrix4x4 worldTransform = GetBuildingSideTransform (building, facade.direction);
			
			if (_performFacadeOperations && architectureStyle.usesOperations) {
				List<SymbolicOperations.SymbolGroup<FacadeOperation>> operationGroups = SymbolicOperations.ExtractGroups (facade.operationsPattern.ToMatrix (), facade.widthInTiles, facade.heightInTiles);
				for (int i = 0; i < operationGroups.Count; i++) {
					Mesh mesh = CreateMeshForSymbolGroup (facade, uvRect1, uvRect2, uvRect3, uvRect4, operationGroups [i], hue, facadeYOffset);
					
					CombineInstance combineInstance = new CombineInstance ();
					combineInstance.mesh = mesh;
					combineInstance.transform = worldTransform;
					combineInstances.Add (combineInstance);
				}
			} else {
				Mesh mesh = MeshUtils.CreateFrontFaceMesh (0, facade.width, facade.height + facadeYOffset, facadeYOffset, 0, facade.widthInTiles, facade.heightInTiles, uvRect1, uvRect2, hue);
				MeshUtils.CalculateTangents (mesh);
				
				CombineInstance combineInstance = new CombineInstance ();
				combineInstance.mesh = mesh;
				combineInstance.transform = worldTransform;
				combineInstances.Add (combineInstance);
			}
			
            if (_addFacadeItems)
			    AddFacadeItems (facade, hue, facadeYOffset, worldTransform, combineInstances);
		}
	}
	
	void AddFloor (Building building, Rect floorTextureRect, Rect floorBumpTextureRect, List<CombineInstance> combineInstances)
	{
		ArchitectureStyle architectureStyle = building.architectureStyle;
		float textureAtlasSize = (float)architectureStyle.textureAtlas.size;
		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<Vector2> uvs1 = new List<Vector2> ();
		List<Vector2> uvs2 = new List<Vector2> ();
		List<int> indices = new List<int> ();
		List<Color> colors = new List<Color> ();

		Rect uvRect1 = new Rect (floorTextureRect.x / textureAtlasSize, floorTextureRect.y / textureAtlasSize, floorTextureRect.width / textureAtlasSize, floorTextureRect.height / textureAtlasSize);
		Rect uvRect2 = new Rect (floorBumpTextureRect.x / textureAtlasSize, floorBumpTextureRect.y / textureAtlasSize, floorBumpTextureRect.width / textureAtlasSize, floorBumpTextureRect.height / textureAtlasSize);
		MeshUtils.CreateTopFace (0, building.width, 0, building.depth, 0, building.widthInTiles, building.depthInTiles, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, Color.white);
		
		Mesh mesh = new Mesh ();
		
		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.triangles = indices.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.uv2 = uvs2.ToArray ();
		MeshUtils.CalculateTangents (mesh);
		mesh.RecalculateBounds ();
		
		CombineInstance combineInstance = new CombineInstance ();
		combineInstance.mesh = mesh;
		combineInstance.transform = Matrix4x4.identity;
		
		combineInstances.Add (combineInstance);
	}
	
	void CreateSideWallsAndTerrace (Building building, Rect wallTextureRect, Rect wallBumpTextureRect, Rect terraceTextureRect, Rect terraceBumpTextureRect, Color hue, float facadeYOffset, List<CombineInstance> combineInstances)
	{
		ArchitectureStyle architectureStyle = building.architectureStyle;
		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<Vector2> uvs1 = new List<Vector2> ();
		List<Vector2> uvs2 = new List<Vector2> ();
		List<int> indices = new List<int> ();
		List<Color> colors = new List<Color> ();
		
		float x1 = 0;
		float x2 = building.width;
		float y1 = building.height + facadeYOffset;
		float y2 = (architectureStyle.spacing <= 0) ? 0 : facadeYOffset;
		float z1 = 0;
		float z2 = building.depth;
		
		bool hasFrontFace = true, hasBackFace = true, hasRightFace = true, hasLeftFace = true;
		foreach (Facade facade in building.facades) {
			switch (facade.direction) {
			case Direction.FRONT:
				hasFrontFace = false;
				break;
			case Direction.BACK:
				hasBackFace = false;
				break;
			case Direction.RIGHT:
				hasRightFace = false;
				break;
			case Direction.LEFT:
				hasLeftFace = false;
				break;
			}
		}
		
		float textureAtlasSize = (float)architectureStyle.textureAtlas.size;
		Rect uvRect1 = new Rect (wallTextureRect.x / textureAtlasSize, wallTextureRect.y / textureAtlasSize, wallTextureRect.width / textureAtlasSize, wallTextureRect.height / textureAtlasSize);
		Rect uvRect2 = new Rect (wallBumpTextureRect.x / textureAtlasSize, wallBumpTextureRect.y / textureAtlasSize, wallBumpTextureRect.width / textureAtlasSize, wallBumpTextureRect.height / textureAtlasSize);
		
		if (hasLeftFace) {
			MeshUtils.CreateLeftFace (y1, y2, z1, z2, x1, building.depthInTiles, building.heightInTiles, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
		}
	
		if (hasRightFace) {
			MeshUtils.CreateRightFace (y1, y2, z1, z2, x2, building.depthInTiles, building.heightInTiles, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
		}
		
		if (hasBackFace) {
			MeshUtils.CreateBackFace (x1, x2, y1, y2, z2, building.widthInTiles, building.heightInTiles, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
		}
	
		if (hasFrontFace) {
			MeshUtils.CreateFrontFace (x1, x2, y1, y2, z1, building.widthInTiles, building.heightInTiles, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, hue);
		}
		
		uvRect1 = new Rect (terraceTextureRect.x / textureAtlasSize, terraceTextureRect.y / textureAtlasSize, terraceTextureRect.width / textureAtlasSize, terraceTextureRect.height / textureAtlasSize);
		uvRect2 = new Rect (terraceBumpTextureRect.x / textureAtlasSize, terraceBumpTextureRect.y / textureAtlasSize, terraceBumpTextureRect.width / textureAtlasSize, terraceBumpTextureRect.height / textureAtlasSize);
		float terraceExtension = architectureStyle.terraceExtension;
		x1 = ((hasLeftFace) ? 0.0f : -terraceExtension);
		x2 = ((hasRightFace) ? 0.0f : terraceExtension) + building.width;
		z1 = ((hasFrontFace) ? 0.0f : -terraceExtension);
		z2 = ((hasBackFace) ? 0.0f : terraceExtension) + building.depth;
		MeshUtils.CreateTopFace (x1, x2, z1, z2, building.height + facadeYOffset, building.widthInTiles, building.depthInTiles, uvRect1, uvRect2, vertices, normals, uvs1, uvs2, indices, colors, Color.white);
		
		Mesh mesh = new Mesh ();
		
		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.triangles = indices.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.uv2 = uvs2.ToArray ();
		MeshUtils.CalculateTangents (mesh);
		mesh.RecalculateBounds ();
		
		CombineInstance combineInstance = new CombineInstance ();
		combineInstance.mesh = mesh;
		combineInstance.transform = Matrix4x4.identity;
		
		combineInstances.Add (combineInstance);
	}

	public void AddRooftopItems (Building building, float facadeYOffset, List<CombineInstance> combineInstances)
	{
		ArchitectureStyle architectureStyle = building.architectureStyle;
		
		IResultSet resultSet = Repository.List ("rooftop_item").Filter ("style", architectureStyle.name);
		
		if (resultSet.Count () == 0) {
			return;
		}
		
		Header header = architectureStyle.header;
		
		int rooftopArea;
		if (header != Header.NONE) {
			rooftopArea = (building.widthInTiles - 2) * (building.depthInTiles - 2);
		} else {
			rooftopArea = (building.widthInTiles - 1) * (building.depthInTiles - 1);
		}
		
		if (rooftopArea <= 0) {
			return;
		}
		
		float rooftopFillRate = UnityEngine.Random.Range (building.architectureStyle.minRooftopFillRate, building.architectureStyle.maxRooftopFillRate);
		int numRooftopItems = Mathf.CeilToInt (rooftopFillRate * rooftopArea);
		
		List<Vector2> positions = new List<Vector2> ();
		if (header != Header.NONE) {
			positions.AddRange (Combinatorics.PositionsInArea (1, 1, building.widthInTiles - 2, building.depthInTiles - 2));
		} else {
			positions.AddRange (Combinatorics.PositionsInArea (0, 0, building.widthInTiles - 1, building.depthInTiles - 1));
		}
		
		int tileWidth = building.architectureStyle.tileWidth;
		float halfTileWidth = tileWidth / 2.0f;
		float worldY = building.height + facadeYOffset;
		Dictionary<IModel, List<Matrix4x4>> localTransforms = new Dictionary<IModel, List<Matrix4x4>> ();
		for (int i = 0; i < numRooftopItems; i++) {
			IModel model = resultSet.Random (true);
			
			Vector2 position = positions [UnityEngine.Random.Range (0, positions.Count)];
			positions.Remove (position);
			
			Matrix4x4 localTransform = Matrix4x4.TRS (new Vector3 (position.x * tileWidth + halfTileWidth, worldY, position.y * tileWidth + halfTileWidth), Quaternion.identity, Vector3.one);
		
			List<Matrix4x4> transforms;
			if (!localTransforms.TryGetValue (model, out transforms)) {
				transforms = new List<Matrix4x4> ();
				localTransforms.Add (model, transforms);
			}
			transforms.Add (localTransform);
			
			if (resultSet.Count () == 0) {
				resultSet = Repository.List ("rooftop_item").Filter ("style", architectureStyle.name);
			}
		}
		
		foreach (KeyValuePair<IModel, List<Matrix4x4>> entry in localTransforms) {
			IModel model = entry.Key;
			
			Mesh mesh;
			Matrix4x4 modelTransform;
			CloneModelMesh (model, out mesh, out modelTransform);
			
			List<Matrix4x4> transforms = entry.Value;
			for (int i = 0; i < transforms.Count; i++) {
				transforms [i] = transforms [i] * modelTransform;
			}
			
			CreateCombineInstances (mesh, transforms, combineInstances);
		}
	}
	
	public override void Execute (List<Building> buildings)
	{
		GameObject buildingsGameObject = new GameObject ("Buildings");
		
		buildingsGameObject.transform.position = Vector3.zero;
		buildingsGameObject.transform.rotation = Quaternion.identity;

        Color[] hues = new Color[buildings.Count];
        for (int i = 0; i < buildings.Count; i++)
            hues[i] = buildings[i].architectureStyle.randomHue;

        for (int i = 0; i < buildings.Count; i++) {
            Building building = buildings[i];

            GameObject buildingGameObject = new GameObject ("Building");
	
			buildingGameObject.transform.parent = buildingsGameObject.transform;
			buildingGameObject.transform.position = new Vector3 (building.x, 0, building.y);
	
			ArchitectureStyle architectureStyle = building.architectureStyle;
			
			Header header = architectureStyle.header;
			
			Color hue = hues[i];
			Rect wallTextureRect;
			Rect wallBumpTextureRect;
			Rect terraceTextureRect;
			Rect terraceBumpTextureRect;
			architectureStyle.GetRandomWallTextureRect (out wallTextureRect, out wallBumpTextureRect);
			architectureStyle.GetRandomTerraceTextureRect (out terraceTextureRect, out terraceBumpTextureRect);
			
			List<CombineInstance> buildingCombineInstances = new List<CombineInstance> ();
			
			// ==============================
			// Ground Floor
			// ==============================
			
			IResultSet resultSet;
			float facadeYOffset = 0;
            if (_groundFloorsOpts != GroundFloorOpts.NONE)
            {
                switch (architectureStyle.groundFloor)
                {
                    case GroundFloor.FIRST_FLOOR_BARE:
                        facadeYOffset = 0;
                        break;
                    case GroundFloor.FIRST_FLOOR_FOOTER:
                        resultSet = Repository.List("ground_floor").Filter("type", "footer").Filter("style", architectureStyle.name);

                        if (resultSet.Count() == 0)
                        {
                            throw new Exception("architecture style doesn't have any footer model");
                        }

                        IModel footerModel = resultSet.Random();
                        facadeYOffset = footerModel.MetadataAsFloat("height");

                        if (_groundFloorsOpts != GroundFloorOpts.FLOOR)
                            AddGroundFloorItems(building, footerModel, hue, buildingCombineInstances);

                        break;
                    case GroundFloor.STORE:
                        // TODO:
                        facadeYOffset = 0;
                        break;
                    case GroundFloor.PILOTIS:
                        resultSet = Repository.List("ground_floor").Filter("type", "pilotis").Filter("style", architectureStyle.name);

                        if (resultSet.Count() == 0)
                        {
                            throw new Exception("architecture style doesn't have any pilotis model");
                        }

                        IModel pilotisModel = resultSet.Random();
                        facadeYOffset = pilotisModel.MetadataAsInt("height");

                        if (_groundFloorsOpts != GroundFloorOpts.FLOOR)
                            AddGroundFloorItems(building, pilotisModel, hue, buildingCombineInstances);

                        if (_groundFloorsOpts != GroundFloorOpts.ITEMS)
                            AddFloor(building, terraceTextureRect, terraceBumpTextureRect, buildingCombineInstances);

                        break;
                    default:
                        // FIXME: checking invariants
                        throw new Exception("unknown ground floor: " + architectureStyle.groundFloor);
                }
            }

            if (_addHeaders)
            {
                // ==============================
                // Header
                // ==============================

                if (header != Header.NONE)
                {
                    resultSet = Repository.List("header").Filter("style", architectureStyle.name);

                    if (resultSet.Count() == 0)
                    {
                        throw new Exception("architecture style doesn't have any header model");
                    }

                    IModel headerModel = resultSet.Random();

                    CreateHeader(building, headerModel, hue, facadeYOffset, buildingCombineInstances);
                }
            }
            
            if (_addRailings)
            { 
                // ==============================
                // Railing
                // ==============================

                if (header != Header.ALL_AROUND)
                {
                    IModel railingModel = Repository.List("railing").Filter("style", architectureStyle.name).Single();

                    if (railingModel != null)
                    {
                        CreateRailing(building, railingModel, hue, facadeYOffset, buildingCombineInstances);
                    }
                }
            }

            // ==============================
            // Side Walls and Terrace
            // ==============================

            CreateSideWallsAndTerrace (building, wallTextureRect, wallBumpTextureRect, terraceTextureRect, terraceBumpTextureRect, hue, facadeYOffset, buildingCombineInstances);
			
			// ==============================
			// Façades
			// ==============================
			
			CreateFacades (building, wallTextureRect, wallBumpTextureRect, terraceTextureRect, terraceBumpTextureRect, hue, facadeYOffset, buildingCombineInstances);

            // ==============================
            // Rooftop items
            // ==============================

            if (_addRooftopItems)
            {
                List<CombineInstance> rooftopCombinedInstances = new List<CombineInstance>();
                AddRooftopItems(building, facadeYOffset, rooftopCombinedInstances);

                GameObject rooftopGameObject = new GameObject("Rooftop Items");

                rooftopGameObject.transform.parent = buildingGameObject.transform;
                rooftopGameObject.transform.localPosition = Vector3.zero;
                rooftopGameObject.transform.localRotation = Quaternion.identity;

                Mesh rooftopItemsMesh = new Mesh();
                rooftopItemsMesh.CombineMeshes(rooftopCombinedInstances.ToArray());
                rooftopGameObject.AddComponent<MeshFilter>().mesh = rooftopItemsMesh;

                rooftopGameObject.AddComponent<MeshRenderer>().sharedMaterial = architectureStyle.textureAtlasMaterial;
            }

            // ==============================
            // Final mesh
            // ==============================

            Mesh buildingMesh = new Mesh ();
			buildingMesh.CombineMeshes (buildingCombineInstances.ToArray ());
			buildingGameObject.AddComponent<MeshFilter> ().mesh = buildingMesh;
			
			buildingGameObject.AddComponent<MeshRenderer> ().sharedMaterial = architectureStyle.textureAtlasMaterial;
			buildingGameObject.AddComponent<MeshCollider> ().sharedMesh = buildingMesh;
        }
	}

}
