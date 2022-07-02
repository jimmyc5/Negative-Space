using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goToObject : MonoBehaviour
{
    public Transform goTo;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        tf.position = goTo.position;
    }

    // Update is called once per frame
    void Update()
    {
        tf.position = goTo.position;
    }
}
