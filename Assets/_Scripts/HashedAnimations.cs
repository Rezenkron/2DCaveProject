using UnityEngine;

public class HashedAnimations
{
    public static readonly int IdleLeft = Animator.StringToHash("IdleLeft");
    public static readonly int IdleUp = Animator.StringToHash("IdleUp");
    public static readonly int IdleDown = Animator.StringToHash("IdleDown");
    public static readonly int IdleRight = Animator.StringToHash("IdleRight");
    public static readonly int WalkLeft = Animator.StringToHash("WalkLeft");
    public static readonly int WalkUp = Animator.StringToHash("WalkUp");
    public static readonly int WalkDown = Animator.StringToHash("WalkDown");
    public static readonly int WalkRight = Animator.StringToHash("WalkRight");

    public static readonly int Vertical = Animator.StringToHash("Vertical");
    public static readonly int Horizontal = Animator.StringToHash("Horizontal");
}
