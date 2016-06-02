using UnityEngine;

public class PointOfView : Singleton<PointOfView>
{
	[SerializeField]
	private bool _firstPerson;
	[SerializeField]
	private GameObject _overviewController;
	[SerializeField]
	private GameObject _firstPersonController;
	
	void OnGUI ()
	{
		if ((_firstPerson = GUI.Toggle (new Rect (10, 10, 120, 20), _firstPerson, "First Person"))) {
			SetFirstPersonPoV ();
		} else {
			SetOverviewPoV ();
		}
	}
	
	public void SetFirstPersonPoV ()
	{
		_overviewController.SetActive (false);
		_firstPersonController.SetActive (true);
	}
	
	public void SetOverviewPoV ()
	{
		_firstPersonController.SetActive (false);
		_overviewController.SetActive (true);
	}
	
}