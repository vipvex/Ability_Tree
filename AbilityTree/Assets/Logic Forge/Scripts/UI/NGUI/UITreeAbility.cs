using UnityEngine;
using System.Collections;

public class UITreeAbility : MonoBehaviour {

    public string spriteName;


    public UISprite sprite;
    public UISprite availableSprite;
    public UISprite deavtiveSprite;
    public UILabel levelLabel; 


    public UITweener[] animations;



    public Node node;
    public LogicObject ability;
    public AbilityManager AbilityManager;
    public NGUIUtility nguiUtil;
	

    void OnClick()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            IncreaseLevel();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            DecreaseLevel();
        }
    }

	void Update ()
	{
	}

	void OnHover (bool value)
	{
		UIMenuManager.showTooltip = value;
		UIMenuManager.tooltipText = node.description;
	}


	/// <summary>
	/// Called when GUI button is clicked
	/// </summary>
	public void IncreaseLevel()
    {
        if (node != null) nguiUtil.IncreaseLevel(node);
    }

	/// <summary>
	/// Called when GUI button is clicked
	/// </summary>
    public void DecreaseLevel()
    {
        if (node != null) nguiUtil.DecreaseLevel(node);
    }

	public void UpdateUI()
    {
        if (levelLabel) levelLabel.text = "" + node.level;
        if (availableSprite) availableSprite.enabled = (node.activated && node.level == 0) ? true : false;
        if (deavtiveSprite) deavtiveSprite.enabled = (!node.activated && node.level == 0) ? true : false;

        if (node != null)
        {
            if (node.level > 0)
            {
                ability = node.abilityObjects[node.level];
                if (ability.texture) sprite.spriteName = ability.texture.name;
            }
            else
            {
                if (node.baseTexture) sprite.spriteName = node.baseTexture.name;
                else sprite.spriteName = "PlainColor";
            }
        }
    }

	public void Initialize(AbilityManager AbilityManager, Node node)
	{
		
		this.AbilityManager = AbilityManager;
		this.node = node;
	
		
		gameObject.name = node.name;
		//if (node.LogicObjectObjects[AbilityManager.selectedLogicObject]) sprite.spriteName = node.LogicObjectObjects[AbilityManager.selectedLogicObject].name;
		UpdateUI();

	}


}
