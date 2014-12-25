using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all logic objects
/// </summary>
public class LogicObject : ScriptableObject 
{

	public string name = "";
	public string description = "";

	public Texture texture;


    /// <summary>
    /// Whether this ability is selected.
    /// </summary>
    public bool selected = false;

    /// <summary>
    /// Toggles whether the ability is selected.
    /// </summary>
    public virtual void Select(bool value)
    {
		selected = value;
		OnSelect(value);
    }

	/// <summary>
	/// Called once the ability has been selected.
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public virtual void OnSelect (bool value)
	{

	}

    /// <summary>
    /// Initializing the ability. Should be used to assign variables if need be.
    /// </summary>
    /// <param name="manager"></param>
    public virtual void Initialize(MonoBehaviour script)
    {
		  
    }

    /// <summary>
    /// Called when the ability is no longer used. Use this to destroy any objects it has created during it's lifetime.
    /// </summary>
    public virtual void Terminate()
    {

    }
	
}