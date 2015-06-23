﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour
{
    public static GameControl game;

    public Player player;
    public Farm farm;
    public List<Cow> cows;

    void Awake()
    {
        if (game == null)
        {
            DontDestroyOnLoad(gameObject);
            game = this;
            //Load();
            //control.player.name = "Farmer Joe";
            game.player.cash = 500;
        }
        else if (game != this)
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {
        try
        {
            FileStream file;
            BinaryFormatter bf = new BinaryFormatter();


            //save player data
            file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, player);
            file.Close();

            //save cow data
            file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, cows);
            file.Close();

            //save farm data
            file = File.Open(Application.persistentDataPath + "/farm.dat", FileMode.OpenOrCreate);
            bf.Serialize(file, farm);
            file.Close();
        }
        catch (UnityException e)
        {
            Debug.Log("Saving Failed! - " + e);
        }
    }

    public void Load()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;
            //load player data
            if (File.Exists(Application.persistentDataPath + "/player.dat"))
            {
                file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
                player = (Player)bf.Deserialize(file);
                file.Close();
            }
            //load cow data
            if (File.Exists(Application.persistentDataPath + "/cows.dat"))
            {
                file = File.Open(Application.persistentDataPath + "/cows.dat", FileMode.Open);
                cows = (List<Cow>)bf.Deserialize(file);
                file.Close();
            }
            //load farm data
            if (File.Exists(Application.persistentDataPath + "/farm.dat"))
            {
                file = File.Open(Application.persistentDataPath + "/farm.dat", FileMode.Open);
                farm = (Farm)bf.Deserialize(file);
                file.Close();
            }

        }
        catch (UnityException e)
        {
            Debug.Log("Loading Failed! - " + e);
        }
    }



    [System.Serializable]
    public class Player
    {
        public string name;
        public double cash;

    }

    [System.Serializable]
    public class Cow
    {
        public string name { get; set; }
        public int gameObjectID { get; set; }
        public bool ownedByPlayer { get; set; }
        public int age { get; set; }
        public int breed { get; set; }
        public int happiness { get; set; }
        public int health { get; set; }
        public bool preggers { get; set; }
        public bool sexMale { get; set; }
        public float weight { get; set; }

        public Cow(string name)
        {
            this.name = name;
        }

        public Cow(string name, int age, int breed, int happiness, int health, bool preggers, bool sexMale, float weight)
        {
            this.name = name;
            this.age = age;
            this.breed = breed;
            this.happiness = happiness;
            this.health = health;
            this.sexMale = sexMale;
            this.preggers = preggers;
            this.weight = weight;
        }


        public void buy()
        {
            if (game.player.cash > 100)
            {
                print("You bought " + name + "!");
                ownedByPlayer = true;
                game.player.cash -= 100;

            }
        }


        public void Sell()
        {
            if (ownedByPlayer)
            {
                print("You Sold " + name + " :(");
                ownedByPlayer = false;
                game.player.cash += 100;

            }
        }
    }

    [System.Serializable]
    public class Farm
    {
        // Maybe save state of the farm here, the amount of cows the player has...etc
    }


}