using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class FirebaseController : MonoBehaviour
{
    public static string key;
    public static Player player1 = new Player();
    public static Player player2 = new Player();

    public static bool isKeyCorrect;

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
        
    }

    public static IEnumerator CreateGameFB(string p1Name)
    {
        key = databaseReference.Child("Games").Push().Key;

        player1.Name = p1Name;
        string p1Json = JsonUtility.ToJson(player1);

        yield return databaseReference.Child("Games").Child(key).Child("Player 1").SetRawJsonValueAsync(p1Json);

        databaseReference.Child("Games").Child(key).ValueChanged += HandlePlayerChanged;

        Debug.Log("Player 1 added to Firebase");

        GameManager.LoadScene("Lobby");
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
                if (player.Key == "Player 1")
                {
                    foreach (var child in player.Children)
                    {
                        if (child.Key == "Name")
                        {
                            player1.Name = child.Value.ToString();
                        }
                    }

                }
                else if (player.Key == "Player 2")
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
        string p2Json = JsonUtility.ToJson(player2);

        yield return databaseReference.Child("Games").Child(key).Child("Player 2").SetRawJsonValueAsync(p2Json);

        databaseReference.Child("Games").Child(key).ValueChanged += HandlePlayerChanged;

        Debug.Log("Player 2 added to Firebase");

        GameManager.LoadScene("Lobby");
    }
}
