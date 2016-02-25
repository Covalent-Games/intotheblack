using UnityEngine;
using UnityEngine.Extensions;
using System.Collections;
using UnityEngine.UI;

public class StarSystemInfoDisplay : MonoBehaviour {

	private Text _systemName;
	private StarSystemData _systemSelected;
	private Text _hostilityText;
	private Image _hostilityBar;
	private Text _economyText;
	private Image _economyBar;
	private Text _stationNumber;

	private void Awake() {

		_systemName = transform.FindChildRecursive("SystemName_Text").GetComponent<Text>();
		_hostilityText = transform.FindChildRecursive("Hostility_Text").GetComponent<Text>();
		_hostilityBar = transform.FindChildRecursive("HostilityBar_Image").GetComponent<Image>();
		_economyText = transform.FindChildRecursive("Economy_Text").GetComponent<Text>();
		_economyBar = transform.FindChildRecursive("EconomyBar_Image").GetComponent<Image>();
		_stationNumber = transform.FindChildRecursive("StationNumber_Text").GetComponent<Text>();
	}

	public void SetTargettedSystemInfo(StarSystemData starSystem) {

		_systemSelected = starSystem;

		_systemName.text = starSystem.Name;
		_hostilityBar.fillAmount = starSystem.Hostility;
		_economyBar.fillAmount = starSystem.EconomyState;
		_stationNumber.text = "Number of stations: " + starSystem.IdToStations.Count;

		if (starSystem.Hostility > .85f) {
			_hostilityText.text = "All-out war";
		} else if (starSystem.Hostility > .5f) {
			_hostilityText.text = "Large skirmishes";
		} else if (starSystem.Hostility > 0.25f) {
			_hostilityText.text = "Small skirmeshes";
		} else if (starSystem.Hostility != 0f) {
			_hostilityText.text = "Minimal fighting";			
		} else {
			_hostilityText.text = "System at peace";
		}

		if (starSystem.EconomyState > 0.85f) {
			_economyText.text = "Booming";
		} else if (starSystem.EconomyState > .5f) {
			_economyText.text = "Thriving";
		} else if (starSystem.EconomyState > .25f) {
			_economyText.text = "Surviving";
		} else if (starSystem.EconomyState != 0f) {
			_economyText.text = "Struggling";
		} else {
			_economyText.text = "Total poverty";
		}
	}

	public void JumpToSystem() {

		StarSystemData.StarSystemLoaded = _systemSelected;
		Application.LoadLevel("Scene1");
	}
}
