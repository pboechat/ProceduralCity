using UnityEngine;
using System.Collections.Generic;

public abstract class BuildingsGenerator : MonoBehaviour
{
	protected List<Building> _buildings;
	
	public List<Building> buildings {
		get {
			return this._buildings;
		}
	}
	
	public abstract void Execute (List<Allotment> allotments);
	
}

