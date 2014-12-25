using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class LF_EditorWindow : EditorWindow 
{

    // Draws the paramaters, node info, and tags
    void DrawInspector()
    {

        // Draw the inspector background
        GUI.color = LogicForgeSettings.inspectorColor;
        GUI.DrawTexture(new Rect(0, 0, inspectorWidth, Screen.height), plainTexture);
        GUI.color = new Color(LogicForgeSettings.inspectorColor.r - 0.1f, LogicForgeSettings.inspectorColor.g - 0.1f, LogicForgeSettings.inspectorColor.b - 0.1f, LogicForgeSettings.inspectorColor.a);
        GUI.DrawTexture(new Rect(0, 0, 1, Screen.height), plainTexture);
        GUI.color = Color.white;


        Rect inspectorRect = new Rect(-inspectorWidth, barTexture.height, inspectorWidth + 10, Screen.height);
        if (inspectorRect.Contains(mousePos))
            mouseOverInspector = true;
        else
            mouseOverInspector = false;


        //EditorGUIUtility.AddCursorRect(new Rect(Screen.width - inspectorWidth - 5, 5, 15, Screen.height), MouseCursor.ResizeHorizontal);
        //if (new Rect(Screen.width - inspectorWidth - 7, 5, 15, Screen.height).Contains(mousePos) && Event.current.type == EventType.mouseDown)
        //    resizingInspector = true;

        //if (new Rect(Screen.width - inspectorWidth - 7, 5, 15, Screen.height).Contains(mousePos) && Event.current.type == EventType.mouseUp)
        //   resizingInspector = false;

        //if (resizingInspector)
        //   inspectorWidth = Mathf.Clamp(Screen.width - Event.current.mousePosition.x, 200, 400);


        GUILayout.BeginArea(new Rect(0, barTexture.height, inspectorWidth, Screen.height - 20));
        paramatersScrollView = GUILayout.BeginScrollView(paramatersScrollView, false, false);

        GUILayout.Space(5);

        /*
        Node selectedNode = null;

        if (selectedNodes.Count != 0 && logicSystem.nodes.Count != 0)
        {

            // Show the first node we have selected
            selectedNode = selectedNodes[0];



            GUI.color = Color.white;
            if (EditorGUIExtension.Header("Node", "Node"))
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);

                // EXPERIMENT WITH THIS SHIT
                // Show the node info
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:", GUILayout.Width(73));
                selectedNode.name = EditorGUILayout.TextArea(selectedNode.name, GUILayout.Width(inspectorWidth - 110));
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Description:", GUILayout.ExpandWidth(false));
                selectedNode.description = EditorGUILayout.TextArea(selectedNode.description, GUILayout.Width(inspectorWidth - 110));
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();


                // Find the index of the node's tags
                int tagIndex = 0;
                foreach (string tag in logicSystem.tags) 
                    if (selectedNode.tag == tag)
                        tagIndex = logicSystem.tags.IndexOf(selectedNode.tag);



                GUILayout.Label("Tag:", GUILayout.Width(72));

                // Make sure every one have the None tag
                if (logicSystem.tags.Count > 0 && logicSystem.tags[0] != "None") 
                    logicSystem.tags.Insert(0, "None");
                
                String[] tags = logicSystem.tags.ToArray();
                tagIndex = EditorGUILayout.Popup(tagIndex, tags);

                // If we have no tags then the tag should always be None
                if (logicSystem.tags.Count > 0)
                    selectedNode.tag = logicSystem.tags[tagIndex];
                else
                    selectedNode.tag = "None";

                GUILayout.EndHorizontal();


                GUILayout.Space(10);


                // Display the level
                GUILayout.BeginHorizontal();
                GUILayout.Label("Level: " + (selectedNode.level), GUILayout.Width(60));
                selectedNode.level = EditorGUILayout.IntSlider(selectedNode.level, 0, selectedNode.abilityObjects.Count - 1);
                GUILayout.EndHorizontal();


                for (int a = 0; a < selectedNode.abilityObjects.Count; a++)
                {

                    GUILayout.BeginHorizontal();

                    // Highlight the current level
                    if (selectedNode.level == a) 
                        GUI.color = Color.blue;
                    
                    if (a == 0)
                        GUILayout.Label("B:", GUILayout.ExpandWidth(false));
                    else
                        GUILayout.Label(a + ":", GUILayout.ExpandWidth(false));
                    
                    GUI.color = Color.white;

                    // 0 is for the base texture
                    if (a == 0)
                    {
                        selectedNode.baseTexture = (Texture)EditorGUILayout.ObjectField(selectedNode.baseTexture, typeof(Texture), false);
                        
                        if (GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(15))) 
                            selectedNode.abilityObjects.Add(selectedNode.abilityObjects[selectedNode.abilityObjects.Count - 1]);
                    }
                    else
                    {
                        selectedNode.abilityObjects[a] = (LogicObject)EditorGUILayout.ObjectField(selectedNode.abilityObjects[a], typeof(LogicObject), false);
                        
                        if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15))) 
                            selectedNode.abilityObjects.RemoveAt(a);
                    }


                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(5);
                
                GUILayout.EndVertical();
            }


            GUILayout.Space(10);



            if (EditorGUIExtension.Header("Conditions", "Conditions"))
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);


                
                // Create a list of parameters
                List<string> paramaterNameList = new List<string>();
                paramaterNameList.Add("Active Connections");

  
                foreach (ParameterBool boolParamater in logicSystem.boolParameters)
                    paramaterNameList.Add(boolParamater.name);
                
                foreach (ParameterInt intParamater in logicSystem.intParameters)
                    paramaterNameList.Add(intParamater.name);

                foreach (ParameterFloat floatParamater in logicSystem.floatParameters)
                    paramaterNameList.Add(floatParamater.name);
            
                foreach (ParameterString stringParamater in logicSystem.stringParameters)
                    paramaterNameList.Add(stringParamater.name);


                Condition condition = null;
                 // Display all the Condition
                for (int c=0, a=selectedNode.conditions.Count; c<a; c++)
                {

                    condition = selectedNode.conditions[c];

                    GUILayout.BeginHorizontal();


                    foreach (ParameterBool boolParamater in logicSystem.boolParameters)
                        if (boolParamater.name == condition.paramater1Name)
                            condition.paramater1Type = "Boolean";

                    foreach (ParameterInt intParamater in logicSystem.intParameters)
                        if (intParamater.name == condition.paramater1Name)
                            condition.paramater1Type = "Intiger";

                    foreach (ParameterFloat floatParamater in logicSystem.floatParameters)
                        if (floatParamater.name == condition.paramater1Name)
                            condition.paramater1Type = "Float";

                    foreach (ParameterString stringParamater in logicSystem.stringParameters)
                        if (stringParamater.name == condition.paramater1Name)
                            condition.paramater1Type = "String";



                    if (paramaterNameList.Count <= condition.paramater1Index)
                        condition.paramater1Index = 0;


                    // Display the paramaters of the condition
                    if (paramaterNameList.Count > 0)
                    {
                        condition.paramater1Index = EditorGUILayout.Popup(condition.paramater1Index, paramaterNameList.ToArray());
                        condition.paramater1Name = paramaterNameList[condition.paramater1Index];
                    }

                    // Display the condition type
                    condition.type = EditorGUILayout.Popup(condition.type, condition.types, GUILayout.Width(35));


                    // Active Connections
                    if (condition.paramater1Name == "Active Connections")
                    {
                        int tempInt = 0;
                        if (int.TryParse(condition.paramater2Value, out tempInt))
                        {
                            condition.paramater2Value = EditorGUILayout.IntField(tempInt).ToString();
                        }
                        else
                        {
                            condition.paramater2Value = "0";
                        }
                    }
                    else
                    {
                        switch (condition.paramater1Type)
                        {
                            case "Intiger":
                                int tempInt = 0;
                                if (int.TryParse(condition.paramater2Value, out tempInt))
                                {
                                    condition.paramater2Value = EditorGUILayout.IntField(tempInt).ToString();
                                }
                                else
                                {
                                    condition.paramater2Value = "0";
                                }
                                break;

                            case "Float":
                                float tempFloat = 0;
                                if (float.TryParse(condition.paramater2Value, out tempFloat))
                                {
                                    condition.paramater2Value = EditorGUILayout.FloatField(tempFloat).ToString();
                                }
                                else
                                {
                                    condition.paramater2Value = "0";
                                }
                                break;

                            case "Boolean":
                                condition.type = 0;

                                String[] boolMenu = new String[2] { "False", "True" };
                                int boolMenuIndex = (condition.paramater2Value == "False") ? 0 : 1;

                                boolMenuIndex = EditorGUILayout.Popup(boolMenuIndex, boolMenu, GUILayout.Width(100));

                                condition.paramater2Value = boolMenu[boolMenuIndex];
                                break;

                            case "String":
                                condition.paramater2Value = GUILayout.TextField(condition.paramater2Value, GUILayout.Width(100));
                                break;

                            default:
                                break;
                        }

                    }

                    if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15)))
                    {
                        selectedNode.conditions.Remove(condition);
                        GUILayout.EndHorizontal();
                        return;
                    }

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Condition", GUILayout.Height(20))) 
                { 
                    selectedNode.conditions.Add(new Condition("Intiger", "Active Connections", 0, "1", 2)); 
                }
                
                GUILayout.Space(5);
                
                GUILayout.EndVertical();
            }


        }else{


            
            
        }
        */
        //GUILayout.Space(10); //10


        if (paramaterList != null)
        {
            paramaterList.DoLayoutList();
        }
        else
        {
            UpdateParamaterList();
        }

        if (tagList != null)
        {
            tagList.DoLayoutList();
        }
        else
        {
            UpdateTagList();
        }


        GUILayout.Space(15);
        GUILayout.EndScrollView();
        GUILayout.EndArea();

    }

}
