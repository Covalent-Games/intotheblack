using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Component {

	public int PrimaryEffect;
	public int SecondaryEffect;
	public int ComponentType;

	public int PrimaryEffectCondition;
	public int SecondaryEffectCondition;

	public int PrimaryEffectApplcationType;
	public int SecondaryEffectApplicationType;

	public Color PrimaryColor;
	public Color SecondaryColor;

	public int PrimaryPotency;
	public int SecondaryPotency;

	public GameObject PrimaryPrefab;
	public GameObject SecondaryPrefab;

	public bool IsActive;
	
	// Default component
	public Component() {
		this.PrimaryEffect = (int)EffectTypeEnum.Damage;
		this.SecondaryEffect = (int)EffectTypeEnum.DisruptTargetting;
		this.ComponentType = (int)ComponentTypeEnum.Auxiliary;

		this.PrimaryEffectCondition = (int)EffectConditionTypeEnum.OnCollision;
		this.SecondaryEffectCondition = (int)EffectConditionTypeEnum.OnActivation;

		this.PrimaryEffectApplcationType = (int)EffectApplicationTypeEnum.SingleTarget;
		this.SecondaryEffectApplicationType = (int)EffectApplicationTypeEnum.Radius;

		this.PrimaryColor = new Color(1, 0, 0);
		this.SecondaryColor = new Color(0, 0, 1);

		this.PrimaryPotency = 40;
		this.SecondaryPotency = 5;
	}

	//public Component(int primEffect, int secEffect) {
	//	this.PrimaryEffect = (int)EffectTypeEnum.Damage;
	//	this.SecondaryEffect = (int)EffectTypeEnum.DisruptTargetting;
	//	this.ComponentType = (int)ComponentTypeEnum.Auxiliary;

	//	this.PrimaryEffectCondition = (int)EffectConditionTypeEnum.OnCollision;
	//	this.SecondaryEffectCondition = (int)EffectConditionTypeEnum.OnActivation;

	//	this.PrimaryEffectApplcationType = (int)EffectApplicationTypeEnum.SingleTarget;
	//	this.SecondaryEffectApplicationType = (int)EffectApplicationTypeEnum.Radius;

	//	this.PrimaryColor = new Color(1, 0, 0);
	//	this.SecondaryColor = new Color(0, 0, 1);

	//	this.PrimaryPotency = 40;
	//	this.SecondaryPotency = 5;
	//}

	public void CreateRandomComponent(int type, int potencyLevel) {
		ComponentResources componentSvc = new ComponentResources();

		this.PrimaryEffect = componentSvc.GetRandomEffect();
		this.SecondaryEffect = (int)EffectTypeEnum.DisruptTargetting;
		this.ComponentType = (int)ComponentTypeEnum.Auxiliary;

		this.PrimaryEffectCondition = (int)EffectConditionTypeEnum.OnCollision;
		this.SecondaryEffectCondition = (int)EffectConditionTypeEnum.OnActivation;

		this.PrimaryEffectApplcationType = (int)EffectApplicationTypeEnum.SingleTarget;
		this.SecondaryEffectApplicationType = (int)EffectApplicationTypeEnum.Radius;

		this.PrimaryColor = componentSvc.GetRandomColor();// new Color(1, 0, 0);
		this.SecondaryColor = componentSvc.GetRandomColor();//new Color(0, 0, 1);

		this.PrimaryPotency = 40;
		this.SecondaryPotency = 5;

		switch(type) {
			case (int)ComponentTypeEnum.PrimaryGun:

				break;
			case (int)ComponentTypeEnum.Auxiliary:
				break;
			case (int)ComponentTypeEnum.Engine:
				break;
			case (int)ComponentTypeEnum.Hull:
				break;
			case (int)ComponentTypeEnum.Sheild:
				this.PrimaryPrefab = componentSvc.GenerateShieldObject(this);
				break;
		}
	}


}
