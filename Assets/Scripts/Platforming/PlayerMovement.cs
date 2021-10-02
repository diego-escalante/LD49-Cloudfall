using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CollisionController), typeof(TimeShifter))]
public class PlayerMovement : MonoBehaviour {
    
    [Header("Basic Jumping")]
    [SerializeField]
    private float jumpHeight = 3.25f;
    [SerializeField]
    private float timeToJumpApex = 0.33f;
    private float gravity, maxJumpVelocity;

    [Header("Basic Running")]
    // How fast to move in the horizontal direction.
    [SerializeField]
    private float runSpeed = 10f;
    

    [Header("Terminal Velocity")]
    // terminalVelocityFactor defines terminalVelocity as a multiplier of jumpVelocity.
    [SerializeField]
    private float terminalVelocityFactor = 3f;
    private float terminalVelocity;

    [Header("Multijumping")]
    // Jumps describes the number of jumps the player can perform before having to be grounded again.
    // The first "main" jump is can only be done when grounded (or within coyote time.)
    // MultiJumpFactor describes how strong subsequent jumps are relative to the first jump.
    [SerializeField]
    private int jumps = 1;
    [SerializeField]
    private float multijumpHeight = 2f;
    private float maxMultiJumpVelocity;
    private int jumpsLeft;

    [Header("Variable Jumping")]
    // Ability to let the player modify jump height. minJumpHeight defines minJumpVelocity.
    [SerializeField]
    private bool VariableJumpEnabled = true;
    [SerializeField]
    private float minJumpHeight = 1.25f;
    [SerializeField]
    private float minMultiJumpHeight = 1f;
    private float minJumpVelocity;
    private float minMultiJumpVelocity;

    [Header("Coyote Time")]
    // Coyote Time describes the amount of time AFTER not being grounded that the player can still jump.
    [SerializeField]
    private bool coyoteTimeEnabled = true;
    [SerializeField]
    private float coyoteTime = 0.1f;
    private float coyoteTimeLeft;

    [Header("Jump Buffering")]
    // The Jump Buffer adds a small time buffer for the jump input, so if you hit it a little too early before
    // touching the ground, the character will still automatically jump on the frame it is grounded.
    [SerializeField]
    private float jumpBufferTime = 0.09f;
    private float jumpBufferTimeLeft;

    // The internal velocity of the object.
    private Vector2 velocity = Vector2.zero;
    
    // Reference to CollisionController and its CollisionInfo struct for collision checking.
    CollisionController collisionController;
    CollisionController.CollisionInfo collisionInfo;

    private TimeShifter _timeShifter;
    
    public void OnValidate() {
        jumpHeight = Mathf.Max(0, jumpHeight);
        minJumpHeight = Mathf.Clamp(minJumpHeight, 0, jumpHeight);
        timeToJumpApex = Mathf.Max(0.01f, timeToJumpApex);
        terminalVelocityFactor = Mathf.Max(1, terminalVelocityFactor);
        runSpeed = Mathf.Max(0, runSpeed);
        coyoteTime = Mathf.Max(0, coyoteTime);
        jumps = Mathf.Max(0, jumps);
        multijumpHeight = Mathf.Max(0, multijumpHeight);
        minMultiJumpHeight = Mathf.Clamp(minMultiJumpHeight, 0, multijumpHeight);
        jumpBufferTime = Mathf.Max(0, jumpBufferTime);
        UpdateKinematics();
    }
    
    public void Awake() {
        _timeShifter = GetComponent<TimeShifter>();
        collisionController = GetComponent<CollisionController>();
        UpdateKinematics();
    }
    
    public void Update() {
        bool isDropping = false;

        if (coyoteTimeEnabled) {
            coyoteTimeLeft -= Time.deltaTime * _timeShifter.GetFactor();
            // Get rid of primary jump if no coyote time is left.
            if (jumpsLeft == jumps && coyoteTimeLeft <= 0) {
                jumpsLeft--;
            }
        } else {
            // Get rid of primary jump if not on the ground.
            if (jumpsLeft == jumps && !collisionInfo.collisionBelow) {
                jumpsLeft--;
            }
        }

        // Register jump input ahead of time for buffer and use it if applicable, keep track of time since jump input.
        if (jumpBufferTimeLeft >= 0) {
            jumpBufferTimeLeft -= Time.deltaTime /** _timeShifter.GetFactor()*/; //Don't timeshift jumpbuffer.
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Special case: If holding the down arrow while jumping and while grounded, drop instead of jump.
            if (Input.GetKey(KeyCode.DownArrow) && collisionInfo.collisionBelow) {
                isDropping = true;
            } else {
                jumpBufferTimeLeft = jumpBufferTime;
            }
        }

        // Calculate horizontal movement.
        velocity.x = Input.GetAxisRaw("Horizontal") * runSpeed;

        // Calculate vertical movement. (By gravity or by jumping.)
        velocity.y = Mathf.Clamp(velocity.y + gravity * Time.deltaTime * _timeShifter.GetFactor(), -terminalVelocity, terminalVelocity);
        if (jumpBufferTimeLeft >= 0 && jumpsLeft > 0) {
            // Regular jumps
            velocity.y = (jumps == jumpsLeft ? maxJumpVelocity : maxMultiJumpVelocity);
            jumpsLeft--;
            // Just set the jump timer to negative to "consume" input.
            jumpBufferTimeLeft = -1;
        }
        
        // Shortening jumps by releasing space.
        if (VariableJumpEnabled && Input.GetKeyUp(KeyCode.Space)) {
            if (jumps == jumpsLeft+1) {
                if (velocity.y > minJumpVelocity) {
                    velocity.y = minJumpVelocity;
                }
            } else {
                if (velocity.y > minMultiJumpVelocity) {
                    velocity.y = minMultiJumpVelocity;
                }
            }
        }

        // Calculate change in position based on collisions.
        collisionInfo = collisionController.Check(velocity * Time.deltaTime * _timeShifter.GetFactor(), isDropping);

        // React to vertical collisions.
        if (collisionInfo.colliderVertical != null) {
            velocity.y = 0;
            // If collision is below, reset jumping and coyote time (if enabled).
            if (collisionInfo.collisionBelow) {
                if (coyoteTimeEnabled) {
                    coyoteTimeLeft = coyoteTime;
                }
                jumpsLeft = jumps;
            }
        }

        // React to horizontal collisions. 
        if (collisionInfo.colliderHorizontal != null) {
            velocity.x = 0;
        }

        // Move the object.
        transform.Translate(collisionInfo.moveVector);
	}

    public bool IsGrounded() {
        return collisionInfo.collisionBelow;
    }

    public float GetJumpHeight() {
        return jumpHeight;
    }

    public float GetJumpsLeft() {
        return jumpsLeft;
    }
    
    private void UpdateKinematics(){
        gravity = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        maxMultiJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * multijumpHeight);
        minMultiJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minMultiJumpHeight);
        terminalVelocity = maxJumpVelocity * terminalVelocityFactor;
    }

}
