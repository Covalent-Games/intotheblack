using UnityEngine;
using UnityEngine.Extensions;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class OverlayUI : MonoBehaviour {

	public Text PlayerLevelText;
	public Text ObjectivesCompletedText;
	public GameManager Manager;

	private List<NavigatorArrow> _navigatorArrowList = new List<NavigatorArrow>();
	private Transform _navigator;
	private GameObject _navigatorArrow;
	private Destructable _playerStats;
	private ShipController _playerShipController;
	private ShipWeapons _shipWeapons;
	private Image _playerHealthBar;
	private Image _boostCooldown;
	private Image _torpedoCooldown;
	private Image _experienceBar;
	private Text _gameOverText;
	private Transform _objectiveArrow;
	private Image _objectiveArrowIcon;
	private NavigatorArrow _stationArrow;
	private Image _stationArrowIcon;
	private Text _notification;
	private Queue<string> _notifcationQueue = new Queue<string>();
	private Text _score;
	private Image _shieldCooldown;

	void Awake() {

		GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
		_playerStats = playerGo.GetComponent<Destructable>();
		_shipWeapons = playerGo.GetComponent<ShipWeapons>();
		_playerShipController = playerGo.GetComponent<ShipController>();
		_navigator = transform.FindChildRecursive("Navigator");
		_navigatorArrow = transform.FindChildRecursive("NavigatorArrow").gameObject;
		_playerHealthBar = transform.FindChildRecursive("PlayerHealth_Panel").GetComponent<Image>();
		_boostCooldown = transform.FindChildRecursive("BoostCooldown_Image").GetComponent<Image>();
		_torpedoCooldown = transform.FindChildRecursive("TorpedoCooldown_Image").GetComponent<Image>();
		_experienceBar = transform.FindChildRecursive("ExperienceBar_Image").GetComponent<Image>();
		PlayerLevelText = transform.FindChildRecursive("PlayerLevel_Text").GetComponent<Text>();
		_gameOverText = transform.FindChildRecursive("GameOver_Text").GetComponent<Text>();
		ObjectivesCompletedText = transform.FindChildRecursive("ObjectiveCount_Text").GetComponent<Text>();
		_objectiveArrow = transform.FindChildRecursive("ObjectiveArrow");
		_objectiveArrowIcon = _objectiveArrow.GetChild(0).GetComponent<Image>();
		_stationArrow = transform.FindChildRecursive("StationArrow").GetComponent<NavigatorArrow>();
		_stationArrowIcon = _stationArrow.transform.GetChild(0).GetComponent<Image>();
		_notification = transform.FindChildRecursive("Notification_Text").GetComponent<Text>();
		_score = transform.FindChildRecursive("Score_Text").GetComponent<Text>();
		_shieldCooldown = transform.FindChildRecursive("ShieldCooldown_Image").GetComponent<Image>();
	}

	void Start() {

		_navigatorArrowList.Add(_navigatorArrow.GetComponent<NavigatorArrow>());
		if (StarSystemData.StarSystemLoaded.LocalSpaceStation.StationTransform != null) {
			_stationArrow.GetComponent<NavigatorArrow>().TargetRenderer = StarSystemData
				.StarSystemLoaded
				.LocalSpaceStation
				.StationTransform
				.FindChild("SpaceStationRenderer")
				.GetComponent<Renderer>();
			_stationArrow.gameObject.SetActive(true);
		}
	}

	public void LinkNewArrowToEnemy(Transform entity) {

		NavigatorArrow arrow = null;
		for (int i = 0; i < _navigatorArrowList.Count; i++) {
			if (!_navigatorArrowList[i].gameObject.activeSelf) {
				arrow = _navigatorArrowList[i];
				break;
			}
		}

		if (arrow == null) {
			arrow = Instantiate(_navigatorArrow).GetComponent<NavigatorArrow>();
			_navigatorArrowList.Add(arrow);
		}

		arrow.TargetRenderer = entity.FindChild("EnemyShip_Renderer").GetComponent<Renderer>();
		arrow.Target = entity;
		arrow.gameObject.SetActive(true);
		arrow.transform.SetParent(_navigator, false);
	}

	public void DisplayGameOverText() {

		_gameOverText.text = "Pow, you're dead. Score: " + GameManager.PlayerScore;
		_gameOverText.enabled = true;
	}

	public void UpdateObjectiveText(int count) {

		ObjectivesCompletedText.text = count.ToString();
	}
	
	// Update is called once per frame
	void Update () {

		_playerHealthBar.fillAmount = (float)_playerStats.Health / _playerStats.MaxHealth;
		_boostCooldown.fillAmount = 1 -
			_playerShipController.BoostCooldownCurrent / _playerShipController.BoostCooldown;
		_torpedoCooldown.fillAmount = 1 -
			_shipWeapons.TorpedoCooldownCurrent / _shipWeapons.TorpedoCooldown;
		_shieldCooldown.fillAmount = 1 -
			_shipWeapons.ShieldCooldownCurrent / _shipWeapons.ShieldCooldown;

		_experienceBar.fillAmount =
			(float)GameManager.PlayerExperience / GameManager.ExperienceToLevel;
		PlayerLevelText.text = GameManager.PlayerLevel.ToString();
		_score.text = GameManager.PlayerScore.ToString();

		UpdateArrows();
		DisplayMessages();
	}

	private void DisplayMessages() {
		
		if (_notifcationQueue.Count > 0 && !_notification.enabled) {
			StartCoroutine(DisplayMessageRoutine(_notifcationQueue.Dequeue()));
		}
	}

	public void DisplayMessage(string msg) {

		_notifcationQueue.Enqueue(msg);
	}

	private IEnumerator DisplayMessageRoutine(string msg) {

		_notification.text = msg;
		_notification.enabled = true;
		yield return new WaitForSeconds(4f);
		_notification.text = "";
		_notification.enabled = false;
	}

	private void UpdateArrows() {

		NavigatorArrow arrow;
		// Enemy arrows
		for (int i = 0; i < _navigatorArrowList.Count; i++) {
			arrow = _navigatorArrowList[i];
			if (arrow.Target == null) {
				arrow.gameObject.SetActive(false);
				continue;
			} else {
				if (!arrow.TargetRenderer.isVisible) {
					if (!arrow.gameObject.activeSelf) {
						arrow.gameObject.SetActive(true);
					}
					if (!arrow.ArrowImage.enabled) {
						arrow.ArrowImage.enabled = true;
					}
					arrow.transform.rotation =
						Quaternion.LookRotation(
							Vector3.forward, 
							arrow.Target.position - _playerStats.transform.position);
				} else if (arrow.TargetRenderer.isVisible && arrow.ArrowImage.enabled) {
					arrow.ArrowImage.enabled = false;
				}
			}
		}
		// Objective arrow
		if (Manager.CurrentObjective.gameObject.activeSelf) {
			if (!_objectiveArrowIcon.enabled) {
				_objectiveArrowIcon.enabled = true;
			}
			_objectiveArrow.rotation =
				Quaternion.LookRotation(
							Vector3.forward,
							Manager.CurrentObjective.transform.position - _playerStats.transform.position);
		} else {
			if (_objectiveArrowIcon.enabled) {
				_objectiveArrowIcon.enabled = false;
			}
		}

		// Station Arrow
		if (!_stationArrow.TargetRenderer.isVisible) {
			if (!_stationArrow.gameObject.activeSelf) {
				_stationArrow.gameObject.SetActive(true);
			}
			_stationArrow.transform.rotation =
				Quaternion.LookRotation(
							Vector3.forward,
							StarSystemData.StarSystemLoaded.LocalSpaceStation.StationTransform.position
							- _playerStats.transform.position);
		} else {
			if (_stationArrow.gameObject.activeSelf) {
				_stationArrow.gameObject.SetActive(false);
			}
		}
	}
}
