using UnityEngine;
using System.Collections;

public class RedStar : MonoBehaviour {
	
	void Update () {

		transform.Rotate(new Vector3(0f, 0f, -75 * Time.deltaTime));
	}
}
