using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BricksController : MonoBehaviour
{
    private SmallBrick[] SmallBricks;

    private float CollisionTime;
    private int NumberCollisions;

    private void Awake()
    {
        SmallBricks = GetComponentsInChildren<SmallBrick>();
    }

    private void Update()
    {
        bool any = false;
        for (int i = 0; i < SmallBricks.Length && !any; ++i)
        {
            any = SmallBricks[i].AnyCollision;
        }
        if (any)
        {
            CollisionTime += Time.deltaTime;
        }
    }

    public void AddCollision()
    {
        NumberCollisions += 1;
    }
}
