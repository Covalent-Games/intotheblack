using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Covalent.Data;

[Serializable]
public class StarSystemData {

	public static Dictionary<Guid, StarSystemData> StartSystemMapTable = new Dictionary<Guid, StarSystemData>();

	private static StarSystemData _starSystemLoaded;
	public static StarSystemData StarSystemLoaded {
		get {
			return _starSystemLoaded;
		}
		set {
			_starSystemLoaded = value;
			GameStateData.State.PlayerOccupiedSystem = _starSystemLoaded.ID;
		}
	}

	private static StarSystemData _home;
	public static StarSystemData PlayerHome {
		get	{
			if (_home == null) {
				_home = FindPlayerHome();
			}
			return _home;
		}
	}
	private static StarSystemData _enemyHome;
	public static StarSystemData EnemyHome {
		get	{
			if (_enemyHome == null) {
				_enemyHome = FindEnemyHome();
			}
			return _enemyHome;
		}
	}
	private float _prosperity;
	public float Prosperity {
		get {
			return _prosperity;
		} 
		set {
			_prosperity = Mathf.Clamp(value, 0f, 1f);
		}
	}
	private float _hostility;
	public float Hostility {
		get {
			return _hostility;
		}
		set	{
			_hostility = Mathf.Clamp(value, 0f, 1f);
		}
	}
	private float _systemDefense;
	public float SystemDefense {
		get {
			return _systemDefense;
		}
		set {
			_systemDefense = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public Guid ID;
	public bool IsPlayerHome = false;
	public bool IsEnemyHome = false;
	public string Name;
	public List<Guid> ConnectedSystems = new List<Guid>();
	public int Population;
	public float PositionX;
	public float PositionY;
	private SpaceStationData _localSpaceStation;
	public SpaceStationData LocalSpaceStation {
		get {
			if (_localSpaceStation == null) {
				_localSpaceStation = new SpaceStationData();
				Save();
			}
			return _localSpaceStation;
		}
	}

	public static StarSystemData GetSystem(Guid id) {

		return StarSystemData.StartSystemMapTable[id];
	}

	public StarSystemData(string name, float economy, Vector3 position) {

		if (ID == Guid.Empty) {
			ID = Guid.NewGuid();
		}

		Name = name;
		Prosperity = economy;
		PositionX = position.x;
		PositionY = position.y;

		StartSystemMapTable.Add(ID, this);
	}

	public void InstantiateStation() {

		UnityEngine.Object prefab = Resources.Load("Prefabs/SpaceStation");
		if (prefab == null) {
			Debug.LogError("Prefab is null");
			Debug.Break();
		}
		GameObject station = (GameObject)GameObject.Instantiate(prefab, LocalSpaceStation.Position, Quaternion.identity);
		LocalSpaceStation.StationTransform = station.transform;
	}

	public static float GetTotalHostilityRating() {

		float hostility = 0f;

		foreach (var kvPair in StartSystemMapTable) {
			hostility += kvPair.Value.Hostility;
		}

		return hostility;
	}

	public static void Save() {

		DataSerializer.SerializeData(StartSystemMapTable, FilePaths.StarMapPath);
	}

	public static bool Load() {

		var map = DataSerializer.DeserializeData<Dictionary<Guid, StarSystemData>>(FilePaths.StarMapPath);
		if (map != default(Dictionary<Guid, StarSystemData>)) {
			StartSystemMapTable = map;
			Debug.Log("StarSystemMapTable loaded!");
			return true;
		} else {
			return false;
		}
	}


	private static StarSystemData FindPlayerHome() {

		StarSystemData home = null;

		foreach (StarSystemData data in StartSystemMapTable.Values) {
			if (data.IsPlayerHome) {
				home = data;
			}
		}
		return home;
	}

	private static StarSystemData FindEnemyHome() {

		StarSystemData enemyHome = null;

		foreach (StarSystemData data in StartSystemMapTable.Values) {
			if (data.IsEnemyHome) {
				enemyHome = data;
			}
		}
		return enemyHome;
	}

	public Vector3 GetPosition() {

		return new Vector3(PositionX, PositionY, 0f);
	}
}
