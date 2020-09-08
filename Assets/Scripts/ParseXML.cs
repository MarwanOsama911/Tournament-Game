using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

struct animalbeated
{
    public string animal1 { get; set; }
    public string animal2 { get; set; }
}

public class ParseXML : MonoBehaviour
{
    Dictionary<string, animalbeated> gameRules;

    List<playerData> players;
    List<playerData> playersInfoTemp;

    //define animals of the game
    animalbeated Elephant, Tiger, Lion, Dog, Giraffe;
    TextAsset xmlRawFile;
    XmlDocument xmlDoc;

    /// <summary>
    /// seting up the game rules
    /// </summary>
    void SettingGameRules()
    {
        //setting animal struct data to show that which animal can fight which
        Elephant = new animalbeated { animal1 = "T", animal2 = "G" };
        Tiger = new animalbeated { animal1 = "L", animal2 = "D" };
        Lion = new animalbeated { animal1 = "G", animal2 = "E" };
        Dog = new animalbeated { animal1 = "E", animal2 = "L" };
        Giraffe = new animalbeated { animal1 = "D", animal2 = "T" };
        //setting rules dictionary
        gameRules.Add("E", Elephant);
        gameRules.Add("T", Tiger);
        gameRules.Add("L", Lion);
        gameRules.Add("D", Dog);
        gameRules.Add("G", Giraffe);
    }
    void Start()
    {
        players = new List<playerData>();
        gameRules = new Dictionary<string, animalbeated>();
        xmlRawFile = Resources.Load<TextAsset>("Xml/Competition");
        xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlRawFile.text);
        SettingGameRules();
        ParsePlayerInfo();
    }
    /// <summary>
    /// find the node with id of the gameobject to find it's properties node and parsing the node of it to read the properties of the prefab
    /// </summary>
    public void ParsePlayerInfo()
    {
        XmlNodeList currentnode = xmlDoc.SelectNodes("/PlayerDetails/Player");
        if (currentnode == null)
        {
            Debug.LogError("couldn't find the data in PlayerDetails.xml");
            return;
        }
        foreach (XmlNode item in currentnode)
        {
            //getting data and store it in dictionary
            PlayerDataGetter(item);
        }
        //call algrothim to determine the winner player
        string winnerPlayer = DetermineWinner();
        print("The Winner Player is " + winnerPlayer);
    }
    /// <summary>
    /// determine the winner player of the list of players
    /// </summary>
    /// <returns> the id of the player which win the tournment</returns>
    string DetermineWinner()
    {
        string animalType1;
        string animalType2;
        int length = Convert.ToInt32(Math.Log(players.Count, 2));
        //tournament Algorthim
        for (int i = 0; i < length; i++)
        {
            playersInfoTemp = new List<playerData>(players); // use temp to restore the new players winner again in the main list for them
            players.Clear(); //clearing the player list to open new one
            for (int j = 0; j < playersInfoTemp.Count / 2; j++)
            {
                //define round
                animalType1 = playersInfoTemp[j].animalType;
                animalType2 = playersInfoTemp[j + 1].animalType;
                ApllyGameRoles(playersInfoTemp[j].ID, animalType1, playersInfoTemp[j + 1].ID, animalType2);
            }
        }
        return players[0].ID.ToString();
    }
    /// <summary>
    /// checking every round the two player data and determine which will one
    /// </summary>
    /// <param name="playerindex1"> the first player id </param>
    /// <param name="type1"> the first player animal </param>
    /// <param name="playerindex2"> the second player id</param>
    /// <param name="type2"> the second playeranimal </param>
    private void ApllyGameRoles(int playerindex1, string type1, int playerindex2, string type2)
    {
        //check if the player have the same animal
        if (type1 == type2)
        {
            if (playerindex1 > playerindex2)
            {
                players.Add(new playerData() { ID = playerindex1, animalType = type1 });
                return;
            }
            else
            {
                players.Add(new playerData() { ID = playerindex2, animalType = type2 });
                return;

            }
        }
        //checking the roles
        if (gameRules[type1].animal1 == type2 || gameRules[type1].animal2 == type2)
        {
            //type one win
            players.Add(new playerData() { ID = playerindex1, animalType = type1 });
        }
        else
        {
            //type two win
            players.Add(new playerData() { ID = playerindex2, animalType = type2 });
        }
    }

    /// <summary>
    /// getting the player info data from xml file
    /// </summary>
    /// <param name="item"> each node which hold the player data for every single player</param>
    void PlayerDataGetter(XmlNode item)
    {
        PlayerPropertiesHolder _playerInfo = new PlayerPropertiesHolder(item);
        players.Add(new playerData() { ID = _playerInfo.playerID, animalType = _playerInfo.animalType });
    }

    /// <summary>
    /// the class is to parsing the entire node which we sent to in the constructor
    /// the class parsing the node in xml file 
    /// </summary>
    class PlayerPropertiesHolder
    {
        //defining the propreties for each player
        public int playerID { get; private set; }
        public string animalType { get; private set; }
        /// <summary>
        /// parsing node to get the data for the entir item
        /// </summary>
        /// <param name="curItemNode"> passing the node holder path to get the data for each mode </param>
        public PlayerPropertiesHolder(XmlNode curItemNode)
        {
            playerID = int.Parse(curItemNode.Attributes["ID"].Value);
            animalType = curItemNode.Attributes["AnimalType"].Value;
        }
    }

    /// <summary>
    /// class to hold the players data while parsing it
    /// </summary>
    class playerData
    {
        public int ID { get; set; }
        public string animalType { get; set; }
    }
}