using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class LF_EditorWindow : EditorWindow
{

    void DrawTopMenu()
    {

        GUI.Box(new Rect(0, 0, Screen.width, 15), "", EditorStyles.toolbar);


        List<string> logicSystemNames = new List<string>();
        for (int l = 0, a = projectLogicSystems.Count; l < a; l++)
            logicSystemNames.Add(projectLogicSystems[l].name);
        logicSystemNames.Add("");
        logicSystemNames.Add("Menu");


        selectedLogicSystem = EditorGUI.Popup(new Rect(5, 0, 125, barTexture.height), selectedLogicSystem, logicSystemNames.ToArray(), EditorStyles.toolbarDropDown);


        if (logicSystemNames[selectedLogicSystem] == "Menu")
        {
            logicSystem = null;
            selectedLogicSystem = 0;
        }


        if (logicSystem != projectLogicSystems[selectedLogicSystem])
        {
            SelectLogicSystem(projectLogicSystems[selectedLogicSystem]);
        }


        // Settings
        if (GUI.Button(new Rect(Screen.width - barTexture.height - 5, 0, barTexture.height + 2, barTexture.height), barClickedTexture, EditorStyles.toolbarButton))
            EditorWindow.GetWindow<LogicForgeSettings>();


        GUI.DrawTexture(new Rect(Screen.width - barTexture.height - 1, 2, gearTexture.width + 2, gearTexture.height + 2), gearTexture);

        //if (selectedNodes.Count > 0 && selectedNodes[0] != null)
        //  GUI.Label(new Rect(Screen.width - 294, 1, 296, barTexture.height), "Node Inspector");
        //else
        //GUI.Label(new Rect(Screen.width - 294, 1, 296, barTexture.height), "Settings ");


        if (GUI.Button(new Rect(Screen.width - 182, 0, 80, barTexture.height), "Update", EditorStyles.toolbarButton))
        {
            logicSystem.CalculateConditions();
            Debug.Log("Updateed");
        }
        autoUpdate = GUI.Toggle(new Rect(Screen.width - 102, 0, 80, barTexture.height), autoUpdate, "Auto Update", EditorStyles.toolbarButton);



    }
}
