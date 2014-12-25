using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NGUIUtility : MonoBehaviour {


    public UpgradeType upgradeType;
	


	public UILabel chargeLabel;
	public UIProgressBar chargeBar;

    public UILabel castLabel;
    public UIProgressBar castBar;


    
    public GameObject abilityBarContainer;
    public GameObject abilityBarPrefab;
    public InitializeMethod barInitMethod;

    
    public GameObject abilityTreeContainer;
    public GameObject abilityTreePrefab;
    public InitializeMethod treeInitMethod;


    public GameObject connectionPrefab;


    const float nodeSide = 75;
    public float abilityUIScale = 55;
    public Vector2 abilityPositionScale = new Vector3(1, 1);


    public AudioSource increaseLevelSound;
    public AudioSource decreaseLevelSound;
    public AudioSource failLevelSound;


    public List<UIAbility> abilityBarList;
    public List<UITreeAbility> abilityTreeList;
   
    
    public AbilityManager abilityManager;
	public LogicSystem logicSystem;


    public enum UpgradeType { Increasable, Decreasable, Both }
	public enum InitializeMethod { None, Generate, Update } 


	GameObject connectionsContainer;


	public List<ParamaterLevelBehavior> paramaterLevelBehaviors;
	[System.Serializable]
	public struct ParamaterLevelBehavior
	{
		public string name;
		public float value;
	}
	


	public void Initialize (AbilityManager abilityManager) 
	{
        this.abilityManager = abilityManager;
        logicSystem = abilityManager.logicSystem;
        
		if (treeInitMethod == InitializeMethod.Generate)
			GenerateTree();

		if (barInitMethod == InitializeMethod.Generate)
			GenerateBar();
	
		UpdateUI();
	
    }

    public void UpdateUI()
    {

		UpdateTree();
		UpdateBar();

    }

	void FixedUpdate ()
	{
        DisplayChargeBars();
	}

	public void UpdateGUIRate ()
	{
		UpdateUI();
	}

    public void DisplayChargeBars()
    {

        if (abilityManager == null || logicSystem == null || 
		    abilityManager.abilitySlots.Count == 0 || 
		    abilityManager.selectedAbility == -1 || 
		    abilityManager.abilitySlots[abilityManager.selectedAbility].ability == null ||
		    (abilityManager.abilitySlots[abilityManager.selectedAbility].ability is AbilityCaster) == false)
		{
			if (chargeBar.gameObject.activeSelf) chargeBar.gameObject.SetActive(false);
			if (castBar.gameObject.activeSelf) castBar.gameObject.SetActive(false);
			return;
		}


        AbilityCaster selectedAbility = (AbilityCaster)abilityManager.abilitySlots[abilityManager.selectedAbility].ability;

        // Display charging bar
        if (selectedAbility.state == AbilityCaster.State.Charging)
        {
            if (chargeBar.gameObject.activeSelf == false && selectedAbility.chargeMax > 0) chargeBar.gameObject.SetActive(true);


            float chargeValue = selectedAbility.chargeAmount / selectedAbility.chargeMax;


            if (chargeLabel) chargeLabel.text = "" + Mathf.Round(chargeValue * 100f) / 100f;
            if (chargeBar) chargeBar.value = chargeValue;

        }
        else
            if (chargeBar.gameObject.activeSelf) chargeBar.gameObject.SetActive(false);


        // Displays casting bar
        if (selectedAbility.state == AbilityCaster.State.Casting)
        {
            if (castBar.gameObject.activeSelf == false && selectedAbility.castLength > 0) castBar.gameObject.SetActive(true);


            float castValue = selectedAbility.castCountdown / selectedAbility.castLength;


            if (castLabel) castLabel.text = "" + Mathf.Round(castValue * 100f) / 100f;
            if (castBar) castBar.value = castValue;

        }
        else
            if (castBar.gameObject.activeSelf) castBar.gameObject.SetActive(false);

    }


	[ContextMenu("Generate Bar")]
	public void GenerateBar ()
	{
		foreach (AbilitySlot abilitySlot in abilityManager.abilitySlots)
		{

            GameObject newAbility = (GameObject)Instantiate(abilityBarPrefab, new Vector3(), Quaternion.identity);
            newAbility.transform.parent = abilityBarContainer.transform;
            newAbility.transform.localScale = new Vector3(1, 1, 1);

            newAbility.GetComponent<UIAbility>().Initialize(abilityManager, abilitySlot);
                
            abilityBarList.Add(newAbility.GetComponent<UIAbility>());
    
		}
        // Have the grid reposition all the abilities
        abilityBarContainer.GetComponent<UIGrid>().repositionNow = true;

	}

    [ContextMenu("Generate Tree")]
    public void GenerateTree()
    {

		connectionsContainer = (GameObject)Instantiate(new GameObject());
		connectionsContainer.name = "Connections";
		connectionsContainer.transform.parent = abilityTreeContainer.transform;

        // Populate the tree with abilities
        foreach (Node node in logicSystem.nodes)
        {

            if (!abilityTreePrefab)
                return;

            GameObject newTreeNode = (GameObject)Instantiate(abilityTreePrefab, new Vector3(), Quaternion.identity);
            
            newTreeNode.transform.parent = abilityTreeContainer.transform;
            newTreeNode.transform.localScale = new Vector3(1, 1, 1);
            newTreeNode.transform.localPosition = new Vector3(((5000 - node.window.x) * (1 - nodeSide / abilityUIScale)) * abilityPositionScale.x, ((node.window.y - 5000) * (1 - nodeSide / abilityUIScale)) * abilityPositionScale.y, 0);


            newTreeNode.GetComponent<UITreeAbility>().Initialize(abilityManager, node);
            newTreeNode.GetComponent<UITreeAbility>().nguiUtil = this; 
            newTreeNode.GetComponent<UITreeAbility>().node = node;
                
            
            abilityTreeList.Add(newTreeNode.GetComponent<UITreeAbility>());


            for (int c = 0, a = node.connections.Count; c < a; c++)
            {
                
                GameObject connection = CreateVisualConneciton(new Vector2(newTreeNode.transform.localPosition.x, newTreeNode.transform.localPosition.y),
                                       new Vector2((5000 - logicSystem.nodes[node.connections[c]].window.x) * (1 - nodeSide / abilityUIScale) * abilityPositionScale.x, (logicSystem.nodes[node.connections[c]].window.y - 5000) * (1 - nodeSide / abilityUIScale) * abilityPositionScale.y));
				connection.transform.parent = connectionsContainer.transform;

            }
        }
    }

	[ContextMenu("Update Tree")]
	public void UpdateTree ()
	{
		foreach (UITreeAbility ability in abilityTreeList)
		{
			ability.UpdateUI();
		}
	}
	[ContextMenu("Update Bar")]
	public void UpdateBar ()
	{
		foreach (UIAbility ability in abilityBarList)
		{
			ability.UpdateUI();
		}
	}

	[ContextMenu("Clear Tree")]
    public void ClearTree()
    {
		// Not destroying all of them
		foreach (UITreeAbility ability in abilityTreeList)
			DestroyImmediate(ability.gameObject);

		foreach (Transform child in connectionsContainer.transform)
			DestroyImmediate(child.gameObject);


        abilityTreeList = new List<UITreeAbility>();
    }

	[ContextMenu("Clear Bar")]
    public void ClearAbilityBar()
    {
        foreach (Transform child in abilityBarContainer.transform)
            Destroy(child.gameObject);

        abilityBarList = new List<UIAbility>();
    }

    //[ContextMenu("Generate LogicObject Tree Connections")]
	public GameObject CreateVisualConneciton (Vector2 start, Vector2 end)
	{

        GameObject newConnection = (GameObject)Instantiate(connectionPrefab, new Vector3(), Quaternion.identity);

        newConnection.transform.parent = abilityTreeContainer.transform;


        newConnection.transform.localScale = new Vector3(1, 1, 1);
        newConnection.transform.localPosition = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2, 0);
        newConnection.transform.GetComponent<UISprite>().SetDimensions(42, (int)Vector2.Distance(start, end));


        float angle = Mathf.Atan2(start.y - end.y, start.x - end.x) * Mathf.Rad2Deg - 90;
        newConnection.transform.localEulerAngles = new Vector3(0, 0, angle);

		return newConnection;

	}

    public void SelectAbility(int index)
    {
        for (int a = 0; a < abilityBarList.Count; a++)
        {
            if (abilityBarList[a]) abilityBarList[a].ToggleSelect(false);
        }

		if (index != -1 && index < abilityBarList.Count && abilityBarList[index])
            abilityBarList[index].ToggleSelect(true);

    }

    public void IncreaseLevel(Node node)
    {

        if (upgradeType == UpgradeType.Decreasable || logicSystem.IncreaseLevel(node) == false) 
        {
            if (failLevelSound) failLevelSound.Play();
            return;
        }

		foreach(ParamaterLevelBehavior level in paramaterLevelBehaviors)
		{
            logicSystem.SetInt(level.name, logicSystem.GetInt(level.name) + (int)level.value);
		}

        logicSystem.CalculateConditions();
		abilityManager.UpdateAbilities();
		UpdateUI();


        if (increaseLevelSound) increaseLevelSound.Play();

    }

    public void DecreaseLevel(Node node)
    {
        if (upgradeType == UpgradeType.Increasable || logicSystem.DecreaseLevel(node) == false)
        {
            if (failLevelSound) failLevelSound.Play();
            return;
        }

		foreach(ParamaterLevelBehavior level in paramaterLevelBehaviors)
		{
            logicSystem.SetInt(level.name, logicSystem.GetInt(level.name) - (int)level.value);
		}

        logicSystem.CalculateConditions();
		
		abilityManager.UpdateAbilities();
		UpdateUI();


		if (increaseLevelSound) increaseLevelSound.Play();

    }
}
