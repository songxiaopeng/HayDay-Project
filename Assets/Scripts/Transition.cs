﻿using UnityEngine;
using System.Collections;

public class Transition : MonoBehaviour
{
	void FixedUpdate () 
	{
		transform.Rotate (0, 2, 0);
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.name.Equals ("Player"))
		{
			GlobalVars.sceneTransitionUI = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name.Equals ("Player"))
		{
			GlobalVars.sceneTransitionUI = false;
		}
	}
}