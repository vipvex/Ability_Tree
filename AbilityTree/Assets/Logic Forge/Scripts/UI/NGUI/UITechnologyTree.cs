using UnityEngine;
using System.Collections;

public class UITechnologyTree : MonoBehaviour 
{

    public TechTreeManager techTreeManager;


    public UILabel researchRate;
    public UILabel totalResearch;
    public UILabel totalTurns;
    public UILabel technologyCount;


    void FixedUpdate()
    {

        researchRate.text = "Research Rate: " + techTreeManager.researchRate;
        totalResearch.text = "Total Research: " + techTreeManager.totalResearch;
        technologyCount.text = "Technology Count: " + techTreeManager.technologies.Count;
        totalTurns.text = "Total Turns: " + techTreeManager.turnCount;


    }
}
