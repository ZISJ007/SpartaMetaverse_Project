using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("캐릭터 이동")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("캐릭터 점프")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isJumpPressed = false;

    private Vector2 movementDirection = Vector2.zero;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Update()
    {
    }
    private void FixedUpdate()
    {
        MoveHandler();
        JumpHandler();
    }
    private void OnMove(InputValue inputValue)
    {
        movementDirection = inputValue.Get<Vector2>();
        movementDirection = movementDirection.normalized;
       
    }
    private void MoveHandler()
    {
        rb.velocity = movementDirection * moveSpeed;

        if (spriteRenderer == null) { return; }
        if (Mathf.Abs(movementDirection.x) > 0.01f)
        {
            // 왼쪽으로 가면 flipX = true, 오른쪽이면 false
            spriteRenderer.flipX = movementDirection.x < 0f;
            animator.SetBool("IsWalk", true);
        }
        else if(Mathf.Abs(movementDirection.x) == 0f)
        {
            animator.SetBool("IsWalk", false);
        }
       
    }

    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            isJumpPressed = true;
        }
        
    }

    private void JumpHandler()
    {
        if (isJumpPressed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger("IsJump");    // 점프 애니메이션 트리거가 있다면
        }
        isJumpPressed = false;
    }
}
