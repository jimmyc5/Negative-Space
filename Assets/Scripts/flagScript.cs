using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagScript : MonoBehaviour
{
    private GameManager gm;
    private ParticleSystem part;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        part = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            part.Play();
            gm.winLevel();
        }
    }
}
