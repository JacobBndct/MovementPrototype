using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    //private playerMovement pm
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask IsGrappleableLayer;
    [SerializeField] private LineRenderer grappleRope;
    
    [Header("Grappling")]
    [SerializeField] private float maxGrappleDist;
    [SerializeField] private float grappleDelay;

    //what if this was effected by delta mouse pos during the grapple :OOOOOO
    [SerializeField] private float overshootYAxis;
    [SerializeField] private float pullDelay;

    private Vector3 grapplePoint;

    [SerializeField] private float grappleCD;
    private float grappleCDTimer;

    private bool isGrappling;

    private Rigidbody rb;
    public bool freeze;
    private bool activeGrapple;

    [Header("Spring")]
    [SerializeField] private int quality;
    [SerializeField] Spring spring;
    [SerializeField] float damper;
    [SerializeField] float strenght;
    [SerializeField] float velocity;
    [SerializeField] float waveCount;
    [SerializeField] float waveHeight;
    [SerializeField] AnimationCurve affectCurve;

    private void Start()
    {
        //get playerMovement;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (grappleCDTimer > 0)
        {
            grappleCDTimer -= Time.deltaTime;
        }

        if (freeze)
        {
            rb.constraints |= RigidbodyConstraints.FreezePosition;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePosition;
        }
    }

    private void LateUpdate()
    {
        if(isGrappling)
        {
            grappleRope.SetPosition(0, gunTip.position);
            /*grappleRope.SetPosition(1, Vector3.Lerp());*/
        }
    }

    public void GrappleEnter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartGrapple();
        }
    }

    //This looks like it could be in a state!
    private void StartGrapple()
    {
        if (grappleCDTimer > 0) return;

        isGrappling = true;

        freeze = true;

        //This looks like it could share functionality with the Lidar!
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDist, IsGrappleableLayer))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDist;

            Invoke(nameof(StopGrapple), grappleDelay);
        }

        grappleRope.enabled = true;
        grappleRope.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    private void StopGrapple()
    {
        isGrappling = false;

        freeze = false;

        grappleCDTimer = grappleCD;

        grappleRope.enabled = false;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startingPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startingPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startingPoint.x, 0f, endPoint.z - startingPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityY + velocityXZ;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), pullDelay);
    }
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
    }
}
