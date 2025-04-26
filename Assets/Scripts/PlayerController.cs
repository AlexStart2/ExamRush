using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Miscare")]
    public float maxSpeed = 10f;
    public float acceleration = 20f;
    public float deceleration = 30f;

    private float currentSpeed = 0f;
    private float inputX = 0f;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // citeste inputul
        inputX = Input.GetAxisRaw("Horizontal");

        // flip la directie
        if (inputX > 0) sr.flipX = false;
        else if (inputX < 0) sr.flipX = true;
    }

    void FixedUpdate()
    {
        // calculeaza viteza tinta
        float targetSpeed = inputX * maxSpeed;
        // alege rata de schimbare (accelereaza sau franeaza)
        float rate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // trece curentSpeed treptat spre targetSpeed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);

        // aplica viteza
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        // trimite spre Animator valoarea absoluta a vitezei, cu damping
        anim.SetFloat("Speed", Mathf.Abs(currentSpeed), 0.1f, Time.deltaTime);
    }
}
