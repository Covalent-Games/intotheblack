using UnityEngine;
using System.Collections;

public class ParticleAutoDestroy : MonoBehaviour {

	private ParticleSystem _particles;
	private bool _readyToDestroy = false;

	private void Start() {

		_particles = GetComponent<ParticleSystem>();
	}

	void Update () {
		
		if (_readyToDestroy && !_particles.IsAlive()) {
			Destroy(gameObject);
		}
		if (_particles.isPlaying) {
			_readyToDestroy = true;
		}
	}
}
