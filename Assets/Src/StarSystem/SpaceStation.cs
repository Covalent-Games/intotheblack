using UnityEngine;
using System.Collections;

public class SpaceStation : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		transform.Rotate(new Vector3(0f, 0f, 2 * Time.deltaTime));
	}
}
