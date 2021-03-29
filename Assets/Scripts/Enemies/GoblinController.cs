using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : MonoBehaviour
{
    private enum State
    {
        Running,
        Knockback,
        Death
    }

    [SerializeField] private Transform groundCheck, wallCheck;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float groundCheckDistance, wallCheckDistance, movementSpeed, maxHealth, knockbackDuration;

    [SerializeField] private Vector2 knockbackSpeed;

    private float currentHealth;
    private float knockbackStartTime;

    private bool groundDetected;
    private bool wallDetected;

    private int facingDirection;
    private int damageDirection;

    private State currentState;

    private Vector2 movement;

    private GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;

    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update()
    {
        switch(currentState)
        {
            case State.Running:
                UpdateRunningState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Death:
                UpdateDeathState();
                break;
        }
    }

    // RUNNING STATE
    private void EnterRunningState()
    {

    }

    private void UpdateRunningState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if(!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }
    
    private void ExitRunningState()
    {

    }

    // KNOCKBACK STATE
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Running);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("knockback", false);
    }

    // DEATH STATE
    private void EnterDeathState()
    {
        Destroy(gameObject);
    }

    private void UpdateDeathState()
    {

    }

    private void ExitDeathState()
    {

    }

    // OTHER FUNCTIONS
    
    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        if(attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if(currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        }
        else if(currentHealth <= 0.0f)
        {
            SwitchState(State.Death);
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    private void SwitchState(State state)
    {
        switch(currentState)
        {
            case State.Running:
                ExitRunningState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Death:
                ExitDeathState();
                break;
        }

        switch (state)
        {
            case State.Running:
                EnterRunningState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Death:
                EnterDeathState();
                break;
        }

        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
