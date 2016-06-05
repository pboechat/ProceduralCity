using UnityEngine;
using System.Collections.Generic;
using BinPack;

public class MaxRectsController : RectangleDrawingObserver
{
	enum State
	{
		DRAWING_ITEMS,
		DRAWING_CONTAINER,
		PACK_BINS,
        DISCARDING_NON_EDGE_BINS
	}
	
	[SerializeField]
	private RectanglesRenderer _renderer;
	[SerializeField]
	private Material _itemMaterial;
	[SerializeField]
	private Material _containerMaterial;
	[SerializeField]
	private Material _outputMaterial;
	[SerializeField]
	private bool _tryToFillContainer;
    private List<Rect> _bins;
    private State _state = State.DRAWING_ITEMS;
	private List<Rect> _items = new List<Rect> ();
	private Rect _container;
	
	void Start ()
	{
		_renderer.Push (_itemMaterial, true);
	}
	
	public override void OnRectangleChange (Rect previousRectangle, Rect newRectangle)
	{
		switch (_state) {
		case State.DRAWING_ITEMS:
			_renderer.Remove (previousRectangle);
			_renderer.Add (newRectangle);
			break;
		case State.DRAWING_CONTAINER:
			_renderer.Remove (previousRectangle);
			_renderer.Add (newRectangle);
			break;
		case State.PACK_BINS:
			break;
		}
	}

	public override void OnRectangleDrawn (Rect rectangle)
	{
		switch (_state) {
		case State.DRAWING_ITEMS:
			_items.Add (new Rect (0, 0, rectangle.width, rectangle.height));
			break;
		case State.DRAWING_CONTAINER:
			_container = new Rect (rectangle.x, rectangle.y, rectangle.width, rectangle.height);
			break;
		case State.PACK_BINS:
			break;
		}
	}

	bool IsDegenerate (Rect output)
	{
		return output.width == 0 || output.height == 0;
	}

	void PackBins()
	{
		MaxRects maxRects = new MaxRects ((int)_container.width, (int)_container.height, false);
		_bins = new List<Rect> ();
		
		if (_tryToFillContainer) {
			List<Rect> inputs = new List<Rect> (_items);
			while (inputs.Count > 0) {
				Rect input = inputs [UnityEngine.Random.Range (0, inputs.Count)];
				Rect output = maxRects.Insert ((int)input.width, (int)input.height, MaxRects.FreeRectChoiceHeuristic.RectContactPointRule);
				if (IsDegenerate (output)) {
					inputs.Remove (input);
				} else {
					_bins.Add (new Rect (output.x + _container.x, output.y + _container.y, output.width, output.height));
				}
			}
		} else {
			foreach (Rect input in _items) {
				Rect output = maxRects.Insert ((int)input.width, (int)input.height, MaxRects.FreeRectChoiceHeuristic.RectContactPointRule);
				_bins.Add (new Rect (output.x + _container.x, output.y + _container.y, output.width, output.height));
			}
		}
	}
	
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			switch (_state) {
			case State.DRAWING_ITEMS:
                _renderer.Push (_containerMaterial, true);
				_state = State.DRAWING_CONTAINER;
				break;
			case State.DRAWING_CONTAINER:
                _renderer.Push(_outputMaterial, false);
                PackBins();
                foreach (Rect bin in _bins)
                    _renderer.Add(bin);
                _state = State.PACK_BINS;
                break;
            case State.PACK_BINS:
                _renderer.Pop();
                _renderer.Push(_outputMaterial, false);
                DiscardNonEdgeBin();
                foreach (Rect bin in _bins)
                    _renderer.Add(bin);
                _state = State.DISCARDING_NON_EDGE_BINS;
                break;
            case State.DISCARDING_NON_EDGE_BINS:
                _items.Clear ();
				_container = new Rect (0, 0, 0, 0);
				_renderer.Clear ();
				_renderer.Push (_itemMaterial, true);
				_state = State.DRAWING_ITEMS;
				break;
			}
		}
	}

    void DiscardNonEdgeBin()
    {
        List<Rect> newBins = new List<Rect>();
        foreach (var bin in _bins)
        {
            if (bin.xMin != _container.xMin &&
                bin.xMax != _container.xMax &&
                bin.yMin != _container.yMin &&
                bin.yMax != _container.yMax)
                continue;
            newBins.Add(bin);
        }
        _bins = newBins;
    }

}
