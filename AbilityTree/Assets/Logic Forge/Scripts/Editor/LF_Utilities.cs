using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class LF_EditorWindow : EditorWindow
{

    // Creates a new node at the mouse cursor
    void AddNewNode()
    {

        Node newNode = logicSystem.AddNode();
        newNode.window = new Rect(viewPort.x + mousePos.x - 35, viewPort.y + mousePos.y - 35, 75, 75);
     

        // If this is our first node then make it the root
        if (logicSystem.nodes.Count == 1)
            newNode.isRoot = true;

        // Select the new node
        selectedNodes = new List<Node>();
        selectedNodes.Add(newNode);

    }

    // Called to create new connections
    void CreateConnection()
    {
        creatingConnection = true;
    }

    // Toggles the selected 
    void ToggleAsRoot()
    {
        foreach (Node node in selectedNodes)
        {
            node.isRoot = !node.isRoot;
        }
    }

    // Coppies the selected nodes
    void CopyNodes()
    {
        copiedNodes = new List<Node>();
        for (int n = 0; n < selectedNodes.Count; n++)
        {
            copiedNodes.Add(selectedNodes[n]);
        }
    }

    // Pastes the nodes
    void PasteNodes()
    {
        List<Node> pastedNodes = new List<Node>();

        for (int n = 0; n < copiedNodes.Count; n++)
        {
            Node newNode = logicSystem.DuplicateNode(copiedNodes[n]);
            pastedNodes.Add(newNode);
        }
        selectedNodes = pastedNodes;
    }

    // Duplicates the selected nodes
    void DuplicatedNodes()
    {
        Debug.Log("Trying to dup");
        List<Node> duplicatedNodes = new List<Node>();
        for (int n = 0; n < selectedNodes.Count; n++)
        {
            Node newNode = logicSystem.DuplicateNode(selectedNodes[n]);
            duplicatedNodes.Add(newNode);
        }
        selectedNodes = duplicatedNodes;

    }

    // Removes all nodes from the selected list
    void DeleteSelectedNodes()
    {
        for (int s = 0; s < selectedNodes.Count; s++)
        {
            logicSystem.RemoveNode(selectedNodes[s]);
        }
    }

    // Increses the selected nodes level
    void IncreaseNodeLevels()
    {
        for (int s = 0; s < selectedNodes.Count; s++)
        {
            logicSystem.IncreaseLevel(selectedNodes[s]);
        }
    }

    // Decreases the selected nodes level
    void DecreaseNodeLevels()
    {
        for (int s = 0; s < selectedNodes.Count; s++)
        {
            logicSystem.DecreaseLevel(selectedNodes[s]);
        }
    }

    void SelectLogicSystem(LogicSystem newLogicSystem)
    {

        logicSystem = newLogicSystem;
        Selection.activeObject = newLogicSystem;

        selectedNodes = new List<Node>();

        UnityEngine.Object[] list = LF_Editor.GetAssetsOfType(typeof(LogicSystem), ".asset");

        projectLogicSystems = new List<LogicSystem>();
        for (int l = 0, a = list.Length; l < a; l++) projectLogicSystems.Add((LogicSystem)list[l]);

        if (projectLogicSystems.Contains(newLogicSystem) == false)
        {
            projectLogicSystems.Add(newLogicSystem);
        }

        selectedLogicSystem = projectLogicSystems.IndexOf(newLogicSystem);


        UpdateParamaterList();
        UpdateTagList();

    }

}
