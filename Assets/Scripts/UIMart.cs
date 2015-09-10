﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UIMart : GameController 	// Fix this stuff, finish me! - Menu to finish!
{
	public bool cowMenuUI = true;
	public bool cowBuyBidUI;
	public bool cowSellBidUI;

	public GUIStyle buttonMainMenu;
	public GUIStyle buttonBuy;
	public GUIStyle buttonSell;
	public GUIStyle buttonFarm;
	public GUIStyle buttonBid;
	public GUIStyle buttonCattle;
	public GUIStyle buttonX;
	public GUIStyle labelAge;
	public GUIStyle labelBreed;
	public GUIStyle labelHappiness;
	public GUIStyle labelHealth;
	public GUIStyle labelPregnant;
	public GUIStyle labelGender;
	public GUIStyle labelWeight;
	public GUIStyle labelLoading;

	public GUIStyle customTextStyle;

	public Image healthBar;
	public Image happinessBar;

	public Color backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);
	public Color foregroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public Image[] bars = new Image[2];

	public static int currentCowBid = 0;
	public static int currentTimer = 60;
	public static bool playerBidLast = false;
	public static bool biddingOver = false;

	Cow defaultCow;
	string cowGender = "Male";
	string cowPregnant = "No";
	bool timerStart = false;
	bool timerChecked = true;
	bool bidStarted = false;
	bool cowInRing = false;
	float doesAIBid;
	int bidCount = 0;
	double startMoney = 0;

	public GameObject AudioObject;

	public AudioClip buttonSound;

	Rect windowRect;
	bool isLoading = false;

    void Start()
    {
		bars = GetComponentsInChildren<Image>();
		healthBar = bars[0];
		happinessBar = bars[1];      
        defaultCow = new Cow("", 0, "", 0, 0, false, false, 0);
    }

    void OnGUI()
    {
		GUI.color = foregroundColor;
		
		if(cowMenuUI)
		{
			windowRect = new Rect(Screen.width * .7f, Screen.height - 500f, 350, 450);
			windowRect = GUI.Window(0, windowRect, Menu, "Menu");
		}
		else if(cowBuyBidUI)
		{
			windowRect = new Rect(Screen.width * .7f, Screen.height - 550f, 350, 500);
			windowRect = GUI.Window(0, windowRect, CowBuyBid, "Buying");
		}
		else if(cowSellBidUI)
		{
			windowRect = new Rect(Screen.width * .7f, Screen.height - 550f, 350, 500);
			windowRect = GUI.Window(0, windowRect, CowSellBid, "Selling");
		}

		if(timerStart)
		{
			if(!timerChecked)
			{
				StartCoroutine(TimerDec(1));
			}
		}
	}

	void Menu(int windowID)		// Fix this stuff, finish me!
	{
		GUI.contentColor = backgroundColor;
		
		SetHealth(0f);
		SetHappiness(0f);

		if(isLoading)
			GUI.Label(new Rect(45, 200, 270, 50), "", labelLoading);

		if(!isLoading)
			if (GUI.Button (new Rect (30, 50, 290, 52), "", buttonMainMenu))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				isLoading = true;
				StartCoroutine(WaitFor(0));
			}

		if(!isLoading)
			if (GUI.Button (new Rect (105, 150, 140, 60), "", buttonBuy))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				cowMenuUI = false;
				cowBuyBidUI = true;
			}

		if(!isLoading)
			if (GUI.Button (new Rect (105, 250, 140, 50), "", buttonSell))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				cowMenuUI = false;
				cowSellBidUI = true;
			}

		if(!isLoading)
			if (GUI.Button (new Rect (85, 350, 175, 50), "", buttonFarm))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				isLoading = true;
				StartCoroutine(WaitFor(3));
			}
	}

	void CowBuyBid(int windowID)
    {
		GUI.contentColor = backgroundColor;

		SetHealth(cow.health / 100f);
		SetHappiness(cow.happiness / 10f);
		
		healthBar.transform.position = new Vector2 (windowRect.center.x + 64, 330);
		happinessBar.transform.position = new Vector2 (windowRect.center.x + 64, 295);
		
		if (!cow.gender == true)
		{
			cowGender = "Female";
			
			if(cow.pregnant == true)
				cowPregnant = "Yes";
		}
		else
		{
			cowGender = "Male";
			cow.pregnant = false;
			cowPregnant = "No";
		}

		GUI.Label(new Rect(110, 20, 150, 30), game.player.name, customTextStyle);
		GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + game.player.cash, customTextStyle);
		GUI.Label(new Rect(110, 90, 150, 30), "Timer: " + currentTimer, customTextStyle);
		GUI.Label(new Rect(25, 135, 80, 25), "", labelAge);
		GUI.Label(new Rect(132, 132, 150, 30), "" + cow.age, customTextStyle);
		GUI.Label(new Rect(25, 170, 90, 25), "", labelBreed);
		GUI.Label(new Rect(132, 170, 150, 30), "" + cow.breed, customTextStyle);
		GUI.Label(new Rect(25, 205, 110, 30), "", labelHappiness);
		GUI.Label(new Rect(25, 240, 90, 25), "", labelHealth);
		GUI.Label(new Rect(25, 275, 110, 30), "", labelPregnant);
		GUI.Label(new Rect(150, 275, 150, 30), "" + cowPregnant, customTextStyle);
		GUI.Label(new Rect(25, 313, 90, 25), "", labelGender);
		GUI.Label(new Rect(132, 313, 150, 30), "" + cowGender, customTextStyle);
		GUI.Label(new Rect(25, 350, 90, 30), "", labelWeight);
		GUI.Label(new Rect(132, 350, 150, 30), "" + cow.weight, customTextStyle);

		GUI.Label(new Rect(70, 400, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);

		if(!cowInRing)
		{
			if (GUI.Button (new Rect (105, 445, 120, 40), "", buttonCattle))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				Vector3 spawnLocation = new Vector3(Random.Range(112f, 112f), 0, Random.Range(156f, 156f));
				cow = CowMaker.GenerateCow(spawnLocation);
				cowInRing = true;
			}
		}

		if(!playerBidLast && cowInRing)
		{
			if(GUI.Button (new Rect (125, 445, 80, 40), "", buttonBid))
			{
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);

				if(game.player.cash >= currentCowBid)
				{
					currentCowBid += 1000;
					playerBidLast = true;
					bidCount++;

					if(!timerStart)
					{
						bidStarted = true;
						timerStart = true;
						timerChecked = false;
						startMoney = game.player.cash;
					}
				}

				doesAIBid = Random.Range(45f, 100f);

				Debug.Log ("AI bid: " + doesAIBid);

				if(doesAIBid > 45f)
				{
					if(bidStarted)
					{
						StartCoroutine(AIBid(Random.Range(2, 5),Random.Range(0,2), doesAIBid));
						bidCount++;
					}
				}
			}
		}

		if(currentTimer <= 0)
		{
			if(playerBidLast)
			{
				Debug.Log ("Player won the bid!");
				game.player.cash -= currentCowBid;
				// If the player won the bid add the cow to list player owns
			}
			else
			{
				Debug.Log ("Player lost the bid!");
				game.player.cash = startMoney;
				// Wait for cow to leave bidding area and destory the cow object
			}

			ClearStats();
		}

		if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			cowMenuUI = true;
			cowBuyBidUI = false;
		}
    }

	void CowSellBid(int windowID)
	{
		GUI.contentColor = backgroundColor;

		bool cowSelected = false;

		// Generate list of cows here using 'game.cows' list

		if(cowSelected)
		{
			SetHealth(cow.health / 100f);
			SetHappiness(cow.happiness / 10f);
			
			healthBar.transform.position = new Vector2 (windowRect.center.x + 64, 330);
			happinessBar.transform.position = new Vector2 (windowRect.center.x + 64, 295);
			
			if (!cow.gender == true)
			{
				cowGender = "Female";
				
				if(cow.pregnant == true)
					cowPregnant = "Yes";
			}
			else
			{
				cowGender = "Male";
				cow.pregnant = false;
				cowPregnant = "No";
			}
			
			GUI.Label(new Rect(110, 20, 150, 30), game.player.name, customTextStyle);
			GUI.Label(new Rect(90, 50, 150, 30), "Cash: € " + game.player.cash, customTextStyle);
			GUI.Label(new Rect(110, 90, 150, 30), "Timer: " + currentTimer, customTextStyle);
			GUI.Label(new Rect(25, 135, 80, 25), "", labelAge);
			GUI.Label(new Rect(132, 132, 150, 30), "" + cow.age, customTextStyle);
			GUI.Label(new Rect(25, 170, 90, 25), "", labelBreed);
			GUI.Label(new Rect(132, 170, 150, 30), "" + cow.breed, customTextStyle);
			GUI.Label(new Rect(25, 205, 110, 30), "", labelHappiness);
			GUI.Label(new Rect(25, 240, 90, 25), "", labelHealth);
			GUI.Label(new Rect(25, 275, 110, 30), "", labelPregnant);
			GUI.Label(new Rect(150, 275, 150, 30), "" + cowPregnant, customTextStyle);
			GUI.Label(new Rect(25, 313, 90, 25), "", labelGender);
			GUI.Label(new Rect(132, 313, 150, 30), "" + cowGender, customTextStyle);
			GUI.Label(new Rect(25, 350, 90, 30), "", labelWeight);
			GUI.Label(new Rect(132, 350, 150, 30), "" + cow.weight, customTextStyle);
			
			GUI.Label(new Rect(70, 400, 150, 30), "Current Bid: € " + currentCowBid, customTextStyle);
		}
		
		if(!cowInRing)
		{
			if (GUI.Button (new Rect (105, 445, 120, 40), "", buttonCattle))
			{	
				GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
				Vector3 spawnLocation = new Vector3(Random.Range(112f, 112f), 0, Random.Range(156f, 156f));
				cow = CowMaker.GenerateCow(spawnLocation);
				cowInRing = true;
			}
		}

		if (GUI.Button(new Rect(295, 450, 35, 35), "", buttonX))
		{
			GetComponent<AudioSource>().PlayOneShot(buttonSound, 0.7f);
			cowMenuUI = true;
			cowSellBidUI = false;
		}
	}

	public void ClearStats()
	{
        cow = defaultCow;

		bidCount = 0;
		currentTimer = 60;
		currentCowBid = 0;

		timerStart = false;
		bidStarted = false;
		timerChecked = true;
		cowInRing = false;
		biddingOver = true;
		playerBidLast = false;

		SetHealth(0);
		SetHappiness(0);
	}

	IEnumerator AIBid(float seconds, int option, float aiInterestPercentage) 
	{
		yield return new WaitForSeconds(seconds);

		if(bidStarted)
		{
			// Override AI interest if bidding has only begun, makes a little more difficult
			if (bidCount < 6)
				aiInterestPercentage = 95f;

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
					currentCowBid += 1000;
					playerBidLast = false;
				break;
				case 1:
					currentCowBid += 2000;
					playerBidLast = false;
				break;
				case 2:
					currentCowBid += 3500;
					playerBidLast = false;
				break;
				case 3:
					currentCowBid += 5000;
					playerBidLast = false;
				break;
			}
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

	void SetHealth(float health)
	{
		healthBar.fillAmount = health;
	}
	
	void SetHappiness(float happiness)
	{
		happinessBar.fillAmount = happiness;
	}

	IEnumerator WaitFor(int level) 
	{
		yield return new WaitForSeconds(1.0f);
		
		if (level == 10) 
		{
			Application.Quit ();	
		} 
		else 
		{
			Application.LoadLevel (level);
		}
	}
}