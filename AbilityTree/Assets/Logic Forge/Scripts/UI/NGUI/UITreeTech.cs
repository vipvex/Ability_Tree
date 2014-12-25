using UnityEngine;
using System.Collections;

/// <summary>
/// Inherets the main functionality of UI Tree node.
/// Customized to fit the civilization like technology tree.
/// </summary>
public class UITreeTech : UITreeNode 
{

    public UILabel technologyName;
    public UILabel technologyTurnCount;


    public UISprite backgroundSprite;

    public Color availableColor;
    public Color unavailableColor;

    public Color selectedColor;
    public Color upgradedColor;


    public TechTreeManager techTreeManager;

    
    public override void IncreaseLevel()
    {
        if (node != null)
        {
            techTreeManager.AddTechToQue(node); 
            uiTreeGenerator.UpdateTreeUI();
        }
    }


    public override void DecreaseLevel()
    {
        if (node != null)
        {
            techTreeManager.AddTechToQue(node);
            uiTreeGenerator.UpdateTreeUI();
        }
    }


    
    public override void UpdateUI ()
    {
        base.UpdateUI();

        if (node != null && techTreeManager != null)
        {

            if (techTreeManager.technologyQue.Contains(node)) backgroundSprite.color = selectedColor;
            else if (node.activated && node.level > 0) backgroundSprite.color = upgradedColor;
            else if (node.activated && node.level == 0) backgroundSprite.color = availableColor;
            else if (node.activated == false) backgroundSprite.color = unavailableColor;
             

            technologyName.text = node.name;
            if (node.abilityObjects.Count >= 2 && node.abilityObjects[1]) 
            { 
                Technology technology = (Technology)node.abilityObjects[1];
                technologyTurnCount.text = Mathf.Ceil(technology.researchCost / techTreeManager.researchRate - technology.reseachedAmount / techTreeManager.researchRate) + " turns";
                if (technology.researchCost <= technology.reseachedAmount)
                    technologyTurnCount.text = "";
            }
        }

    }

    
    public override void Initialize(LogicSystem treeSystem, Node node, UITreeGenerator uiTreeGenerator)
    {
        base.Initialize(treeSystem, node, uiTreeGenerator);
        techTreeManager = GameObject.FindGameObjectWithTag("MainTechTree").GetComponent<TechTreeManager>();
        UpdateUI();
    }

}
