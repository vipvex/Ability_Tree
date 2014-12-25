using UnityEngine;
using System.Collections;

public class UIAbility : MonoBehaviour {


	public string spriteName = "PlainColor";

    public UISprite sprite;
    public UILabel keyLabel;
	public UILabel cooldownLabel;
	public UISprite cooldownSprite;
	


    public UITweener[] animations;


    public AbilitySlot abilitySlot;
    public AbilityManager AbilityManager;

    void Start()
    {

    }
	
	void FixedUpdate () 
    {

        if (abilitySlot.ability == null) return;

        if (abilitySlot.ability is AbilityCaster)
        {
            AbilityCaster LogicObject = (AbilityCaster)abilitySlot.ability;
            cooldownLabel.text = (LogicObject.cooldownCountdown == 0) ? "" : "" + Mathf.Round(LogicObject.cooldownCountdown * 100f) / 100f;
            cooldownSprite.fillAmount = LogicObject.cooldownCountdown / LogicObject.cooldownLength;
        }

	}

    public void ToggleSelect (bool value)
    {
        for (int a=0; a<animations.Length; a++)
        {
            if (value)
                animations[a].PlayForward();
            else
                animations[a].PlayReverse();   
        }
    }

    public void UI_Select()
    {
        //AbilityManager.SelectLogicObject(LogicObject);
    }

	public void UpdateUI ()
	{

		ToggleSelect(false);
		
		gameObject.name = abilitySlot.name;
        keyLabel.text = abilitySlot.key;
		
		if (abilitySlot.ability && abilitySlot.ability.texture) 
		{
			sprite.spriteName = abilitySlot.ability.texture.name;
		}else{
            sprite.spriteName = "PlainColor";
			cooldownLabel.text = "";
			cooldownSprite.fillAmount = 0;
		}

	}

    public void Initialize(AbilityManager AbilityManager, AbilitySlot abilitySlot)
    {

        this.AbilityManager = AbilityManager;
        this.abilitySlot = abilitySlot;
		UpdateUI();

    }

}
