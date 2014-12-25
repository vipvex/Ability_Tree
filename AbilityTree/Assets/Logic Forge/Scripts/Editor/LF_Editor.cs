using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class LF_Editor : EditorWindow
{

    [MenuItem("Window/Logic Forge", false, -50)]
    static void ShowEditor()
    {

        EditorWindow.GetWindow<LF_EditorWindow>("Logic Forge", true);

    }


    [MenuItem("Logic Forge/New Logic System", false, 1)]
    [MenuItem("Assets/Logic Forge/New Logic System", false, -14)]
    public static void NewLogicSystem()
    {

        ScriptableObject asset = ScriptableObject.CreateInstance<LogicSystem>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New LogicObject System" + ".asset");


        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }


    [MenuItem("Assets/Logic Forge/Duplicate as Logic  Object", false, -15)]
    public static void CreateScriptableObject()
    {

        //Debug.Log (Selection.activeObject is MonoScript);
        //Debug.Log(ScriptableObject.CreateInstance(Selection.activeObject.GetType().ToString()));
        if (Selection.activeObject is MonoScript)
        {
            if (ScriptableObject.CreateInstance(((MonoScript)Selection.activeObject).GetClass().ToString()) == null)
                return;
        }
        else
            return;

        ScriptableObject asset = ScriptableObject.CreateInstance(((MonoScript)Selection.activeObject).GetClass().ToString());

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + ((MonoScript)Selection.activeObject).GetClass().ToString() + ".asset");


        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

    }
	

	[MenuItem("Logic Forge/Prefrences", false, 2)]
	public static void ShowPrefrences()
	{
        EditorWindow.GetWindow<LogicForgeSettings>();
	}

	[MenuItem("Logic Forge/Help/Documentation", false, 15)]
	public static void Documentation()
	{

	}

    [MenuItem("Logic Forge/Help/Website", false, 15)]
	public static void Website()
	{
		
	}

    [MenuItem("Logic Forge/Help/YouTube Tutorials", false, 15)]
	public static void YouTube()
	{
		
	}

    [MenuItem("Logic Forge/Help/Report Bug", false, 15)]
	public static void ReportBug()
	{
		
	}

	/// <summary>
	/// Used to get assets of a certain type and file extension from entire project
	/// </summary>
	/// <param name="type">The type to retrieve. eg typeof(GameObject).</param>
	/// <param name="fileExtension">The file extention the type uses eg ".prefab".</param>
	/// <returns>An Object array of assets.</returns>
	public static Object[] GetAssetsOfType(System.Type type, string fileExtension)
	{
		List<Object> tempObjects = new List<Object>();
		DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
		FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);
		
		int i = 0; int goFileInfoLength = goFileInfo.Length;
		FileInfo tempGoFileInfo; string tempFilePath;
		Object tempGO;
		for (; i < goFileInfoLength; i++)
		{
			tempGoFileInfo = goFileInfo[i];
			if (tempGoFileInfo == null)
				continue;
			
			tempFilePath = tempGoFileInfo.FullName;
			tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

			tempGO = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(Object)) as Object;
			if (tempGO == null)
			{
				//Debug.LogWarning("Skipping Null");
				continue;
			}
			else if (tempGO.GetType() != type)
			{
				//Debug.LogWarning("Skipping " + tempGO.GetType().ToString());
				continue;
			}
			
			tempObjects.Add(tempGO);
		}
		
		return tempObjects.ToArray();
	}

    [MenuItem("Assets/Save Editor Skin")]
    static public void SaveEditorSkin()
    {
        GUISkin skin = ScriptableObject.Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector)) as GUISkin;
        AssetDatabase.CreateAsset(skin, "Assets/EditorSkin.guiskin");
    }

}