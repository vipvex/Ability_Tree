using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityPassive : LogicObject 
{

	public MonoBehaviour passiveReciever;
	public AbilityManager AbilityManager;


	[System.NonSerialized]
	public bool activated = false;


	public List<PassiveEffect> passiveEffects;


	public override void Initialize (MonoBehaviour AbilityManager)
	{

		this.AbilityManager = (AbilityManager)AbilityManager;
		activated = true;

		foreach (PassiveEffect passiveEffect in passiveEffects)
			AbilityManager.StartCoroutine(passiveEffect.LifeTime());

	}

	public void Calculate ()
	{

		// Loop through the effects that this LogicObject has
		foreach (PassiveEffect passiveEffect in passiveEffects)
		{
			if (activated == false || passiveEffect.active == false)
				return;

			passiveReciever.SendMessage(passiveEffect.methodName, passiveEffect.value);

			if (passiveEffect.active == false)
				Terminate();

			if (passiveEffect.frequency == PassiveEffect.Frequency.Once)
				passiveEffect.active = false;


			if (passiveEffect.frequency == PassiveEffect.Frequency.TimedOnTick || passiveEffect.frequency == PassiveEffect.Frequency.TimedOnTick)
			{



			}
		}
	}

	public override void Terminate ()
	{
		activated = false;
		foreach (PassiveEffect passiveEffect in passiveEffects)
		{
			passiveReciever.SendMessage(passiveEffect.methodName, -passiveEffect.value);
			passiveEffect.active = true;
		}
	}

}

[System.Serializable]
public class PassiveEffect 
{

	[System.NonSerialized]
	public bool active = true;


	public string methodName;
	public int value;


	public float lifeTime;

	public Frequency frequency;
	public enum Frequency { Once, OnTick, Timed, TimedOnTick }

	public IEnumerator LifeTime ()
	{
		yield return new WaitForSeconds(lifeTime);
		active = false;
	}

}
