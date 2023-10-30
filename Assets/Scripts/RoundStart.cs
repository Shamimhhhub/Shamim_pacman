using System.Collections;
using TMPro;
using UnityEngine;

public class RoundStart : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private PacStudentController pacStudentController;
    void Start()
    {
        StartCoroutine(RoundStartRoutine());
    }

    private IEnumerator RoundStartRoutine()
    {
        textMesh.text = "3";
        var wait1Sec = new WaitForSeconds(1);
        yield return wait1Sec;
        textMesh.text = "2";
        yield return wait1Sec;
        textMesh.text = "1";
        yield return wait1Sec;
        textMesh.text = "GO!";
        yield return wait1Sec;
        pacStudentController.enabled = true;
        gameObject.SetActive(false);
    }
}
