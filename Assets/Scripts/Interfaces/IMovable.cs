using UnityEngine;

public interface IMovable
{
    void Move(Vector2 direction, float speedMultiplier = 1F);
}