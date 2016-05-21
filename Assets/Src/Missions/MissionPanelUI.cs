using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Extensions;
using System;

public class MissionPanelUI : MonoBehaviour {

	public Text MissionDescriptionText { get; private set; }
	public Text MissionGiverName { get; private set; }
	public Text MissionTitleText { get; private set; }

	private void Awake() {

		MissionGiverName = transform.FindChildRecursive("MissionGivername_Text").GetComponent<Text>();
		MissionTitleText = transform.FindChildRecursive("MissionTitle_Text").GetComponent<Text>();
		MissionDescriptionText = transform.FindChildRecursive("MissionDescription_Text").GetComponent<Text>();
	}

	internal void SetUI(MissionObject currentSystemMission) {

		MissionDescriptionText.text = currentSystemMission.Description;
	}
}
