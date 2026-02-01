using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public MicReader micReader;
    public AirBar airBar;
    public Diver diver;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winText;
    public Button restartButton;

    private bool isGameOver = false;

    public List<string> noAirMessages = new List<string>
    {
        "Out of air. Skill issue.",
        "Oxygen subscription expired.",
        "Your tank is empty. Sucks to be you.",     
    };

    public List<string> spikeMessages = new List<string>
    {
        "You are now a kebab. \n Underwater edition.",
        "Stabbed by geology.",
        "Rock bottom. Literally.",
        "Spiky boi.",
        "You tried to hug a rock.\n It did not hug back.",
        "Cave said: ‘No touchy.’ \n You touched.",
        "Congratulations! \n cYou are now part of the environment."
    };

    public List<string> winMessages = new List<string>
    {
        "Congratulations! You may breathe like a normal person again.",
        "You live! Please leave a 5-star review for Oxygen™.",
        "You made it out. The cave says ‘bruh.’",
        "Success! Your lungs have filed a complaint anyway.",
    };

    void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        if (micReader != null && micReader.air <= 0f)
        {
            TriggerGameOver("no_air");
        }
    }

    public void TriggerGameOver(string cause)
    {
        isGameOver = true;

        if (cause == "no_air")
        {
            gameOverText.text = noAirMessages[Random.Range(0, noAirMessages.Count)];
        }
        else if (cause == "spike")
        {
            gameOverText.text = spikeMessages[Random.Range(0, spikeMessages.Count)];
        }
        else
        {
            gameOverText.text = "Game Over";
        }

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        // Common options:
        // 1) Freeze gameplay:
        Time.timeScale = 0f;
        micReader.StopMic();


        // 2) Or disable player controls / spawners instead of timeScale
        // diver.enabled = false;  // only if Diver is a MonoBehaviour component
    }

    public void TriggerWin()
    {
        isGameOver = true;

        winText.text = winMessages[Random.Range(0, winMessages.Count)];

        if (winText != null)
            winText.gameObject.SetActive(true);

        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        // Common options:
        // 1) Freeze gameplay:
        Time.timeScale = 0f;
        micReader.StopMic();


        // 2) Or disable player controls / spawners instead of timeScale
        // diver.enabled = false;  // only if Diver is a MonoBehaviour component
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
