using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyClass{

	public int Add(int a,int b)
	{
		return a + b;
	}

	IEnumerator Sub(int a,int b)
	{
		yield return a - b;
	}
}
