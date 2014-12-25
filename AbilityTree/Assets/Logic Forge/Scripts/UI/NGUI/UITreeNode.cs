using UnityEngine;
using System.Collections;

/// <summary>
/// Base refrence class that implements the basic features of a UI tree node.
/// To customize make a new script and derive from UITreeNode.
/// </summary>
public class UITreeNode : MonoBehaviour 
{

    /// <summary>
    /// The current sprite that will be desiplayed based on the level of the node.
    /// </summary>
    public UISprite nodeSprite;

    /// <summary>
    ///  The sprite that will be activated if the ability is available to upgrade.
    /// </summary>
    public UISprite availableSprite;

    /// <summary>
    ///  The sprite that will be activated if the ability is not available and not upgraded. 
    /// </summary>
    public UISprite deavtiveSprite;


    /// <summary>
    /// Animations
    /// </summary>
    public UITweener[] animations;


    /// <summary>
    /// The node that this UI takes the info from.
    /// </summary>
    public Node node;

    /// <summary>
    /// The Tree System that the node bellongs too.
    /// </summary>
    public LogicSystem treeSystem;

    /// <summary>
    /// A refrence to the tree generator that created this UI node.
    /// </summary>
    public UITreeGenerator uiTreeGenerator;


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

    void OnHover(bool value)
    {
        // When hovering over ability change the static tooltip text.
        UIMenuManager.showTooltip = value;
        UIMenuManager.tooltipText = node.description;
    }


    /// <summary>
    /// Called when GUI button is clicked
    /// </summary>
    public virtual void IncreaseLevel()
    {
        if (node != null)
        {
            treeSystem.IncreaseLevel(node);
            uiTreeGenerator.UpdateTreeUI();
        }
    }

    /// <summary>
    /// Called when GUI button is clicked
    /// </summary>
    public virtual void DecreaseLevel()
    {
        if (node != null)
        {
            treeSystem.DecreaseLevel(node);
            uiTreeGenerator.UpdateTreeUI();
        }
    }

    /// <summary>
    /// Call this whenever you want to refresh the info of the node UI.
    /// </summary>
    public virtual void UpdateUI()
    {
        // Activate and deactivete the sprites based on whether the ability available or disabled
        if (availableSprite) availableSprite.enabled = (node.activated && node.level == 0) ? true : false;
        if (deavtiveSprite) deavtiveSprite.enabled = (!node.activated && node.level == 0) ? true : false;

        if (node != null)
        {
            if (node.level > 0)
            {
                if (node.abilityObjects[node.level] && node.abilityObjects[node.level].texture)
                    nodeSprite.spriteName = node.abilityObjects[node.level].texture.name;
            }
            else
            {
                if (node.baseTexture)
                    nodeSprite.spriteName = node.baseTexture.name;
                else
                    nodeSprite.spriteName = "PlainTexture";
            }    
        }
    }


    public virtual void Initialize(LogicSystem treeSystem, Node node, UITreeGenerator uiTreeGenerator)
    {
        this.treeSystem = treeSystem;
        this.node = node;
        this.uiTreeGenerator = uiTreeGenerator;

        gameObject.name = node.name;
        UpdateUI();
    }


}
