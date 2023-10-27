using System;
using System.Collections;
using UnityEngine;

public enum InputDirection
{
    None,
    Up,
    Left,
    Down,
    Right
}
public class PacStudentController : MonoBehaviour
{
    [SerializeField] private LevelGenerator level;
    //Sfx
    [SerializeField] private AudioSource movingSfx;
    //Animator
    [SerializeField] private Animator animator;
    //Movement
    private int currentRow = 1;
    private int currentCol = 1;
    private bool moving = false;
    //Input
    private InputDirection lastInput = InputDirection.None;
    private InputDirection currentInput = InputDirection.None;
    private void Update()
    {
        GetInput();
        if (!moving)
            MoveInGrid();
        UpdateSfxStates();
        UpdateAnimatorState();
    }

    private void UpdateAnimatorState()
    {
        animator.SetBool("Moving", moving);
        if(lastInput == InputDirection.Left)
            animator.SetBool("FacingRight", false);
        else if(lastInput == InputDirection.Right)
            animator.SetBool("FacingRight", true);
    }

    private void UpdateSfxStates()
    {
        movingSfx.enabled = moving;
    }

    private void MoveInGrid()
    {
        var row = currentRow;
        var col = currentCol;
        switch (lastInput)
        {
            case InputDirection.Up:
                row--;
                break;
            case InputDirection.Left:
                col--;
                break;
            case InputDirection.Down:
                row++;
                break;
            case InputDirection.Right:
                col++;
                break;
        }

        if (level.IsPath(row, col) && lastInput != InputDirection.None)
        {
            currentRow = row;
            currentCol = col;
            currentInput = lastInput;
            StartCoroutine(MoveWithCurrentInput());
        }
    }

    private IEnumerator MoveWithCurrentInput()
    {
        var startPos = transform.position;
        var endPos = GetEndPos();
        moving = true;
        float t = 0;
        while (t < .2f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t/.2f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        moving = false;
        MoveInGrid();
    }

    private Vector3 GetEndPos()
    {
        switch (currentInput)
        {
            case InputDirection.Up:
                return transform.position + Vector3.up;
            case InputDirection.Left:
                return transform.position + Vector3.left;
            case InputDirection.Down:
                return transform.position + Vector3.down;
            case InputDirection.Right:
                return transform.position + Vector3.right;
        }
        return transform.position;
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            lastInput = InputDirection.Up;
        if (Input.GetKeyDown(KeyCode.A))
            lastInput = InputDirection.Left;
        if (Input.GetKeyDown(KeyCode.S))
            lastInput = InputDirection.Down;
        if (Input.GetKeyDown(KeyCode.D))
            lastInput = InputDirection.Right;
    }
    
}
