using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StarSystemMapPoint : MonoBehaviour {

	public static bool CameraMoving = false;

	public Guid ID;

	private static StarSystemInfoDisplay _infoDisplay;

	private List<GameObject> lines = new List<GameObject>();


	private void Awake() {

		_infoDisplay = FindObjectOfType<StarSystemInfoDisplay>();
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

		if (!CameraMoving) {
			StartCoroutine(MoveTowardsSelectedRoutine());
			_infoDisplay.SetTargettedSystemInfo(StarSystemData.StartSystemMapTable[ID]);
		}
	}

	private IEnumerator MoveTowardsSelectedRoutine() {

		StarSystemMapPoint.CameraMoving = true;

		Vector3 v = Vector3.zero;

		// The 10.000001 is to account for the Z distance. Really we're just checking if the camera is 0.00001 orthagonically.
		while (Vector3.Distance(Camera.main.transform.position, transform.position) > 10.00001f) {
			Camera.main.transform.position = Vector3.SmoothDamp(
				Camera.main.transform.position,
				new Vector3(
					transform.position.x,
					transform.position.y,
					Camera.main.transform.position.z),
				ref v,
				.10f,
				200f);
			yield return null;
		}

		StarSystemMapPoint.CameraMoving = false;
	}
}
