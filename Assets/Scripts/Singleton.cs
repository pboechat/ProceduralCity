using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;
 
	public static T instance {
		get {
			if (_instance == null) {
				_instance = (T)FindObjectOfType (typeof(T));
 
				if (_instance == null) {
					Debug.LogError ("An instance of " + typeof(T) + " needed in the scene, but there is none.");
				}
			}
 
			return _instance;
		}
	}
}