using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public Transform head;

    public GameObject targetFood;

    public float maxSpeed = 2;
    public float steerStrength = 2;
    public float wanderStrength = 0.17f;
    public float viewRadius = 2;
    [Range(0, 360)]
    public float viewAngle = 2;
    public float foodPickupRadius = 0.05f;

    public LayerMask foodMask;
    public LayerMask obstructionMask;

    public bool canSeeFood;

    Vector3 position;
    Vector3 forward;
    Vector3 velocity;
    Vector3 desiredDirection;
    Vector2 dd;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        targetFood = GameObject.FindGameObjectWithTag("Food");
        HandleMovement();
        


    }

    private void HandleMovement()
    {
        //desiredDirection = ((Vector3)target.position - position).normalized;
        if (!canSeeFood)
        {
            dd = (dd + Random.insideUnitCircle * wanderStrength).normalized;

            desiredDirection = new Vector3(dd.x, 0, dd.y);
        }
           
        Vector3 desiredVelocity = desiredDirection * maxSpeed;
        Vector3 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

        velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, angle, 0));
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;

        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            HandleFood();
        }
    }

    void HandleFood()
    {
         //Get all food objects within perception radius
        Collider[] rangeChecks = Physics.OverlapSphere(position, viewRadius, foodMask);

        if (rangeChecks.Length != 0)
        {
            //Transform target = rangeChecks[Random.Range(0, rangeChecks.Length)].transform;

            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    desiredDirection = (target.position - transform.position).normalized;
                    Debug.Log("!!!!!!! Heading to -> " + desiredDirection);
                    canSeeFood = true;
                    Debug.Log("$$$ distance to target = " + distanceToTarget);
                    // try picking up the food it is close enough
                    if (distanceToTarget < foodPickupRadius)
                    {
                        Debug.Log(">> PICKUP!");
                        targetFood.transform.position = head.position;
                        targetFood.transform.parent = head;
                        targetFood = null;
                    }
                }
                else
                    canSeeFood = false;
            }
            else
                canSeeFood = false;
        }
        else if (canSeeFood)
            canSeeFood = false;

        //if(targetFood == null)
        //{
        //    // Getall food objects within perception radius
        //    Collider[] allFood = Physics.OverlapSphere(position, viewRadius, foodLayer);
        //    Debug.Log("all food -> " + allFood);

        //    if (allFood.Length > 0)
        //    {
        //        // Select one of the food objects at random
        //        Transform food = allFood[Random.Range(0, allFood.Length)].transform;
        //        Vector3 dirToFood = (food.position - head.position).normalized; // this might cause issues with y axis
                
        //        // Start targeting the food if it is within view angle
        //        if (Vector3.Angle(forward, dirToFood) < viewAngle / 2)
        //        {
        //            //food.gameObject.layer = Layers.takenFoodLayer;
        //            targetFood = food;
        //        }
        //    }
        //} else
        //{
        //    Debug.Log("FOUND!");
        //    // Try move towards the target food
        //    desiredDirection = (targetFood.position - head.position).normalized;

        //    // Pick up the food if it is close enough
        //    const float foodPickupRadius = 0.05f;
        //    if(Vector3.Distance(targetFood.position, head.position) < foodPickupRadius)
        //    {
        //        targetFood.position = head.position;
        //        targetFood.parent = head;
        //        targetFood = null;
        //    }
        //}
    }
}
