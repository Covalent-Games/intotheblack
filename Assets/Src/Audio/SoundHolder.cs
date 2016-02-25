using UnityEngine;
using System.Collections;

public class SoundHolder : MonoBehaviour {

	public static SoundHolder Instance;

	public AudioClip PulseLaser_01;
	public AudioClip[] Explosions;
	public AudioClip SuccessTone;


	private void Awake() {

		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
	}
}
