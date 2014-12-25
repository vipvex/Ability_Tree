using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AbilityPassive))]
public class LogicObjectPassiveEditor : Editor {


	SerializedProperty passiveEffects;

	void OnEnable () {

		// Setup the SerializedProperties
		passiveEffects = serializedObject.FindProperty ("passiveEffects");
	
	}


	public override void OnInspectorGUI() 
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update ();
		
		DrawDefaultInspector();
		

		// Display spawn objects
		GUILayout.BeginHorizontal();
		GUILayout.Label("Passive effects (" + passiveEffects.arraySize + "):");
		if (GUILayout.Button("+", GUILayout.Width(25))) passiveEffects.InsertArrayElementAtIndex(passiveEffects.arraySize);
		GUILayout.EndHorizontal();
		
		

		for (int i = 0; i < passiveEffects.arraySize; i++)
		{
			GUILayout.BeginHorizontal();
			//GUILayout.Space(15);
			
			
			SerializedProperty passive = passiveEffects.GetArrayElementAtIndex(i);
			SerializedProperty passiveMethod = passive.FindPropertyRelative("methodName");
			SerializedProperty passiveValue = passive.FindPropertyRelative("value");

			SerializedProperty passiveLifetime = passive.FindPropertyRelative("lifeTime");
			SerializedProperty passiveFrequency = passive.FindPropertyRelative("frequency");

			
			if (passiveMethod.stringValue == "")
				GUI.color = Color.red;
			else
				GUI.color = Color.white;
			
			EditorGUIUtility.labelWidth = 50;
			GUILayout.Label("Method:");
			passiveMethod.stringValue = GUILayout.TextField(passiveMethod.stringValue, GUILayout.MinWidth(50));

			GUILayout.Label("Value:");
			passiveValue.intValue = EditorGUILayout.IntField(passiveValue.intValue, GUILayout.MinWidth(50));

			GUILayout.Label("Frequency:");
			PassiveEffect.Frequency tempFrequency = (PassiveEffect.Frequency)passiveFrequency.enumValueIndex;
			tempFrequency = (PassiveEffect.Frequency)EditorGUILayout.EnumPopup(tempFrequency);
			passiveFrequency.enumValueIndex = (int)tempFrequency;


			if (tempFrequency == PassiveEffect.Frequency.Timed || tempFrequency == PassiveEffect.Frequency.TimedOnTick)
			{
				GUILayout.Label("Life Time:");
				passiveLifetime.floatValue = EditorGUILayout.FloatField(passiveLifetime.floatValue, GUILayout.MaxWidth(30));
			}

			EditorGUIUtility.labelWidth = 0;
			
			if (GUILayout.Button("-", GUILayout.Width(25))) passiveEffects.DeleteArrayElementAtIndex(i);
			
			GUILayout.EndHorizontal();
		}

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties ();

	}

}
