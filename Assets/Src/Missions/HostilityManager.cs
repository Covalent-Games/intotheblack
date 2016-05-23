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
			// Don't bother increasing systems that are already very hostile
			if (kvPair.Value.Hostility > 0.85) {
				continue;
			}

			float nearbyHostility = 0f;
			foreach (Guid systemID in kvPair.Value.ConnectedSystems) {
				nearbyHostility += StarSystemData.StartSystemMapTable[systemID].Hostility;
			}

			float threshold = Mathf.Clamp(
				Mathf.Sqrt(kvPair.Value.Population / 750000f * kvPair.Value.ConnectedSystems.Count),
				0f,
				kvPair.Value.ConnectedSystems.Count * 0.9f);
			//Debug.Log(string.Format("{0}: Neighboring Hostility: {1}, Threshold: {2}",
			//	kvPair.Value.Name, nearbyHostility, threshold));
			if (nearbyHostility >= threshold) {
				SystemsUnderAttack.Add(new HostilityGrowthObject(kvPair.Value, nearbyHostility * 0.05f));
				
			}
		}
	}
}
