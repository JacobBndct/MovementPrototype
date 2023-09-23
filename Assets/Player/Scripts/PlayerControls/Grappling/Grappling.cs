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

    //what if this was effected by delta mouse pos during the grapple :OOOOOO
    [SerializeField] private float overshootYAxis;
    [SerializeField] private float pullDelay;
    [SerializeField] private float grappleSpeed;

    private Vector3 grapplePoint;
    private float grappleProgression;
    private float grappleDelay;
    private float grappleTime;

    [SerializeField] private float grappleCD;
    private float grappleCDTimer;

    private bool isGrappling;

    private Rigidbody rb;
    private bool freeze;
/*    private bool activeGrapple;*/

    [Header("Spring")]
    [SerializeField] private int quality;
    [SerializeField] Spring spring;
    [SerializeField] float damper;
    [SerializeField] float strength;
/*    [SerializeField] float velocity;*/
    [SerializeField] float waveCount;
    [SerializeField] float waveHeight;
    [SerializeField] AnimationCurve affectCurve;

    private void Awake()
    {
        //get playerMovement;
        rb = GetComponent<Rigidbody>();
        spring = new Spring();
        spring.SetTarget(0);
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
            /*            grappleRope.SetPosition(0, gunTip.position);
                        grappleProgression += grappleTime * Time.deltaTime;
                        grappleRope.SetPosition(1, Vector3.Lerp(gunTip.position, grapplePoint, grappleProgression));*/
            DrawRope();
        } else
        {
            grappleRope.positionCount = 0;
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
        grappleProgression = 0.0f;
        freeze = true;

        //This looks like it could share functionality with the Lidar!
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDist, IsGrappleableLayer))
        {
            grapplePoint = hit.point;

            SetGrappleTime();
            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDist;

            SetGrappleTime();
            Invoke(nameof(StopGrapple), grappleDelay);
        }

        grappleRope.enabled = true;
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

        spring.Reset();
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
    private void SetGrappleTime()
    {
        float distance = Vector3.Distance(gunTip.position, grapplePoint);
        grappleTime = grappleSpeed / distance;
        grappleDelay = 1 / grappleTime;
    }

    void DrawRope()
    {
        if (grappleRope.positionCount == 0)
        {
            spring.SetVelocity(grappleSpeed);
            grappleRope.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var up = Quaternion.LookRotation((grapplePoint - gunTip.position).normalized) * Vector3.up;

        grappleProgression += grappleTime * Time.deltaTime;
        Vector3 currentGrapplePosition = Vector3.Lerp(gunTip.position, grapplePoint, grappleProgression);

        for (var i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            grappleRope.SetPosition(i, Vector3.Lerp(gunTip.position, currentGrapplePosition, delta) + offset);
        }
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), pullDelay);
    }
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        rb.velocity = velocityToSet;
    }

}
