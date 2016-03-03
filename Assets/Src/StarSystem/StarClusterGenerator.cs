using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Covalent.Generators;

public class StarClusterGenerator : MonoBehaviour {

	[HideInInspector]
	public bool GenerateNewMap = false;
	public int StarCount = 50;
	public float StarConenctionRange = 8f;
	public float MinimumStarDistance = 3.5f;
	public Transform StarCluster;

	[SerializeField]
	private GameObject _starPrefab;
	[SerializeField]
	private AnimationCurve _hostilityDistanceCurve;
	private int _currentStarCount;
	private Queue<StarSystemData> _connectionQueue = new Queue<StarSystemData>();
	private MarkovNameGenerator NameGenerator = new MarkovNameGenerator(MarkovNameGenerator.TempSampleData);

	private void Start() {

		// TODO: This should send in a massive file of names so we have a good sample set.
		DrawStarMap();
	}

	public void GenerateNewStarSystems() {

		//Sector generator should use a normalized random inside circle* minimum sector distance instead 
		//of randomized vector2. This should make for a more varied layout with more consistent connection
		//distances, because hypotenuses are jerks.

	   StarSystemData star = GenerateNewStar(Vector3.zero);
		_connectionQueue.Enqueue(star);
		while (_currentStarCount < StarCount) {
			int count = Random.Range(2, 6);
			GrowStarConnections(_connectionQueue.Dequeue(), count);
			_currentStarCount += count;
		}
		ConnectNearbyStars();
		SetHomeSystems();
		SetHostilityRatings();
		BuildSpaceStations();
		StarSystemData.Save();
	}

	private void GrowStarConnections(StarSystemData star, int numberOfConnections) {

		Vector3 pos;
		StarSystemData connectedStar = null;
		bool useExistingStar = false;

		for (int i = 0; i < numberOfConnections; i++) {
			//Debug.Log(Random.insideUnitCircle);
			//Debug.Log((Vector3)(Random.insideUnitCircle * Random.Range(-StarConenctionRange, StarConenctionRange)) + 
			//	star.GetPosition());
			pos = new Vector3(
				Random.Range(-StarConenctionRange, StarConenctionRange),
				Random.Range(-StarConenctionRange, StarConenctionRange),
				0f) + star.GetPosition();
			//If a child star is too close, push it directly away
			if (Vector3.Distance(pos, star.GetPosition()) < MinimumStarDistance) {
				Vector3 direction = Vector3.Normalize(pos - star.GetPosition());
				pos += (direction * 2);
			}
			// If we're close to an existing star, use that star instead
			foreach (var kvPair in StarSystemData.StartSystemMapTable) {
				if (Vector3.Distance(kvPair.Value.GetPosition(), pos) < MinimumStarDistance) {
					useExistingStar = true;
					connectedStar = kvPair.Value;
					break;
				}
			}
			if (!useExistingStar) {
				connectedStar = GenerateNewStar(pos);
				_connectionQueue.Enqueue(connectedStar);
			} else {
				// Reset bool;
				useExistingStar = false;
				// We didn't make a new star... :(
				_currentStarCount--;
			}
			
			
			star.ConnectedSystems.Add(connectedStar.ID);
			connectedStar.ConnectedSystems.Add(star.ID);
		}
	}

	private StarSystemData GenerateNewStar(Vector3 pos) {

		StarSystemData star = new StarSystemData(
			NameGenerator.GenerateName(),
			GeneratePopulation(0),
			Random.Range(.45f, .65f),
			pos);
		return star;
	}

	private void ConnectNearbyStars() {
		
		foreach (StarSystemData star in StarSystemData.StartSystemMapTable.Values) {
			foreach (var kvPair in StarSystemData.StartSystemMapTable) {
				if (Vector3.Distance(star.GetPosition(), kvPair.Value.GetPosition()) <= StarConenctionRange * 0.75f) {
					if (star != kvPair.Value && !star.ConnectedSystems.Contains(kvPair.Value.ID)) {
						star.ConnectedSystems.Add(kvPair.Value.ID);
						kvPair.Value.ConnectedSystems.Add(star.ID);
					}
				}
			}
		}
	}

	private void SetHomeSystems() {

		List<StarSystemData> possibleEnemyHomes = new List<StarSystemData>();
		StarSystemData enemyHome = null;
		StarSystemData home = null;

		foreach (StarSystemData data in StarSystemData.StartSystemMapTable.Values) {
			if (data.ConnectedSystems.Count == 1) {
				possibleEnemyHomes.Add(data);
			}
		}
		enemyHome = possibleEnemyHomes[Random.Range(0, possibleEnemyHomes.Count)];
		enemyHome.IsEnemyHome = true;

		float farthestDistance = 0f;
		float distance;
		foreach (StarSystemData data in StarSystemData.StartSystemMapTable.Values) {
			distance = Vector3.Distance(data.GetPosition(), enemyHome.GetPosition());
			if (distance > farthestDistance){
				home = data;
				farthestDistance = distance;
			}
		}
		home.IsPlayerHome = true;
		enemyHome.Hostility = 1f;
		enemyHome.EconomyState -= .25f;
		home.EconomyState += .25f;
		home.Hostility = 0f;
	}

	private void SetHostilityRatings() {

		float d = Vector3.Distance(
			StarSystemData.EnemyHome.GetPosition(), 
			StarSystemData.PlayerHome.GetPosition());

		foreach (StarSystemData data in StarSystemData.StartSystemMapTable.Values) {
			data.Hostility = _hostilityDistanceCurve.Evaluate(
				Vector3.Distance(data.GetPosition(), StarSystemData.EnemyHome.GetPosition()) / d);
			//data.Hostility = 1 - Vector3.Distance(data.GetPosition(), StarSystemData.EnemyHome.GetPosition()) / d;
		}
	}

	private void BuildSpaceStations() {
		
		foreach (StarSystemData data in StarSystemData.StartSystemMapTable.Values) {
			float economicStateModifier = 0.5f + (data.ConnectedSystems.Count / 7f) * 0.5f;

			//Debug.Log(string.Format("Connected systems: {0}, changed state of {1} with a modifier of {2} to {3}",
			//	data.ConnectedSystems.Count,
			//	data.EconomyState, economicStateModifier, data.EconomyState *= economicStateModifier));
			data.EconomyState *= economicStateModifier;
			data.AddStations();
		}
	}

	private void DrawStarMap() {

		Image backdropImage = GameObject.FindGameObjectWithTag("Backdrop").GetComponent<Image>();
		Color[] pixels = ((Texture2D)backdropImage.mainTexture).GetPixels();
		List<Color> colors = new List<Color>();

		for (int i = 0; i < pixels.Length; i++) {
			if (pixels[i].maxColorComponent > .9f && pixels[i].g < .9f) {
				pixels[i].a = 1f;
				colors.Add(pixels[i]);
			} else {
				pixels[i] = Color.black;
			}
		}

		StarSystemMapPoint mapPoint;
		foreach (var kvPair in StarSystemData.StartSystemMapTable) {
			GameObject go = (GameObject)Instantiate(_starPrefab, kvPair.Value.GetPosition(), Quaternion.identity);;
			go.transform.SetParent(StarCluster);
			mapPoint = go.GetComponent<StarSystemMapPoint>();
			mapPoint.ID = kvPair.Value.ID;
			mapPoint.SystemName.text = kvPair.Value.Name;
			go.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Count)];
			go.name = kvPair.Value.ID.ToString();
			float scale = Random.Range(0.85f, 1.15f);
			go.transform.localScale = new Vector3(scale, scale, 1);
		}

	}

	private int GeneratePopulation(int population) {

		int add = Random.Range(1000, 1000000);
		population += add;
		
		if (add > 500000) {
			population += GeneratePopulation(population);
		}

		return population;
	}
}
