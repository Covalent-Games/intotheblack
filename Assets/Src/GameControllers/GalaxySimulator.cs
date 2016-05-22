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

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("Hostility of system required to expand to an adjacent system")]
	private float _hostilityAdvanceThreshold;
	[SerializeField]
	private int MaxSystemPopulation = 1000000000; // 1 Billion
	[SerializeField]
	private GameObject _redStarPrefab;

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
			if (system.Hostility > HostilityAdvanceThreshold) {
				UpdateHostility(system); 
			}
		}
	}

	private void ExpandHostility(List<HostilityGrowthObject> hostilityExpansions) {

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

		//TODO: Check if the system defense is low enough to expand out.

		// Get a list of systems with hostility lower than the local system.Hostility.
		List<Guid> systemList = system.ConnectedSystems
			.Where(s => StarSystemData.StartSystemMapTable[s].Hostility < system.Hostility).ToList();

		if (systemList.Count < 1) {	return;	}

		// Randomly choose ID from systemList and getStarSystemData from the table.
		StarSystemData selectedSystem = StarSystemData.StartSystemMapTable[systemList[Random.Range(0, systemList.Count)]];
		float expansionValue = (system.Hostility - selectedSystem.Hostility) * 0.5f;

		HostilityExpansions.Add(new HostilityGrowthObject(selectedSystem.ID, system.ID, expansionValue));

		// Create a red star indicating it's being invaded.
		Instantiate(_redStarPrefab, selectedSystem.GetPosition(), Quaternion.identity);

	}
}
