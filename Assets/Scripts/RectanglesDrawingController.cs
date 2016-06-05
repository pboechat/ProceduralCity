using UnityEngine;
using System.Collections;

public class RectanglesDrawingController : MonoBehaviour
{
	[SerializeField]
	private int _drawButton;
	[SerializeField]
	private RectangleDrawingObserver[] _observers;
	[SerializeField]
	private bool _forceIntCoords;
	private Vector3 _startingWorldPosition;
	private Rect _rectangleBeingDrawn;
	private bool _drawing = false;
	
	void NotifyRectangleRectangleChange (Rect previousRectangle, Rect newRectangle)
	{
		foreach (RectangleDrawingObserver observer in _observers) {
			observer.OnRectangleChange (previousRectangle, newRectangle);
		}
	}
	
	void NotifyRectangleDrawn (Rect rectangle)
	{
		foreach (RectangleDrawingObserver observer in _observers) {
			observer.OnRectangleDrawn (rectangle);
		}
	}

	Vector3 GetMouseWorldPosition ()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 10;
		return Camera.main.ScreenToWorldPoint (mousePosition);
	}
	
	void Update ()
	{
		if (Input.GetMouseButton (_drawButton)) {
			if (_drawing) {
				Vector3 currentWorldPosition = GetMouseWorldPosition ();
				Vector3 direction = currentWorldPosition - _startingWorldPosition;
			
				if (direction.sqrMagnitude < 0.333f) {
					return;
				}
				
				float left;
				float top;
				float right;
				float bottom;
				if (_forceIntCoords) {
					left = (float)Mathf.Min ((int)currentWorldPosition.x, (int)_startingWorldPosition.x);
					top = (float)Mathf.Min ((int)currentWorldPosition.y, (int)_startingWorldPosition.y);
					right = (float)Mathf.Max ((int)currentWorldPosition.x, (int)_startingWorldPosition.x);
					bottom = (float)Mathf.Max ((int)currentWorldPosition.y, (int)_startingWorldPosition.y);
				} else {
					left = Mathf.Min (currentWorldPosition.x, _startingWorldPosition.x);
					top = Mathf.Min (currentWorldPosition.y, _startingWorldPosition.y);
					right = Mathf.Max (currentWorldPosition.x, _startingWorldPosition.x);
					bottom = Mathf.Max (currentWorldPosition.y, _startingWorldPosition.y);
				}
				
				Rect newRectangle = new Rect (left, top, right - left, bottom - top);
				NotifyRectangleRectangleChange (_rectangleBeingDrawn, newRectangle);
				_rectangleBeingDrawn = newRectangle;
			
			} else {
				Vector3 mousePosition = Input.mousePosition;
				mousePosition.z = 10;
				_startingWorldPosition = Camera.main.ScreenToWorldPoint (mousePosition);
				_drawing = true;
			}
		} else {
			if (_drawing) {
				NotifyRectangleDrawn (_rectangleBeingDrawn);
				_rectangleBeingDrawn = new Rect (0, 0, 0, 0);
			}
			_drawing = false;
		}
	}
}
