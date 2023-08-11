using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool OnGround;

    [SerializeField] Transform feetTransform;
    [SerializeField] float maxDistance;

    private void Update()
    {
        IsOnGround();
    }

    private void IsOnGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(feetTransform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            OnGround = hit.distance < maxDistance;
        }
    }
}
