using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingObject : MonoBehaviour
{
    private GameManager gm;
    public GameObject staticVersion;
    public GameObject nonStaticVersion;
    private bool isStatic = false;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gm.groundIsWhite && isStatic && gm.transitionFramesLeft <= 0)
        {
            nonStaticVersion.SetActive(true);
            isStatic = false;
            staticVersion.SetActive(false);
        }
        if(!gm.groundIsWhite && !isStatic)
        {
            nonStaticVersion.SetActive(false);
            isStatic = true;
            staticVersion.transform.localScale = nonStaticVersion.transform.localScale;
            staticVersion.transform.localPosition = nonStaticVersion.transform.localPosition;
            staticVersion.transform.localRotation = nonStaticVersion.transform.localRotation;
            staticVersion.SetActive(true);
        }
    }
}
