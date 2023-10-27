using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuHighScoreSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI time;
    private const string HighScoreKey = "HighScore";
    private const string HighTimeKey = "HighTime";
    private void OnEnable()
    {
        score.text = PlayerPrefs.GetInt(HighScoreKey, 0).ToString();
        var timespan = TimeSpan.FromSeconds(PlayerPrefs.GetFloat(HighTimeKey,0));
        time.text = timespan.ToString("mm':'ss':'ff");
    }
}
