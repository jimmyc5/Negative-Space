using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerGraphics : MonoBehaviour
{
    private Transform tf;
    private float xScale;
    private float yScale;
    private float zScale;

    private GameManager gm;
    private Animator anim;

    public Transform whiteSquare;
    public Transform blackSquare;

    public int jumpVertFrames = 6;
    public float stretchPerJumpFrame = 0.05f;


    private bool transitioning = false;     //transitioning between black and white
    private bool whiteToBlack = false;
    private int transFrames = 0;
    private int transFramesDone = 0;


    public float rotationSpeed = 0.5f;
    public float maxRotation = 2f;
    private ParticleSystem ps;

    private ParticleSystem.MainModule ma;

    public GameObject PlayerEyes1;
    public GameObject PlayerEyes2;

    void Start()
    {
        tf = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        ps = GetComponent<ParticleSystem>();
        ma = ps.main;
        gm = FindObjectOfType<GameManager>();
        xScale = tf.localScale.x;
        yScale = tf.localScale.y;
        zScale = tf.localScale.z;
    }
    void Update()
    {
 
    }
    public void startTransition(int transitionFrames)
    {
        transFrames = transitionFrames;
        transFramesDone = 0;
        transitioning = true;
        if (gm.groundIsWhite)
        {
            whiteToBlack = true;
        }
        else
        {
            whiteToBlack = false;
        }
        tf.localScale = new Vector3(xScale, yScale, zScale);
    }
    void FixedUpdate()
    {
        if (transitioning)
        {
            if (transFramesDone < transFrames)
            {
                int thisFrame;
                if (whiteToBlack)
                {
                    thisFrame = transFramesDone + 1;
                }
                else
                {
                    thisFrame = transFrames - transFramesDone;
                }
                if (true)
                {
                    blackSquare.localScale = new Vector3(1f, (float)thisFrame / (float)transFrames, 1f);
                    blackSquare.localPosition = new Vector3(0f, -0.5f + (float)thisFrame / (float)transFrames / 2f, 0f);

                    whiteSquare.localScale = new Vector3(1f, 1f - (float)thisFrame / (float)transFrames, 1f);
                    whiteSquare.localPosition = new Vector3(0f, 0.5f - (1f - (float)thisFrame / (float)transFrames) / 2f, 0f);
                }
                else
                {
                    //this.transform.position -= new Vector3(0, (2 * box.bounds.extents.y + 0.01f) / transFrames, 0);
                }
                transFramesDone++;
            }
            else
            {
                if (whiteToBlack)
                {
                    blackSquare.localScale = new Vector3(1f, 1f, 1f);
                    blackSquare.localPosition = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    whiteSquare.localScale = new Vector3(1f, 1f, 1f);
                    whiteSquare.localPosition = new Vector3(0f, 0f, 0f);
                }
                transitioning = false;
                transFrames = 0;
                transFramesDone = 0;
            }
        }else if (!gm.groundIsWhite)
        {
            blackSquare.localScale = new Vector3(1f, 1f, 1f);
            whiteSquare.localScale = new Vector3(0f, 0f, 0f);
        }
        else
        {
            whiteSquare.localScale = new Vector3(1f, 1f, 1f);
            blackSquare.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    public void startJumpAnimation()
    {
        anim.SetTrigger("jump");
    }

    public void explode()
    {
        if (gm.groundIsWhite)
        {
            ma.startColor = new Color(255f,255f,255f,255f);
        }
        else
        {
            //Debug.Log("h");
            //ma.startColor = new Color(30f,30f,30f,255f);
        }
        ps.Play();
        blackSquare.gameObject.SetActive(false);
        whiteSquare.gameObject.SetActive(false);
        PlayerEyes1.SetActive(false);
        PlayerEyes2.SetActive(false);
    }
}
