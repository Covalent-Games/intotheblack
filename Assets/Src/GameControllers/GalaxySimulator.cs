using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class GalaxySimulator : MonoBehaviour {

	internal static List<HostilityGrowthObject> HostilityExpansions = new List<HostilityGrowthObject>();
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

		public Guid StarSystemTo;
		public Guid StarSystemFrom;
		public float ChangeAmount;

		public HostilityGrowthObject(Guid systemTo, Guid systemFrom, float amount) {
			StarSystemTo = systemTo;
			StarSystemFrom = systemFrom;
			ChangeAmount = amount;
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
				foreach(HostilityGrowthObject growth in HostilityExpansions) {
					ExpansionDot dot = Instantiate(HostilityExpansionDotPrefab);
					dot.To = StarSystemData.StartSystemMapTable[growth.StarSystemTo].GetPosition();
					dot.transform.position = StarSystemData.StartSystemMapTable[growth.StarSystemFrom].GetPosition();
				}
			}
			yield return null; 
		}
	}

	public void UpdateGalaxy() {

		if (HostilityExpansions.Count > 0) {
			ExpandHostility(HostilityExpansions);
		}
		// Keep the enemy base at 100% hostility to feed the other systems.
		StarSystemData.EnemyHome.Hostility = 1f;
		Debug.Log("Set the EnemyHome hostility to 1");
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

	private void ExpandHostility(List<HostilityGrowthObject> hostilityExpansions) {

		//TODO: Check that the resources are still available before changing. For instance, if the player
		// reduces the "from" system's hostility they should thwart the expansion. 
		for (int i = 0; i < hostilityExpansions.Count; i++) {
			StarSystemData.StartSystemMapTable[hostilityExpansions[i].StarSystemTo].Hostility
				+= hostilityExpansions[i].ChangeAmount;
			StarSystemData.StartSystemMapTable[hostilityExpansions[i].StarSystemFrom].Hostility
				-= hostilityExpansions[i].ChangeAmount;
			//TODO: This is a bad way to do it, but the "good" way isn't working, and it's really only bad
			// out of principle.
			if (StarSystemData.StartSystemMapTable[hostilityExpansions[i].StarSystemFrom].IsEnemyHome) {
				Debug.Log("Setting enemy base to 1 hostility \"the bad way\".");
				StarSystemData.StartSystemMapTable[hostilityExpansions[i].StarSystemFrom].Hostility = 1f;
			}
		}
		hostilityExpansions.Clear();
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

			//TODO: Expand to multiple systems if possible.
			//StarSystemData selectedSystem = StarSystemData.StartSystemMapTable[systemList[Random.Range(0, systemList.Count)]];
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

			HostilityExpansions.Add(new HostilityGrowthObject(selectedSystem.ID, system.ID, expansionValue));
		}
	}
}
