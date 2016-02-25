using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class StarMapCamerController : MonoBehaviour {

	private Vector3 _cameraVelocity = Vector3.zero;

	void Start() {

		//TODO: This is the wrong file to check. The star map doesn't hold player location.
		if (File.Exists(FilePaths.StarMapPath)) {
			SetCameraToCurrentSystem();
		} else {
			MoveCamToHome();
		}
	}

	private void SetCameraToCurrentSystem() {

		//TODO: StarSystemLoaded isn't currently persistent across saves. Needs to be saved in GameData still.
		Vector3 newCamPos;
		if (StarSystemData.StarSystemLoaded != null) {
			newCamPos = StarSystemData.StarSystemLoaded.GetPosition();
			FindObjectOfType<StarSystemInfoDisplay>().SetTargettedSystemInfo(StarSystemData.StarSystemLoaded); 
		} else {
			newCamPos = StarSystemData.PlayerHome.GetPosition();
			FindObjectOfType<StarSystemInfoDisplay>().SetTargettedSystemInfo(StarSystemData.PlayerHome);
		}
		newCamPos.z = -10;
		Camera.main.transform.position = newCamPos;
	}

	private void MoveCamToHome() {

		Vector3 startCamPos = StarSystemData.EnemyHome.GetPosition();
		Vector3 endCamPos = StarSystemData.PlayerHome.GetPosition();
		startCamPos.z = Camera.main.transform.position.z;
		endCamPos.z = Camera.main.transform.position.z;
		Camera.main.transform.position = startCamPos;
		StartCoroutine(MoveCamToHomeRoutine(endCamPos));
		
	}

	private IEnumerator MoveCamToHomeRoutine(Vector3 destination) {

		StarSystemMapPoint.CameraMoving = true;

		yield return new WaitForSeconds(2f);

		Vector3 velocity = Vector3.zero;

		while (Vector3.Distance(Camera.main.transform.position, destination) > 0.001f) {
			Camera.main.transform.position = Vector3.SmoothDamp(
				Camera.main.transform.position,
				new Vector3(
					destination.x,
					destination.y,
					Camera.main.transform.position.z),
				ref velocity,
				.5f,
				100f);
			yield return null;
		}
		StarSystemMapPoint.CameraMoving = false;
		FindObjectOfType<StarSystemInfoDisplay>().SetTargettedSystemInfo(StarSystemData.PlayerHome);
	}

	private bool MouseInScreen() {

		Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
		if (screenRect.Contains(Input.mousePosition)) {
			return true;
		} else {
			return false;
		}
	}
}
