using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	private Rigidbody2D rb2d;
	private Transform trans;
	private Animator anim;
	private bool isGrounded = true;
    private bool isJumpAssisting = false;
    private float jumpAssistStart;
    private float initialGravityScale;

	public Transform groundCheckTopLeft;
	public Transform groundCheckBotRight;

	public float walkAccel = 80f;
	public float walkDeccel = 60f;
	public float jumpForce;
	public float normalTopWalkSpeed = 10f;
    public float normalTopFallSpeed;
    public float jumpAssistMax; // The max time a jump can be assisted by holding the jump button
    public float jumpAssistUpGravMod;
    public float jumpUpGravMod;

    // Use this for initialization
    void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		trans = GetComponent<Transform> ();
		anim = GetComponent<Animator> ();
        initialGravityScale = rb2d.gravityScale;
	}
	
	// Update is called once per frame
	void Update () {
		setIsGrounded ();

        // The start of a jump all jumps start with an assist period
		if (Input.GetButtonDown ("Jump") && isGrounded) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            rb2d.gravityScale = initialGravityScale * jumpAssistUpGravMod;
            isJumpAssisting = true;
            jumpAssistStart = Time.time;
		}
        // In an assited jump but button was let up or assist time is up
        if (!isGrounded && isJumpAssisting
            && (Input.GetButtonUp("Jump") 
                || (Time.time - jumpAssistStart >= jumpAssistMax))) {
            isJumpAssisting = false;
            rb2d.gravityScale = initialGravityScale * jumpUpGravMod;
        }
        // If we've stopped jumping up remove gravity mods
        if (rb2d.velocity.y <= 0) {
            rb2d.gravityScale = initialGravityScale;
            // ensure we're not falling faster than max
            if (rb2d.velocity.y < normalTopFallSpeed) {
                Vector3 vel = rb2d.velocity;
                vel.y = normalTopFallSpeed;
                rb2d.velocity = vel;
            }
        }
        float moveH = Input.GetAxis ("Horizontal");

		anim.SetBool ("isRunning", moveH != 0);

		if (moveH != 0) {
			// stop the player if input is different to movement
			if (Mathf.Sign (moveH) != Mathf.Sign (rb2d.velocity.x)) {
				rb2d.velocity = new Vector2 (0, rb2d.velocity.y);
			}
			// accelerate
			if (moveH * rb2d.velocity.x < normalTopWalkSpeed) {
				rb2d.AddForce (new Vector2 (moveH * walkAccel, 0));
			}
			// if necessary bring it back to max walk speed
			if (Mathf.Abs (rb2d.velocity.x) > normalTopWalkSpeed) {
				rb2d.velocity = new Vector2 (Mathf.Sign (moveH) * normalTopWalkSpeed, rb2d.velocity.y);
			}
		// stop if were on the ground with no input
		} else if (rb2d.velocity.x != 0 && isGrounded) {
			// we still have a ways to slow down so try to do it smoothly
			if (Mathf.Abs (rb2d.velocity.x) > (normalTopWalkSpeed * 0.2)) {
				// notice that we slow down at 0.5 the acceleration of speeding up.
				rb2d.AddForce (new Vector2 (Mathf.Sign (rb2d.velocity.x) * walkDeccel * -1f, 0f));
			// we're close to stopped so force stop instead of risking overshooting.
			} else {
				rb2d.velocity = new Vector2 (0, rb2d.velocity.y);
			}
		}

		flipIfNeeded ();
	}

	void setIsGrounded() {
		isGrounded = Physics2D.OverlapArea(groundCheckTopLeft.position, groundCheckBotRight.position,
								1 << LayerMask.NameToLayer("Background"))
							!= null;
	}

	void flipIfNeeded() {
		// if the vel and scale aren't both positive or negative flip the scale.
		if (rb2d.velocity.x * trans.localScale.x < 0) {
			Vector3 temp = trans.localScale;
			temp.x *= -1;
			trans.localScale = temp;
		}
	}
}
