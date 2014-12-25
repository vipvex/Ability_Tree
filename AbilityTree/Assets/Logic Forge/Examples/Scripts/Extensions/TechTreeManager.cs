using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// An example script of how to implement a tech tree with for a turn based game.
/// </summary>
public class TechTreeManager : MonoBehaviour 
{

    public LogicSystem techTree;
    public UITreeGenerator techTreeGenerator;

    /// <summary>
    /// The list of technologies that are qued up to be researched.
    /// </summary>
    public List<Node> technologyQue = new List<Node>();

    /// <summary>
    /// The technologies that have been researched
    /// </summary>
    public List<Technology> technologies;


    /// <summary>
    /// The total amount of research
    /// </summary>
    public float totalResearch = 0;

    /// <summary>
    /// The total turn count.
    /// </summary>
    public int turnCount = 0;

    /// <summary>
    /// The research speed.
    /// </summary>
    public float researchRate = 10;


    void Start()
    {
        if (techTree)
        {
            techTree = (LogicSystem)Instantiate(techTree);
            foreach (Node node in techTree.nodes) if (node.abilityObjects.Count >= 2 && node.abilityObjects[1]) node.abilityObjects[1] = (LogicObject)Instantiate(node.abilityObjects[1]);
            techTree.CalculateConditions();
            
            techTreeGenerator.treeSystem = techTree;
            techTreeGenerator.GenerateTree();
        }
    }


    public void AddTechToQue(Node node)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            technologyQue.Add(node);
        else
        {
            technologyQue = new List<Node>();
            technologyQue.Add(node);
        }
    }


    public void NextTurn()
    {

        if (technologyQue.Count == 0)
            return;

        Technology technology = (Technology)technologyQue[0].abilityObjects[1];

        if (!technologies.Contains(technology))
        {
            technology.reseachedAmount += researchRate;
            totalResearch += researchRate;
        }

        if (technology.reseachedAmount >= technology.researchCost && !technologies.Contains(technology))
        {
            techTree.IncreaseLevel(technologyQue[0]);
            technologies.Add(technology);
            technologyQue.RemoveAt(0);
        }

        techTree.CalculateConditions();
        techTreeGenerator.UpdateTreeUI();

        turnCount++;

    }

}
