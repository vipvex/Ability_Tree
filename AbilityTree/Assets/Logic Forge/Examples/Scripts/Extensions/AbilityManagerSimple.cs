using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A very simple script that allows you to cast abilities.
/// of a script that contains all the abilities of a certain character. 
/// Perfect to start your own ability manager.
/// </summary>
public class AbilityManagerSimple : MonoBehaviour 
{

	/// <summary>
	/// The ability system that this script uses to get it's abiltiies
	/// </summary>
	public LogicSystem LogicSystem;

	/// <summary>
	/// The character that will recieve all the passive effect messages.
	/// </summary>
	public MonoBehaviour character;
	

	public List<LogicObject> abilities;
	public List<AbilityPassive> passiveAbilities;

	public float tickRate = 1;
	

	private int selectedAbility = 0;


	void Start () 
	{

		// If there is no ability system assinged to this script disable it. 
		if (LogicSystem == null)
		{
			Debug.LogWarning ("No ability system assigned to ability manager!");
			this.enabled = false;
			return;
		}
		
		// Instanitate creates a perfect copy of ability data you give it. In this case
		// we tell it to create a copy our ability system. If we don't do this the script will be directly
		// changing the ability system in the assets folder and will cause problems if we have multiple scripts looking 
		// at the ability system and trying to make changes to it.
		LogicSystem = (LogicSystem)Instantiate(LogicSystem);

		// Recalculate the ability system nodes to makes sure they are refreshed.
		LogicSystem.CalculateConditions(); 

		// Get all the upgraded abiltiies
		UpdateAbilities();


	}
	
	void Update () 
	{
		// Loop through the abilities and see if the player is try to cast it
		for (int a = 0; a < abilities.Count; a++)
		{
			if (Input.GetButtonUp(a + 1 + ""))
			{
				// See what type the abilty is and then do the casting logic appropriate to it

				if (abilities[a] is SimpleAbilityProjectile)
					((SimpleAbilityProjectile)abilities[a]).Cast();

				if (abilities[a] is AbilityCaster)
					((AbilityCaster)abilities[a]).Cast();

				// For custom abilities
				//if (abilities[a] is YourCustomAbility)
					//((YourCustomAbility)abilities[a]).YourCustomLogic();

			}
		}
	}

	/// <summary>
	/// Gets all the upgraded abilities from the LogicObject System and populates the abilities and passives.
	/// </summary>
	public void UpdateAbilities ()
	{

		LogicObject[] upgradedAbilities = LogicSystem.GetUpgradedAbilities();

		// Clear the abilities
		abilities = new List<LogicObject>();
		passiveAbilities = new List<AbilityPassive>();


		// Loop through the list of abilities and sort the based on whether it is a passive or not.
		foreach (LogicObject ability in upgradedAbilities)
		{
			if (ability is AbilityPassive)
				passiveAbilities.Add((AbilityPassive)ability);
			else
				abilities.Add(ability);
		}

		// Make sure to stop the old tick rate and any other corutines that may be runnings.
		StopAllCoroutines();

		// Start the tick rate for the passives.
		StartCoroutine(PassiveTickrate());

	}

	/// <summary>
	/// A courutine that runs at the set tick rate.
	/// Every tick it calculates the passive effects.
	/// </summary>
	public IEnumerator PassiveTickrate ()
	{
		// Every Tick the passive effects will be calculated
		while(true) 
		{
			CalculatePassives();
			yield return new WaitForSeconds(tickRate);
		}
	}

	/// <summary>
	/// Calculates the passive effects.
	/// </summary>
	public void CalculatePassives ()
	{
		foreach (AbilityPassive passive in passiveAbilities)
		{
			passive.Calculate();
		}
	}

	void OnGUI ()
	{

		int abilityGUISize = 60;
		int spacing = 10;

		int a = 0;

		// Loop through all the abilities and display them with GUI boxes
		foreach (LogicObject ability in abilities)
		{

			// Change the color of the GUI Button if the thit ability is selected
			GUI.color = (a == selectedAbility) ? Color.blue : Color.white;


			Rect abilityGUIPos = new Rect(Screen.width / 2 - abilities.Count / 2  * abilityGUISize + (a-0.5f) * abilityGUISize + spacing, 
			                              Screen.height - abilityGUISize - spacing, 
			                              abilityGUISize, 
			                              abilityGUISize);


			GUI.Button (abilityGUIPos, ability.texture);

			a++;
			
		}
	}
}
