using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Parameter <T>
{
	public string name = "";
	public T value;
	
	public Parameter (string name, T value)
	{
		this.name = name;
		this.value = value;
	}

	public Parameter (){}

}

[System.Serializable]
public class ParameterInt 
{
    public string name;

	public int value = 0;
	public ParameterInt (string name, int value) 
	{
		this.name = name;
		this.value = value;
	}

    public ParameterInt()
    {
        name = "";
        value = 0;
    }
}

[System.Serializable]
public class ParameterFloat 
{
	public string name = "";

	public float value = 0;
	public ParameterFloat (string name, float value) 
	{
		this.name = name;
		this.value = value;
	}
    public ParameterFloat()
    {
        name = "";
        value = 0;
    }
}

[System.Serializable]
public class ParameterString 
{
	public string name = "";

	public string value = "";
	public ParameterString (string name, string value) 
	{
		this.name = name;
		this.value = value;
	}
    public ParameterString()
    {
        name = "";
        value = "";
    }
}

[System.Serializable]
public class ParameterBool  
{
	public string name = "";

	public bool value = false;
    public ParameterBool(string name, bool value) 
	{
		this.name = name;
		this.value = value;
	}
    public ParameterBool()
    {
        name = "";
        value = false;
    }
}