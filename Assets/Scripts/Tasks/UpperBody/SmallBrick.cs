using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBrick : MonoBehaviour
{
    public bool AnyCollision
    {
        get
        {
            return Colliders.Count > 0;
        }
    }

    private List<Collider> Colliders = new List<Collider>();
    private BricksController BricksController;

    private void Awake()
    {
        BricksController = GetComponentInParent<BricksController>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (Colliders.Count == 0)
        {
            BricksController.AddCollision();
        }
        Colliders.Add(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        Colliders.Remove(collider);
    }

    private void OnDrawGizmos()
    {
        if (AnyCollision)
        {
            Collider collider = GetComponent<Collider>();
            Gizmos.color = Color.green;
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.extents * 2);
        }
    }
}
