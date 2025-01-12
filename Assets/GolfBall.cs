using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public static GolfBall Instance;
    public Vector2 velocity; 
    public float friction = 0.95f;


    public CircleCollider2D ballCollider;
    public int stepsPerFrame = 10;
    public LayerMask collisionLayer; 

    private float ballRadius;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        ballRadius = ballCollider.radius * transform.localScale.x;
    }
    void FixedUpdate()
    {
        if(ballCollider.enabled == false) { return; }

        Vector2 movement = velocity * Time.fixedDeltaTime;

        int steps = Mathf.Max(1, (int)velocity.magnitude); 
        float stepSize = movement.magnitude / steps; 
        for (int i = 0; i < steps; i++)
        {
            RaycastHit2D hitX = Physics2D.Raycast(transform.position, new Vector2(velocity.x, 0), ballRadius, collisionLayer);
            if (hitX.collider != null)
            {
                if (hitX.collider.isTrigger) { Goal(hitX.collider); }
                else { velocity = new Vector2(-velocity.x, velocity.y); }
            }

            RaycastHit2D hitY = Physics2D.Raycast(transform.position, new Vector2(0, velocity.y), ballRadius, collisionLayer);
            if (hitY.collider != null)
            {
                if (hitY.collider.isTrigger) { Goal(hitY.collider); }
                else { velocity = new Vector2(velocity.x, -velocity.y); }
            }
            transform.position += (Vector3)(velocity.normalized * stepSize);
        }

        if(velocity.magnitude >= Game.Instance.minWindVel)
        {
            velocity += Game.Instance.windForce;
        }
        velocity += ElevationForce();
        velocity *= friction;

        if (velocity.magnitude < 0.05f)
        {
            velocity = Vector2.zero;
        }
    }
    private Vector2 ElevationForce()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ballRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Elevation"))
            {
                Elevation elevation = collider.gameObject.GetComponent<Elevation>();
                if(elevation != null)
                {
                    return elevation.forceDir * elevation.elevationForce;
                }
            }
        }
        return Vector2.zero;
    }
    public void SetVelocity(Vector2 stroke)
    {
        float angle = stroke.x;
        float force = stroke.y;

        float angleInRadians = angle * Mathf.Deg2Rad;

        velocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * force;
    }
    void Goal(Collider2D collider)
    {
        if(collider.CompareTag("Goal"))
        {
            velocity = Vector2.zero;
            transform.position = collider.transform.position;
            //Game.Instance.MoveToNextLevel(true);
            ballCollider.enabled = false;
            UIAnimator.Instance.PlayLevelChange();
        }    
    }

}
