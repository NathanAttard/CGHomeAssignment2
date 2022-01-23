using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class FirebaseController : MonoBehaviour
{
    public static string key;
    public static Player player1 = new Player();
    public static Player player2 = new Player();

    public static bool isPlayer1 = false;
    public static bool isPlayer2 = false;

    public static bool isKeyCorrect;

    public static bool player1Ready = false;
    public static bool player2Ready = false;

    public static string player1Choice = "";    
    public static string player2Choice = "";

    public static int player1Wins = 0;
    public static int player2Wins = 0;

    static string p1WinsString;
    static string p2WinsString;

    public static string winner = "";

    private static DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        p1WinsString = player1Wins.ToString();
        p2WinsString = player2Wins.ToString();
    }

    public static IEnumerator CreateGameFB(string p1Name)
    {
        key = databaseReference.Child("Games").Push().Key;

        player1.Name = p1Name;
        player1.ID = 1;
        player1.Wins = "0";
        string p1Json = JsonUtility.ToJson(player1);

        yield return databaseReference.Child("Games").Child(key).Child("Player_1").SetRawJsonValueAsync(p1Json);

        databaseReference.Child("Games").Child(key).ValueChanged += HandlePlayerChanged;

        Debug.Log("Player 1 added to Firebase");

        GameManager.LoadScene("Lobby");

        isPlayer1 = true;

        Debug.Log("This is P1: " + isPlayer1);
        Debug.Log("This is P2: " + isPlayer2);
    }

    public static void HandlePlayerChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Player Joining");

            foreach (var player in args.Snapshot.Children)
            {
                if (player.Key == "Player_1")
                {
                    foreach (var child in player.Children)
                    {
                        if (child.Key == "Name")
                        {
                            player1.Name = child.Value.ToString();
                        }
                    }

                }
                else if (player.Key == "Player_2")
                {
                    foreach (var child in player.Children)
                    {
                        if (child.Key == "Name")
                        {
                            player2.Name = child.Value.ToString();
                        }
                    }
                }
            }

            Debug.Log("Player Joined");

            Debug.Log("This is P1: " + isPlayer1);
            Debug.Log("This is P2: " + isPlayer2);
        }
    }

    public static IEnumerator CheckKey(string key)
    {
        yield return databaseReference.Child("Games").Child(key).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.Value);

                if (snapshot.Value != null)
                {
                    Debug.Log("Correct Key");
                    isKeyCorrect = true;
                }
                else
                {
                    Debug.Log("Incorrect Key");
                    isKeyCorrect = false;
                }
            }
        });
    }

    public static IEnumerator AddSecondPlayerFB(string key)
    {
        player2.ID = 2;
        player2.Wins = "0";
        string p2Json = JsonUtility.ToJson(player2);

        yield return databaseReference.Child("Games").Child(key).Child("Player_2").SetRawJsonValueAsync(p2Json);

        databaseReference.Child("Games").Child(key).ValueChanged += HandlePlayerChanged;

        Debug.Log("Player 2 added to Firebase");

        GameManager.LoadScene("Lobby");

        isPlayer2 = true;

        Debug.Log("This is P1: " + isPlayer1);
        Debug.Log("This is P2: " + isPlayer2);
    }

    public static void UpdatePlayerStatusFB(Player player, bool playerStatus)
    {
        Debug.Log("Updating Player Status");

        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["Games/" + key + "/Player_" + player.ID + "/isReady"] = playerStatus;

        databaseReference.UpdateChildrenAsync(result);
        databaseReference.Child("Games").Child(key).ValueChanged += HandleStatusChanged;

        Debug.Log("Player Status Updated");
    }

    public static void HandleStatusChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Status Changed");
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

            foreach (var player in args.Snapshot.Children)
            {
                switch (player.Key)
                {
                    case "Player_1":
                        player1Ready = (bool)player.Child("isReady").Value;
                        break;
                    case "Player_2":
                        player2Ready = (bool)player.Child("isReady").Value;
                        break;
                    default:
                        break;
                }
            }

            Debug.Log("Status Updated");
        }
    }

    public static void CheckStatus()
    {
        if (player1Ready == true && player2Ready == true)
        {
            GameManager.LoadScene("Game");
        }
    }

    public static void UpdatePlayerChoiceFB(Player player, string playerChoice)
    {
        Debug.Log("Updating Player Choice");

        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["Games/" + key + "/Player_" + player.ID + "/Choice"] = playerChoice;

        databaseReference.UpdateChildrenAsync(result);
        databaseReference.Child("Games").Child(key).ValueChanged += HandleChoiceChanged;

        Debug.Log("Player Choice Updated");
    }

    public static void HandleChoiceChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Choice Changed");
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

            foreach (var player in args.Snapshot.Children)
            {
                switch (player.Key)
                {
                    case "Player_1":
                        player1Choice = player.Child("Choice").Value.ToString();
                        break;
                    case "Player_2":
                        player2Choice = player.Child("Choice").Value.ToString();
                        break;
                    default:
                        break;
                }
            }

            Debug.Log("Choice Updated");
        }
    }

    public static void CheckChoices()
    {
        Debug.Log("Player 1 Choice " + player1Choice);
        Debug.Log("Player 2 Choice " + player2Choice);
    }

    public static void UpdatePlayerWinsFB(Player player, int playerWins)
    {
        Debug.Log("Updating Player Wins");

        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["Games/" + key + "/Player_" + player.ID + "/Wins"] = playerWins;

        databaseReference.UpdateChildrenAsync(result);
        databaseReference.Child("Games").Child(key).ValueChanged += HandleWinsChanged;

        Debug.Log("Player Wins Updated");
    }

    public static void HandleWinsChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Wins Changed");
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

            foreach (var player in args.Snapshot.Children)
            {
                switch (player.Key)
                {
                    case "Player_1":
                        p1WinsString = player.Child("Wins").Value.ToString();
                        break;
                    case "Player_2":
                        p2WinsString = player.Child("Wins").Value.ToString();
                        break;
                    default:
                        break;
                }
            }

            Debug.Log("Wins Updated");
        }
    }
}
