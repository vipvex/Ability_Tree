using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class LogicSystem : ScriptableObject 
{

	[SerializeField] public List<Node> nodes;


    /// <summary>
    /// The new tag's name that we are about to add.
    /// </summary>
    public string newTagName = "";

    /// <summary>
    /// A list of tags used in the Logic System.
    /// </summary>
    [SerializeField] public List<string> tags;
   

    /// <summary>
    /// The new paramater's name that we are adding.
    /// </summary>
	public string newParamaterName = "";

    /// <summary>
    /// The new paramater's index type name that we are adding.
    /// </summary>
	public int newParamaterTypeIndex = 0;
	
    /// <summary>
    /// The paramater types that the Logic Systems support.
    /// </summary>
    public string[] paramaterTypes = new [] { "Integer", "Float", "String", "Boolean"};


	/// <summary>
	/// The ID of the new node that it will create.
	/// </summary>
    [SerializeField] private int newNodeID = 0;


    /// <summary>
    /// Unity, unfortunately, does not serialize Hashtables so we must store them in a seperate class
    /// so that when we reopen Unity the paramaters aren't gone.
    /// </summary>
    [SerializeField] public List<ParameterInt> intParameters;
    [SerializeField] public List<ParameterFloat> floatParameters;
    [SerializeField] public List<ParameterString> stringParameters;
    [SerializeField] public List<ParameterBool> boolParameters;



	void OnEnable ()
	{
		
		if (nodes == null)
			nodes = new List<Node>();
		
        if (tags == null)
			tags = new List<string>();

        if (intParameters == null)
            intParameters = new List<ParameterInt>();
        if (floatParameters == null)
            floatParameters = new List<ParameterFloat>();
        if (stringParameters == null)
            stringParameters = new List<ParameterString>();
        if (boolParameters == null)
            boolParameters = new List<ParameterBool>();


        if (!tags.Contains("None"))
            tags.Insert(0, "None");

        if (!tags.Contains("Ignore"))
            tags.Insert(0, "Ignore");

	}


	public bool IncreaseLevel (Node node)
	{

        if (node.level == node.abilityObjects.Count - 1 || node.abilityObjects.Count == 1 || node.activated == false)
			return false;

		node.level++;

		return true;
	}

	public bool HasDependencies (Node node)
	{
		int storedLevel = node.level;
		List<bool> oldStates = new List<bool>();
		List<bool> newStates = new List<bool>();


        for (int c = 0, a = node.connections.Count; c < a; c++)
        {
            if (nodes[node.connections[c]].activated && nodes[node.connections[c]].level > 0)
            {
                oldStates.Add(true);
            }
            else
            {
                oldStates.Add(false);
            }
        }


		node.activated = false;
		node.level = 0;
		CalculateConditions();


		for (int c = 0, a = node.connections.Count; c < a; c++)
        {
			if (nodes[node.connections[c]].activated && nodes[node.connections[c]].level > 0)
			{
				newStates.Add(true);
			}else{
				newStates.Add(false);
			}
		}


		// IF the states don't change so no other nodes depend on this
		for (int s=0; s < oldStates.Count; s++)
		{
			if (oldStates[s] != newStates[s])
			{
				node.activated = true;
				node.level = storedLevel;
				CalculateConditions();
				return true;
			}
		}

		node.activated = true;
		node.level = storedLevel;
		CalculateConditions();
		
		return false;

	}

	public bool DecreaseLevel (Node node)
	{
		if (node.level < 1 || HasDependencies(node) == true)
		{	
			return false; 
		}

		node.level--;
		CalculateConditions();
		
		return true;
	}


	public void CalculateConditions ()
	{

		// Deactivate all nodes before calculating the log
		foreach (Node node in nodes)  
			node.activated = false;
		
		foreach (Node node in nodes) 
		{
			if (node.isRoot) 
			{
				node.CheckConditions(this);	
			}
		}

	}
    /*
	public void Connect (Node fromNode, Node toNode){

		Connection fromConnection = null;
		Connection toConnection = null;

        if (fromNode == toNode)
        {
            return;
        }


		// Find the connections that correspond with our from and to nodes
		foreach (Connection connection in connections)
		{
			if (connection.ID == fromNode.ID) fromConnection = connection;
			if (connection.ID == toNode.ID) toConnection = connection;
		}

		// Check the same connection already exists
		if (toConnection.connectionIDs.Contains(fromNode.ID))
		{
			return;
		}

		toConnection.connectionIDs.Add (fromNode.ID);
		
	}
    */

    public void Connect(Node fromNode, Node toNode)
    {

        int toIndex = nodes.IndexOf(toNode);

        // We can't connect the node to itself!
        if (fromNode == toNode)
        {
            return;
        }


		// Check the same connection already exists
		if (fromNode.connections.Contains(toIndex))
		{
			return;
		}

		fromNode.connections.Add (toIndex);        
    }


	public Node AddNode ()
	{

        Node newNode = (Node)ScriptableObject.CreateInstance("Node");
        newNode.Init("LogicObject " + nodes.Count);


        newNode.name = "New Node";
        newNode.hideFlags = HideFlags.HideInHierarchy;
        AssetDatabase.AddObjectToAsset(newNode, this);


        nodes.Add((Node)newNode);

        return (Node)newNode;
	}

	public Node DuplicateNode (Node node)
	{
        Node newNode = AddNode();
        newNode.Init(node.window, node.name, node.description, node.conditions, node.baseTexture, node.abilityObjects, node.level, node.tag, node.isRoot);

        nodes.Add(newNode);

        return newNode;
	}


	public void RemoveNode (Node node)
	{

        int nodeIndex = nodes.IndexOf(node);

        for (int n = 0, a = nodes.Count; n < a; n++)
        {

            if (nodeIndex == n)
                continue;

            for (int c = 0, k = nodes[n].connections.Count; c < k; c++)
            {

                // Move over the connection indexes since we have just modefied the List
                if (nodes[n].connections[c] > nodeIndex)
                    nodes[n].connections[c]--;


                // If the node is connecting to the node we are removing remove the connection
                //if (nodeIndex == nodes[n].connections[c])

            }

            nodes[n].connections.Remove(nodeIndex);


        }


        DestroyImmediate(node, true);
        //AssetDatabase.SaveAssets();


        //string nodePath = AssetDatabase.GetAssetPath(node);
        //AssetDatabase.DeleteAsset(nodePath);
        nodes.RemoveAt(nodeIndex);


	}


	public void AddNewParamater (string paramaterType)
	{

        switch (paramaterType)
		{
		    case "Integer":

                intParameters.Add(new ParameterInt("New Integer", 0));
			    break;
            case "Float":

                floatParameters.Add(new ParameterFloat("New Float", 0.0f));
                break;
		    case "String":

                stringParameters.Add(new ParameterString("New String", ""));
			    break;
		    case "Boolean":

                boolParameters.Add(new ParameterBool("New Boolean", false));
			    break;
		}
        
	}


	/// <summary>
	/// How many dependecies does a node have
	/// </summary>
	/// <returns>The count.</returns>
	/// <param name="node">Node.</param>
	public int DependenciesCount(Node node)
	{
		int nodeIndex = nodes.IndexOf(node);
		int depenCount = 0;
		bool hasRootDependency = false;

        for (int c = 0, a = node.connections.Count; c < a; c++)
		{
			if (nodes[node.connections[c]].activated == true &&
                nodes[node.connections[c]].level > 0 && 
			    ActiveConnectionsCount(nodes[node.connections[c]]) == 1)
			{
				depenCount++;
                if (nodes[node.connections[c]].isRoot)
					hasRootDependency = true;
			}
		}

		//Debug.Log (depenCount);
		// if the node is dependant apon only 1 other node and that node is a root node then the dependencie does not exist
		//if (depenCount == 1 && hasRootDependency) 
			//depenCount = 0;

		return depenCount;
	}


   	/// <summary>
   	/// How many active connections does a node have
   	/// </summary>
   	/// <returns>The count.</returns>
   	/// <param name="node">Node.</param>
    public int ActiveConnectionsCount(Node node)
    {
        int nodeIndex = nodes.IndexOf(node);
		int activeConCount = 0;

        for (int c = 0, a = node.connections.Count; c < a; c++)
		{
            if (nodes[node.connections[c]].activated == true && nodes[node.connections[c]].level > 0)
			{
				activeConCount++;
			}
		}
		return activeConCount;
    }

	// Check if this node has connections to other nodes and if they depend on it being active
	public int LeveledUpConnections(Node node)
	{
		int connectionCount = 0;
        for (int c = 0, a = node.connections.Count; c < a; c++)
        {

		    if (nodes[node.connections[c]].level > 0)
			    connectionCount++;

		}

		return connectionCount;
	}

	/// <summary>
	/// Gets the upgraded abilities from this LogicObject System.
	/// </summary>
	/// <returns>The upgraded abilities.</returns>
    public LogicObject[] GetUpgradedAbilities()
    {

        List<LogicObject> abilities = new List<LogicObject>();

        // Loop through all the nodes and add all the upgraded onces to the list of abilities
        foreach (Node node in nodes)
        {
            if (node.activated && node.abilityObjects.Count > 0 && node.abilityObjects[node.level] != null)
                abilities.Add(node.abilityObjects[node.level]);
        }

        return abilities.ToArray();

    }


	public int GetInt(string name)
	{
		foreach (ParameterInt intParamater in intParameters)
            if (intParamater.name == name) 
                return intParamater.value;

        Debug.Log(this.name + " does not contain int paramater " + name + "!");
        return 0;
	}

    public float GetFloat(string name)
    {
        foreach (ParameterFloat floatParamater in floatParameters)
            if (floatParamater.name == name) 
                return floatParamater.value;

        Debug.Log(this.name + " does not contain float paramater " + name + "!");
        return 0;
    }

    public string GetString(string name)
    {
        foreach (ParameterString stringParamater in stringParameters)
            if (stringParamater.name == name) 
                return "";

        Debug.Log(this.name + " does not contain string paramater " + name + "!");
        return "";
    }

    public bool GetBool(string name)
    {
        foreach (ParameterBool boolParamater in boolParameters)
            if (boolParamater.name == name) 
                return boolParamater.value;

        Debug.Log(this.name + " does not contain bool paramater " + name + "!");
        return false;
    }



    public void SetInt(string name, int value)
    {
        foreach (ParameterInt intParamater in intParameters)
        {
            if (intParamater.name == name)
            {
                intParamater.value = value;
                return;
            }
        }
    }

    public void SetFloat(string name, float value)
    {
        foreach (ParameterFloat floatParamater in floatParameters)
        {
            if (floatParamater.name == name)
            {
                floatParamater.value = value;
                return;
            }
        }
    }

    public void SetString(string name, string value)
    {
        foreach (ParameterString stringParamater in stringParameters)
        {
            if (stringParamater.name == name)
            {
                stringParamater.value = value;
                return;
            }
        }
    }

    public void SetBool(string name, bool value)
    {
        foreach (ParameterBool boolParamater in boolParameters)
        {
            if (boolParamater.name == name)
            {
                boolParamater.value = value;
                return;
            }
        }
    }
	
}
