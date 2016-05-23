using UnityEngine;
using UnityEngine.Extensions;
using System.Collections;
using UnityEngine.UI;

public class StarSystemInfoDisplay : MonoBehaviour {

	private Text _systemName;
	private Text _hostilityText;
	private Image _hostilityBar;
	private Text _economyText;
	private Image _economyBar;
	private Text _population;
	private Image _systemDefense;
	private Text _hostilityPercentage;
	private Text _systemDefensePercentage;
	private Text _prosperityPercentage;

	private void Awake() {

		_systemName = transform.FindChildRecursive("SystemName_Text").GetComponent<Text>();
		_hostilityText = transform.FindChildRecursive("Hostility_Text").GetComponent<Text>();
		_hostilityBar = transform.FindChildRecursive("HostilityBar_Image").GetComponent<Image>();
		_economyText = transform.FindChildRecursive("Prosperity_Text").GetComponent<Text>();
		_economyBar = transform.FindChildRecursive("ProsperityBar_Image").GetComponent<Image>();
		_population = transform.FindChildRecursive("SystemPopulation_Text").GetComponent<Text>();
		_systemDefense = transform.FindChildRecursive("SystemDefenseBar_Image").GetComponent<Image>();
		_hostilityPercentage = transform.FindChildRecursive("HostilityPercentage_Text").GetComponent<Text>();
		_systemDefensePercentage = transform.FindChildRecursive("SystemDefensePercentage_Text").GetComponent<Text>();
		_prosperityPercentage = transform.FindChildRecursive("ProsperityPercentage_Text").GetComponent<Text>();
	}

	public void SetTargettedSystemInfo(StarSystemData starSystem) {

		_systemName.text = starSystem.Name;
		_population.text = starSystem.Population.ToString("N0");
		_hostilityBar.fillAmount = starSystem.Hostility;
		_economyBar.fillAmount = starSystem.Prosperity;
		_systemDefense.fillAmount = starSystem.SystemDefense;
		_hostilityPercentage.text = Mathf.RoundToInt(starSystem.Hostility * 100).ToString() + "%";
		_systemDefensePercentage.text = Mathf.RoundToInt(starSystem.SystemDefense * 100).ToString() + "%";
		_prosperityPercentage.text = Mathf.RoundToInt(starSystem.Prosperity * 100).ToString() + "%";


		if (starSystem.Hostility > .75f) {
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

		// TODO: There should be a label for each level of defense ship types so there's a clear
		// indicator of what quality of ships will be seen in the system.
		if (starSystem.Prosperity > 0.75f) {
			_economyText.text = "Booming";
		} else if (starSystem.Prosperity > .5f) {
			_economyText.text = "Thriving";
		} else if (starSystem.Prosperity > .25f) {
			_economyText.text = "Surviving";
		} else if (starSystem.Prosperity != 0f) {
			_economyText.text = "Struggling";
		} else {
			_economyText.text = "Total poverty";
		}
	}
}
