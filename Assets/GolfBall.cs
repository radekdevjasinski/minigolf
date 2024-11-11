using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public static GolfBall Instance;
    public Vector2 velocity; 
    public float friction = 0.99f;
    private CircleCollider2D ballCollider;
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
        ballCollider = GetComponent<CircleCollider2D>();
        ballRadius = ballCollider.radius * transform.localScale.x;
    }
    void FixedUpdate()
    {
        Vector2 movement = velocity * Time.fixedDeltaTime;

        // Okreœlenie liczby kroków na podstawie prêdkoœci pi³ki
        int steps = Mathf.Max(1, (int)(velocity.magnitude)); 
        float stepSize = movement.magnitude / steps; 
        for (int i = 0; i < steps; i++)
        {
            RaycastHit2D hitX = Physics2D.Raycast(transform.position, new Vector2(velocity.x, 0), ballRadius, collisionLayer);
            if (hitX.collider != null)
            {
                velocity = new Vector2(-velocity.x, velocity.y); 
            }

            RaycastHit2D hitY = Physics2D.Raycast(transform.position, new Vector2(0, velocity.y), ballRadius, collisionLayer);
            if (hitY.collider != null)
            {
                velocity = new Vector2(velocity.x, -velocity.y); 
            }

       
            transform.position += (Vector3)(velocity.normalized * stepSize);
        }

        velocity *= friction;

        if (velocity.magnitude < 0.05f)
        {
            velocity = Vector2.zero;
        }
    }
    public void SetVelocity(Vector2 stroke)
    {
        float angle = stroke.x;
        float force = stroke.y;

        float angleInRadians = angle * Mathf.Deg2Rad;

        velocity = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * force;
    }
}
