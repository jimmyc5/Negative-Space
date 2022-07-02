using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatorm : MonoBehaviour
{
    public float speed = 1.0f;
    public List<Vector2> pointsOnPath;

    private GameManager gm;
    private int currentPointInList = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (pointsOnPath.Count == 0)
        {
            pointsOnPath.Add(new Vector2(transform.position.x, transform.position.y));
        }
        currentPointInList = 0;
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(gm.transitionFramesLeft==0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(pointsOnPath[currentPointInList].x, pointsOnPath[currentPointInList].y, transform.position.z), speed);
        }
        //If at the point, start moving towards the next point
        if (Vector3.Distance(transform.position, new Vector3(pointsOnPath[currentPointInList].x, pointsOnPath[currentPointInList].y, transform.position.z)) == 0f)
        {
            currentPointInList++;
            currentPointInList = currentPointInList % pointsOnPath.Count;
        }
    }
    // If you enable the trigger collider for the moving platform in the inspector, then this code will ensure that players move with the platform when they are standing on it. 
    // The problem is that it's a bit bugged: when the platform is merged with other geometry, the trigger stays, and can move players with the platform unexpectedly. 
    // So I'm leaving it disabled for now.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<playermovement>().isGrounded())
            {
                collision.gameObject.transform.parent.parent = transform;
            }
            else
            {
                collision.gameObject.transform.parent.parent = null;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.parent.parent = null;
        }
    }

}
