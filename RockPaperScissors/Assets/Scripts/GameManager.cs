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

    [Header("Lobby")]
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_Text p1Name;
    [SerializeField] private TMP_Text p2Name;
    [SerializeField] private GameObject waitingForOpponent;

    [Header("Game UI")]
    [SerializeField] private TMP_Text player1Name;
    [SerializeField] private TMP_Text player2Name;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private GameObject waitingForChoice;
    [SerializeField] private TMP_Text p1Choice;
    [SerializeField] private TMP_Text p2Choice;
    [SerializeField] private TMP_Text p1Win;
    [SerializeField] private TMP_Text p2Win;

    [Header("Buttons")]
    [SerializeField] private GameObject rockBtn;
    [SerializeField] private GameObject paperBtn;
    [SerializeField] private GameObject scissorsBtn;

    Coroutine roundTimer;
    float roundTime = 0.10f;
    float currentTime;
    
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
            case "Game":
                player1Name.text = FirebaseController.player1.Name;
                player2Name.text = FirebaseController.player2.Name;
                p1Win.text = FirebaseController.player1.Wins.ToString();
                p2Win.text = FirebaseController.player2.Wins.ToString();
                waitingForChoice.SetActive(false);
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTime = roundTime;
        roundTimer = StartCoroutine(RoundTime());
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
                DisplayResults();
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

    public IEnumerator RoundTime()
    {
        while (true)
        {
            currentTime = currentTime - 0.01f;
            currentTime = (float)Mathf.Round(currentTime * 100f) / 100f;
            string time = currentTime.ToString("00.00").Replace('.', ':');

            timer.text = time;

            yield return new WaitForSeconds(1f);
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

            Debug.Log("PLAYER 2 CHOICE: " + FirebaseController.player2Choice);

            if (FirebaseController.player2Choice == "")
            {
                waitingForChoice.SetActive(true);
            }

            FirebaseController.UpdatePlayerChoiceFB(FirebaseController.player1, FirebaseController.player1Choice);

            Debug.Log("Player 1 Choice " + FirebaseController.player1Choice);
            Debug.Log("Player 2 Choice " + FirebaseController.player2Choice);
        }
        else if (FirebaseController.isPlayer2 == true)
        {
            FirebaseController.player2Choice = option;

            if (FirebaseController.player1Choice == "")
            {
                waitingForChoice.SetActive(true);
            }

            FirebaseController.UpdatePlayerChoiceFB(FirebaseController.player2, FirebaseController.player2Choice);

            Debug.Log("Player 1 Choice " + FirebaseController.player1Choice);
            Debug.Log("Player 2 Choice " + FirebaseController.player2Choice);
        }
    }

    public void DisplayResults()
    {
        if (FirebaseController.player1Choice != "" && FirebaseController.player2Choice != "" && currentTime > 0f)
        {
            waitingForChoice.SetActive(false);
            StopCoroutine(roundTimer);
            Win(FirebaseController.player1Choice, FirebaseController.player2Choice);
        }
        else if (currentTime <= 0f)
        {
            if (FirebaseController.player1Choice == "")
            {
                rockBtn.GetComponent<Button>().interactable = false;
                paperBtn.GetComponent<Button>().interactable = false;
                scissorsBtn.GetComponent<Button>().interactable = false;
                FirebaseController.UpdatePlayerChoiceFB(FirebaseController.player1, "None");
            }
            else if (FirebaseController.player2Choice == "")
            {
                rockBtn.GetComponent<Button>().interactable = false;
                paperBtn.GetComponent<Button>().interactable = false;
                scissorsBtn.GetComponent<Button>().interactable = false;
                FirebaseController.UpdatePlayerChoiceFB(FirebaseController.player2, "None");
            }

            waitingForChoice.SetActive(false);
            StopCoroutine(roundTimer);
            Win(FirebaseController.player1Choice, FirebaseController.player2Choice);
        }

        p1Choice.text = FirebaseController.player1Choice;
        p2Choice.text = FirebaseController.player2Choice;

        
    }

    public void Win(string player1Result, string player2Result)
    {
        switch (player1Result)
        {
            case "Rock":
                switch (player2Result)
                {
                    case "Rock":
                        break;
                    case "Paper":
                        FirebaseController.player2.Wins++;
                        break;
                    case "Scissors":
                        FirebaseController.player1.Wins++;
                        break;
                    case "None":
                        FirebaseController.player1.Wins++;
                        break;
                }
                break;
            case "Paper":
                switch (player2Result)
                {
                    case "Rock":
                        FirebaseController.player1.Wins++;
                        break;
                    case "Paper":
                        break;
                    case "Scissors":
                        FirebaseController.player2.Wins++;
                        break;
                    case "None":
                        FirebaseController.player1.Wins++;
                        break;
                }
                break;
            case "Scissors":
                switch (player2Result)
                {
                    case "Rock":
                        FirebaseController.player2.Wins++;
                        break;
                    case "Paper":
                        FirebaseController.player1.Wins++;
                        break;
                    case "Scissors":
                        break;
                    case "None":
                        FirebaseController.player1.Wins++;
                        break;
                }
                break;
            case "None":
                if (player2Result != "None")
                {
                    FirebaseController.player2.Wins++;
                }
                break;
        }

        //FirebaseController.player1Choice = "";
        //FirebaseController.player2Choice = "";

        Debug.Log("P1 Score:" + FirebaseController.player1.Wins);
        Debug.Log("P2 Score:" + FirebaseController.player2.Wins);
    }
}
