using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance { get; private set; }
    [SerializeField] TextMeshProUGUI yellowScoreText;
    [SerializeField] TextMeshProUGUI purpleScoreText;
    [SerializeField] TextMeshProUGUI blueScoreText;
    [SerializeField] TextMeshProUGUI greenScoreText;

    [SerializeField] GameObject finalImage;
    [SerializeField] TextMeshProUGUI finalMessage;

    [SerializeField] List<int> scores;
    public int yellowScore = 0;
    public int purpleScore = 0;
    public int blueScore = 0;
    public int greenScore = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ChangeScore(Team team)
    {
        switch (team)
        {
            case Team.Green:
                greenScore++;
                greenScoreText.text = greenScore.ToString();
                break;
            case Team.Purple: 
                purpleScore++;
                purpleScoreText.text = purpleScore.ToString();
                break;
            case Team.Blue: 
                blueScore++;
                blueScoreText.text = blueScore.ToString();
                break;
            case Team.Yellow: 
                yellowScore++;
                yellowScoreText.text = yellowScore.ToString();
                break;
        }

    }

    public void ShowFinalMessage()
    {
        finalImage.SetActive(true);
        scores = new List<int>();
        scores.Add(yellowScore);
        scores.Add(purpleScore);
        scores.Add(blueScore);
        scores.Add(greenScore);
        scores.Sort();

        int highestScore = scores[scores.Count - 1];

        if(highestScore == yellowScore)
        {
            finalMessage.text = "Yellow Team WINS";
        }
        else if(highestScore == purpleScore)
        {
            finalMessage.text = "Purple Team WINS";
        }
        else if(highestScore == greenScore)
        {
            finalMessage.text = "Green Team WINS";
        }
        else if(highestScore == blueScore)
        {
            finalMessage.text = "Blue Team WINS";
        }

        StartCoroutine(RestartGame(6f));
    }
    IEnumerator RestartGame(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        SceneManager.LoadScene(0);

    }
    
}
