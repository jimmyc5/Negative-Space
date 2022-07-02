using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    public playerGraphics graphics;
    public Rigidbody2D rb;
    private GameManager gm;

    public float sidewaysForce = 50;

    public float jumpForce = 15;

    public float maxSpeed = 10;

    public BoxCollider2D box;

    public float fastfall = 10;

    public float gravity = 80;
    public float jumpGravity = 40;

    public int GroundLayer = (1 << 12);
    public int BoxLayer = (1 << 15);
    //private int NinjaLayer = (1 << 15);


    private float distToGround;
    private float distToSide;
    public float HorizontalDrag;

    public ParticleSystem ps;

    private bool isJumping;
    private bool dPress;
    private bool aPress;
    private bool wPress;
    private bool sPress = false;
    private bool spacePress;

    private bool leftPress;
    private bool rightPress;
    private bool downPress;
    private bool upPress;

    private double proposedForce;

    private bool isTransitioning = false;   //used to keep track of whether it's transitioning between modes

    private bool restarting = false;
    public int restartFrames = 20;

    public int transitionFrames = 5;
    private int transitionFramesLeft;

    private float transitionDirection = 0.0f;   //the angle that a transition will happen at


    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        distToGround = box.bounds.extents.y;
        distToSide = box.bounds.extents.x;
        transitionFramesLeft = transitionFrames;
    }

    public bool isGrounded()   //function to determine if the player is standing on ground (used for jumping)
    {
        return isGroundedAngled();

        //the rest of this is the old code- checks directly below the player always)
        /*
        int finalLayer = GroundLayer;
        if (gm.groundIsWhite)
        {
            return Physics2D.BoxCast(transform.position, new Vector2(distToSide, 0.06f), 0f, -Vector2.up, distToGround + 0.06f, finalLayer);
        }
        else
        {
            return Physics2D.BoxCast(transform.position, new Vector2(distToSide, 0.06f), 0f, Vector2.up, distToGround + 0.06f, finalLayer);
        }
        */
        //return Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.06f, GroundLayer) || Physics2D.Raycast(transform.position, new Vector2(0.08f, -0.99f), distToGround + 0.06f, GroundLayer) || Physics2D.Raycast(transform.position, new Vector2(-0.08f, -0.99f), distToGround + 0.06f, GroundLayer);
    }

    public bool isGroundedAngled() // function to check if there's ground in the direction that the transition between black/white will happen
    {
        int finalLayer = GroundLayer;

        float dir = getDirectionOfTransition();
        Vector2 unitVectorOfTransition = new Vector2(Mathf.Cos(dir * Mathf.PI / 180f), Mathf.Sin(dir * Mathf.PI / 180f));
        Vector3 ThreeDUnitVector = new Vector3(unitVectorOfTransition.x, unitVectorOfTransition.y, 0);
        float magnitudeToGround = distToGround + 0.05f;

        return Physics2D.BoxCast(transform.position, new Vector2(distToSide, 0.06f), dir, unitVectorOfTransition, distToGround + 0.06f, finalLayer);
    }

    Rigidbody2D findRbOfBoxBelow()
    {
        int finalLayer = BoxLayer;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(distToSide, 0.06f), 0f, -Vector2.up, distToGround + 0.06f, finalLayer);
        if (!hit|| hit.collider == null)
        {
            return null;
        }
        else if(hit.collider.gameObject.GetComponent<Rigidbody2D>() == null)
        {
            return null;
        }
        return hit.collider.gameObject.GetComponent<Rigidbody2D>();
        
        //return Physics2D.Raycast(transform.position, -Vector2.up, distToGround + 0.06f, GroundLayer) || Physics2D.Raycast(transform.position, new Vector2(0.08f, -0.99f), distToGround + 0.06f, GroundLayer) || Physics2D.Raycast(transform.position, new Vector2(-0.08f, -0.99f), distToGround + 0.06f, GroundLayer);
    }


    bool isSquished()
    {
        //return false;
        return Physics2D.Raycast(transform.position, -Vector2.up, distToGround / 2, GroundLayer) && Physics2D.Raycast(transform.position, Vector2.up, distToGround / 2, GroundLayer) || Physics2D.Raycast(transform.position, -Vector2.right, distToGround / 2, GroundLayer) && Physics2D.Raycast(transform.position, Vector2.left, distToGround / 2, GroundLayer);
    }

    // Update is called once per frame
    void Update()
    {
        // Update Inputs
        //WASD
        if (Input.GetKeyDown(KeyCode.D))
        {
            dPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            dPress = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            aPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            aPress = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            wPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            wPress = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            sPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            sPress = false;
        }
        //space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePress = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            spacePress = false;
        }
        //arrow keys
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightPress = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftPress = false;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            downPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            downPress = false;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upPress = true;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            upPress = false;
        }
    }
    private bool jumpButtonPressed()
    {
        return spacePress; //((gm.groundIsWhite && wPress) || (!gm.groundIsWhite && sPress) || spacePress);
    }
    private bool transitionButtonPressed()
    {
        return ((gm.groundIsWhite && sPress) || (!gm.groundIsWhite && wPress) || (gm.groundIsWhite && downPress) || (!gm.groundIsWhite && upPress));
    }
    
    private float getDirectionOfTransition()    // function to figure out what direction (angle) the transition between black/white should be in.
    {
        // figure out what direction to cast in- which face of the square is most downwards?
        // process: simulate casting a unit vector from the center of the box to each face. Whichever vector has the smallest (most negative) y component is the side that is furthest downwards. Try to swap on that side.
        float[] possibleDirections = { transform.eulerAngles.z, (transform.eulerAngles.z + 90f) % 360f, (transform.eulerAngles.z + 180f) % 360f, (transform.eulerAngles.z + 270f) % 360f };
        if (gm.groundIsWhite)
        {
            float minDir = transform.eulerAngles.z;
            float minHeight = Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f);
            foreach (float dir in possibleDirections)
            {
                if (Mathf.Sin(dir * Mathf.PI / 180f) < minHeight)
                {
                    minHeight = Mathf.Sin(dir * Mathf.PI / 180f);
                    minDir = dir;
                }
            }
            return minDir;
        }
        else
        {
            //similar logic if the square is black, but we want the maximum y value.

            float maxDir = transform.eulerAngles.z;
            float maxHeight = Mathf.Sin(transform.eulerAngles.z * Mathf.PI / 180f);
            foreach (float dir in possibleDirections)
            {
                if (Mathf.Sin(dir * Mathf.PI / 180f) > maxHeight)
                {
                    maxHeight = Mathf.Sin(dir * Mathf.PI / 180f);
                    maxDir = dir;
                }
            }
            return maxDir;
        }
        

    }

    private bool canTransition()    //function to determine whether you can switch between black/white modes
    {
        if (!isGroundedAngled())  //if there isn't ground in the transition direction, you can't transition
        {
            return false;
        }

        int finalLayer = GroundLayer;

        float dir = getDirectionOfTransition();
        transitionDirection = dir;

        Vector2 unitVectorOfTransition = new Vector2(Mathf.Cos(dir* Mathf.PI / 180f), Mathf.Sin(dir * Mathf.PI / 180f));
        Vector3 ThreeDUnitVector = new Vector3(unitVectorOfTransition.x, unitVectorOfTransition.y, 0);
        float magnitudeToGround = distToGround + 0.05f;

        return !Physics2D.BoxCast(transform.position + (magnitudeToGround * ThreeDUnitVector), new Vector2(2f * distToSide - 0.05f, 0.01f), dir-90f, unitVectorOfTransition, distToGround * 2f - 0.01f, finalLayer);

        //Legacy code for just switching up/down on flat surfaces- can revert everything after setting GroundLayer to this if need be
        /*
        if (gm.groundIsWhite)
        {
            return !Physics2D.BoxCast(transform.position + new Vector3(0, -distToGround - 0.05f, 0), new Vector2(2f*distToSide - 0.05f, 0.01f), 0f, -Vector2.up, distToGround * 2f -0.01f, finalLayer);
        }
        else
        {
            return !Physics2D.BoxCast(transform.position + new Vector3(0, distToGround + 0.05f, 0), new Vector2(2f*distToSide - 0.05f, 0.01f), 0f, Vector2.up, distToGround * 2f -0.01f, finalLayer);
        }
        */
    }
    void FixedUpdate()
    {
        if (!restarting)
        {
            if (!isTransitioning)
            {
                if (transitionButtonPressed() && canTransition())
                {
                    isTransitioning = true;
                    graphics.startJumpAnimation();
                    graphics.startTransition(transitionFrames);
                    if (gm.groundIsWhite)
                    {
                        graphics.transform.rotation = Quaternion.Euler(0, 0, (transitionDirection + 90f));
                        FindObjectOfType<AudioManager>().SetTheme(false);
                    }
                    else
                    {
                        graphics.transform.rotation = Quaternion.Euler(0, 0, (transitionDirection - 90f));
                        FindObjectOfType<AudioManager>().SetTheme(true);
                    }
                    gm.switchMode(transitionFrames);
                    rb.isKinematic = true;
                    rb.velocity = new Vector2(0, 0);
                    rb.angularVelocity = 0;
                    //transform.rotation = Quaternion.identity;
                    FindObjectOfType<AudioManager>().Play("Swap");
                    if (wPress)
                    {
                        wPress = false;
                    }
                    if (sPress)
                    {
                        sPress = false;
                    }
                    if (spacePress)
                    {
                        spacePress = false;
                    }
                }
                else
                {
                    bool moveNegX = aPress || leftPress;
                    bool movePosX = dPress || rightPress;
                    if (gm.groundIsWhite)
                    {
                        gravity = Mathf.Abs(gravity);
                        jumpGravity = Mathf.Abs(jumpGravity);
                        jumpForce = Mathf.Abs(jumpForce);
                    }
                    else
                    {
                        gravity = -1f * Mathf.Abs(gravity);
                        jumpGravity = -1f * Mathf.Abs(jumpGravity);
                        jumpForce = -1f * Mathf.Abs(jumpForce);
                    }
                    if (movePosX)
                    {
                        proposedForce = sidewaysForce;
                        if (rb.velocity.x < maxSpeed)
                        {
                            if (rb.velocity.x + proposedForce * Time.deltaTime > maxSpeed)
                            {
                                proposedForce = maxSpeed - rb.velocity.x;
                            }
                            if (proposedForce > 0)
                            {
                                rb.AddForce(new Vector2((float)proposedForce, 0));
                            }
                        }
                    }

                    if (moveNegX)
                    {
                        proposedForce = -sidewaysForce;
                        if (rb.velocity.x > -maxSpeed)
                        {
                            if (rb.velocity.x + proposedForce * Time.deltaTime < -maxSpeed)
                            {
                                proposedForce = -maxSpeed - rb.velocity.x;
                            }
                            if (proposedForce < 0)
                            {
                                rb.AddForce(new Vector2((float)proposedForce, 0));
                            }
                        }
                    }

                    if (rb.velocity.y < 0)
                    {
                        isJumping = false;
                    }

                    Rigidbody2D potentialBox = findRbOfBoxBelow();
                    if (jumpButtonPressed() && isGrounded())
                    {
                        FindObjectOfType<AudioManager>().Play("Jump");
                        float rotation = this.transform.rotation.eulerAngles.z;
                        // reset gaphics rotation if it's been changed from transitioning before
                        graphics.transform.localRotation = Quaternion.identity;
                        //rotate 90 degrees if necessary to make sure the jump animation looks right (only reason this doesn't mess up physics is bc the player hitbox is a square)
                        if ((45f < rotation && rotation < 135f) || (225f < rotation && rotation < 315f))
                        {
                            this.transform.Rotate(new Vector3(0f, 0f, 90f));
                        }
                        graphics.startJumpAnimation();
                        rb.AddForce(new Vector2(0, -rb.velocity.y + jumpForce), ForceMode2D.Impulse);
                        isJumping = true;
                    }
                    if (jumpButtonPressed() && (potentialBox != null))
                    {
                        potentialBox.AddForce(new Vector2(0, 10f * (rb.velocity.y - jumpForce)), ForceMode2D.Impulse);
                    }

                    if (jumpButtonPressed() && isJumping)
                    {
                        rb.AddForce(new Vector2(0, -jumpGravity));
                    }
                    else
                    {
                        isJumping = false;
                        rb.AddForce(new Vector2(0, -gravity));
                    }

                    if (sPress)
                    {
                        rb.AddForce(new Vector2(0, -fastfall));
                    }
                    rb.AddForce(new Vector2((HorizontalDrag - 1) * rb.velocity.x / Time.fixedDeltaTime, 0));

                    if (isSquished() && !restarting)
                    {
                        Die();
                    }
                }
            }
            else
            {
                //do the following if the player is transitioning between black/white modes

                if(transitionFramesLeft > 0)
                {
                    float dir = transitionDirection;    // this should have been updated in canTransition() just recently- last direction that we've confirmed is safe to transition into
                    Vector3 unitVectorOfTransition = new Vector3(Mathf.Cos(dir * Mathf.PI / 180f), Mathf.Sin(dir * Mathf.PI / 180f), 0);    // get the unit vector in that direction
                    this.transform.position += unitVectorOfTransition * ((2 * distToGround + 0.01f) / transitionFrames);                    // move in that direction
                    transitionFramesLeft--;

                    //Legacy code for just transitioning up/down- can revert to this if needed
                    /*
                    if (gm.groundIsWhite)
                    {
                        this.transform.position += new Vector3(0, (2 * box.bounds.extents.y + 0.01f) / transitionFrames, 0);
                    }
                    else
                    {
                        this.transform.position -= new Vector3(0, (2 * box.bounds.extents.y + 0.01f) / transitionFrames, 0);
                    }
                    transitionFramesLeft--;
                    */
                }
                else
                {
                    isTransitioning = false;
                    rb.isKinematic = false;
                    transitionFramesLeft = transitionFrames;
                }
                
            }
            {

            }
        }

        if (restarting)
        {
            restartFrames--;
        }
        if (restartFrames < 1)
        {
            FindObjectOfType<GameManager>().Restart();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "killbox")
        {
            Die();
        }
        if(collision.gameObject.tag == "forceTransition")
        {
            isTransitioning = true;
            graphics.startTransition(1);
            gm.switchMode(1);
            rb.isKinematic = true;
            rb.velocity = new Vector2(0, 0);
            //transform.rotation = Quaternion.identity;
        }
        if (collision.gameObject.tag == "forceTransitionToBlack" && gm.groundIsWhite)
        {
            isTransitioning = true;
            graphics.startTransition(1);
            gm.switchMode(1);
            rb.isKinematic = true;
            rb.velocity = new Vector2(0, 0);
            //transform.rotation = Quaternion.identity;
        }
    }

    private void Die()
    {
        restarting = true;
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        graphics.explode();
        gm.Restart();
        rb.isKinematic = true;
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col != null && col.relativeVelocity.magnitude > 7f)
        {
            FindObjectOfType<AudioManager>().Play("Thud");
        }
    }
    

}
