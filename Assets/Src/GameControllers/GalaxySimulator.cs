using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class GalaxySimulator : MonoBehaviour {

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
			LowerForces(system);
			if (system.Hostility > HostilityAdvanceThreshold) {
				UpdateHostility(system); 
			}
			if (system.Hostility > 0) {
				SetHostilityIndicators(system);
			}
		}
		// Keep the enemy base at 100% hostility to feed the other systems.
		StarSystemData.EnemyHome.Hostility = 1f;
		Debug.Log("Set the EnemyHome hostility to 1");
	}

	private void SetHostilityIndicators(StarSystemData system) {

		if (system.Hostility > system.SystemDefense) {
			//A red star means the system is falling, or has fallen.
			Instantiate(_redStarPrefab, system.GetPosition(), Quaternion.identity);
		} else if (system.Hostility < system.SystemDefense) {
			// A yellow star means the system is under attack, but not yet at risk of falling.
			Instantiate(_yellowStarPrefab, system.GetPosition(), Quaternion.identity);
		}
	}

	private void LowerForces(StarSystemData system) {

		float hostility = system.Hostility;
		float defense = system.SystemDefense;
		// Lower each force by 8-12% of the opposing forces starting value
		system.Hostility -= defense * (_forceReductionScale + Random.Range(-0.02f, 0.02f));
		system.SystemDefense -= hostility * (_forceReductionScale + Random.Range(-0.02f, 0.02f));
	}

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

			// Get a list of systems with hostility lower than the local system.Hostility.
			List<Guid> systemList = system.ConnectedSystems
				.Where(s => StarSystemData.StartSystemMapTable[s].Hostility < system.Hostility).ToList();

			// If there were no elligible systems, return.
			if (systemList.Count < 1) { return; }

			//TODO: Expand to multiple systems if possible, but don't overlap.
			StarSystemData selectedSystem = null;
			StarSystemData comparedSystem = null;
			for (int i = 0; i < system.ConnectedSystems.Count; i++) {
				comparedSystem = StarSystemData.GetSystem(system.ConnectedSystems[i]);
				//Find a system that would be placed in an advantageous position with this system's enemy's spare resources.
				if (comparedSystem.SystemDefense - comparedSystem.Hostility > 0) {
					if (comparedSystem.SystemDefense - comparedSystem.Hostility <= availableResources) {
						selectedSystem = comparedSystem;
						Debug.Log(selectedSystem.Name + " is getting vital backup!");
						break;
					}
				}
			}
			//If we're here, then no nearby systems would *immediately* benefit from advancement.
			//With this logic the enemy is "playing it safe".
			if (selectedSystem == null) {
				//At this point the enemy should still advance, so just pick a random system.
				selectedSystem = StarSystemData.GetSystem(systemList[Random.Range(0, systemList.Count)]);
			}
			//TODO: Divided by number of expansions from this system
			float expansionValue = (availableResources - selectedSystem.Hostility);
			HostilityGrowthObject growth = new HostilityGrowthObject(system.ID);
			growth.ChangeList.Add(selectedSystem.ID, expansionValue);

			HostilityExpansions.Add(system.ID, growth);
		}
	}
}
