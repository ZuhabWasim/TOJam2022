using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Dialogue 
{
	public bool onStart;
	public bool onEnd;
	public string name; 
	[TextArea(3,10)]
	public string[] sentences;
	
	
}
