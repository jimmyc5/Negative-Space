using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingGround : MonoBehaviour
{
    private GameManager gm;

    public float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gm.transitionFramesLeft == 0)
        {
            transform.Rotate(0, 0, speed);
        }
    }
}
