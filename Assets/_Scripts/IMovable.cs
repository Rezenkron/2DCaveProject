using System;
using UnityEngine;

public interface IMovable
{
    void Move(Vector2 direction);
    event Action OnMove;
}