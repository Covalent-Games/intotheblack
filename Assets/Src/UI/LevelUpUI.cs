using UnityEngine;
using UnityEngine.Extensions;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour {

	private PauseMenuController _pauseMenuController;
	private Text _upgradePointsRemaining;

	private void Awake() {

		_pauseMenuController = GetComponent<PauseMenuController>();
		_upgradePointsRemaining = transform.FindChildRecursive("PointsRemaining_Text").GetComponent<Text>();
	}

	private void Update() {
		_upgradePointsRemaining.text = PlayerData.State.UpgradePoints.ToString();
	}

	private void CloseIfApplicable() {

		if (PlayerData.State.UpgradePoints == 0) {
			_pauseMenuController.CloseLevelUpPanel();
			enabled = false;
		}
	}

	public void LevelUpGuns() {

		ShipWeapons weps = GameManager.Player.GetComponent<ShipWeapons>();

		if (weps.RateOfFire < 8) {
			weps.RateOfFire += 1;
			PlayerData.State.UpgradePoints--; 
		}
		CloseIfApplicable();
	}

	public void LevelUpTorpedoes() {

		if (GameManager.Player.GetComponent<ShipWeapons>().TorpedoFireRate < 4) {
			GameManager.Player.GetComponent<ShipWeapons>().TorpedoFireRate++;
			PlayerData.State.UpgradePoints--;
		}
		CloseIfApplicable();
	}

	public void LevelUpHealth() {

		Destructable stats = GameManager.Player.GetComponent<Destructable>();
		if (stats.MaxHealth < 100) {
			stats.Sethealth(stats.MaxHealth + 10);
			PlayerData.State.UpgradePoints--; 
		}
		CloseIfApplicable();
	}

	public void LevelUpFunctions() {

		ShipWeapons weps = GameManager.Player.GetComponent<ShipWeapons>();
		if (weps.TorpedoCooldown > 8) {
			ShipController engines = GameManager.Player.GetComponent<ShipController>();

			weps.TorpedoCooldown -= 2;
			weps.ShieldCooldown -= 2;
			engines.BoostCooldown -= 1;

			PlayerData.State.UpgradePoints--;
		}
		CloseIfApplicable();
	}
}
