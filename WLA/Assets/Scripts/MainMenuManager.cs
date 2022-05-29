using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI timeTMP;
    public TextMeshProUGUI scoreTMP;
    public GameObject resetButtom;

    void Start()
    {
        SetHighScore();
    }

    void SetHighScore()
    {
        int bestTime;
        int highScore;
        bestTime = PlayerPrefs.GetInt("BestTime", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        timeTMP.text = "";
        scoreTMP.text = "";

        bool showBestGame = (bestTime != 0) && (highScore != 0);
        
        resetButtom.SetActive(false);
        if (showBestGame)
        {
            timeTMP.text = "Best time: " + bestTime;
            scoreTMP.text = "Best score: " + highScore;
            resetButtom.SetActive(true);
        }

    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        SetHighScore();
    }

    public void StartGame(int index)
    {
        string seed = inputField.GetComponent<TMP_InputField>().text;

        // Debug.Log("Initial seed = " + seed);
        GameReferences.initialSeed = seed.ToLower();

        SceneManager.LoadScene(1);
    }

    public void QuitApplication()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}

