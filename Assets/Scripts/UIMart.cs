﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UIMart : GameController 
{
    public bool playerUI;

	public GUIStyle buttonBid;
	public GUIStyle buttonCattle;
	public GUIStyle customTextStyle;

    public Cow cow;
    public GameObject cowGameObject;
	
	public static Image healthBar;
	public static Image happinessBar;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public Image[] bars = new Image[2];

	public static int cowAge;
	public static string breed;
	public static int happiness;
	public static int health;
	public static bool preggers;
	public static bool sexMale;
	public static float weight;
	
	public static int currentCowBid = 0;
	public static int currentTimer = 60;
	public static bool playerBidLast = false;

	bool timerStart = false;
	bool timerChecked = true;
	float doesAIBid;
	double startMoney = 0;

	public GameObject AudioObject;

	Rect windowRect;

    void Start()
    {
        playerUI = true;
		//bars = new Image[2];
		bars = GetComponentsInChildren<Image>();
		healthBar = bars[0];
		happinessBar = bars[1];

		AudioObject = GameObject.Find("Background Music");
		Destroy(AudioObject);
    }

    void OnGUI()
    {
		windowRect = new Rect(Screen.width * .72f, Screen.height - 550f, 350, 500);

		GUI.color = foregroundColor;
		
        if (playerUI)
        {
			windowRect = GUI.Window(0, windowRect, PlayerInfo, "Mart");
		}

		if(!timerChecked)
		{
			StartCoroutine(TimerDec(1));
		}
	}

    void PlayerInfo(int windowID)
    {
		GUI.contentColor = backgroundColor;

		GUI.Label(new Rect(110, 20, 150, 30), game.player.name, customTextStyle);
		GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + game.player.cash, customTextStyle);
		GUI.Label(new Rect(110, 90, 150, 30), "Timer: " + currentTimer, customTextStyle);
		GUI.Label(new Rect(25, 128, 150, 30), "Age: " + cowAge, customTextStyle);
		GUI.Label(new Rect(25, 158, 150, 30), "Breed: " + breed, customTextStyle);
		GUI.Label(new Rect(25, 188, 150, 30), "Happiness: " + happiness, customTextStyle);
		GUI.Label(new Rect(25, 218, 150, 30), "Health: " + health, customTextStyle);
		GUI.Label(new Rect(25, 248, 150, 30), "Preggers: " + preggers, customTextStyle);
		GUI.Label(new Rect(25, 278, 150, 30), "Sex: " + sexMale, customTextStyle);
		GUI.Label(new Rect(25, 308, 150, 30), "Weight: " + weight, customTextStyle);

		GUI.Label(new Rect(25, 350, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);

		if (GUI.Button (new Rect (105, 400, 120, 40), "", buttonCattle))
		{
			Vector3 spawnLocation = new Vector3(112f, 0, 154f);
			CowMaker.GenerateCow(spawnLocation);
		}

		if(GUI.Button (new Rect (125, 450, 80, 40), "", buttonBid))
		{
			if(game.player.cash > currentCowBid)
			{
				currentCowBid += 500;
				playerBidLast = true;
				if(!timerStart)
				{
					timerStart = true;
					timerChecked = false;
					startMoney = game.player.cash;
				}
			}

			doesAIBid = Random.Range(45f, 100f);

			Debug.Log ("AI bid: " + doesAIBid);

			if(doesAIBid > 50f)
			{
				StartCoroutine(AIBid(Random.Range(2, 5),Random.Range(0,2), doesAIBid));
			}
		}

		if(currentTimer <= 0)
		{
			CowController.martState = "exitCow";

			if(playerBidLast)
			{
				Debug.Log ("Player won the bid!");
				game.player.cash -= currentCowBid;
			}
			else
			{
				Debug.Log ("Player lost the bid!");
				game.player.cash = startMoney;
			}

			ClearStats();
		}
    }

	public void ClearStats()
	{
		cowAge = 0;
		breed = "";
		happiness = 0;
		health = 0;
		preggers = false;
		sexMale = false;
		weight = 0f;

		currentTimer = 60;
		currentCowBid = 0;

		timerStart = false;
	}

	IEnumerator AIBid(float seconds, int option, float aiInterestPercentage) 
	{
		yield return new WaitForSeconds(seconds);

		// Override AI option with its interest in bidding for that cow
		// If the AI is really interested in bidding then set option to 3 for example
		if (aiInterestPercentage > 90f)
		{
			option = 3;
		}
		else if(aiInterestPercentage > 75f)
		{
			option = 2;
		}
		else if(aiInterestPercentage > 60f)
		{
			option = 1;
		}

		switch(option)
		{
			case 0:
				currentCowBid += 500;
				playerBidLast = false;
			break;
			case 1:
				currentCowBid += 1000;
				playerBidLast = false;
			break;
			case 2:
				currentCowBid += 2500;
				playerBidLast = false;
			break;
			case 3:
				currentCowBid += 5000;
				playerBidLast = false;
			break;
		}
	}

	IEnumerator TimerDec(int seconds) 
	{	
		if(timerStart)
		{
			currentTimer -= seconds;
			timerChecked = true;
		}

		yield return new WaitForSeconds(seconds);

		timerChecked = false;
	}

	public static void SetHealth(float health)
	{
		healthBar.fillAmount = health;
	}

	public static void SetHappiness(float happiness)
	{
		happinessBar.fillAmount = happiness;
	}
}