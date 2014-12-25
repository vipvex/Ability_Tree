using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node : ScriptableObject
{

    
    public string name = "";
    public string description = "";


    public string key = "";
    public string tag = "";


    public int level = 0;



    public Rect window;
    public Texture baseTexture;


	public bool activated = false;
	public bool isRoot = false;


    public List<int> connections;
    public List<Condition> conditions;

	public List<LogicObject> abilityObjects;



    public void OnEnabled()
    {
        hideFlags = HideFlags.HideInHierarchy;
    }




	public void CheckConditions (LogicSystem logicSystem)
	{

		// If the node has no conditions then it is automatically set to true
		if (conditions.Count == 0){

			if (activated == false)
			{
				activated = true;
                for (int c = 0, a = connections.Count; c < a; c++)
                {
                    logicSystem.nodes[connections[c]].CheckConditions(logicSystem);
                }
            }

			return;
		}


        Condition condition = null;
		int trueConditions = 0;
        for (int c = 0, a = conditions.Count; c < a; c++)
		{
            condition = conditions[c];
			if (condition.paramater1Name == "Active Connections")
			{
                // Get how many leveled up connectiosn this node has
                int leveledUpConnections = logicSystem.LeveledUpConnections(this);
				int paramValue = int.Parse(condition.paramater2Value);

				if (condition.types[condition.type].ToString() == "==")
					if (leveledUpConnections == paramValue) trueConditions++;
				
				if (condition.types[condition.type].ToString() == "<=")
					if (leveledUpConnections <= paramValue) trueConditions++;
				
				if (condition.types[condition.type].ToString() == ">=")
					if (leveledUpConnections >= paramValue) trueConditions++;
				
				if (condition.types[condition.type].ToString() == "<")
					if (leveledUpConnections < paramValue) trueConditions++;
				
				if (condition.types[condition.type].ToString() == ">")
					if (leveledUpConnections > paramValue) trueConditions++;
				
				if (condition.types[condition.type].ToString() == "!=")
					if (leveledUpConnections != paramValue) trueConditions++;

				continue;
			}


            // If there are is a invalid paramater then skip this iteration
			if (condition.paramater2Value == "" || condition.paramater1Name == "") //  LogicSystem.parameters.Contains(condition.paramater1Name) == false
			    continue;


            // Strings
            if (condition.paramater1Type == "String")
            {
                if (condition.types[condition.type].ToString() == "==")
                {
                    if (condition.paramater2Value == logicSystem.GetString(condition.paramater1Name))
                    {
                        trueConditions++;
                    }
                }
            }
			
			// Integers
            if (condition.paramater1Type == "Intiger")
			{
                switch (condition.types[condition.type].ToString())
                {
                    case "==":
                        if (logicSystem.GetInt(condition.paramater1Name) == int.Parse(condition.paramater2Value)) 
                            trueConditions++;
                        break;

                    case "<=":
                        if (logicSystem.GetInt(condition.paramater1Name) <= int.Parse(condition.paramater2Value)) 
                            trueConditions++;
                        break;

                    case ">=":
                        if (logicSystem.GetInt(condition.paramater1Name) >= int.Parse(condition.paramater2Value)) 
                            trueConditions++;
                        break;

                    case "<":
                        if (logicSystem.GetInt(condition.paramater1Name) < int.Parse(condition.paramater2Value)) 
                            trueConditions++;
                        break;

                    case ">":
                        if (logicSystem.GetInt(condition.paramater1Name) > int.Parse(condition.paramater2Value)) 
                            trueConditions++;
                        break;

                    case "!=":
                        if (logicSystem.GetInt(condition.paramater1Name) != int.Parse(condition.paramater2Value)) 
                            trueConditions++;
                        break;

                    default:
                        break;
                }
			}

            // Floats
            if (condition.paramater1Type == "Float")
            {
                switch (condition.types[condition.type].ToString())
                {
                    case "==":
                        if (logicSystem.GetFloat(condition.paramater1Name) == float.Parse(condition.paramater2Value))
                            trueConditions++;
                        break;

                    case "<=":
                        if (logicSystem.GetFloat(condition.paramater1Name) <= float.Parse(condition.paramater2Value))
                            trueConditions++;
                        break;

                    case ">=":
                        if (logicSystem.GetFloat(condition.paramater1Name) >= float.Parse(condition.paramater2Value))
                            trueConditions++;
                        break;

                    case "<":
                        if (logicSystem.GetFloat(condition.paramater1Name) < float.Parse(condition.paramater2Value))
                            trueConditions++;
                        break;

                    case ">":
                        if (logicSystem.GetFloat(condition.paramater1Name) > float.Parse(condition.paramater2Value))
                            trueConditions++;
                        break;

                    case "!=":
                        if (logicSystem.GetFloat(condition.paramater1Name) != float.Parse(condition.paramater2Value))
                            trueConditions++;
                        break;

                    default:
                        break;
                }
            }
			
			// Booleans
            if (condition.paramater1Type == "Boolean")
			{				
				if (condition.types[condition.type].ToString() == "==")
                    if (logicSystem.GetBool(condition.paramater1Name) == bool.Parse(condition.paramater2Value)) trueConditions++;
				
			}
		}

		if (trueConditions == conditions.Count)
		{
			if (activated == false)
			{
				activated = true;
                for (int c = 0, a = connections.Count; c < a; c++)
                {
                    logicSystem.nodes[connections[c]].CheckConditions(logicSystem);
                }
			}else
				activated = true;
		}else
			activated = false;

	}


    public void Init(string name)
    {
        this.name = name;
        baseTexture = null;
        connections = new List<int>();
        conditions = new List<Condition>();
        abilityObjects = new List<LogicObject>();
        abilityObjects.Add(null);
    }
	
	public void Init(Rect window, string name)
	{
		this.window = window;
		this.name = name;
        baseTexture = null;
        connections = new List<int>();
        conditions = new List<Condition>();
		abilityObjects = new List<LogicObject>();
		abilityObjects.Add(null);
	}

    public void Init(Rect window, string name, string description, List<Condition> conditions, Texture baseTexture, List<LogicObject> abilityObjects, int level, string tag, bool isRoot)
    {
        this.window = window;
        this.name = name;
        this.description = description;
        this.baseTexture = baseTexture;

        connections = new List<int>();
        this.conditions = new List<Condition>();
        foreach (Condition condition in conditions)
            this.conditions.Add(new Condition(condition.paramater1Type, condition.paramater1Name, condition.paramater1Index, condition.paramater2Value, condition.type));


        this.abilityObjects = new List<LogicObject>();
        foreach (LogicObject ability in abilityObjects)
            this.abilityObjects.Add(ability);

        this.level = level;
        this.tag = tag;
        this.isRoot = isRoot;
    }
	
}