using UnityEditor;
using UnityEngine;
using System.Collections;

public static class EditorGUIExtension
{

    /// <summary>
    /// Draw a header seperator
    /// </summary>
    static public bool Header (string text, string key)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

        EditorGUILayout.BeginHorizontal();
        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) 
            text = "\u25BC " + text;
        else 
            text = "\u25BA " + text;
        
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;


        if (GUI.changed) EditorPrefs.SetBool(key, state);

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!state) 
            GUILayout.Space(3f);
        return state;
    }

}