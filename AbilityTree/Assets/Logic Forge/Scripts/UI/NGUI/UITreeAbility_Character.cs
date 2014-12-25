using UnityEngine;
using System.Collections;

public class UITreeAbility_Character : MonoBehaviour {

    public string spriteName;


    public UISprite sprite;
    public UISprite availableSprite;
    public UISprite deavtiveSprite;
    public UILabel levelLabel; 


    public UITweener[] animations;



    public Node node;
    public LogicObject ability;
    public AbilityManager abilityManager;
    public NGUIUtility nguiUtil;



	public int pointValue = 0;
	public string valueName = "";


    void Start()
    {

        UpdateGUI();

    }

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

    public void IncreaseLevel()
    {
        if (node != null) nguiUtil.IncreaseLevel(node);
        UpdateGUI();
        abilityManager.logicSystem.SetInt(valueName, pointValue);
    }

    public void DecreaseLevel()
    {
        if (node != null) nguiUtil.DecreaseLevel(node);
        UpdateGUI();
        abilityManager.logicSystem.SetInt(valueName, pointValue);
    }

    public void UpdateGUI()
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
                    else sprite.spriteName = "PlainColor";
            }
            else
            {
                if (node.baseTexture) sprite.spriteName = node.baseTexture.name;
                else sprite.spriteName = "PlainColor";
                Debug.Log(sprite.spriteName);
            }
        }
    }

	public void Initialize(AbilityManager abilityManager, Node node)
	{
		
		this.abilityManager = abilityManager;
		this.node = node;
	
		
		gameObject.name = node.name;
		if (node.abilityObjects[abilityManager.selectedAbility]) sprite.spriteName = node.abilityObjects[abilityManager.selectedAbility].name;

        pointValue = abilityManager.logicSystem.GetInt("Points"); ;

		UpdateGUI();

	}


}
