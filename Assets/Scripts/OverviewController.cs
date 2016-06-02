using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class OverviewController : MonoBehaviour
{
	[SerializeField]
	private float _panSpeed;
	[SerializeField]
	private float _zoomSpeed;
	[SerializeField]
	private float _minHeight;
	[SerializeField]
	private float _maxHeight;
	[SerializeField]
	private int _panMouseButton;
	private Vector3 _lastMousePosition;
	private CharacterController _controller;
	
	void Awake ()
	{
		_controller = GetComponent<CharacterController> ();
	}

	void TryToMove (Vector3 move)
	{
		Vector3 position = transform.position;
		if (_controller.Move (move) != CollisionFlags.None) {
			transform.position = position;
		}
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown (_panMouseButton)) {
			_lastMousePosition = Input.mousePosition;
		} else if (Input.GetMouseButton (_panMouseButton)) {
			Vector3 mousePosition = Input.mousePosition;
			
			if (_lastMousePosition == mousePosition) {
				return;
			}
			
			Vector3 pan = (mousePosition - _lastMousePosition) * _panSpeed * Time.deltaTime * -1;
			Vector3 move = transform.TransformDirection (new Vector3 (pan.x, 0, 0)) + new Vector3 (0, 0, pan.y);
			TryToMove (move);
			
			_lastMousePosition = mousePosition;
		}
		
		float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
		if (scrollWheel != 0) {
			float verticalMove = scrollWheel * 10 * _zoomSpeed * Time.deltaTime * -1;
			if (verticalMove > 0 && transform.position.y + verticalMove >= _maxHeight) {
				verticalMove = _maxHeight - transform.position.y;
			} else if (verticalMove < 0 && transform.position.y + verticalMove <= _minHeight) {
				verticalMove = _minHeight - transform.position.y;
			}
			Vector3 move = transform.TransformDirection (new Vector3 (0, verticalMove, 0));
			TryToMove (move);
		}
	}
	
}