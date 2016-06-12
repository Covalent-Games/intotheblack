using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour {

	private GameObject _asteroidPrefab;
	private List<GameObject> _asteroidList = new List<GameObject>();

	private void Awake() {

		_asteroidPrefab = (GameObject)Resources.Load("Prefabs/Asteroid");
	}

	public IEnumerator SpawnAsteroidRoutine() {

		float previousSize = 1f;
		float waitTime = 2f;

		while (gameObject.activeSelf) {
			yield return new WaitForSeconds(Random.Range(waitTime, waitTime + 2));
			GameObject roid = GetNextAsteroid();
			Vector3 heading = GameManager.Player.transform.up * 2f;
			if (heading.x < 1 && heading.x > 0 && heading.y < 1 && heading.y > 0) {
				heading.x += 1;
			}
			Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(
				Random.Range(heading.x - 0.5f, heading.x + 0.5f),
				Random.Range(heading.y - 0.5f, heading.y + 0.5f),
				-Camera.main.transform.position.z));
			roid.transform.position = spawnPos;
			Rigidbody2D rb = roid.GetComponent<Rigidbody2D>();
			rb.velocity = new Vector3(Random.value, Random.value, 0) * Random.Range(1f, 2f);
			rb.angularVelocity = Random.value;
			float scale = Mathf.Clamp(Random.Range(previousSize - .85f, previousSize + .2f), .1f, 5f);
			roid.transform.localScale = new Vector3(scale, scale);
			rb.mass = roid.transform.localScale.magnitude;
			roid.SetActive(true);

			if (Random.value > 0.09f) {
				previousSize = roid.transform.localScale.x;
			} else {
				previousSize = Random.Range(.1f, 4f);
			}
			waitTime = previousSize * (previousSize * 0.5f);
			CleanOldAsteroids();
		}
	}

	private void CleanOldAsteroids() {

		GameObject roid;
		for (int i = 0; i < _asteroidList.Count; i++) {
			if (_asteroidList[i].activeSelf) {
				roid = _asteroidList[i];
				if (Vector3.Distance(roid.transform.position, GameManager.Player.transform.position) > 100) {
					roid.SetActive(false);
				}
			}
		}
	}

	private GameObject GetNextAsteroid() {

		GameObject roid = null;
		for (int i = 0; i < _asteroidList.Count; i++) {
			if (!_asteroidList[i].activeSelf) {
				roid = _asteroidList[i];
				break;
			}
		}
		if (roid == null) {
			roid = Instantiate(_asteroidPrefab);
			_asteroidList.Add(roid);
		}
		return roid;
	}
}
