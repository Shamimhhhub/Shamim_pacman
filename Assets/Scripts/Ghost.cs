using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;
    }

    private bool scared = false;
    private bool alive = true;
    public bool IsScared()
    {
        return scared;
    }

    public bool IsAlive()
    {
        return alive;
    }

    public void Scare()
    {
        if(!alive)
            return;
        scared = true;
        animator.SetTrigger("Scare");
        StartCoroutine(ScareTime());
    }

    private IEnumerator ScareTime()
    {
        if(!alive)
            yield break;
        yield return new WaitForSeconds(7);
        if(scared)
            animator.SetTrigger("Recover");
        yield return new WaitForSeconds(3);
        if (scared)
            scared = false;
    }
    
    public void DeathAndRespawn()
    {
        StopAllCoroutines();
        StartCoroutine(DeathAndSpawnRoutine());
    }

    private IEnumerator DeathAndSpawnRoutine()
    {
        alive = false;
        scared = false;
        animator.SetTrigger("Death");
        float time = 0;
        var currentPos = transform.position;
        while (time < 5)
        {
            transform.position = Vector3.Lerp(currentPos, startPos, time / 5);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
        animator.SetTrigger("Respawn");
        alive = true;
    }
}
