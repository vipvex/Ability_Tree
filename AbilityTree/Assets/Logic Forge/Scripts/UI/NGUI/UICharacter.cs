using UnityEngine;
using System.Collections;

public class UICharacter : MonoBehaviour {
	
	public UILabel strength;
	public UILabel dexterity;
	public UILabel intelligence;
	
	public UILabel health;
	public UILabel mana;
	public UILabel healthRegen;
	public UILabel manaRegen;


	public UISprite healthSprite;
	public UISprite manaSprite;


	public UILabel level;
	public UILabel xp;
	public UILabel points;


	public Character character;


	void FixedUpdate ()
	{
		if (character)
		{
			strength.text = "Strength " + character.strength;
			dexterity.text = "Dexterity " + character.dexterity;
			intelligence.text = "Intelligence " + character.intelligence; 
			
			health.text = "Health " + character.health + "/" + character.healthMax;
			mana.text = "Mana " + character.mana + "/" + character.manaMax;
			healthRegen.text = "HR " + character.healthRegen;
			manaRegen.text = "MR " + character.manaRegen;

			healthSprite.fillAmount = character.health / character.healthMax;
			manaSprite.fillAmount = character.mana / character.manaMax;

			level.text = "Level " + character.level;
            xp.text = "Xp " + character.xp;
            points.text = "Points " + character.points;
		}
	}

}
