using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class EnemyShipAI : BaseShipAI {

	public override void AssignNewTarget() {

		// TODO: Create target assignment logic here based on threat.
		Target = GameManager.Player;
		TargetRb = Target.GetComponent<Rigidbody2D>();
	}

}
