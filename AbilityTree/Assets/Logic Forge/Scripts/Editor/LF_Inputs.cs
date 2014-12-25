using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public partial class LF_EditorWindow : EditorWindow
{

    // Handels the inputs and interaction in the editor
    void Inputs()
    {
        Event e = Event.current;


        float scrollSpeed = 1.5f;
        if ((Mathf.Abs(dragSelectBox.width - (mouseDownPos.x - mousePos.x)) > 15 || Mathf.Abs(dragSelectBox.height - (mouseDownPos.y - mousePos.y)) > 15) && (dragSelect || draggingNodes))
        {

            // Makes it so that when you drag select and your mouse goes off the screen the viewprot pans with it.
            if (mousePos.x < 10)
            {
                viewPort.x -= scrollSpeed;

                if (dragSelect)
                    dragSelectBox.width -= scrollSpeed;

                if (draggingNodes)
                {
                    for (int s = 0, a = selectedNodes.Count; s < a; s++)
                    {
                        selectedNodes[s].window.x -= scrollSpeed;
                    }
                }

            }

            if (mousePos.x > Screen.width - inspectorWidth)
            {
                viewPort.x += scrollSpeed;
                if (dragSelect)
                    dragSelectBox.width += scrollSpeed;


                if (draggingNodes)
                {
                    for (int s = 0, a = selectedNodes.Count; s < a; s++)
                    {
                        selectedNodes[s].window.x += scrollSpeed;
                    }
                }
            }

            if (mousePos.y < 15)
            {
                viewPort.y -= scrollSpeed;
                if (dragSelect)
                    dragSelectBox.height -= scrollSpeed;


                if (draggingNodes)
                {
                    for (int s = 0, a = selectedNodes.Count; s < a; s++)
                    {
                        selectedNodes[s].window.y -= scrollSpeed;
                    }
                }
            }

            if (mousePos.y > Screen.height - 25)
            {
                viewPort.y += scrollSpeed;
                if (dragSelect)
                    dragSelectBox.height += scrollSpeed;


                if (draggingNodes)
                {
                    for (int s = 0, a = selectedNodes.Count; s < a; s++)
                    {
                        selectedNodes[s].window.y += scrollSpeed;
                    }
                }
            }

        }

        // Move group of nodes
        if (draggingNodes && mouseButton == MouseButton.Left && inputType == EventType.mouseDrag)
        {

            for (int s = 0, a = selectedNodes.Count; s < a; s++)
            {

                selectedNodes[s].window.x += Event.current.delta.x;
                selectedNodes[s].window.y += Event.current.delta.y;

            }

        }


        // Delete
        if (selectedNodes.Count > 0 && e.keyCode == KeyCode.Delete && mouseOverInspector == false)
            DeleteSelectedNodes();

        if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
            mouseDownPos = mousePos;

        // Starts mouse panning in the viewport
        if (e.button == 2 && e.type == EventType.mouseDown)
        {
            panMousePos = -mousePos;
            panViewPort = viewPort;
            panViewPort.x += inspectorWidth;
        }

        // Drags the viewport around with middle click
        if (e.button == 2 && Event.current.type == EventType.mouseDrag)
            viewPort = new Vector2(-Event.current.mousePosition.x - panMousePos.x + panViewPort.x, -Event.current.mousePosition.y - panMousePos.y + panViewPort.y);


        // Show the context menu menu when right click
        if (e.button == 1 && e.type == EventType.mouseUp && !draggingNodes && mouseOverInspector == false)
        {
            dragSelectBox = new Rect();
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Node"), false, AddNewNode);
            menu.AddItem(new GUIContent("Paste"), false, PasteNodes);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Center Viewport"), false, CenterViewPort);
            menu.ShowAsContext();
        }

        // Deselect everything when we click the background
        if (e.button == 0 && e.type == EventType.mouseUp && mouseOverInspector == false)
        {

            GUI.FocusControl("");
            UnFocus();
            draggingNodes = false;
            creatingConnection = false;
            doubleConnection = false;
            dragSelect = false;
        }


        if (e.keyCode == KeyCode.C && Event.current.modifiers != EventModifiers.Control && Event.current.type == EventType.keyUp && mouseOverInspector == false && creatingConnection == true)
            if (doubleConnection)
                doubleConnection = false;
            else
                doubleConnection = true;



        // Connect shortcut
        if (e.keyCode == KeyCode.C && Event.current.modifiers != EventModifiers.Control && Event.current.type == EventType.keyUp && mouseOverInspector == false)
            creatingConnection = true;

        // Controll N. Shortcut for creating a new node. 
        if (e.keyCode == KeyCode.N && Event.current.type == EventType.keyUp && mouseOverInspector == false)
            AddNewNode();

        // Copy
        if (e.keyCode == KeyCode.C && Event.current.modifiers == EventModifiers.Control && Event.current.type == EventType.layout)
            CopyNodes();

        // Paste
        if (e.keyCode == KeyCode.V && Event.current.modifiers == EventModifiers.Control && Event.current.type == EventType.layout)
            PasteNodes();

        // Cut
        if (e.keyCode == KeyCode.X && Event.current.modifiers == EventModifiers.Control && Event.current.type == EventType.layout)
        {
            CopyNodes();
            DeleteSelectedNodes();
        }

        // Duplicate
        if (e.keyCode == KeyCode.D && Event.current.modifiers == EventModifiers.Control && Event.current.type == EventType.layout)
            DuplicatedNodes();


        // Support fro drag and dropping assets into the view
        // Check if the dragging assets are textures or Logic Objects then proceed.
        for (int a = 0; a < DragAndDrop.objectReferences.Length; a++)
            if (!(DragAndDrop.objectReferences[a] is Texture) && !(DragAndDrop.objectReferences[a] is LogicObject))
                return;

        EventType eventType = Event.current.type;
        if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform))
        {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                // Create and object for each asset dragged in
                for (int a = 0; a < DragAndDrop.objectReferences.Length; a++)
                {

                    if (DragAndDrop.objectReferences[a] is LogicObject)
                    {
                        Node newNode = logicSystem.AddNode();
                        newNode.window.x = -viewPort.x + mousePos.x - 35 + 85 * a;
                        newNode.window.y = -viewPort.y + mousePos.y - 35;
                        newNode.name = ((LogicObject)DragAndDrop.objectReferences[a]).name;
                        newNode.description = ((LogicObject)DragAndDrop.objectReferences[a]).description;
                        newNode.baseTexture = ((LogicObject)DragAndDrop.objectReferences[a]).texture;
                        newNode.abilityObjects.Add((LogicObject)DragAndDrop.objectReferences[a]);
                    }
                }
            }

            Event.current.Use();
        }

    }

}
