using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Interacts with all the abilities
/// </summary>
public class AbilityManager : MonoBehaviour
{

    public LogicSystem logicSystem;
	public MonoBehaviour characterScript;


	[HideInInspector] 
    public bool castingAbility;



    public int selectedAbility = 0;

    public AbilitySlotType abilitySlotType;
    public enum AbilitySlotType { MatchTags, UseNodeKeys, PlayerDefined }

	//[HideInInspector]
	public List<AbilitySlot> abilitySlots; 
	public List<AbilityPassive> passiveAbilities;


    public CastType castType;
    public enum CastType { OnSelect, CastKeys }


	public string chargeKey;
    public string castKey;
    public string cancelCastKey;


	public AudioSource selectAbilitySound;
	public AudioSource failAbilityCastSound;


	public float tickRate = 1;


    [HideInInspector]
    public List<Spawn> spawns = new List<Spawn>();


    public GUIType GUI_type;
    public enum GUIType { UnityGUI, NGUI, None }

    [HideInInspector]
    public NGUIUtility nguiUtil;


	void Start ()
	{
        if (logicSystem)
        {
            logicSystem = (LogicSystem)Instantiate(logicSystem);
            logicSystem.CalculateConditions(); 
            UpdateAbilities();
        }

        if (GUI_type == GUIType.NGUI)
        {
            if (GameObject.FindGameObjectWithTag("AbilityUI"))
            {
                nguiUtil = GameObject.FindGameObjectWithTag("AbilityUI").GetComponent<NGUIUtility>();
                nguiUtil.Initialize(this);
            }
        }
        
    }

	public IEnumerator OnTick ()
	{
		while(true) {
			Tick();
			yield return new WaitForSeconds(tickRate);
		}
	}

	public void Tick ()
	{
		foreach (AbilityPassive passive in passiveAbilities)
		{
			passive.Calculate();
		}
	}

	void Update ()
	{

		// Select abilitySlots based on the assigned hotkeys
		for (int a = 0; a < abilitySlots.Count && a < abilitySlots.Count; a++)
		{
			if (Input.GetButtonDown(abilitySlots[a].key) && castingAbility == false)
			{
				if (castingAbility == false)
					SelectAbility(a);
				else
					OnSelectAbilityFailed();
			}
		}

		if (abilitySlots.Count == 0 || UIMenuManager.inMenu || selectedAbility == -1)
            return;

        // Begin Charging
		if (Input.GetButtonDown(chargeKey) && castingAbility == false) 
		{
			ToggleChargingAbility(selectedAbility, true);
		}

        // Charge
		if (Input.GetButton(chargeKey)) 
		{
            ChargeAbility(selectedAbility);
		}

        // Cast
		if (Input.GetButtonUp(castKey))
		{
			CastAbility(selectedAbility);
		}

        // Cancel
		if (Input.GetButtonDown(cancelCastKey) && castingAbility == true) 
		{
            CancelAbility(selectedAbility);

			if (failAbilityCastSound)
				failAbilityCastSound.Play();

		}
	}



    [RPC]
    public virtual void SelectAbility(int value)
    {
	
		// Select the new abilitys & deselect the old one
		if (selectedAbility != -1 && abilitySlots[selectedAbility].ability) abilitySlots[selectedAbility].ability.Select(false);
		if (value != -1 && abilitySlots[value].ability) abilitySlots[value].ability.Select(true);

        selectedAbility = value;

        if (selectAbilitySound)
            selectAbilitySound.Play();


        if (GUI_type == GUIType.NGUI && nguiUtil)
            nguiUtil.SelectAbility(selectedAbility);

    }

    public virtual void OnSelectAbilityFailed()
    {
        
    }


	public virtual void ToggleCastingAbility (bool value)
	{
		castingAbility = value;
		if (value == false)
			SelectAbility(-1);
	}

	[RPC]
	public virtual void ToggleChargingAbility (int index, bool value)
	{

		if (abilitySlots[index].ability is AbilityCaster)
			((AbilityCaster)abilitySlots[index].ability).Charge();

	}

    [RPC]
    public virtual void ChargeAbility (int index)
    {
        if (abilitySlots[index].ability is AbilityCaster) 
		{
			((AbilityCaster) abilitySlots[index].ability).Charging();
		}
    }

	[RPC]
	public virtual void CastAbility (int index)
	{

        if (abilitySlots[index].ability is AbilityCaster)
        {
            ((AbilityCaster)abilitySlots[selectedAbility].ability).Cast();
        }

		if (abilitySlots[index].ability is SimpleAbilityProjectile)  
			((SimpleAbilityProjectile) abilitySlots[selectedAbility].ability).Cast();

		//SelectAbility(-1);

	}

    [RPC]
    public virtual void CancelAbility(int index)
    {

		if (abilitySlots[index].ability is AbilityCaster)
			((AbilityCaster)abilitySlots[index].ability).Cancel();
    
		SelectAbility(-1);
	
	}

	// Calles the initialize function in all the abilitySlots
    [RPC]
	public void UpdateAbilities ()
	{

        // Have the abilitySlots do their terminating logic before creating new onces
        foreach (AbilitySlot abilitySlot in abilitySlots)
		{
            if (abilitySlot.ability)
			{
				abilitySlot.ability.Terminate();
				abilitySlot.ability = null;
			}
		}

		// Have the abilitySlots do their terminating logic before creating new onces
		foreach (LogicObject passiveAbility in passiveAbilities)
		{
			if (passiveAbility)
			{
				passiveAbility.Terminate();
			}
		}

		passiveAbilities = new List<AbilityPassive>();
		StopAllCoroutines();

        //abilitySlots = new List<AbilitySlot>();

		// Add abilies that have a level and are active 
        foreach (Node node in logicSystem.nodes)
		{
			if (node.level == -1)
				Debug.Log (node.name + " has -1 level!");
            // Todo: Add sorting
			if (node.activated && node.abilityObjects.Count > 0 && node.level >= 0 && node.abilityObjects[node.level] != null)
			{

				// If this is a passive ability add it to the list so that it may be called with the OnTick
				if (node.abilityObjects[node.level] is AbilityPassive)
				{
					//passiveAbilities.Add (Test.Clone((AbilityPassive)node.abilityObjects[node.level]));
					passiveAbilities.Add((AbilityPassive)Instantiate((Object)node.abilityObjects[node.level]));
					passiveAbilities[passiveAbilities.Count - 1].passiveReciever = characterScript;
					passiveAbilities[passiveAbilities.Count - 1].Initialize(this);
				}

                // Find the corresponding ability slot tag and use it's assigned key for selecting
                if (abilitySlotType == AbilitySlotType.MatchTags)
				{
					for (int a=0; a < abilitySlots.Count; a++)
					{
                        if (node.tag == abilitySlots[a].tag)  
						{
							abilitySlots[a].ability = (LogicObject)Instantiate((Object)node.abilityObjects[node.level]);
							abilitySlots[a].ability.Initialize(this);
						}
					}
				}

				//abilitySlots[a] = new AbilitySlot(node.abilityObjects[node.level].name, node.tag, abilitySlots[a].key, node.abilityObjects[node.level]);  
				

				// Assign the select key
				//if (abilitySlots[abilitySlots.Count-1].ability) 
                		
			}
		}

		// Start the Tick rate
		StartCoroutine(OnTick());


        //Debug.Log(abilitySlots.Count);
        if (abilitySlots.Count == 0)
            return;


        selectedAbility = 0;
        
		if (abilitySlots.Count > 0 && abilitySlots[selectedAbility].ability)
            SelectAbility(-1);
	
	}

	public void UpdateAbility (LogicObject remove, LogicObject add)
	{

		if (remove is AbilityPassive || add is AbilityPassive)
		{
			foreach (LogicObject ability in passiveAbilities)
				if (remove)
				{

					Debug.Log ("Found match");
					ability.Terminate();
					passiveAbilities.Remove((AbilityPassive)ability);
					break;
				}

			//if (remove) passiveAbilities[passiveAbilities.IndexOf((AbilityPa)remove)].Terminate();
			//if (remove) passiveAbilities.Remove((AbilityPassive)remove);
			
			if (add)
			{
				add = (AbilityPassive)Instantiate(add);
				passiveAbilities.Add ((AbilityPassive)add);
				add.Initialize(this);
			}

		}else{

			if (remove) Debug.Log ("Not a passive ability! " + remove.name);
		
			foreach (AbilitySlot slot in abilitySlots)
			{
				if (slot.ability == remove)
				{
					Debug.Log (slot.ability);
					if (slot.ability) slot.ability.Terminate();

					if (add) slot.ability = (AbilityPassive)Instantiate(add);
					if (slot.ability != null) slot.ability.Initialize(this);
					break;
				}
			}
		}

		StopAllCoroutines();
		StartCoroutine(OnTick());

	}


	#region GetThisOutOfHere
    public void SetSpawn(string name, Transform obj)
    {
        foreach (Spawn spawn in spawns)
            if (spawn.name == name) spawn.obj = obj;
    }

    public void AddSpawn(string name, Transform obj)
    {
        spawns.Add(new Spawn("Name", obj));
    }

    public Transform GetSpawn(string name)
    {
        foreach (Spawn spawn in spawns)
            if (spawn.name == name) return spawn.obj;
        
		Debug.Log ("Didn't find " + name);
        return null;
    }

    public void RemoveSpawn(string name)
    {
        foreach (Spawn spawn in spawns)
            if (spawn.name == name) spawns.Remove(spawn);
    }

    [System.Serializable]
    public class Spawn
    {
        public string name = "";
        public Transform obj;
        public Spawn(string name, Transform obj)
        {
            this.name = name;
            this.obj = obj;
        }
    }
    #endregion

	void OnGUI ()
    {

		if (GUI_type != GUIType.UnityGUI)
			return;

		foreach (AbilitySlot abilitySlot in abilitySlots){

			float barWidth = 200;
            if (abilitySlot.ability.GetType() != typeof(AbilityCaster))
                continue;

            AbilityCaster ability = (AbilityCaster)abilitySlot.ability;

			if (ability.state == AbilityCaster.State.Charging)
            {

                float chargePercent = ability.chargeAmount / ability.chargeMax;
                GUI.Box(new Rect(Screen.width * 0.5f - barWidth / 2 * chargePercent, Screen.height * 0.7f, barWidth * chargePercent, 30), "Charge: " + Mathf.Round(ability.chargeAmount * 100f) / 100f);

			}

            if (ability.state == AbilityCaster.State.Casting)
            {

                float castPercent = ability.castCountdown / ability.castLength;
                GUI.Box(new Rect(Screen.width * 0.5f - barWidth / 2 * castPercent, Screen.height * 0.7f, barWidth * castPercent, 30), "Cast: " + Mathf.Round(ability.castCountdown * 100f) / 100f);
				
			}
			
		}

		for (int a=0; a<abilitySlots.Count; a++){
			
			float abilitySize = 60;
			float spacing = 10;

			GUI.color = (a == selectedAbility) ? Color.blue : Color.white;
 			GUI.Box (new Rect(Screen.width / 2 - abilitySlots.Count / 2  * abilitySize + (a-0.5f) * abilitySize + spacing, Screen.height - abilitySize - spacing, abilitySize, abilitySize), abilitySlots[a].name);

            if (abilitySlots[a].GetType() == typeof(AbilityCaster))
            {
                GUI.Box(new Rect(Screen.width / 2 - abilitySlots.Count / 2 * abilitySize + (a - 0.5f) * abilitySize + spacing, Screen.height - abilitySize - spacing, abilitySize, abilitySize), "" + Mathf.Round(((AbilityCaster)abilitySlots[a].ability).cooldownCountdown * 100f) / 100f);
            }

		}
		
	}	

}

[System.Serializable]
public class AbilitySlot
{
    public string name;
    public string tag;
    public string key;
    public LogicObject ability;

    public AbilitySlot(string name, string tag, string key, LogicObject ability)
    {
        this.name = name;
        this.tag = tag;
        this.ability = ability;
    }
}