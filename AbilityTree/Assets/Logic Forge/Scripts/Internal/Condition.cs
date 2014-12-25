using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Condition
{

    public string paramater1Type = "";
    public string paramater1Name = "";
    public int paramater1Index = 0;
	
 
	public string paramater2Value = "";


	
	public int type = 0;


    public string[] types = { "==", "<=", ">=", "<", ">", "!=" };


    public Condition() { }

    public Condition(string paramater1Type, string paramater1Name, int paramater1Index, string paramater2Value, int type)
    {
        this.paramater1Type = paramater1Type;
        this.paramater1Name = paramater1Name;
        this.paramater1Index = paramater1Index;

        this.paramater2Value = paramater2Value;

        this.type = type;
    }
}