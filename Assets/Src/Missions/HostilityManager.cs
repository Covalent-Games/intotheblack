using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class HostilityManager : MonoBehaviour {

	private List<HostilityGrowthObject> SystemsUnderAttack = new List<HostilityGrowthObject>();
	[SerializeField]
	private GameObject _redStarPrefab;

	private class HostilityGrowthObject {

		public StarSystemData StarSystem;
		public float IncreaseAmount;

		public HostilityGrowthObject(StarSystemData system, float amount) {
			StarSystem = system;
			IncreaseAmount = amount;
		}
	}

	public void UpdateSystemHostility() {

		if (SystemsUnderAttack.Count == 0) {
			AssignNewAttackableSystems();
		}

		if (GameStateData.State.ExpandHostility) {
			for (int i = 0; i < SystemsUnderAttack.Count; i++) {
				SystemsUnderAttack[i].StarSystem.Hostility += SystemsUnderAttack[i].IncreaseAmount;
			}
			Debug.Log("The hostility in the sector has grown!");
			GameStateData.State.ExpandHostility = false; 
		}

		SystemsUnderAttack.Clear();

		AssignNewAttackableSystems();
		for (int i = 0; i < SystemsUnderAttack.Count; i++) {
			Instantiate(_redStarPrefab, SystemsUnderAttack[i].StarSystem.GetPosition(), Quaternion.identity); 
		}
	}

	private void AssignNewAttackableSystems() {

		foreach (var kvPair in StarSystemData.StartSystemMapTable) {
			float nearbyHostility = 0f;
			StarSystemData connectedSystem;
			foreach (Guid systemID in kvPair.Value.ConnectedSystems) {
				connectedSystem = StarSystemData.StartSystemMapTable[systemID];
				nearbyHostility += connectedSystem.Hostility;
			}
			float threshold = Mathf.Sqrt(kvPair.Value.Population * 0.0000005f) * kvPair.Value.ConnectedSystems.Count;
			//Debug.Log(string.Format("{0}: Neighboring Hostility: {1}, Threshold: {2}",
			//	kvPair.Value.Name, nearbyHostility, threshold));
			if (nearbyHostility >= threshold) {
				SystemsUnderAttack.Add(new HostilityGrowthObject(kvPair.Value, nearbyHostility * 0.05f));
				
			}
		}
	}
}
