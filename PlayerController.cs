using UnityEngine;

/* Player movement/input control script.  Also holds other properties for
 * the Player. Player has various moves like a jump, dash, and eat carrots.
 */

public class PlayerController : MonoBehaviour
{
    // Adjustable properties
    public float movementSpeed;
    public float dashSpeed;
    public float jumpSpeed;
    public GameObject spawnPoint;

    // Game object components
    [SerializeField] private LayerMask platformMask;
    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider2D;

    // Movement
    private float horizontal;
    private bool isJumping;
    private bool isDashing;
    private float maxDashTimer = .5f;
    private float dashTimer;

    // Goal
    private bool haveCarrot = false;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        SetInitialPosition();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        isJumping = Input.GetButton("Jump");

        if (Input.GetKeyDown(KeyCode.E))
        {
            isDashing = true;
        }

    }

    // Fixedupdate is called every fixed framerate update
    private void FixedUpdate()
    {
        dashTimer -= Time.deltaTime;

      
        if (horizontal != 0)
        {
            HandleRotation();
            MoveHorizontal();
        }

        if (isJumping)
        {
            if (IsGrounded())
            {
                Jump();
            }
        }

        if (isDashing)
        {
            if (dashTimer < 0)
            {
                Dash();
            }
        }

    }

    // Adjust carrot value when Carrot game object is collided with
    public void GrabbedCarrot()
    {
        haveCarrot = true;
    }

    public bool HaveCarrot()
    {
        return haveCarrot;
    }

    // Interaction for Paddle/Wind turbine enemy. When player collides with
    // one of those obstacles, they'll be pushed into a direction based on
    // where the collision happens.
    public void PushBack(float multiplier, string direction)
    {
        switch (direction)
        {
            case "up":
                rigidBody.AddForce(Vector2.up * multiplier);
                break;
            case "down":
                rigidBody.AddForce(Vector2.down * multiplier);
                break;
            case "left":
                rigidBody.AddForce(Vector2.left * multiplier);
                break;
            case "right":
                rigidBody.AddForce(Vector2.right * multiplier);
                break;
        }

    }

    // Check if player is on a game object on platform layer. Prevents
    // spamming jump to propel into the air indefinitely.
    private bool IsGrounded()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, .1f, platformMask);
        return raycast.collider != null;
    }

    // Move left/right - horizontal will be negative when moving left
    private void MoveHorizontal()
    {
        rigidBody.AddForce(Vector2.right * horizontal * movementSpeed, ForceMode2D.Impulse);
    }

    // Jump up
    private void Jump()
    {
        rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    // Dash left/right - horizontal will be negative when moving left
    private void Dash()
    {
        Vector2 target = new Vector2(rigidBody.position.x + dashSpeed * horizontal, rigidBody.position.y);
        rigidBody.MovePosition(target);
        dashTimer = maxDashTimer;
        isDashing = false;
    }

    // Moves player to spawn location that can be positioned anywhere on map
    private void SetInitialPosition()
    {
        transform.position = spawnPoint.transform.position + new Vector3(0f, .8f, 0f);
    }

    // Change player roation based on inputted value
    private void AdjustRotationY(float y)
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.y = y;
        transform.rotation = Quaternion.Euler(rotation);
    }

    // Makes player sprite look left or right based on the direction they're moving
    private void HandleRotation()
    {
        if (horizontal > 0)
        {
            AdjustRotationY(0);
        }
        else if (horizontal < 0)
        {
            AdjustRotationY(180);
        }
    }

}
