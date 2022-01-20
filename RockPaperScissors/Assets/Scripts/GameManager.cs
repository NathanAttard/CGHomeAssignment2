using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField gameCode;
    [SerializeField] private TMP_InputField enterGameCode;
    [SerializeField] private TMP_Text p1Name;
    [SerializeField] private TMP_Text p2Name;
    [SerializeField] private GameObject waitingForOpponent;

    private void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Lobby":
                waitingForOpponent.SetActive(false);
                gameCode.text = FirebaseController.key;
                p1Name.text = FirebaseController.player1.Name;

                if (FirebaseController.player2.Name != "")
                {
                    p2Name.text = FirebaseController.player2.Name;
                }
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Lobby":
                UpdateLobbyUI();
                FirebaseController.CheckStatus();
                break;
            default:
                break;
        }
    }

    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void CreateGame()
    {
        if (playerName.text != "")
        {
            StartCoroutine(FirebaseController.CreateGameFB(playerName.text));
        }
    }

    public void EnterCode()
    {
        if (playerName.text != "")
        {
            FirebaseController.player2.Name = playerName.text;

            LoadScene("Join");
        }
    }

    public void JoinGame()
    {
        if (enterGameCode.text != "")
        {
            StartCoroutine(FirebaseController.CheckKey(enterGameCode.text));

            if (FirebaseController.isKeyCorrect == true)
            {
                StartCoroutine(FirebaseController.AddSecondPlayerFB(enterGameCode.text));
            }

            FirebaseController.key = enterGameCode.text;
        }
    }

    public void UpdateLobbyUI()
    {
        if (FirebaseController.player1.Name != "" && FirebaseController.player2.Name != "" && FirebaseController.key != "")
        {
            gameCode.text = FirebaseController.key;
            p1Name.text = FirebaseController.player1.Name;
            p2Name.text = FirebaseController.player2.Name;
        }

        if (p1Name.text == "")
        {
            p1Name.text = FirebaseController.player1.Name;
        }
        else if (p2Name.text == "")
        {
            p2Name.text = FirebaseController.player2.Name;
        }
    }

    public void StartGame()
    {
        waitingForOpponent.SetActive(true);

        if (FirebaseController.isPlayer1 == true)
        {
            FirebaseController.player1Ready = true;

            FirebaseController.UpdatePlayerStatusFB(FirebaseController.player1, FirebaseController.player1Ready);

            Debug.Log("Player 1 Ready? " + FirebaseController.player1Ready);
            Debug.Log("Player 2 Ready? " + FirebaseController.player2Ready);
        }
        else if (FirebaseController.isPlayer2 == true)
        {
            FirebaseController.player2Ready = true;

            FirebaseController.UpdatePlayerStatusFB(FirebaseController.player2, FirebaseController.player2Ready);

            Debug.Log("Player 1 Ready? " + FirebaseController.player1Ready);
            Debug.Log("Player 2 Ready? " + FirebaseController.player2Ready);
        }
    }
}
