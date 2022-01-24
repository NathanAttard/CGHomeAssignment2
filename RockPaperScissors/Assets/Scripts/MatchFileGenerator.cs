using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MatchFileGenerator : MonoBehaviour
{
    private string matchID;
    private int player1Moves;
    private int player2Moves;
    private string gameWinner;
    private string matchTime;

    // Start is called before the first frame update
    void Start()
    {
        matchID = FirebaseController.key;
        player1Moves = FirebaseController.player1.Moves;
        player2Moves = FirebaseController.player2.Moves;
        gameWinner = FirebaseController.winner;
        matchTime = FirebaseController.matchTime;

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageReference = storage.RootReference;

        string dataString = "Match ID: " + matchID + "\nPlayer 1 Moves: " + player1Moves + "\nPlayer 2 Moves: " + player2Moves + "\nWinner: " + gameWinner + "\nMatch Time: " + matchTime;
        byte[] data = Encoding.ASCII.GetBytes(dataString);

        StartCoroutine(UploadTextFile(storageReference, data));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator UploadTextFile(StorageReference storageRef, byte[] data)
    {
        StorageReference textFile = storageRef.Child(FirebaseController.key + ".txt");

        yield return textFile.PutBytesAsync(data).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                StorageMetadata metadata = task.Result;
                string mdHash = metadata.Md5Hash;
                Debug.Log("Finished uploading ...");
                Debug.Log("MD5 Hash = " + mdHash);
            }
        });
    }
}
