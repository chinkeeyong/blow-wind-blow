using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This script handles the movement of the player's ship, as well as collision.
 * It depends on Wind.cs
*/

public class PlayerShip : MonoBehaviour
{

    public float forwardSpeed;
    public float turnLerpAmount;
    public float maxTurnSpeed;
    public float boatDepth;
    public float xTiltMagnitude;
    public float zTiltMagnitude;
    public float zTiltLerpAmount;
    public float zTiltMaximum;
    public CameraFollow camerafollow;
    public Text healthText;
    public int maxHealth;
    public float healthRegenPerSecond;
    public float collisionScreenShake;
    public float collisionBounceMagnitude;
    public int collisionDamageBase;
    public int collisionDamageMultiplier;

    private int terrainLayer;
    private int health;
    private float timeToNextHealth;

    private Rigidbody rb;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        terrainLayer = 9;

    }

    void Update()
    {

        MoveShip();
        RegenHealth();
    }

    private void MoveShip()
    {
        // Apply input movement to the rigidbody
        rb.AddForce(Wind.velocity * forwardSpeed);

        // While moving, turn toward the target rotation and tilt left or right
        if (rb.velocity.magnitude > 0.001F)
        {
            // Get target rotation from the rigidbody's current velocity
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity, Vector3.up);

            // We first lerp the boat toward the rotation. If the turn angle is too large then we discard the lerp and cap it at the max turn speed.
            Quaternion testLerpRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnLerpAmount);
            float turnAngle = Quaternion.Angle(transform.rotation, testLerpRotation);
            if (turnAngle < maxTurnSpeed)
            {
                transform.rotation = testLerpRotation;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed);
            }

            // Get the angle between our current velocity and our current rotation
            float angleToVelocity = Vector3.SignedAngle(transform.forward, rb.velocity, Vector3.up);

            // Tilt left or right depending on the turn angle
            transform.Rotate(Vector3.forward, angleToVelocity * Time.deltaTime * zTiltMagnitude);
        }

        // Get the y position from the water controller and translate us to it
        // This also gives us the y delta to play with later.
        float yPosition = WaterController.current.GetWaveYPos(transform.position, Time.time) - boatDepth;
        float yDelta = yPosition - transform.position.y;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, yPosition, transform.position.z), Time.deltaTime);

        // Sanity check - lerp back to resting position, and cap at z tilt maximum
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), Time.deltaTime * zTiltLerpAmount);
        if (transform.eulerAngles.z > zTiltMaximum)
        {
            if (transform.eulerAngles.z < 180)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zTiltMaximum);
            }
            else if (transform.eulerAngles.z < 360 - zTiltMaximum)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 360 - zTiltMaximum);
            }
        }

        // Tilt up or down depending on the y delta
        transform.Rotate(Vector3.right * yDelta * xTiltMagnitude * Time.deltaTime * rb.velocity.magnitude);
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + health;
    }

    private void RegenHealth()
    {
        timeToNextHealth -= Time.deltaTime;
        if (timeToNextHealth <= 0)
        {
            heal(1);
            timeToNextHealth = 1 / healthRegenPerSecond;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == terrainLayer)
        {
            camerafollow.shake(rb.velocity.magnitude * collisionScreenShake);
            int collisionDamage = collisionDamageBase + Mathf.RoundToInt(rb.velocity.magnitude * collisionDamageMultiplier);
            takeDamage(collisionDamage);
            rb.AddForce(collision.impulse * collisionBounceMagnitude);
        }
    }

    private void heal(int healing)
    {
        health += healing;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealthText();
    }

    private void takeDamage(int damage)
    {
        if (health >= maxHealth) // Protection from 1 hit KO
        {
            health -= damage;
            if (health < 1)
            {
                health = 1;
            }
        }
        else
        {
            health -= damage;
            if (health < 0)
            {
                health = 0;
            }
        }
        UpdateHealthText();
    }
}
