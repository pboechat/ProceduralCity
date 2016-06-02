using UnityEngine;

public class Normalmap
{
	public static Texture2D Generate (Texture2D source, float strength)
	{
		strength = Mathf.Clamp (strength, 0.0f, 10.0f);
		Texture2D result;
		float xLeft;
		float xRight;
		float yUp;
		float yDown;
		float yDelta;
		float xDelta;
		result = new Texture2D (source.width, source.height, TextureFormat.ARGB32, true);
		for (int by=0; by<result.height; by++) {
			for (int bx=0; bx<result.width; bx++) {
				xLeft = source.GetPixel (bx - 1, by).grayscale * strength;
				xRight = source.GetPixel (bx + 1, by).grayscale * strength;
				yUp = source.GetPixel (bx, by - 1).grayscale * strength;
				yDown = source.GetPixel (bx, by + 1).grayscale * strength;
				xDelta = ((xLeft - xRight) + 1) * 0.5f;
				yDelta = ((yUp - yDown) + 1) * 0.5f;
				result.SetPixel (bx, by, new Color (xDelta, yDelta, 1.0f, yDelta));
			}
		}
		result.Apply ();
		return result;
	}
	
}