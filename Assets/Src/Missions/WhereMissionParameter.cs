using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class WhereMissionParameter  {

	public Guid Where;
	public WhyMissionParameter Why;

	private List<string> _whyList = new List<string>() {
		"Attack from aliens",
		"Attack from enemy faction",
		"Mechanical failure of transport vessel",
	};

	public WhereMissionParameter(Guid guid) {

		Where = guid;
		Why = new WhyMissionParameter(_whyList[UnityEngine.Random.Range(0, _whyList.Count)]);
	}

}
