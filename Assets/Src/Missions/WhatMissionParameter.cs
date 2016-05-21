using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhatMissionParameter {

	public string What;
	public WhoMissionParameter Who;
	
	private List<string> _whoList = new List<string> {
		"Military Transport",
	};

	public WhatMissionParameter(string what) {

		What = what;
		switch (what) {
			case "Rescue":
				Who = new WhoMissionParameter(_whoList[UnityEngine.Random.Range(0, _whoList.Count)]);
				break;
		}
	}
}
