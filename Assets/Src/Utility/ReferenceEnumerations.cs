using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

public enum EffectTypeEnum {
		
	[Description("Stun")]
	Stun = 1,
	[Description("Damage")]
	Damage = 2,
	[Description("Disrupt Targetting")]
	DisruptTargetting = 3,
	[Description("Impact")]
	Impact = 4,
	[Description("Boost")]
	Boost = 5,
	[Description("Damage Over Time")]
	DamageOverTime = 6,
	[Description("Bounce")]
	Bounce = 7,
	[Description("Armor")]
	Armor = 8,
	[Description("Chain")]
	Chain = 9
}

public enum EffectConditionTypeEnum {

	[Description("On Collision")]
	OnCollision = 1,
	[Description("On Activation")]
	OnActivation = 2,
	[Description("Over Time")]
	OverTime = 3,

}

public enum EffectApplicationTypeEnum {

	[Description("Single Target")]
	SingleTarget = 1,
	Radius = 2,
	Chain = 3,
	Spread = 4
	

}

public enum ComponentTypeEnum {

	[Description("Primary Gun")]
	PrimaryGun = 1,
	[Description("Auxiliary")]
	Auxiliary = 2,
	[Description("Sheild")]
	Sheild = 3,
	[Description("Hull")]
	Hull = 4,
	[Description("Engine")]
	Engine = 5
}

public class ComponentBehavior {

	public static void DeployShield(Component component) {

	}
}
