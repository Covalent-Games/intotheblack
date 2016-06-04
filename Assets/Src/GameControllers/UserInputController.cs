using UnityEngine;
using System.Collections;

public class UserInputController : MonoBehaviour {

	private ShipWeapons _shipWeapons;
	private ShipController _shipController;

	void Awake() {

		_shipController = GetComponent<ShipController>();
		_shipWeapons = GetComponent<ShipWeapons>();
	}

	void Update() {

		if (CheckForClick(1)) {
			_shipWeapons.FirePrimaryWeapons();
			_shipController.RotateShip(GetDirectionToMouse());
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (!_shipController.Boosting) {
				_shipController.Boost(GetDirectionToMouse());
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			_shipWeapons.FireTorpedo();
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			if (!_shipWeapons.Shielded) {
				_shipWeapons.ActivateShield();
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			ComponentResources.DeployShield(GameManager.Player.GetComponent<Destructable>().ShieldComponent);
		}
	}

	void FixedUpdate() {

		if (CheckForClick(0)) {
			_shipController.MoveShip(GetDirectionToMouse());
		} else {
			_shipController.SlowShip();
		}
	}

	private bool CheckForClick(int buttonCode) {

		if (Input.GetMouseButton(buttonCode)) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Returns a normalized vector representing the direction of the mouse relative to the ship
	/// </summary>
	/// <returns></returns>
	private Vector3 GetDirectionToMouse() {

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return Vector3.ClampMagnitude(Vector3.Normalize(mousePos - transform.position), 1);
	}
}
