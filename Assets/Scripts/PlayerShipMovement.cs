using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script handles the movement of the player's ship.
 * It depends on Wind.cs
*/

public class PlayerShipMovement : MonoBehaviour
{

    public float forwardSpeed;
    public float turnLerpAmount;
    public float maxTurnSpeed;
    public float boatDepth;
    public float xTiltMagnitude;
    public float zTiltMagnitude;
    public float zTiltLerpAmount;
    public float zTiltMaximum;

    private Rigidbody rb;
    private Quaternion seaRollingRotation;
    private Quaternion targetRotation;
    private Quaternion testLerpRotation;

    //private int _layerMask = 1 << 4;

    void Start()
    {

        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {

        // Apply input movement to the rigidbody
        rb.AddForce(Wind.velocity * forwardSpeed);

        // Get target rotation from the rigidbody's current velocity
        targetRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);

        // Turn toward the target rotation
        if (rb.velocity.magnitude > 0)
        {
            // We first lerp the boat toward the rotation. If the turn angle is too large then we discard the lerp and cap it at the max turn speed.
            // This also gives us the turn angle to play with later.
            testLerpRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnLerpAmount);
            float turnAngle = Quaternion.Angle(transform.rotation, testLerpRotation);
            if (turnAngle < maxTurnSpeed)
            {
                transform.rotation = testLerpRotation;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed);
            }
        }

        // Get the y position from the water controller and translate us to it
        // This also gives us the y delta to play with later.
        float yPosition = WaterController.current.GetWaveYPos(transform.position, Time.time) - boatDepth;
        float yDelta = yPosition - transform.position.y;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, yPosition, transform.position.z), Time.deltaTime);

        // Tilt up or down depending on the y delta
        transform.Rotate(Vector3.right * yDelta * xTiltMagnitude * Time.deltaTime * rb.velocity.magnitude);

        // Get the angle between our current velocity and our current rotation
        float angleToVelocity = Vector3.SignedAngle(transform.forward, rb.velocity, Vector3.up);
        // Tilt left or right depending on the turn angle
        transform.Rotate(Vector3.forward, angleToVelocity * Time.deltaTime * zTiltMagnitude);
        // Sanity check - lerp back to resting position, and cap at z tilt maximum
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), Time.deltaTime * zTiltLerpAmount);
        if (transform.eulerAngles.z > zTiltMaximum)
        {
            if (transform.eulerAngles.z < 180)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zTiltMaximum);
            } else if (transform.eulerAngles.z < 360 - zTiltMaximum)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 360 - zTiltMaximum);
            }
        }

        /*
        // Raycast to the sea collider mesh and rotate to the normal
        Vector3 raycastSource = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 theRay = -transform.up;
        RaycastHit hit;
        if (Physics.Raycast(raycastSource, theRay, out hit, 20, _layerMask))
        {
            Debug.DrawRay(raycastSource, theRay * hit.distance, Color.yellow);
            Debug.DrawRay(raycastSource, hit.normal * 1000, Color.red);
            print("Hit");
            seaRollingRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {
            Debug.DrawRay(raycastSource, theRay * 1000, Color.white);
            print ("NoHit");
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, seaRollingRotation, Time.deltaTime / 0.15f);
        */

    }
}
