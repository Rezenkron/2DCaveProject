using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    private Vector2 lastDirection = Vector2.down;
    private int currentAnimation;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        playerMovement.OnMoveEvent += UpdateAnimation;
    }

    private void OnDisable()
    {
        playerMovement.OnMoveEvent -= UpdateAnimation;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            lastDirection = direction;
            PlayAnimation(GetWalkAnimation(direction));
        }
        else
        {
            PlayAnimation(GetIdleAnimation(lastDirection));
        }
    }

    private void PlayAnimation(int animationName)
    {
        if (currentAnimation == animationName) return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }

    private int GetWalkAnimation(Vector2 direction)
    {
        if (direction.x > 0) return HashedAnimations.WalkRight;
        if (direction.x < 0) return HashedAnimations.WalkLeft;
        if (direction.y > 0) return HashedAnimations.WalkUp;
        return HashedAnimations.WalkDown;
    }

    private int GetIdleAnimation(Vector2 direction)
    {
        if (direction.x > 0) return HashedAnimations.IdleRight;
        if (direction.x < 0) return HashedAnimations.IdleLeft;
        if (direction.y > 0) return HashedAnimations.IdleUp;
        return HashedAnimations.IdleDown;
    }
}