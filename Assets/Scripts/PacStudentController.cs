using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacStudentController : MonoBehaviour
{
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private GameObject wallParticlePrefab;
    [SerializeField] private LevelGenerator level;
    //Sfx
    [Space]
    [SerializeField] private AudioSource movingSfx;
    [SerializeField] private AudioClip pelletSfx;
    [SerializeField] private AudioClip wallSfx;
    [Space]
    //Music
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip normalClip;
    [SerializeField] private AudioClip scareClip;
    [SerializeField] private AudioClip deadClip;
    //References
    [Space] 
    [SerializeField] private GameObject[] lives;
    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Animator animator;
    [SerializeField] private ScoreController score;
    [SerializeField] private TextMeshProUGUI ghostTimer;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Collider2D col2D;
    //Movement
    private int currentRow = 1;
    private int currentCol = 1;
    private bool moving;
    //Input
    private InputDirection lastInput = InputDirection.None;
    private InputDirection currentInput = InputDirection.None;
    //state
    private bool wallBump;
    private bool teleport;
    private bool gameover;
    private int life = 3;
    private bool powerUpped;
    private Coroutine powerPillRoutine;
    private Vector3 startingPos;
    private int pelletsCount;
    private void Start()
    {
        startingPos = transform.position;
        pelletsCount = level.GetPelletCount();
    }

    private void OnEnable()
    {
        col2D.enabled = true;
        music.clip = normalClip;
        music.Play();
    }

    private void Update()
    {
        if(gameover)
            return;
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
        else if(lastInput == InputDirection.Right || lastInput == InputDirection.None)
            animator.SetBool("FacingRight", true);
    }

    private void UpdateSfxStates()
    {
        movingSfx.enabled = moving;
    }

    private void MoveInGrid()
    {
        if (lastInput == InputDirection.None) return;

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

        currentRow = row;
        currentCol = col;
        currentInput = lastInput;
        StartCoroutine(MoveWithCurrentInput());
    }

    private IEnumerator MoveWithCurrentInput()
    {
        var startPos = transform.position;
        var endPos = GetEndPos();
        moving = true;
        float t = 0;
        while (t < .2f && !wallBump)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t/.2f);
            t += Time.deltaTime;
            yield return null;
        }

        if (wallBump)
        {
            lastInput = InputDirection.None;
            endPos = startPos;
            switch (currentInput)
            {
                case InputDirection.Up:
                    currentRow++;
                    break;
                case InputDirection.Left:
                    currentCol++;
                    break;
                case InputDirection.Down:
                    currentRow--;
                    break;
                case InputDirection.Right:
                    currentCol--;
                    break;
            }
        }
   
        transform.position = endPos;
        if (teleport)
        {
            switch (currentInput)
            {
                case InputDirection.Right:
                    transform.position += Vector3.left * 28;
                    currentCol -= 28;
                    break;
                case InputDirection.Left:
                    transform.position += Vector3.right * 28;
                    currentCol += 28;
                    break;
            }
        }
        moving = false;
        if(!wallBump)
            MoveInGrid();
        teleport = false;
        wallBump = false;
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
    
    //Collisions
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            wallBump = true;
            var pos = (other.transform.position + transform.position) / 2;
            Instantiate(wallParticlePrefab, pos, Quaternion.identity);
            AudioSource.PlayClipAtPoint(wallSfx, Vector3.zero);
        }
        else if (other.CompareTag("Teleport"))
        {
            teleport = true;
        }
        else if (other.CompareTag("Pellet"))
        {
            pelletsCount--;
            Destroy(other.gameObject);
            score.AddScore(10);
            animator.SetTrigger("Bite");
            AudioSource.PlayClipAtPoint(pelletSfx,Vector3.zero);
            if (pelletsCount <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(ShowGameOver());
            }
        }
        else if(other.CompareTag("Cherry"))
        {
            Destroy(other.gameObject);
            score.AddScore(100);
        }
        else if(other.CompareTag("PowerPill"))
        {
            Destroy(other.gameObject);
            if(powerPillRoutine!=null)
                StopCoroutine(powerPillRoutine);
            powerPillRoutine = StartCoroutine(PowerPillRoutine());
        }
        else if (other.CompareTag("Ghost"))
        {
            if (powerUpped)
            {
                var ghost = other.GetComponent<Ghost>();
                if (!ghost.IsScared()) return;
                if (music.clip != deadClip)
                {
                    music.clip = deadClip;
                    music.Play();
                }
                ghost.DeathAndRespawn();
                score.AddScore(300);
            }
            else
            {
                DeathAndRespawn();
            }
        }
    }

    private void DeathAndRespawn()
    {
        StopAllCoroutines();
        Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        life--;
        for (var i = 0; i < lives.Length; i++)
        {
            lives[i].SetActive(i < life);
        }
        if (life >= 0)
        {
            transform.position = startingPos;
            currentRow = currentCol = 1;
            lastInput = currentInput = InputDirection.None;
            moving = false;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowGameOver());
        }
    }

    private IEnumerator ShowGameOver()
    {
        movingSfx.enabled = false;
        lastInput = currentInput = InputDirection.None;
        moving = false;
        gameover = true;
        gameOver.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScene");
    }

    private IEnumerator PowerPillRoutine()
    {
        music.clip = scareClip;
        music.Play();
        foreach (var ghost in ghosts)
        {
            ghost.Scare();
        }
        var wait = new WaitForSeconds(1);
        powerUpped = true;
        int time = 10;
        ghostTimer.transform.parent.gameObject.SetActive(true);
        while (time > 0)
        {
            ghostTimer.text = time.ToString();
            yield return wait;
            time--;
        }
        ghostTimer.transform.parent.gameObject.SetActive(false);
        powerUpped = false;
        if (music.clip != normalClip)
        {
            music.clip = normalClip;
            music.Play();
        }
    }

}
