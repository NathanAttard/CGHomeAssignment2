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

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            gameCode.text = FirebaseController.key;
            p1Name.text = FirebaseController.player1.Name;

            if (FirebaseController.player2.Name != "")
            {
                p2Name.text = FirebaseController.player2.Name;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                StartCoroutine(FirebaseController.AddSecondPlayerFB());
            }
        }
    }
}
