using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class LF_EditorWindow : EditorWindow
{

    // Displayes the drag select.
    void SelectArea()
    {

        if (dragSelect == true)
        {

            // We can't be creating a 
            if (creatingConnection == true)
                dragSelect = false;


            Rect selectionBoxPos = new Rect(dragSelectBox.x, dragSelectBox.y, dragSelectBox.width + -(mouseDownPos.x - mousePos.x), dragSelectBox.height + -(mouseDownPos.y - mousePos.y));
            // Reverse the rect if the mouse is behind the drag point
            if (selectionBoxPos.width < -5)
            {
                selectionBoxPos.x = Event.current.mousePosition.x;
                selectionBoxPos.width = dragSelectBox.x - Event.current.mousePosition.x;
            }
            if (selectionBoxPos.height < -5)
            {
                selectionBoxPos.y = Event.current.mousePosition.y;
                selectionBoxPos.height = dragSelectBox.y - Event.current.mousePosition.y;
            }

            // Check if any of the nodes or inside the drag select rectangle.
            if (Event.current.type == EventType.layout && mouseOverInspector == false)
            {
                selectedNodes = new List<Node>();
                foreach (Node node in logicSystem.nodes)
                {
                    if (selectionBoxPos.Overlaps(node.window))
                    {
                        selectedNodes.Add(node);
                    }
                }
                Selection.objects = selectedNodes.ToArray();
            }


            // Draw the drag select box texture
            GUI.color = new Color(0.6f, 0.7f, 1f, 0.125f);
            GUI.DrawTexture(selectionBoxPos, plainTexture);

            
            // Draw outline around the box
            GUI.color = new Color(0.6f, 0.7f, 1f, 0.3f);
            // Top
            GUI.DrawTexture(new Rect(selectionBoxPos.x, selectionBoxPos.y, selectionBoxPos.width, 1), plainTexture);
            // Left 
            GUI.DrawTexture(new Rect(selectionBoxPos.x, selectionBoxPos.y + 1, 1, selectionBoxPos.height - 1), plainTexture);
            // Right
            GUI.DrawTexture(new Rect(selectionBoxPos.x + selectionBoxPos.width - 1, selectionBoxPos.y + 1, 1, selectionBoxPos.height - 1), plainTexture);
            // Bottom
            GUI.DrawTexture(new Rect(selectionBoxPos.x + 1, selectionBoxPos.y + selectionBoxPos.height - 1, selectionBoxPos.width - 2, 1), plainTexture);


            GUI.color = Color.white;

        }

        // Starts the drag select
        if (Event.current.button == 0 && Event.current.type == EventType.mouseDown && !mouseOverInspector && !mouseOverNode && creatingConnection == false)
        {
            dragSelect = true;
            dragSelectBox.x = viewPort.x + mousePos.x;
            dragSelectBox.y = viewPort.y + mousePos.y;
            dragSelectBox.width = 0;
            dragSelectBox.height = 0;
        }

        // Stops the drag select
        if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
            dragSelect = false;
    }

}
