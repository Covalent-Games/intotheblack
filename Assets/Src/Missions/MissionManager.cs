using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MissionManager : MonoBehaviour {

	public MissionObject CurrentSystemMission;
	public MissionPanelUI MissionPanelUI;

	private Guid CurrentSystemID = Guid.Empty;
	private WhatMissionParameter WhatMissionType;

	private void Awake() {

		MissionPanelUI = GetComponent<MissionPanelUI>();
	}

	public void OpenMissionPanel() {

		// IF we don't have a mission or we just changed systems generate a new mission.
		if (CurrentSystemID == Guid.Empty || StarMapSceneManager.SystemSelected.ID != CurrentSystemID) {
			CurrentSystemMission = GenerateNewMission(StarMapSceneManager.SystemSelected);
		}

		MissionPanelUI.SetUI(CurrentSystemMission);

		gameObject.SetActive(true);
	}

	private MissionObject GenerateNewMission(StarSystemData systemSelected) {

		CurrentSystemID = systemSelected.ID;
		MissionObject missionObject = new MissionObject();
		WhatMissionType = new WhatMissionParameter("Rescue");

		missionObject.Description = string.Format("{0} {1} {2} {3}",
			WhatMissionType.What,
			WhatMissionType.Who.Who,
			StarSystemData.StartSystemMapTable[WhatMissionType.Who.Where.Where].Name,
			WhatMissionType.Who.Where.Why.Why);

		return missionObject;
	}

	public void CloseMissionPanel() {

		gameObject.SetActive(false);
	}
}
