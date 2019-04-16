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
    // Camera

    public CameraFollow camerafollow;

    // Compass

    public Compass compass;

    // Rigidbody

    private Rigidbody rb;

    // Movement

    public float forwardSpeed;
    public float turnLerpAmount;
    public float maxTurnSpeed;
    public float boatDepth;
    public float xTiltMagnitude;
    public float zTiltMagnitude;
    public float zTiltLerpAmount;
    public float zTiltMaximum;

    // Health

    public Text healthText;
    public int maxHealth;
    public float healthRegenPerSecond;

    private int health;
    private float timeToNextHealth;

    // Pickup Sounds

    public AudioSource northJingle;
    public AudioSource southJingle;
    public AudioSource eastJingle;
    public AudioSource westJingle;

    // Collision with Terrain

    public float collisionScreenShake;
    public float collisionBounceMagnitude;
    public int collisionDamageBase;
    public int collisionDamageMultiplier;

    public AudioSource crashSound;

    private static readonly int terrainLayer = 9;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        health = maxHealth;

    }

    private void FixedUpdate()
    {
        MoveShip();
    }

    private void Update()
    {
        if (!GamePauser.paused)
        {
            RegenHealth();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandlePickups(other);
        HandleReachingIslands(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Did we collide with terrain?
        if (collision.gameObject.layer == terrainLayer) {

            // Deal damage scaling with velocity
            int collisionDamage = collisionDamageBase + Mathf.RoundToInt(rb.velocity.magnitude * collisionDamageMultiplier);
            TakeDamage(collisionDamage);
        
            // Shake camera
            camerafollow.shake(collisionDamage * collisionScreenShake);

            // Bounce away from impact
            rb.AddForce(collision.impulse * collisionBounceMagnitude);

            // Play sound
            crashSound.pitch = Random.Range(0.4F, 0.7F);
            crashSound.Play();
}
    }

    private void MoveShip()
    {
        // Apply input movement to the rigidbody
        rb.AddForce(Wind.velocity * forwardSpeed * Time.deltaTime);

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

    private void RegenHealth()
    {
        timeToNextHealth -= Time.deltaTime;
        if (timeToNextHealth <= 0)
        {
            Heal(1);
            timeToNextHealth = 1 / healthRegenPerSecond;
        }
    }

    private void UpdateHealthText()
    {
        healthText.text = "Health: " + health;
    }

    private void Heal(int healing)
    {
        health += healing;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealthText();
    }

    private void TakeDamage(int damage)
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

    private void HandlePickups(Collider pickup)
    {
        if (pickup.gameObject.CompareTag("PickupNorth"))
        {
            pickup.gameObject.SetActive(false);
            if (Wind.prevailingWind != 1)
            {
                Wind.SetPrevailingWind(1);
                northJingle.Play();
            }
        }
        else if (pickup.gameObject.CompareTag("PickupSouth"))
        {
            pickup.gameObject.SetActive(false);
            if (Wind.prevailingWind != 2)
            {
                Wind.SetPrevailingWind(2);
                southJingle.Play();
            }
        }
        else if (pickup.gameObject.CompareTag("PickupEast"))
        {
            pickup.gameObject.SetActive(false);
            if (Wind.prevailingWind != 3)
            {
                Wind.SetPrevailingWind(3);
                eastJingle.Play();
            }
        }
        else if (pickup.gameObject.CompareTag("PickupWest"))
        {
            pickup.gameObject.SetActive(false);
            if (Wind.prevailingWind != 4)
            {
                Wind.SetPrevailingWind(4);
                westJingle.Play();
            }
        }
    }

    private void HandleReachingIslands(Collider island)
    {
        if (island.gameObject == Compass.targets[Compass.currentTargetNo]) {
            switch (Wind.prevailingWind)
            {
                case 1:
                    island.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.cyan;
                    Cutscene.Show(11);
                    break;

                case 2:
                    island.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
                    Cutscene.Show(12);
                    break;

                case 3:
                    island.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.magenta;
                    Cutscene.Show(13);
                    break;

                case 4:
                    island.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
                    Cutscene.Show(14);
                    break;

                default:
                    island.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
                    Cutscene.Show(11);
                    break;
            }
            rb.velocity = new Vector3(0F, 0F, 0F);
            island.enabled = false;
            Wind.SetPrevailingWind(0);
            if(!(Compass.currentTargetNo >= Compass.targets.Length)) {
                Compass.currentTargetNo++;
            }
        }
    }
}
