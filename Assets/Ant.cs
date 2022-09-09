using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public LayerMask takenFoodLayer;

    public bool canSeeFood;
    
    private bool isWandering = false;
    private bool isWalking = false;

    private Animator animator;

    Vector3 position;
    Vector3 forward;
    Vector3 velocity;
    Vector3 desiredDirection;
    Vector2 dd;

    private void Start()
    {
        position = transform.position;
        animator = GetComponent<Animator>();

        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        Debug.Log(transform.gameObject.name + " Can See Food ? -- " + canSeeFood );

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }
        if (isWalking == true)
        {
            if (!canSeeFood)
            {
                // Wander around
                dd = (dd + Random.insideUnitCircle * wanderStrength).normalized;
                desiredDirection = new Vector3(dd.x, 0, dd.y);
                Debug.Log(transform.gameObject.name + "Changing Spot to ----- " + desiredDirection);
            }

            Vector3 desiredVelocity = desiredDirection * maxSpeed;
            Vector3 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
            Vector3 acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;

            velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
            position += velocity * Time.deltaTime;

            float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, angle, 0));

            animator.SetFloat("Speed", velocity.magnitude);
        } 


    }
    IEnumerator Wander()
    {
        int walkWait = Random.Range(0, 3);
        int walkTime = Random.Range(5, 12);

        isWandering = true;
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        if(!canSeeFood)
        {
            Debug.Log($"{transform.gameObject.name} waiting for {walkWait}....");
            animator.SetFloat("Speed", 0.005f);
            yield return new WaitForSeconds(walkWait);
        }
        isWandering = false;
        // Todo add head rotation wait? i.e. look around for food
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
        if(targetFood == null)
        {
            Collider[] rangeChecks = Physics.OverlapSphere(position, viewRadius, foodMask);

            if (rangeChecks.Length > 0)
            {
                Debug.Log(transform.gameObject.name + "'s Range Check Length = " + rangeChecks.Length);
                //Transform target = rangeChecks[Random.Range(0, rangeChecks.Length)].transform;

                Transform target = rangeChecks[Random.Range(0, rangeChecks.Length)].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    //target.gameObject.layer = takenFoodLayer;
                    targetFood = target.gameObject;
                    Debug.Log(transform.gameObject.name + " can see gameobject -> " + targetFood.name);
                }
                else
                    canSeeFood = false;
            }
            else if (canSeeFood)
                canSeeFood = false;
        } 
        else
        {
            Vector3 directionToTarget = (targetFood.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetFood.transform.position);
            Debug.Log(transform.gameObject.name + " target " + targetFood.name + "'s tag = " + targetFood.tag);
            Debug.Log(transform.gameObject.name + " target " + targetFood.name + "'s layer  = " + targetFood.layer);

            if(targetFood.tag == "Food Carry")
            {
                canSeeFood = false;
                targetFood = null;
            } 
            else if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                desiredDirection = (targetFood.transform.position - transform.position).normalized;
                Debug.Log(transform.gameObject.name + "!! is heading to -> " + desiredDirection);
                canSeeFood = true;
                Debug.Log(transform.gameObject.name + "'s distance to target = " + distanceToTarget);
                // try picking up the food it is close enough
                if (distanceToTarget < foodPickupRadius)
                {
                    Debug.Log(transform.gameObject.name + ">> PICKedUP!");
                    canSeeFood = false;
                    targetFood.gameObject.layer = LayerMask.NameToLayer("Food Carry");
                    targetFood.tag = "Food Carry";
                    targetFood.transform.position = head.position;
                    targetFood.transform.parent = head;
                    targetFood = null;
                }
            }
            else
                canSeeFood = false; 

        }
    }
}
