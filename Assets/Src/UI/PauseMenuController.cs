using UnityEngine;
using UnityEngine.Extensions;

public class PauseMenuController : MonoBehaviour {

	private GameObject _pauseMenu;
	private GameObject _levelUpEnableButton;
	private GameObject _levelUpPanel;
	private LevelUpUI _levelUpUi;

	private void Awake() {

		_pauseMenu = transform.FindChildRecursive("PauseMenu_Panel").gameObject;
		_pauseMenu.SetActive(false);
		_levelUpEnableButton = transform.FindChildRecursive("LevelUpPanel_Button").gameObject;
		_levelUpPanel = transform.FindChildRecursive("LevelUp_Panel").gameObject;
		_levelUpUi = GetComponent<LevelUpUI>();
	}

	private void Update() {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			TogglePauseGame(!_pauseMenu.activeSelf);
			if (_levelUpUi.enabled) {
				CloseLevelUpPanel();
			}
		}
	}

	public void EnableLevelUpPanel() {

		_levelUpPanel.SetActive(true);
		_levelUpUi.enabled = true;
		_pauseMenu.SetActive(false);
	}

	public void CloseLevelUpPanel() {

		_levelUpPanel.SetActive(false);
		_levelUpUi.enabled = false;
		_levelUpEnableButton.SetActive(false);
		_pauseMenu.SetActive(true);
	}

	public void JumpOutOfSystem() {

		//TODO: Save the game.
		Time.timeScale = 1;
		StarSystemData.SaveStarMapToFile();
		Application.LoadLevel("StarClusterMap");
	}

	public void TogglePauseGame(bool pause) {

		_pauseMenu.SetActive(pause);
		if (GameManager.Instance.UpgradePoints > 0) {
			_levelUpEnableButton.SetActive(true);
		} else {
			_levelUpEnableButton.SetActive(false);
		}
		Time.timeScale = _pauseMenu.activeSelf ? 0 : 1;
	}

	public void Quit() {

		Application.Quit();
	}

	public void Restart() {

		Application.LoadLevel(Application.loadedLevel);
		GameManager.PlayerExperience = 0;
		GameManager.ExperienceToLevel = 125;
		GameManager.PlayerLevel = 1;
		GameManager.ActiveEnemies.Clear();
	}
}
