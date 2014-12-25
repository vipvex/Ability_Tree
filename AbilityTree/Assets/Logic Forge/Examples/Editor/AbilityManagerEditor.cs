using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
	
[CustomEditor(typeof(AbilityManager))]
public class AbilityManagerEditor : Editor 
{

	SerializedProperty GUI_type, nguiUtil, spawns, abilitySlots;


    bool showSpawnObjects, showAbilitySlots;


	void OnEnable () {
		// Setup the SerializedProperties
		GUI_type = serializedObject.FindProperty ("GUI_type");
        nguiUtil = serializedObject.FindProperty("nguiUtil");
        spawns = serializedObject.FindProperty("spawns");
		abilitySlots = serializedObject.FindProperty("abilitySlots");
	}
	
	public override void OnInspectorGUI() 
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update ();

		DrawDefaultInspector();
         
        // Show the custom GUI controls
        if ((AbilityManager.GUIType)GUI_type.enumValueIndex == AbilityManager.GUIType.NGUI)
            EditorGUILayout.PropertyField(nguiUtil);



		// Display spawn objects
		GUILayout.BeginHorizontal();
		showAbilitySlots = EditorGUILayout.Foldout(showAbilitySlots, "LogicObject Slots (" + abilitySlots.arraySize + "):");
		if (GUILayout.Button("+", GUILayout.Width(25))) abilitySlots.InsertArrayElementAtIndex(abilitySlots.arraySize);
		GUILayout.EndHorizontal();
		
		
		if (showAbilitySlots)
		{
			for (int i = 0; i < abilitySlots.arraySize; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(15);
				
				
				SerializedProperty slot = abilitySlots.GetArrayElementAtIndex(i);
				SerializedProperty slotName = slot.FindPropertyRelative("name");
				SerializedProperty slotTag = slot.FindPropertyRelative("tag");
				SerializedProperty slotKey = slot.FindPropertyRelative("key");
				
				if (slotName.stringValue == "")
					GUI.color = Color.red;
				else
					GUI.color = Color.white;

				EditorGUIUtility.labelWidth = 50;
				GUILayout.Label("Name:");
				slotName.stringValue = GUILayout.TextField(slotName.stringValue, GUILayout.MinWidth(75));
				GUILayout.Label("Tag:");
				slotTag.stringValue = GUILayout.TextField(slotTag.stringValue, GUILayout.MinWidth(75));
				GUILayout.Label("Key:");
				slotKey.stringValue = GUILayout.TextField(slotKey.stringValue, GUILayout.MinWidth(75));
				
				EditorGUIUtility.labelWidth = 0;
				
				if (GUILayout.Button("-", GUILayout.Width(25))) abilitySlots.DeleteArrayElementAtIndex(i);
				
				GUILayout.EndHorizontal();
			}
		}


        // Display spawn objects
        GUILayout.BeginHorizontal();
        showSpawnObjects = EditorGUILayout.Foldout(showSpawnObjects, "Spawn Objects (" + spawns.arraySize + "):");
        if (GUILayout.Button("+", GUILayout.Width(25))) spawns.InsertArrayElementAtIndex(spawns.arraySize);
        GUILayout.EndHorizontal();
       

        if (showSpawnObjects)
        {
            for (int i = 0; i < spawns.arraySize; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);


                SerializedProperty spawn = spawns.GetArrayElementAtIndex(i);
                SerializedProperty spawnName = spawn.FindPropertyRelative("name");
                SerializedProperty spawnObj = spawn.FindPropertyRelative("obj");

                if (spawnObj == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;

                spawnName.stringValue = GUILayout.TextField(spawnName.stringValue, GUILayout.ExpandWidth(false), GUILayout.MinWidth(200));

                EditorGUIUtility.labelWidth = 50;
                EditorGUILayout.PropertyField(spawnObj);
                EditorGUIUtility.labelWidth = 0;

                if (GUILayout.Button("-", GUILayout.Width(25))) spawns.DeleteArrayElementAtIndex(i);

                GUILayout.EndHorizontal();
            }
        }
        
		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties ();
	}
}