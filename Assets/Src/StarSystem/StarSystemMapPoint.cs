using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StarSystemMapPoint : MonoBehaviour {

	public Guid ID;

	private static StarSystemInfoDisplay _infoDisplay;
	private static StarMapCameraController _camController;

	private List<GameObject> lines = new List<GameObject>();


	private void Awake() {

		if (_infoDisplay == null) {
			_infoDisplay = FindObjectOfType<StarSystemInfoDisplay>(); 
		}
		if (_camController == null) {
			_camController = FindObjectOfType<StarMapCameraController>(); 
		}
	}

	public void OnMouseEnter() {

		float pop = StarSystemData.StartSystemMapTable[ID].Population;
		string popString = pop.ToString();

		if (pop >= 1000000000) {
			popString = Math.Round((pop / 1000000000f), 1) + "B";
		} else if (pop >= 1000000) {
			popString = Math.Round((pop / 1000000f), 1) + "M";
		}

		Material mat = Resources.Load<Material>("Materials/Laser_Material_01");
		foreach (Guid connection in StarSystemData.StartSystemMapTable[ID].ConnectedSystems) {
			GameObject go = new GameObject();
			lines.Add(go);
			LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
			lineRenderer.material = mat;
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, StarSystemData.StartSystemMapTable[connection].GetPosition());
			lineRenderer.SetWidth(0.1f, 0.1f);
			lineRenderer.SetColors(new Color(1f, 1f, 1f, 0.1f), new Color(1f, 1f, 1f, 0.1f));
		}
	}

	public void OnMouseExit() {

		foreach (GameObject go in lines) {
			Destroy(go);
		}
		lines = new List<GameObject>();
	}

	public void OnMouseDown() {

		if (!StarMapCameraController.CameraMoving) {
			StarMapSceneManager.SystemSelected = StarSystemData.StartSystemMapTable[ID];
			_infoDisplay.SetTargettedSystemInfo(StarSystemData.StartSystemMapTable[ID]);
			_camController.MoveCameraToSelected();
		}
	}
}
