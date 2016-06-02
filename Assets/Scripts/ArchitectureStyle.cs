using UnityEngine;
using System;
using System.Collections.Generic;
using Pattern;

public class ArchitectureStyle : MonoBehaviour
{
	[SerializeField]
	private string _name;
	[SerializeField]
	private int _minWidth;
	[SerializeField]
	private int _maxWidth;
	[SerializeField]
	private int _minHeight;
	[SerializeField]
	private int _maxHeight;
	[SerializeField]
	private int _minDepth;
	[SerializeField]
	private int _maxDepth;
	[SerializeField]
	private int _tileWidth;
	[SerializeField]
	private int _tileHeight;
	[SerializeField]
	private int _spacing;
	[SerializeField]
	private float _extrusionDepth;
	[SerializeField]
	private float _terraceExtension;
	[SerializeField]
	private DoorPosition _doorPosition;
	[SerializeField]
	private GroundFloor _groundFloor;
	[SerializeField]
	private Header _header;
	[SerializeField]
	private float _minRooftopFillRate;
	[SerializeField]
	private float _maxRooftopFillRate;
	[SerializeField]
	private string _defaultFacadeElement;
	[SerializeField]
	private string _defaultFacadeDetail;
	[SerializeField]
	private TextAsset _elementsPatternsFile;
	[SerializeField]
	private TextAsset _detailsPatternsFile;
	[SerializeField]
	private bool _usesOperations;
	[SerializeField]
	private TextAsset _operationsPatternsFile;
	[SerializeField]
	private Color[] _colorPalette;
	[SerializeField]
	private string[] _wallTextures;
	[SerializeField]
	private string[] _terraceTextures;
	[SerializeField]
	private TextAsset _textureAtlasFile;
	[SerializeField]
	private Material _textureAtlasMaterial;
	private TextureAtlas _textureAtlas;
	private List<Rect> _wallTexturesRects = new List<Rect> ();
	private List<Rect> _wallBumpTexturesRects = new List<Rect> ();
	private List<Rect> _terraceTexturesRects = new List<Rect> ();
	private List<Rect> _terraceBumpTexturesRects = new List<Rect> ();
	private Pattern<FacadeItem>[] _elementsPatterns;
	private Pattern<FacadeItem>[] _detailsPatterns;
	private Pattern<FacadeOperation>[] _operationsPatterns;
	
	public float terraceExtension {
		get {
			return _terraceExtension;
		}
	}
	
	public void GetRandomTerraceTextureRect (out Rect terraceTextureRect, out Rect terraceBumpTextureRect)
	{
		int i = UnityEngine.Random.Range (0, _terraceTexturesRects.Count);
		terraceTextureRect = _terraceTexturesRects [i];
		terraceBumpTextureRect = _terraceBumpTexturesRects [i];
	}
	
	public void GetRandomWallTextureRect (out Rect wallTextureRect, out Rect wallBumpTextureRect)
	{
		int i = UnityEngine.Random.Range (0, _wallTexturesRects.Count);
		wallTextureRect = _wallTexturesRects [i];
		wallBumpTextureRect = _wallBumpTexturesRects [i];
	}
	
	public Material textureAtlasMaterial {
		get {
			return this._textureAtlasMaterial;
		}
	}
	
	public TextureAtlas textureAtlas {
		get {
			return this._textureAtlas;
		}
	}
	
	public Color[] colorPalette {
		get {
			return this._colorPalette;
		}
	}

	public char defaultFacadeDetailSymbol {
		get {
			return this._defaultFacadeDetail [0];
		}
	}
	
	public int defaultFacadeDetailIndex {
		get {
			return int.Parse (this._defaultFacadeDetail [1] + "");
		}
	}

	public char defaultFacadeElementSymbol {
		get {
			return this._defaultFacadeElement [0];
		}
	}
	
	public int defaultFacadeElementIndex {
		get {
			return int.Parse (this._defaultFacadeElement [1] + "");
		}
	}

	public DoorPosition doorPosition {
		get {
			return this._doorPosition;
		}
	}

	public float extrusionDepth {
		get {
			return this._extrusionDepth;
		}
	}

	public GroundFloor groundFloor {
		get {
			return this._groundFloor;
		}
	}

	public Header header {
		get {
			return this._header;
		}
	}

	public int maxDepth {
		get {
			return this._maxDepth;
		}
	}

	public int maxHeight {
		get {
			return this._maxHeight;
		}
	}

	public int maxWidth {
		get {
			return this._maxWidth;
		}
	}

	public int minDepth {
		get {
			return this._minDepth;
		}
	}

	public int minHeight {
		get {
			return this._minHeight;
		}
	}

	public int minWidth {
		get {
			return this._minWidth;
		}
	}

	new public string name {
		get {
			return this._name;
		}
	}

	public int tileHeight {
		get {
			return this._tileHeight;
		}
	}

	public int tileWidth {
		get {
			return this._tileWidth;
		}
	}
	
	public bool usesOperations {
		get {
			return this._usesOperations;
		}
	}
	
	public float minRooftopFillRate {
		get {
			return this._minRooftopFillRate;
		}
	}
	
	public float maxRooftopFillRate {
		get {
			return this._maxRooftopFillRate;
		}
	}
	
	public int spacing {
		get {
			return _spacing;
		}
	}
	
	public Pattern<FacadeItem> randomElementsPattern {
		get {
			return _elementsPatterns [UnityEngine.Random.Range (0, _elementsPatterns.Length)];
		}
	}
	
	public Pattern<FacadeItem> randomDetailsPattern {
		get {
			return _detailsPatterns [UnityEngine.Random.Range (0, _detailsPatterns.Length)];
		}
	}
	
	public Pattern<FacadeOperation> randomOperationsPattern {
		get {
			if (!_usesOperations) {
				return null;
			}
			return _operationsPatterns [UnityEngine.Random.Range (0, _operationsPatterns.Length)];
		}
	}
	
	public Color randomHue {
		get {
			return _colorPalette [UnityEngine.Random.Range (0, _colorPalette.Length)];
		}
	}
	
	void Start ()
	{
		_elementsPatterns = PatternParser.ParsePatterns<FacadeItem> (_elementsPatternsFile, this);
		_detailsPatterns = PatternParser.ParsePatterns<FacadeItem> (_detailsPatternsFile, this);
		if (_usesOperations) {
			_operationsPatterns = PatternParser.ParsePatterns<FacadeOperation> (_operationsPatternsFile, this);
		}
		
		if (_textureAtlasFile == null) {
			throw new Exception ("_textureAtlasFile == null");
		}
		
		_textureAtlas = TextureAtlas.LoadFromString (_textureAtlasFile.text);
		
		if (_wallTextures == null || _wallTextures.Length == 0) {
			throw new Exception ("there must be at least 1 wall texture");
		}
		
		if (_terraceTextures == null || _terraceTextures.Length == 0) {
			throw new Exception ("there must be at least 1 terrace texture");
		}
		
		foreach (string wallTexture in _wallTextures) {
			_wallTexturesRects.Add (_textureAtlas.atlasRects [wallTexture]);
			//_wallBumpTexturesRects.Add (_textureAtlas.atlasRects [wallTexture + "-normal"]);
			_wallBumpTexturesRects.Add (new Rect (0, 0, 0, 0));
		}
			
		foreach (string terraceTexture in _terraceTextures) {
			_terraceTexturesRects.Add (_textureAtlas.atlasRects [terraceTexture]);
			//_terraceBumpTexturesRects.Add (_textureAtlas.atlasRects [terraceTexture + "-normal"]);
			_terraceBumpTexturesRects.Add (new Rect (0, 0, 0, 0));
		}
	}
	
}
