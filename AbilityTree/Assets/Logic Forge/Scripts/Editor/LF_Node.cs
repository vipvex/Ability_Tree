using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class LF_EditorWindow : EditorWindow 
{

    void DrawNode(Node node, string nodeType)
    {

        // Draw the background with the correcct type
        GUI.Box(node.window, "", editorSkin.FindStyle("flow node " + nodeType));


        // Draw the node textures
        if (node.level == 0 && node.baseTexture != null)
        {
            GUI.DrawTexture(new Rect(node.window.x + 3, node.window.y + 3, 69, 70), node.baseTexture);
        }


        if (node.abilityObjects[node.level] && node.abilityObjects[node.level].texture)
        {
            GUI.DrawTexture(new Rect(node.window.x + 3, node.window.y + 3, 69, 70), ((LogicObject)node.abilityObjects[node.level]).texture);
        }


        // Display name / description
        if (LogicForgeSettings.showNodeName || LogicForgeSettings.showNodeDescription)
        {

            // Draw the Node name
            GUIContent nodeName = new GUIContent(node.name);

            if (LogicForgeSettings.showNodeLevels)
                nodeName.text += " [" + node.level + "]";

            if (LogicForgeSettings.showNodeDescription && node.description != "")
                nodeName.text += "\n" + node.description;


            // Calculate the size of the text so we can place it in the middle
            Vector2 textSize = EditorStyles.miniButton.CalcSize(nodeName);
            Rect namePos = new Rect(node.window.x - textSize.x / 2 + node.window.width / 2 + 3, node.window.y - textSize.y, textSize.x, textSize.y + 4);


            GUI.Label(namePos, nodeName.text, EditorStyles.miniLabel);

        }





        // If the mouse is hovering over the node
        if (node.window.Contains(mousePos + viewPort) && !mouseOverInspector)
        {

            mouseOverNode = true;

            if (mouseButton == MouseButton.Left && inputType == EventType.mouseDown && !creatingConnection)
            {

                draggingNodes = true;

                // Shift/ctrl selecting
                if (Event.current.modifiers != EventModifiers.Control && Event.current.modifiers != EventModifiers.Shift)
                {
                    if (!selectedNodes.Contains(node))
                    {
                        selectedNodes = new List<Node>();
                    }
                }

                // Select the node
                if (!selectedNodes.Contains(node))
                {
                    selectedNodes.Add(node);
                    Selection.activeObject = node;
                    Selection.objects = selectedNodes.ToArray();
                }

            }

            // Connect the selected nodes to the clicked node
            if (creatingConnection && mouseButton == MouseButton.Left && inputType == EventType.mouseUp)
            {
                for (int s = 0, a = selectedNodes.Count; s < a; s++)
                {
                    logicSystem.Connect(selectedNodes[s], node);
                    if (doubleConnection)
                        logicSystem.Connect(node, selectedNodes[s]);
                }
            }


            // Context menu
            if (mouseButton == MouseButton.Right && inputType == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Connect"), false, CreateConnection);
                menu.AddItem(new GUIContent("Toggle Root"), false, ToggleAsRoot);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Increase Level"), false, IncreaseNodeLevels);
                menu.AddItem(new GUIContent("Decrease Level"), false, DecreaseNodeLevels);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy"), false, CopyNodes);
                menu.AddItem(new GUIContent("Paste"), false, PasteNodes);
                menu.AddItem(new GUIContent("Duplicate"), false, DuplicatedNodes);
                menu.AddItem(new GUIContent("Delete"), false, DeleteSelectedNodes);
                menu.ShowAsContext();
            }

        }


        // Check if we are draggin any Assets into the window
        for (int a = 0; a < DragAndDrop.objectReferences.Length; a++)
            if (!(DragAndDrop.objectReferences[a] is Texture) && !(DragAndDrop.objectReferences[a] is LogicObject))
                return;


        // Drag and drop the selected objects from the Preject folder
        if (inputType == EventType.DragUpdated || inputType == EventType.DragPerform)
        {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (inputType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                for (int a = 0; a < DragAndDrop.objectReferences.Length; a++)
                {
                    if (DragAndDrop.objectReferences[a] is LogicObject)
                        node.abilityObjects.Add((LogicObject)DragAndDrop.objectReferences[a]);

                    if (DragAndDrop.objectReferences[a] is Texture)
                        node.baseTexture = (Texture)DragAndDrop.objectReferences[a];
                }
            }

            Event.current.Use();
        }


    }

}