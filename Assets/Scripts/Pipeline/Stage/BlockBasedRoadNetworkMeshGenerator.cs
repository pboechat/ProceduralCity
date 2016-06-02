using UnityEngine;
using Grid;

public class BlockBasedRoadNetworkMeshGenerator : RoadNetworkMeshGenerator
{
	[SerializeField]
	private float _y;
	[SerializeField]
	private Texture2D _asphaltTexture;
	[SerializeField]
	private Vector2 _asphaltTextureTiling = Vector2.one;
	[SerializeField]
	private Texture2D _sidewalkTexture;
	[SerializeField]
	private Vector2 _sidewalkTextureTiling = Vector2.one;
	[SerializeField]
	private Texture2D _blockInteriorTexture;
	[SerializeField]
	private Vector2 _blockInteriorTextureTiling = Vector2.one;

	Material CreateBlockGroundMaterial (float halfRoadWidth, float sidewalkWidth, float blockGroundWidth, float blockGroundHeight)
	{
		Material blockGroundMaterial = new Material (Shader.Find ("Road/Block Ground"));
		blockGroundMaterial.SetTexture ("_AsphaltTex", _asphaltTexture);
		blockGroundMaterial.SetTextureScale ("_AsphaltTex", _asphaltTextureTiling);
		blockGroundMaterial.SetTexture ("_SidewalkTex", _sidewalkTexture);
		blockGroundMaterial.SetTextureScale ("_SidewalkTex", _sidewalkTextureTiling);
		blockGroundMaterial.SetTexture ("_BlockInteriorTex", _blockInteriorTexture);
		blockGroundMaterial.SetTextureScale ("_BlockInteriorTex", _blockInteriorTextureTiling);
		blockGroundMaterial.SetFloat ("_HalfRoadWidth", halfRoadWidth);
		blockGroundMaterial.SetFloat ("_SidewalkWidth", sidewalkWidth);
		blockGroundMaterial.SetFloat ("_BlockGroundWidth", blockGroundWidth);
		blockGroundMaterial.SetFloat ("_BlockGroundHeight", blockGroundHeight);
		return blockGroundMaterial;
	}
	
	public override void Execute (RoadNetworkParameters roadNetworkParameters, BaseGrid grid)
	{
		GameObject blockGroundsGameObject = new GameObject ("Block Grounds");
		
		blockGroundsGameObject.transform.localPosition = new Vector3 (0, _y, 0);
		blockGroundsGameObject.transform.localRotation = Quaternion.identity;
		
		float halfRoadWidth = roadNetworkParameters.roadWidth * 0.5f;
		
		Mesh mesh;
		foreach (Cell[] row in grid) {
			foreach (Cell cell in row) {
				float halfCellWidth = cell.width * 0.5f;
				float halfCellHeight = cell.height * 0.5f;
				
				float x = cell.center.x - halfCellWidth - halfRoadWidth;
				float z = cell.center.y - halfCellHeight - halfRoadWidth;
				
				GameObject blockGroundGameObject = new GameObject ("Block Ground (" + cell.x + ", " + cell.y + ")");
				
				float blockGroundWidth = cell.width + roadNetworkParameters.roadWidth;
				float blockGroundHeight = cell.height + roadNetworkParameters.roadWidth;
				
				mesh = MeshUtils.CreateTopFaceMesh (0, blockGroundWidth, 0, blockGroundHeight, 0, 1, 1, new Rect (0, 0, blockGroundWidth, blockGroundHeight), new Rect (0, 0, 1, 1), Color.white);
				
				blockGroundGameObject.transform.parent = blockGroundsGameObject.transform;
				blockGroundGameObject.transform.localPosition = new Vector3 (x, 0, z);
				blockGroundGameObject.transform.localRotation = Quaternion.identity;
				
				blockGroundGameObject.AddComponent<MeshFilter> ().mesh = mesh;
				blockGroundGameObject.AddComponent<MeshRenderer> ().material = CreateBlockGroundMaterial (halfRoadWidth, (float)roadNetworkParameters.sidewalkWidth, blockGroundWidth, blockGroundHeight);
			}
		}
		
		BoxCollider boxCollider = blockGroundsGameObject.AddComponent<BoxCollider> ();
		boxCollider.center = new Vector3 (grid.bounds.center.x, 0, grid.bounds.center.y);
		boxCollider.size = new Vector3 (grid.bounds.size.x, 0, grid.bounds.size.y);
	}

}

