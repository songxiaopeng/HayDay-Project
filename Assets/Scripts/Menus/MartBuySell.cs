﻿using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class MartBuySell : MonoBehaviour
	{
		void OnTriggerEnter(Collider other) 
		{
			if (other.gameObject.name.Equals("Player"))
			{
				GameController.Instance().martCowMenuUI = true;
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name.Equals("Player"))
			{
				GameController.Instance().martCowMenuUI = false;
			}
		}
	}
}