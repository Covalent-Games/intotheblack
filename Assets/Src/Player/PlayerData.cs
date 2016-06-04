using System;
using Covalent.Data;
using UnityEngine;

[Serializable]
public class PlayerData {

	private int _playerLevel;
	public int PlayerLevel {
		get {
			return _playerLevel;
		}
		set {
			// TODO: Make this part configurable
			//if (GameManager.Instance.AwardUpgradePointsOnLevelUp)
			_upgradePoints += value - _playerLevel;
			_playerLevel = value;
		}
	}

	private int _currentPlayerExperience;
	public int CurrentPlayerExperience {
		get {
			return _currentPlayerExperience;
		}
		set {
			_currentPlayerExperience = value;
			if (_currentPlayerExperience > GetExperienceNeededForLevel(_playerLevel + 1))
				LevelUp();
		}
	}

	private int _experienceToLevelUp;
	public int ExperienceToLevelUp {
		get {
			return _experienceToLevelUp;
		}
		set {
			_experienceToLevelUp = value;
		}
	}

	private int _playerScore;
	public int PlayerScore {
		get {
			return _playerScore;
		}
		set {
			_playerScore = value;
		}
	}

	private int _upgradePoints;
	public int UpgradePoints {
		get {
			return _upgradePoints;
		}
		set {
			_upgradePoints = value;
		}
	}

	private int _currentHealth;
	public int CurrentHealth {
		get {
			return _currentHealth;
		}
		set {
			_currentHealth = value;
		}
	}

	private int _maxHealth;
	public int MaxHealth {
		get {
			return _maxHealth;
		}
		set {
			_maxHealth = value;
		}
	}

	public Component PrimaryWeaponComponent;
	public Component SecondaryWeaponComponent;
	public Component ShieldComponent;
	public Component HullComponent;
	public Component EngineComponent;

	private int _startingLevel = 1;
	private int _startingExperience = 0;
	private int _startingScore = 0;
	private int _startingUpgradePoints = 0;
	private int _startingHealth = 50;

	private static PlayerData _state;
	public static PlayerData State {
		get {
			if (_state == null) {
				_state = new PlayerData();
			}
			return _state;
		}
		set {
			_state = value;
		}
	}

	public PlayerData() {

		this.ResetPlayerData();
	}

	private void LevelUp() {

		int remaining = State.CurrentPlayerExperience - State.ExperienceToLevelUp;
		State.CurrentPlayerExperience = remaining;
		State.PlayerLevel++;
		ExperienceToLevelUp = GetExperienceNeededForLevel(PlayerLevel + 1);
		GameManager.Instance.OverlayUI.DisplayMessage(GameManager.Instance.LevelUpMessage);
	}

	public void ResetPlayerData() {

		this.PlayerLevel = this._startingLevel;
		this.CurrentPlayerExperience = this._startingExperience;
		this.PlayerScore = this._startingScore;
		this.UpgradePoints = this._startingUpgradePoints;
		this.ExperienceToLevelUp = GetExperienceNeededForLevel(this.PlayerLevel + 1);
		this.MaxHealth = this._startingHealth;
		this.CurrentHealth = this.MaxHealth;
	}

	public int GetExperienceNeededForLevel(int level) {

		// TODO: Make these configurable in some game settings class
		float modifier = 1.5f;// GameManager.Instance.NextLevelExperienceMultiplier;
		int baseExperience = 100;// GameManager.Instance.BaseExperienceNeeded;

		if (level <= 1) {
			return 0;
		} else if (level == 2)
			return baseExperience;

		return Mathf.RoundToInt((level - 2) * modifier * baseExperience);
	}

	public static void Save() {
		if (_state == null)
			_state = new PlayerData();
		DataSerializer.SerializeData(_state, FilePaths.PlayerStatePath);
	}

	public static PlayerData Load() {

		_state = DataSerializer.DeserializeData<PlayerData>(FilePaths.PlayerStatePath);
		if (_state == null)
			_state = new PlayerData();
		return _state;
	}

}
