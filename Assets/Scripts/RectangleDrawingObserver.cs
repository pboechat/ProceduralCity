using UnityEngine;
using System;

public abstract class RectangleDrawingObserver : MonoBehaviour
{
	public abstract void OnRectangleChange (Rect previousRectangle, Rect newRectangle);
	
	public abstract void OnRectangleDrawn (Rect rectangle);
	
}

