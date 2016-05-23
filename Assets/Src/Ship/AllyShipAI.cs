using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.Linq;

public class AllyShipAI : BaseShipAI {

	public override void AssignNewTarget() {

		List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		if (Target != null && enemies.Contains(Target))
			enemies.Remove(Target);
		// TODO: Create target assignment logic here based on threat.
		Target = enemies.ElementAt(Mathf.FloorToInt(Random.Range(0, enemies.Count)));
		TargetRb = Target.GetComponent<Rigidbody2D>();
	}

}
