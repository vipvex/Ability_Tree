using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{

    public int level = 5;
    public int points = 10;
	public int xp = 100;


	public int strength = 5;
	public int dexterity = 5;
	public int intelligence = 5;


	public float health = 100;
	public float healthMax = 100;
	public float mana = 50;
	public float manaMax = 50;
	public int healthRegen = 5;
	public int manaRegen = 5;


	public GUIType guiType;
	public enum GUIType { NGUI }

	public float regenRate = 0.5f;


	public AbilityManager abilityManager;




	void Start ()
	{

		GameObject abilityUI = GameObject.FindGameObjectWithTag("AbilityUI"); 
		if (guiType == GUIType.NGUI && abilityUI.GetComponent<UICharacter>())
			GameObject.FindGameObjectWithTag("AbilityUI").GetComponent<UICharacter>().character = this;

		abilityManager = GetComponent<AbilityManager>();

        UpdateLevel(0);
        UpdatePoints(0);
        UpdateXp(0);

	}

	void FixedUpdate ()
	{
		
	}


	void UpdateLevel (int value) 
	{ 
		level += value;
        abilityManager.logicSystem.SetInt("Level", level);
	}

    void UpdatePoints(int value)
    {
        points += value;
        abilityManager.logicSystem.SetInt("Points", points);
    }

	void UpdateXp (int value) 
	{ 
		xp += value;
        abilityManager.logicSystem.SetInt("Xp", xp);
	}


	void UpdateStrength (int value) { strength += value;  }
	void UpdateDexterity (int value) { dexterity += value;  }
	void UpdateIntelligence (int value) { intelligence += value; }

	void UpdateHealth (int value) { health += value;  }
	void UpdateMaxHealth (int value) { healthMax += value;  }
	void UpdateMana (int value) { mana += value; }
	void UpdateMaxMana (int value) { manaMax += value; }

	void UpdateHealthRegen (int value) { healthRegen += value;  }
    void UpdateManaRegen(int value) { manaRegen += value; }
	
}
