using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class FirebaseController : MonoBehaviour
{
    public static string key;
    public static Player player1;
    public static Player player2;

    public static bool isKeyCorrect;

    private static DatabaseReference databaseReference;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        player1 = new Player();
        player2 = new Player();
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

        Debug.Log("Player 1 added to Firebase");

        GameManager.LoadScene("Lobby");
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

    public static IEnumerator AddSecondPlayerFB()
    {
        new WaitForSeconds(1f);

        string p2Json = JsonUtility.ToJson(player2);

        yield return databaseReference.Child("Games").Child(key).Child("Player 2").SetRawJsonValueAsync(p2Json);

        Debug.Log("Player 2 added to Firebase");

        GameManager.LoadScene("Lobby");
    }
}
