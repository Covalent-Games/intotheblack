using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class GalaxySimulator : MonoBehaviour {

	/// <summary>
	/// Key: Id of system to expand from. Value: Object representing systems expanded to.
	/// </summary>
	internal static Dictionary<Guid, HostilityGrowthObject> HostilityExpansions = new Dictionary<Guid, HostilityGrowthObject>();
	public float HostilityAdvanceThreshold {
		get {
			return _hostilityAdvanceThreshold;
		}
		private set {
			_hostilityAdvanceThreshold = Mathf.Clamp(value, 0f, 1f);
		}
	}
	public ExpansionDot HostilityExpansionDotPrefab;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("Hostility of system required to expand to an adjacent system")]
	private float _hostilityAdvanceThreshold;
	[SerializeField]
	private int MaxSystemPopulation = 1000000000; // 1 Billion
	[SerializeField]
	private GameObject _redStarPrefab;
	[SerializeField]
	private GameObject _yellowStarPrefab;

	[SerializeField]
	private float _forceReductionScale = 0.1f;

	internal class HostilityGrowthObject {

		public Dictionary<Guid, float> ChangeList;
		public Guid StarSystemFrom;

		public HostilityGrowthObject(Guid systemFrom) {
			StarSystemFrom = systemFrom;
			ChangeList = new Dictionary<Guid, float>();
		}
	}

	private void Awake() {

		HostilityExpansionDotPrefab = Resources.Load<ExpansionDot>("Prefabs/HostilityExpansionDot");
	}

	private IEnumerator Start() {

		float delay = 2.5f;
		float timeSnapshot = Time.time;
		while (enabled) {
			if (Time.time - timeSnapshot >= delay) {
				timeSnapshot = Time.time;
				foreach (var kvPair in HostilityExpansions) {
					foreach (var growthPair in kvPair.Value.ChangeList) {
						ExpansionDot dot = Instantiate(HostilityExpansionDotPrefab);
						dot.To = StarSystemData.StartSystemMapTable[growthPair.Key].GetPosition();
						dot.transform.position = StarSystemData.StartSystemMapTable[kvPair.Key].GetPosition();
					}
				}
			}
			yield return null; 
		}
	}

	public void UpdateGalaxy() {

		if (HostilityExpansions.Count > 0) {
			ExpandHostility();
		}
		StarSystemData system;
		foreach (var kvPair in StarSystemData.StartSystemMapTable) {
			system = kvPair.Value;
			LowerForcesPostTurn(system);
			if (system.Hostility > HostilityAdvanceThreshold | (system.Hostility > 0 && system.SystemDefense == 0)) {
				UpdateHostility(system); 
			}
			if (system.Hostility > 0) {
				SetHostilityIndicators(system);
			}
		}
		// Keep the enemy base at 100% hostility to feed the other systems.
		StarSystemData.EnemyHome.Hostility = 1f;
	}

	/// <summary>
	/// Set colored stars behind system stars to indicate their state in the war.
	/// </summary>
	/// <param name="system"></param>
	private void SetHostilityIndicators(StarSystemData system) {

		if (system.Hostility > system.SystemDefense) {
			//A red star means the system is falling, or has fallen.
			Instantiate(_redStarPrefab, system.GetPosition(), Quaternion.identity);
		} else if (system.Hostility < system.SystemDefense) {
			// A yellow star means the system is under attack, but not yet at risk of falling.
			Instantiate(_yellowStarPrefab, system.GetPosition(), Quaternion.identity);
		}
	}

	/// <summary>
	/// Lowers both system forces and hostility by a weighted 10%.
	/// </summary>
	/// <param name="system"></param>
	private void LowerForcesPostTurn(StarSystemData system) {

		float hostility = system.Hostility;
		float defense = system.SystemDefense;
		// Lower each force by 8-12% of the opposing forces starting value
		system.Hostility -= defense * (_forceReductionScale + Random.Range(-0.02f, 0.02f));
		system.SystemDefense -= hostility * (_forceReductionScale + Random.Range(-0.02f, 0.02f));
	}

	/// <summary>
	/// Expands hostility if any HostilityGrowthObjects are present
	/// </summary>
	private void ExpandHostility() {

		//TODO: Check that the resources are still available before changing. For instance, if the player
		// reduces the "from" system's hostility they should thwart the expansion. 
		StarSystemData fromSystem;
		foreach (var kvPair in HostilityExpansions) {
			fromSystem = StarSystemData.StartSystemMapTable[kvPair.Key];
			foreach (var growthPair in kvPair.Value.ChangeList) {
				StarSystemData.StartSystemMapTable[growthPair.Key].Hostility += growthPair.Value;
				fromSystem.Hostility -= growthPair.Value;
			}

			if (fromSystem.IsEnemyHome) {
				fromSystem.Hostility = 1f;
			}
		}
		HostilityExpansions = new Dictionary<Guid, HostilityGrowthObject>();
	}

	private void UpdateHostility(StarSystemData system) {

		//Only expand if the enemy has available resources.
		if (system.Hostility > system.SystemDefense) {
			float availableResources = system.Hostility - system.SystemDefense;

			Dictionary<StarSystemData, int> priorityTable = new Dictionary<StarSystemData, int>();
			foreach (Guid systemId in system.ConnectedSystems) {
				if (StarSystemData.StartSystemMapTable[systemId].Hostility < system.Hostility) {
					priorityTable.Add(StarSystemData.StartSystemMapTable[systemId], 0);
				}
			}

			// Get a list of systems with hostility lower than the local system.Hostility.
			//List<Guid> systemList = system.ConnectedSystems
			//	.Where(s => StarSystemData.StartSystemMapTable[s].Hostility < system.Hostility).ToList();

			// If there were no elligible systems, return.
			if (priorityTable.Count < 1) { return; }

			//TODO: Expand to multiple systems if possible, but don't overlap.
			StarSystemData selectedSystem = null;
			StarSystemData comparedSystem = null;
			List<StarSystemData> systemPriorityKeys = new List<StarSystemData>(priorityTable.Keys);
			//Find a system that would be placed in an advantageous position with this system's enemy's spare resources.
			foreach (StarSystemData systemPriorityKey in systemPriorityKeys) {
				comparedSystem = systemPriorityKey;
				// Zero war activity and the system is conquered. 
				if (comparedSystem.SystemDefense == 0 && comparedSystem.Hostility == 0) {
					priorityTable[systemPriorityKey] -= 1;
				}
				// Zero war activity and the system has never been invaded.
				if (comparedSystem.SystemDefense == 1f && comparedSystem.Hostility == 0f) {
					priorityTable[systemPriorityKey] -= 1;
				}
				//Check if the neighboring system forces are 'winning'.
				if (comparedSystem.SystemDefense - comparedSystem.Hostility > 0) {
					priorityTable[systemPriorityKey] += 2;
					//Check if this system's hostility resources can bring the neighboring hostility ahead.
					if (comparedSystem.SystemDefense - comparedSystem.Hostility <= availableResources) {
						priorityTable[systemPriorityKey] += 3;
					}
				}
				foreach (var expandingFromPair in HostilityExpansions) {
					////If true, we can backfill an expanding system.
					//if (expandingFromPair.Key == comparedSystem.ID) {
					//	float expansionAmount = expandingFromPair.Value.ChangeList.Values.Sum();
					//	if (expansionAmount <= availableResources) {
					//		HostilityGrowthObject backfill = new HostilityGrowthObject(system.ID);
					//		backfill.ChangeList.Add(expandingFromPair.Key, availableResources - expansionAmount);
					//		HostilityExpansions.Add(system.ID, backfill);
					//		//TODO: This WHOLE block of logic needs to be ran first as the reduction of
					//		// resources COMPLETELY ruins the priority system. D:
					//		availableResources -= expansionAmount;
					//	}
					//}
					foreach (var growthPair in expandingFromPair.Value.ChangeList) {
						// If true, then this system already has backup on the way.
						if (comparedSystem.ID == growthPair.Key) {
							priorityTable[systemPriorityKey] -= 2;
						}
					}
				}
			}
			KeyValuePair<StarSystemData, int> highestPriority = priorityTable.First();
			foreach (var systemPriorityPair in priorityTable) {
				if (systemPriorityPair.Value > highestPriority.Value) {
					highestPriority = systemPriorityPair;
				}
			}
			selectedSystem = highestPriority.Key;
			Debug.Log(system.Name + " ===============");
			foreach(var kvp in priorityTable) {
				Debug.Log(kvp.Key.Name + " " + kvp.Value);
			}
			Debug.Log("SELECTED: " + selectedSystem.Name);
			//If we're here, then no nearby systems would *immediately* benefit from advancement.
			//With this logic the enemy is "playing it safe".
			if (selectedSystem == null) {
				//TODO: This is flawed logic in practice and should get phased out. Logically if there's no
				// good place to expand to, don't.
				//At this point the enemy should still advance, so just pick a random system.
				selectedSystem = priorityTable.Keys.ToArray()[Random.Range(0, priorityTable.Count)];
			}
			//TODO: Divided by number of expansions from this system
			float expansionValue = (availableResources - selectedSystem.Hostility);
			HostilityGrowthObject growth = new HostilityGrowthObject(system.ID);
			growth.ChangeList.Add(selectedSystem.ID, expansionValue);

			HostilityExpansions.Add(system.ID, growth);
		}
	}
}
