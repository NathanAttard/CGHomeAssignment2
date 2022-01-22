using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Code")]
    [SerializeField] private TMP_InputField gameCode;
    [SerializeField] private TMP_InputField enterGameCode;
    [SerializeField] private GameObject waitingForOpponent;

    [Header("Players")]
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_Text p1Name;
    [SerializeField] private TMP_Text p2Name;

    [Header("Buttons")]
    [SerializeField] private GameObject rockBtn;
    [SerializeField] private GameObject paperBtn;
    [SerializeField] private GameObject scissorsBtn;
    
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
            case "Game":
                FirebaseController.CheckChoices();
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

    public void OptionChosen(string option)
    {
        Debug.Log("Option Chosen: " + option);

        switch (option)
        {
            case "Rock":
                rockBtn.GetComponent<Button>().enabled = false;
                paperBtn.GetComponent<Button>().interactable = false;
                scissorsBtn.GetComponent<Button>().interactable = false;
                break;
            case "Paper":
                rockBtn.GetComponent<Button>().interactable = false;
                paperBtn.GetComponent<Button>().enabled = false;
                scissorsBtn.GetComponent<Button>().interactable = false;
                break;
            case "Scissors":
                rockBtn.GetComponent<Button>().interactable = false;
                paperBtn.GetComponent<Button>().interactable = false;
                scissorsBtn.GetComponent<Button>().enabled = false;
                break;
        }

        if (FirebaseController.isPlayer1 == true)
        {
            FirebaseController.player1Choice = option;

            FirebaseController.UpdatePlayerChoiceFB(FirebaseController.player1, FirebaseController.player1Choice);

            Debug.Log("Player 1 Choice " + FirebaseController.player1Choice);
            Debug.Log("Player 2 Choice " + FirebaseController.player2Choice);
        }
        else if (FirebaseController.isPlayer2 == true)
        {
            FirebaseController.player2Choice = option;

            FirebaseController.UpdatePlayerChoiceFB(FirebaseController.player2, FirebaseController.player2Choice);

            Debug.Log("Player 1 Choice " + FirebaseController.player1Choice);
            Debug.Log("Player 2 Choice " + FirebaseController.player2Choice);
        }
    }
}
