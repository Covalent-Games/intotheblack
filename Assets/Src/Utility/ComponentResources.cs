using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ComponentResources {

	public static void DeployShield(Component component) {
		GameObject shieldObject = GameObject.Instantiate(component.PrimaryPrefab, GameManager.Player.transform.position, Quaternion.identity) as GameObject;
		shieldObject.transform.SetParent(GameManager.Player.transform);
		PlayerShipShield shieldData = shieldObject.AddComponent<PlayerShipShield>();
		//shieldData.
	}

	public int GetRandomEffect() {
		Array enums = Enum.GetValues(typeof(EffectTypeEnum));
		System.Random randomSelection = new System.Random();
		return (int)enums.GetValue(randomSelection.Next(enums.Length));
	}

	public int GetRandomEffectCondition() {
		Array enums = Enum.GetValues(typeof(EffectConditionTypeEnum));
		System.Random randomSelection = new System.Random();
		return (int)enums.GetValue(randomSelection.Next(enums.Length));
	}

	public int GetRandomEffectApplication() {
		Array enums = Enum.GetValues(typeof(EffectApplicationTypeEnum));
		System.Random randomSelection = new System.Random();
		return (int)enums.GetValue(randomSelection.Next(enums.Length));
	}

	public Color GetRandomColor() {
		float red = UnityEngine.Random.Range(0f, 1f);
		float green = UnityEngine.Random.Range(0f, 1f);
		float blue = UnityEngine.Random.Range(0f, 1f);
		
		// If color is too dark, lighten one of them
		if (red < 0.6 && green < 0.6 && blue < 0.6) {
			char largest = 'r';
			float max = Mathf.Max(red, green, blue);
			if (max == green)
				largest = 'g';
			else if (max == blue)
				largest = 'b';

			switch(largest) {
				case 'r': red = 0.6f;
					break;
				case 'g': green = 0.6f;
					break;
				case 'b': blue = 0.6f;
					break;
			}
		}
		return new Color(red, green, blue);
	}

	public GameObject GenerateShieldObject(Component component) {

		GameObject shieldObject = GameManager.Instance.ComponentPrefabs[0];
		Transform child = shieldObject.transform.GetChild(0);
		ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
		Debug.Log("Shield prefab children: " + shieldObject.transform.childCount);
		particleSystem.startColor = Color.Lerp(component.PrimaryColor, component.SecondaryColor, 1);
		Renderer renderer = particleSystem.GetComponent<Renderer>();
		int randomMaterial = Mathf.FloorToInt(UnityEngine.Random.Range(1, 5));
		string materialPath = "Materials/Shield/Shield_Particle_Material_" + randomMaterial.ToString().PadLeft(2, '0');
		//materialPath = "TestMat";
		UnityEngine.Object tempMat = Resources.Load(materialPath, typeof(Material));
		renderer.sharedMaterial = (Material)tempMat;// Resources.Load(materialPath, typeof(Material));// as Material;

		return shieldObject;
	}

}
