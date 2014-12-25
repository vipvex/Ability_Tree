using System;
using UnityEditor;
using UnityEngine;


public class LogicForgeSettings : EditorWindow 
{

    public static float updateRate = 1;
    public static int gridSize = 12;
    public static int gridSnapping = 12;

    public static Color gridColor = Color.black;
    public static Color backgroundColor = Color.gray;
    public static Color inspectorColor = Color.gray;


    public static Color defaultNodeColor = Color.white;
    public static Color rootNodeColor = Color.white;
    public static Color selectedNodeColor = Color.blue;
    public static Color activeNodeColor = Color.green;
    public static Color availableNodeColor = Color.green;


    public static bool showNodeName = true;
    public static bool showNodeLevels = false;
    public static bool showNodeDescription = false;
    public static bool connectionAntiAliasing = false;



    void OnEnable()
    {
		UpdateColors();
    }

    void OnDisable()
    {

        EditorPrefs.SetInt("gridSize", gridSize);


        EditorPrefs.SetBool("showNodeName", showNodeName);
        EditorPrefs.SetBool("showNodeLevels", showNodeLevels);
        EditorPrefs.SetBool("showNodeDescription", showNodeDescription);
        EditorPrefs.SetBool("connectionAntiAliasing", connectionAntiAliasing);



        SetColor("gridColor", gridColor);
        SetColor("backgroundColor", backgroundColor);
        SetColor("inspectorColor", inspectorColor);


        SetColor("rootNodeColor", rootNodeColor);
        SetColor("defaultNodeColor", defaultNodeColor);
        SetColor("selectedNodeColor", selectedNodeColor);
        SetColor("activeNodeColor", activeNodeColor);  
		SetColor("availableNodeColor", availableNodeColor);  
    }

	public static void UpdateColors ()
	{

        gridSize = EditorPrefs.GetInt("gridSize");

        showNodeName = EditorPrefs.GetBool("showNodeName");
        showNodeLevels = EditorPrefs.GetBool("showNodeLevels");
        showNodeDescription = EditorPrefs.GetBool("showNodeDescription");
        connectionAntiAliasing = EditorPrefs.GetBool("connectionAntiAliasing");

		gridColor = GetColor("gridColor");
		backgroundColor = GetColor("backgroundColor");
		inspectorColor = GetColor("inspectorColor");

        rootNodeColor = GetColor("rootNodeColor");
        defaultNodeColor = GetColor("defaultNodeColor");
		selectedNodeColor = GetColor("selectedNodeColor");
		availableNodeColor = GetColor("availableNodeColor");   
		activeNodeColor = GetColor("activeNodeColor"); 
	}

    void OnGUI()
    {

        showNodeName = GUILayout.Toggle(showNodeName, "Show Node Names");
        showNodeLevels = GUILayout.Toggle(showNodeLevels, "Show Node Levels");
        showNodeDescription = GUILayout.Toggle(showNodeDescription, "Show Node Descritions");
        connectionAntiAliasing = GUILayout.Toggle(connectionAntiAliasing, "Connection Anti-Aliasing");


        //Editor
        backgroundColor = EditorGUILayout.ColorField("Background Color:", backgroundColor);
        gridColor = EditorGUILayout.ColorField("Grid Color:", gridColor);
        inspectorColor = EditorGUILayout.ColorField("Inpsector Color:", inspectorColor);

        defaultNodeColor = EditorGUILayout.ColorField("Default Node Color:", defaultNodeColor);
        rootNodeColor = EditorGUILayout.ColorField("Root Node Color:", rootNodeColor);
        selectedNodeColor = EditorGUILayout.ColorField("Selected Node Color:", selectedNodeColor);
        activeNodeColor = EditorGUILayout.ColorField("Active Node Color:", activeNodeColor);
		availableNodeColor = EditorGUILayout.ColorField("Available Node Color:", availableNodeColor);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size");
        gridSize = EditorGUILayout.IntSlider(gridSize, 0, 1000);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Snapping");
        gridSnapping = EditorGUILayout.IntSlider(gridSnapping, 0, 30);
        GUILayout.EndHorizontal();
    }

    void SetColor(string prefName, Color color)
    {
        EditorPrefs.SetFloat(prefName + "_R", color.r);
        EditorPrefs.SetFloat(prefName + "_G", color.g);
        EditorPrefs.SetFloat(prefName + "_B", color.b);
        EditorPrefs.SetFloat(prefName + "_A", color.a);
    }

    public static Color GetColor(string prefName)
    {
        return new Color(
           EditorPrefs.GetFloat(prefName+ "_R", 1.0f),
           EditorPrefs.GetFloat(prefName+ "_G", 1.0f),
           EditorPrefs.GetFloat(prefName + "_B", 1.0f),
           EditorPrefs.GetFloat(prefName+ "_A", 1.0f)
        );
    }

}
