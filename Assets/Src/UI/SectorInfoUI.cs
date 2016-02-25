using UnityEngine;
using UnityEngine.Extensions;
using System.Collections;
using UnityEngine.UI;
using System;

public class SectorInfoUI : MonoBehaviour {

	private Image _totalHostility;
	private Text _totalHostilityPercent;

	private void Awake() {

		_totalHostility = transform.FindChildRecursive("TotalHostility_Image").GetComponent<Image>();
		_totalHostilityPercent = transform.FindChildRecursive("TotalHostilityPercent_Text").GetComponent<Text>();
	}

	private void Start() {

		UpdateHostility();
	}

	public void UpdateHostility() {

		float hostility = StarSystemData.GetTotalHostilityRating() / StarSystemData.StartSystemMapTable.Count;
		_totalHostility.fillAmount = hostility;
		_totalHostilityPercent.text = string.Format("{0}%", Mathf.RoundToInt(hostility * 100));
	}
}
