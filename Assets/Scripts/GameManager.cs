using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;
    private bool RPressed = false;

    public bool groundIsWhite = true;
    public int transitionFramesLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        RPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !RPressed)
        {
            Restart();
            RPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if(transitionFramesLeft > 0)
        {
            transitionFramesLeft--;
        }
    }

    public void winLevel()
    {
        FindObjectOfType<AudioManager>().Play("WinLevel");
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void switchMode(int frames)
    {
        groundIsWhite = !groundIsWhite;
        transitionFramesLeft = frames;
    }

    public void Restart()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("WipeOut");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
