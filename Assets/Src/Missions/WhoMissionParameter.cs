using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WhoMissionParameter {

	public string Who;
	public WhereMissionParameter Where;

	public WhoMissionParameter(string who) {

		Who = who;

		// Try to pick a nearby system using a count including the current system. If the +1 is picked, choose
		// the current system.
		try {
			Where = new WhereMissionParameter(StarMapSceneManager
				.SystemSelected
				.ConnectedSystems[Random.Range(0, StarMapSceneManager.SystemSelected.ConnectedSystems.Count + 1)]);
		} catch (System.ArgumentOutOfRangeException) {
			Where = new WhereMissionParameter(StarMapSceneManager.SystemSelected.ID);
		}

	}
}
