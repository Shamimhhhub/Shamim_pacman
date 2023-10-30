using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private float time;
    private const string HighTimeKey = "HighTime";
    private void Update()
    {
        time += Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        textMesh.text = timeSpan.ToString("mm':'ss':'ff");
    }

    private void OnApplicationQuit()
    {
        UpdateHighTimePrefs();
    }
    private void OnDestroy()
    {
        UpdateHighTimePrefs();
    }
    public void UpdateHighTimePrefs()
    {
        var highTime = PlayerPrefs.GetFloat(HighTimeKey, 0);
        if (time > highTime)
        {
            PlayerPrefs.SetFloat(HighTimeKey, time);
            PlayerPrefs.Save();
        }
    }
}
