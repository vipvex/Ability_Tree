using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

public partial class LF_EditorWindow : EditorWindow
{

    /// <summary>
    /// The current logic system we are editing.
    /// </summary>
	public LogicSystem logicSystem;

    public int selectedLogicSystem;
    public List<LogicSystem> projectLogicSystems;


    bool autoUpdate = false;


    /// <summary>
    /// Whether we are creating a connection.
    /// </summary>
    bool creatingConnection = false;

    /// <summary>
    /// If we are creating a connection from and too the node. 
    /// For Utility use.
    /// </summary>
    bool doubleConnection = false;


    /// <summary>
    /// A list of the nodes we have currently selected.
    /// </summary>
	List<Node> selectedNodes = new List<Node>();

    /// <summary>
    /// The copy data.
    /// </summary>
	List<Node> copiedNodes = new List<Node>();
    

    // The viewport of the editor.
    Vector2 viewPort = new Vector2(2500, 2500);
    Vector2 gridSize = new Vector2(5000, 5000);

    // The point at which we started panning.
	Vector2 panViewPort;

    // Used to pan through the viewport.
	Vector2 panMousePos;

    /// <summary>
    /// How wide the inspector should be.
    /// </summary>
    float inspectorWidth = 250;

    /// <summary>
    /// Whether we are resizing the isnpector window.
    /// </summary>
    bool resizingInspector = false;


    bool draggingNodes;


    // Whether we are doing a drag select.
	bool dragSelect;
	Rect dragSelectBox;


    // Whetehr the mouse has been clicked.
    bool mouseDown = false;
    // Whether the mouse is hovering over a node
    bool mouseOverNode = false;
    // Whether the mouse has been clicked.
    bool mouseOverInspector = false;




    // For scrolling throught the paramaters.
	Vector2 paramatersScrollView = new Vector2(30, 30);


    public GUISkin editorSkin;

    Texture windowTexture;
	Texture gridTexture;    
	Texture arrowTexture;
	Texture plainTexture;

    
	Texture barTexture;
	Texture barHighlightedTexture;
    Texture barClickedTexture;
    Texture gearTexture;


    Texture big;




    EventType inputType;

    MouseButton mouseButton;
    enum MouseButton { Left = 0, Right = 1, Middle = 2} 


    // The current position of the mouse.
    Vector2 mousePos;

    // The click position of the mouse.
    Vector2 mouseDownPos;
    
    // The starting drag position of a group of nodes
	Vector2 dragNodePos;


    float frameRate = 0;




    ReorderableList paramaterList;
    ReorderableList tagList;



	void OnEnable ()
	{

        // Get all the colors.
		LogicForgeSettings.UpdateColors();
        EditorApplication.update = LogicSystemUpdate;


        editorSkin = (GUISkin)Resources.Load("EditorSkin", typeof(GUISkin));


        // Find and load all the textures
        gridTexture = (Texture)Resources.Load("T_Grid", typeof(Texture));
        windowTexture = (Texture)Resources.Load("T_Window", typeof(Texture));     
		arrowTexture = (Texture)Resources.Load("T_Arrow", typeof(Texture));
        plainTexture = (Texture)Resources.Load("T_Plain", typeof(Texture));
        barTexture = (Texture)Resources.Load("T_BarDefault", typeof(Texture));
        barHighlightedTexture = (Texture)Resources.Load("T_BarHighlighted", typeof(Texture));
        barClickedTexture = (Texture)Resources.Load("T_BarClicked", typeof(Texture));
        gearTexture = (Texture)Resources.Load("T_Gear", typeof(Texture));
        big = (Texture)Resources.Load("Big", typeof(Texture));


	}


	void OnGUI() 
    {

		// Make sure the editor is constantly updates
		Repaint();
		DragAndDrop.AcceptDrag();


		mousePos = Event.current.mousePosition;
        mousePos.x -= inspectorWidth;
        inputType = Event.current.type;
        mouseButton = (MouseButton)Event.current.button;



        // If you select a Logic System in the inspector it will be displayed here
        if (Selection.activeObject is LogicSystem && Selection.activeObject != logicSystem)
        {
            SelectLogicSystem((LogicSystem)Selection.activeObject);
        }

		// If we don't have a LogicObject tree slected
		if (logicSystem == null){ 
			
            //logicSystem = (LogicSystem)EditorGUILayout.ObjectField(logicSystem, typeof(LogicSystem), false);
			//selectedNodes = new List<Node>();

            return;
		}


        // Draw the node editor area
        viewPort = GUI.BeginScrollView(new Rect(inspectorWidth, 0, Screen.width + 15, Screen.height), viewPort, new Rect(0, 0, gridSize.x, gridSize.y), false, false);


        // Draw a plain texture in the editor as the background
        GUI.color = LogicForgeSettings.backgroundColor;
        GUI.DrawTexture(new Rect(viewPort.x, viewPort.y, gridSize.x, gridSize.y), plainTexture);
        


        // Draw the grid
        GUI.color = LogicForgeSettings.gridColor;
        GUI.DrawTextureWithTexCoords(new Rect(0, 0, gridSize.x, gridSize.y), gridTexture, new Rect(0, 0, gridSize.x / LogicForgeSettings.gridSize, gridSize.y / LogicForgeSettings.gridSize), true);
        GUI.DrawTextureWithTexCoords(new Rect(0, 0, gridSize.x, gridSize.y), gridTexture, new Rect(0, 0, gridSize.x / LogicForgeSettings.gridSize, gridSize.y / LogicForgeSettings.gridSize), true);
        GUI.DrawTextureWithTexCoords(new Rect(0, 0, gridSize.x, gridSize.y), gridTexture, new Rect(0, 0, gridSize.x / LogicForgeSettings.gridSize, gridSize.y / LogicForgeSettings.gridSize), true);
        GUI.color = Color.white;


        DrawConnector();


        DrawAllNodes();
        SelectArea();

       
        GUI.EndScrollView();


        Inputs();
		DrawInspector();
        DrawTopMenu();






	    
        EditorUtility.SetDirty(logicSystem);

        // Record any changes to the logic system 
        Undo.RecordObject(logicSystem, "Tree");
        

        GUI.color = Color.green;
        GUI.Label(new Rect(10, Screen.height - 40, 100, 30), "FPS " + Mathf.Round((1 - frameRate / 200) * 100) / 100, EditorStyles.boldLabel);
        GUI.color = Color.white;  

	}
    
   
    void LogicSystemUpdate()
    {
        frameRate++;
        // Make sure it's called every 200 updates. Which is 2 seconds.
        if (frameRate > 200)
        {
            if (logicSystem)
            {
                logicSystem.CalculateConditions();
            }
            frameRate = 0;
         }
    }

    

	void DrawAllNodes ()
	{

        Node node = null;
        Rect nodeWindow = new Rect();
        string nodeType = "0";
        mouseOverNode = false;



        for (int n = 0, i = logicSystem.nodes.Count; n < i; n++)
        {

            // Draw the node connections
            for (int c = 0, d = logicSystem.nodes[n].connections.Count; c < d; c++)
            {

                DrawConnection(n, logicSystem.nodes[n].connections[c]);

            }

        }

        for (int n = 0, i = logicSystem.nodes.Count; n < i; n++)
		{

            node = logicSystem.nodes[n];
            nodeWindow = node.window;
            nodeType = "0";

            // Make the node the right color based on it's state
            if (node.isRoot)
                nodeType = "5";
            else if (node.activated == true && node.level == 0)
                nodeType = "3";
            else if (node.activated == true && node.level > 0)
                nodeType = "1";

            // Check if the node one of our selected nodes
            for (int s=0, j=selectedNodes.Count; s<j; s++)
            {
                if (selectedNodes[s] == node)
                {
                    nodeType += " on";
                    break;
                }
            }


            // Snapping
            //nodeWindow.x = LogicForgeSettings.gridSnapping * Mathf.Round(nodeWindow.x / LogicForgeSettings.gridSnapping);
            //nodeWindow.y = LogicForgeSettings.gridSnapping * Mathf.Round(nodeWindow.y / LogicForgeSettings.gridSnapping);

      
            DrawNode(node, nodeType);

		}
	}

    /// <summary>
    /// Draw the connection between two nodes
    /// </summary>
    /// <param name="fromNodeIndex"> The index of the from Node </param>
    /// <param name="toNodeIndex" The index of the too Node ></param>
    void DrawConnection(int fromNodeIndex, int toNodeIndex)
    {


        Vector2 fromCenter = new Vector2(logicSystem.nodes[fromNodeIndex].window.x + logicSystem.nodes[fromNodeIndex].window.width / 2, logicSystem.nodes[fromNodeIndex].window.y + logicSystem.nodes[fromNodeIndex].window.height / 2);
        Vector2 toCenter = new Vector2(logicSystem.nodes[toNodeIndex].window.x + logicSystem.nodes[toNodeIndex].window.width / 2, logicSystem.nodes[toNodeIndex].window.y + logicSystem.nodes[toNodeIndex].window.height / 2);
        
        bool overArrow = false;

        if (new Rect((fromCenter.x + toCenter.x) / 2 - 12.5f, (fromCenter.y + toCenter.y) / 2 - 20, 25, 25).Contains(mousePos + viewPort))
            overArrow = true;



        if (overArrow)
            Drawing.DrawLine(new Vector2(fromCenter.x, fromCenter.y), new Vector2(toCenter.x, toCenter.y), LogicForgeSettings.selectedNodeColor, 1.8f, LogicForgeSettings.connectionAntiAliasing);
        else
            Drawing.DrawLine(new Vector2(fromCenter.x, fromCenter.y), new Vector2(toCenter.x, toCenter.y), Color.white, 1.8f, LogicForgeSettings.connectionAntiAliasing);



        // Rotate the arrow towards the node
        float angle = Mathf.Atan2(fromCenter.y - toCenter.y, fromCenter.x - toCenter.x) * Mathf.Rad2Deg - 90;
        GUIUtility.RotateAroundPivot(angle, new Vector2((fromCenter.x + toCenter.x) / 2, (fromCenter.y + toCenter.y) / 2));


        if (overArrow)
            GUI.color = LogicForgeSettings.selectedNodeColor;

        GUI.backgroundColor = new Color(0, 0, 0, 0);
        if (GUI.Button(new Rect((fromCenter.x + toCenter.x) / 2 - 12.5f, (fromCenter.y + toCenter.y) / 2 - 20, 25, 25), arrowTexture))
        {
            logicSystem.nodes[fromNodeIndex].connections.Remove(toNodeIndex);
            return;
        }


        GUI.backgroundColor = Color.white;
        GUI.color = Color.white;
        GUI.matrix = Matrix4x4.identity;

    }

    /// <summary>
    /// Draws connections to the mouse when creating new connections.
    /// </summary>
    void DrawConnector ()
    {

        if (creatingConnection)
        {

            for (int s = 0, a = selectedNodes.Count; s < a; s++)
            {

                Drawing.DrawLine(new Vector2(selectedNodes[s].window.x + selectedNodes[s].window.width / 2, selectedNodes[s].window.y + selectedNodes[s].window.height / 2),
                                 new Vector2(viewPort.x + mousePos.x, viewPort.y + mousePos.y), Color.white, 1.8f, LogicForgeSettings.connectionAntiAliasing);


                // If we are creating a double connectio draw a second line
                if (doubleConnection)
                {

                    Vector2 mouseDir = new Vector2(viewPort.x + mousePos.x, viewPort.y + mousePos.y) - new Vector2(selectedNodes[s].window.x + selectedNodes[s].window.width / 2, selectedNodes[s].window.y + selectedNodes[s].window.height / 2);
                    mouseDir.Normalize();
                    mouseDir = new Vector2(-mouseDir.y, mouseDir.x);
                    mouseDir *= 3;

                    Drawing.DrawLine(new Vector2(selectedNodes[s].window.x + selectedNodes[s].window.width / 2, selectedNodes[s].window.y + selectedNodes[s].window.height / 2) + mouseDir,
                                     new Vector2(viewPort.x + mousePos.x, viewPort.y + mousePos.y) + mouseDir,
                                     Color.white, 1.8f, LogicForgeSettings.connectionAntiAliasing);

                }

            }

        }

    }
    
    	

    

    void DrawGrid()
    {

        for (int x = 0, a = (int)(gridSize.x / LogicForgeSettings.gridSize); x < a; x++)
        {

            Drawing.DrawLine(new Vector2(x * LogicForgeSettings.gridSize, 0), new Vector2(x * LogicForgeSettings.gridSize, gridSize.y), LogicForgeSettings.gridColor, 1, false);


            for (int y = 0, b = (int)(gridSize.y / LogicForgeSettings.gridSize); y < b; y++)
            {

                Drawing.DrawLine(new Vector2(0, y * LogicForgeSettings.gridSize), new Vector2(gridSize.x, y * LogicForgeSettings.gridSize), LogicForgeSettings.gridColor, 1, false);

            }

        }

    }




    /// <summary>
    /// Displays the inspector paramaters in a very clear way using reorderable list  
    /// </summary>
    void UpdateParamaterList()
    {

        // callects all the paramaters in the Logic System and adds them to one central list
        List<object> paramaters = new List<object>();
        foreach (ParameterInt intParam in logicSystem.intParameters) paramaters.Add(intParam);
        foreach (ParameterFloat floatParam in logicSystem.floatParameters) paramaters.Add(floatParam);
        foreach (ParameterString stringParam in logicSystem.stringParameters) paramaters.Add(stringParam);
        foreach (ParameterBool boolParam in logicSystem.boolParameters) paramaters.Add(boolParam);


        paramaterList = new ReorderableList(paramaters, typeof(object), true, true, true, true);


        paramaterList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Paramaters");
            if (GUI.Button(new Rect(rect.x + inspectorWidth - 35, rect.y, 15, 15), "+"))// editorSkin.FindStyle("")
            { 
            
            }
        };

        int listMargin = 16;
        paramaterList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {


                if (paramaterList.list[index] is ParameterInt)
                {
                    ParameterInt intParam = (ParameterInt)paramaterList.list[index];
                    rect.y += 2;
                    intParam.name = EditorGUI.TextField(new Rect(rect.x, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), intParam.name);
                    intParam.value = EditorGUI.IntField(new Rect(rect.x + inspectorWidth / 2 - listMargin + 5, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), intParam.value);
                }
                if (paramaterList.list[index] is ParameterFloat)
                {
                    ParameterFloat floatParam = (ParameterFloat)paramaterList.list[index];
                    rect.y += 2;
                    floatParam.name = EditorGUI.TextField(new Rect(rect.x, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), floatParam.name);
                    floatParam.value = EditorGUI.FloatField(new Rect(rect.x + inspectorWidth / 2 - listMargin + 5, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), floatParam.value);
                }

                if (paramaterList.list[index] is ParameterString)
                {
                    ParameterString stringParam = (ParameterString)paramaterList.list[index];
                    rect.y += 2;
                    stringParam.name = EditorGUI.TextField(new Rect(rect.x, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), stringParam.name);
                    stringParam.value = EditorGUI.TextField(new Rect(rect.x + inspectorWidth / 2 - listMargin + 5, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), stringParam.value);
                }

                if (paramaterList.list[index] is ParameterBool)
                {
                    ParameterBool boolParam = (ParameterBool)paramaterList.list[index];
                    rect.y += 2;
                    boolParam.name = EditorGUI.TextField(new Rect(rect.x, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), boolParam.name);
                    boolParam.value = EditorGUI.Toggle(new Rect(rect.x + inspectorWidth / 2 - listMargin + 5, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), boolParam.value);
                }

                if (GUI.Button(new Rect(rect.x + inspectorWidth - 15, rect.y, 15, 15), "-"))
                {

                }

            };


        paramaterList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Intiger"), false, AddParamater, "Integer");
            menu.AddItem(new GUIContent("Float"), false, AddParamater, "Float");
            menu.AddItem(new GUIContent("Bool"), false, AddParamater, "Boolean");
            menu.AddItem(new GUIContent("String"), false, AddParamater, "String");
            menu.ShowAsContext();
        };

        paramaterList.onRemoveCallback = (ReorderableList list) =>
        {

            if (paramaterList.list[list.index] is ParameterInt)
                logicSystem.intParameters.Remove((ParameterInt)paramaterList.list[list.index]);

            if (paramaterList.list[list.index] is ParameterFloat)
                logicSystem.floatParameters.Remove((ParameterFloat)paramaterList.list[list.index]);

            if (paramaterList.list[list.index] is ParameterBool)
                logicSystem.boolParameters.Remove((ParameterBool)paramaterList.list[list.index]);

            if (paramaterList.list[list.index] is ParameterString)
                logicSystem.stringParameters.Remove((ParameterString)paramaterList.list[list.index]);

            UpdateParamaterList();

        };

    }

    void AddParamater(object paramaterType)
    {
        logicSystem.AddNewParamater((string)paramaterType);
        UpdateParamaterList();
    }

    void UpdateTagList()
    {
        tagList = new ReorderableList(logicSystem.tags, typeof(string), true, true, true, true);
        tagList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Tags");
        };


        int listMargin = 18;
        tagList.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 2;
            tagList.list[index] = EditorGUI.TextField(new Rect(rect.x, rect.y, inspectorWidth / 2 - listMargin, EditorGUIUtility.singleLineHeight), (string)tagList.list[index]);
        };

        tagList.onAddCallback = (ReorderableList l) =>
        {
            tagList.list.Add("New Tag");
        };
    }



    // Used to unfocus all controlls
    void UnFocus()
    {
        // Create a fake button and focus to it.
        GUI.SetNextControlName("Yo");
        GUI.Button(new Rect(-10000, -10000, 0, 0), GUIContent.none);
        GUI.FocusControl("Yo");

    }

	void CenterViewPort (){
		viewPort = new Vector2(2500, 2500);
	}
}