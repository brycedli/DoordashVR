using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player-Control/Simple Controller")]


public class MouseLook : MonoBehaviour {

	public GameObject PlayerCamera;

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2, None = 3 };

	public RotationAxes axes = RotationAxes.MouseXAndY;

	public float sensitivityX = 3F;
	public float sensitivityY = 3F;

	public float minimumX = -180F;
	public float maximumX = 180F;

	public float minimumY = -90.0F;
	public float maximumY = 90.0F;

	private float rotationX = 0F;
	private float rotationY = 0F;

	private Quaternion originalRotationX;
	private Quaternion originalRotationY;
	// Use this for initialization
	void Start () {
		originalRotationX = transform.localRotation;
		originalRotationY = PlayerCamera.transform.localRotation;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
			mouseControl ();
		
	}
	void PauseControl(){
		//Code that allows the player to pause and unpause the game
		if (Cursor.visible){
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			Time.timeScale = 1.0f;
		} else
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 0.0f;
		}
	}

	void mouseControl () {
		//Code that controls the mouse.

		//These lines of code are more complex, requiring a knowledge of Quaternions

		//This section controls the X direction mouse movement
		if (axes == RotationAxes.MouseX || axes == RotationAxes.MouseXAndY)
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationX = ClampAngle(rotationX, minimumX, maximumX);

			Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation = originalRotationX * xQuaternion;
		}

		//This controls the Y direction. This is linked to the camera game  object
		if (axes == RotationAxes.MouseY || axes == RotationAxes.MouseXAndY)
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = ClampAngle(rotationY, minimumY, maximumY);

			Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
			PlayerCamera.transform.localRotation = originalRotationY * yQuaternion;
		}
	}
	public static float ClampAngle(float angle, float min, float max)
	{
		//Clamp Angle function, simplifies the code for mouse control
		if (max >= 180.0f && min <= -180.0f){
			if (angle < min)
				angle += 360.0f;
			else if (angle > max)
				angle -= 360.0f;
		}
		return Mathf.Clamp(angle, min, max);
	}
	void OnApplicationPause( bool pauseStatus )
	{
		//This is the other way to handle pausing, this is a wednesday thing
		//isPaused = pauseStatus;
	}

}
