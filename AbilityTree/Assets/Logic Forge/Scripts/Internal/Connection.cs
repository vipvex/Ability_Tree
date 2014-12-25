using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Connection
{
	public int ID;
	public List<int> connectionIDs;
	
	public Connection (int ID)
	{
		this.ID = ID;
		connectionIDs = new List<int>();
	}
}