using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private int score;
    private const string HighScoreKey = "HighScore";
    private void Start()
    {
        score = 0;
        textMesh.text = score.ToString();
        if (!PlayerPrefs.HasKey(HighScoreKey))
        {
            PlayerPrefs.SetInt(HighScoreKey,0);
            PlayerPrefs.Save();
        }
    }

    public void AddScore(int value)
    {
        score += value;
        textMesh.text = score.ToString();
        var highScore = PlayerPrefs.GetInt(HighScoreKey);
        if (score > highScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }
    }
}
