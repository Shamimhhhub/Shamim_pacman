using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CherryMovement : MonoBehaviour
{
    [SerializeField] private GameObject cherryPrefab;

    private void Start()
    {
        StartCoroutine(SpawnCherries());
    }

    private IEnumerator SpawnCherries()
    {
        var wait = new WaitForSeconds(10);
        while (true)
        {
            yield return wait;
            SpawnCherry();
        }
    }

    private void SpawnCherry()
    {
        Vector3 startPosition = new();
        Vector3 endPosition = new();
        var vertical = Random.value < .5f;
        var reverse = Random.value < .5f;
        if (vertical)
        {
            startPosition.x = Random.Range(-12, 13);
            endPosition.x = startPosition.x;
            if (reverse)
            {
                startPosition.y = 18;
                endPosition.y = -18;
            }
            else
            {
                startPosition.y = -18;
                endPosition.y = 18;
            }
        }
        else
        {
            startPosition.y = Random.Range(-12, 13);
            endPosition.y = startPosition.y;
            if (reverse)
            {
                startPosition.x = 31;
                endPosition.x = -31;
            }
            else
            {
                startPosition.x = -31;
                endPosition.x = 31;
            }
        }

        StartCoroutine(SpawnMoveAndDestroyCherry(startPosition, endPosition, vertical ? 5 : 7));
    }

    private IEnumerator SpawnMoveAndDestroyCherry(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        var cherry = Instantiate(cherryPrefab, startPosition, Quaternion.identity, transform);

        float t = 0;
        while (t<duration)
        {
            cherry.transform.position = Vector3.Lerp(startPosition,endPosition,t/duration);
            t += Time.deltaTime;
            yield return null;
        }
        if(cherry!=null)
            Destroy(cherry);
    }

}
