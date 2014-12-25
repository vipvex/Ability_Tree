using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This script generates a ability tree using ether NGUI or Unity GUI.
/// </summary>
public class UITreeGenerator : MonoBehaviour 
{

    /// <summary>
    /// The object that the generated nodes will be paranted too.
    /// </summary>
    public Transform treeContainer;

    /// <summary>
    /// The list of nodes that have been generated
    /// </summary>
    public List<UITreeNode> treeNodeList;



    /// <summary>
    /// The default prefab that will be generated. 
    /// If no tag based node prefabs exist the script will use this object.
    /// </summary>
    public GameObject defaultTreeNodePrefab;

    /// <summary>
    /// Used to generate different looking nodes based on their tag.
    /// </summary>
    public List<TagNodePrefab> tagNodePrefabs;
    [System.Serializable]
    public struct TagNodePrefab
    {
        public string tag;
        public GameObject prefab;
    }


    /// <summary>
    /// The connection prefab to generate between the nodes.
    /// </summary>
    public GameObject connectionPrefab;

    /// <summary>
    /// The object that the generated connections will be paranted too.
    /// </summary>
    private Transform connectionContainer;


    /// <summary>
    /// Whether you want to generate the tree each time you change it or just update it's visuals.
    /// </summary>
    public InitializeMethod treeInitMethod;
    public enum InitializeMethod { None, Generate, Update }
    


    /// <summary>
    /// The tree system we will be looking at to generate and update all the nodes.
    /// </summary>
    public LogicSystem treeSystem;



    public float nodeSide = 75;
    public float uiTreeNodeScale = 55;
    public Vector2 abilityPositionScale = new Vector3(1, 1);


    void Start()
    {

        if (treeInitMethod == InitializeMethod.Generate)
        {
            GenerateTree();
        }

        if (treeInitMethod == InitializeMethod.Update)
        {
            UpdateTreeUI();
        }

    }


    [ContextMenu("Generate Tree UI")]
    public void GenerateTree()
    {

        // Create a new object and rename it
        connectionContainer = ((GameObject)Instantiate(new GameObject())).transform;
        connectionContainer.name = "ConnectionContainer";
        connectionContainer.parent = treeContainer.transform;


        // Populate the tree with abilities
        foreach (Node node in treeSystem.nodes)
        {


            if (defaultTreeNodePrefab == null && tagNodePrefabs.Count == 0)
                return;



            GameObject newTreeNode = (GameObject)Instantiate(defaultTreeNodePrefab, new Vector3(), Quaternion.identity);



            newTreeNode.transform.parent = treeContainer.transform;
            newTreeNode.transform.localScale = new Vector3(1, 1, 1);
            newTreeNode.transform.localPosition = new Vector3(((5000 - node.window.x) * -(uiTreeNodeScale / nodeSide)) * abilityPositionScale.x, ((node.window.y - 5000) * -(uiTreeNodeScale / nodeSide)) * abilityPositionScale.y, 0);


            newTreeNode.GetComponent<UITreeNode>().Initialize(treeSystem, node, this);


            treeNodeList.Add(newTreeNode.GetComponent<UITreeNode>());


            for (int c = 0, a = node.connections.Count; c < a; c++)
            {

                GameObject connection = GenerateNodeConnection(new Vector2(newTreeNode.transform.localPosition.x, newTreeNode.transform.localPosition.y),
                                                               new Vector2((5000 - treeSystem.nodes[node.connections[c]].window.x) * -(uiTreeNodeScale / nodeSide) * abilityPositionScale.x, (treeSystem.nodes[node.connections[c]].window.y - 5000) * -(uiTreeNodeScale / nodeSide) * abilityPositionScale.y));
                connection.transform.parent = connectionContainer.transform;

            }
        }
    }

    public GameObject GenerateNodeConnection (Vector2 start, Vector2 end)
    {

        GameObject newConnection = (GameObject)Instantiate(connectionPrefab, new Vector3(), Quaternion.identity);

        newConnection.transform.parent = treeContainer.transform;


        newConnection.transform.localScale = new Vector3(1, 1, 1);
        newConnection.transform.localPosition = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2, 0);
        newConnection.transform.GetComponent<UISprite>().SetDimensions(42, (int)Vector2.Distance(start, end));


        float angle = Mathf.Atan2(start.y - end.y, start.x - end.x) * Mathf.Rad2Deg - 90;
        newConnection.transform.localEulerAngles = new Vector3(0, 0, angle);

        return newConnection;

    }

    [ContextMenu("Update Tree UI")]
    public void UpdateTreeUI()
    {

        foreach (UITreeNode uiNode in treeNodeList)
            uiNode.UpdateUI();

    }

    [ContextMenu("Clear Tree UI")]
    public void ClearTree()
    {

        foreach (UITreeNode node in treeNodeList)
            DestroyImmediate(node.gameObject);

        foreach (Transform child in connectionContainer.transform)
            DestroyImmediate(child.gameObject);


        DestroyImmediate(treeContainer.gameObject);
        DestroyImmediate(connectionContainer.gameObject);


        treeNodeList = new List<UITreeNode>();

    }

}