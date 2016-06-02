using UnityEngine;
using System;

public class CenterInWorld : MonoBehaviour
{
	[SerializeField]
	private ProceduralCity _proceduralCity;
	[SerializeField]
	private Transform[] _targets;
	
	void Start ()
	{
		if (_proceduralCity == null) {
			throw new Exception ("_proceduralCity == null");
		}
		
		foreach (Transform target in _targets) {
			float height = target.position.y;
			target.position = new Vector3 (_proceduralCity.WorldWidth / 2.0f, height, _proceduralCity.WorldHeight / 2.0f);
		}
	}
	
}
