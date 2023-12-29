using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{

    public GameObject axe;
    public Rigidbody axeRb;
    public GameObject axePosHolder;

    public float axeFlightSpeed = 10f;
    public float axeThrowPower = 10f;
    public float axeRotationSpeed = 10f;

    public AxeCollision axeCollision;

    public enum AxeState { Static, Thrown, Traveling, Returning }
    public AxeState axeState;

    private float startTime;
    private float journeyLength;
    private float journeyTime;
    private Vector3 startPosition;
    private Vector3 endPosition;


    // Start is called before the first frame update
    void Start()
    {
        axeRb = GetComponent<Rigidbody>();
        axeRb.isKinematic = true;
        axeRb.useGravity = false;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            axeState = AxeState.Thrown;
        }

        if (Input.GetMouseButtonDown(1))
        {
            startPosition = axe.transform.position;
            endPosition = axePosHolder.transform.position;
            startTime = Time.time; 
            journeyLength = Vector3.Distance(startPosition, endPosition);
            axeState = AxeState.Returning;
        }

        if (axeState == AxeState.Thrown)
        {
            ThrownAxeWithPhysics();
        }

        if (axeState == AxeState.Traveling || axeState == AxeState.Returning)
        {
            axe.transform.Rotate(6.0f * axeRotationSpeed * Time.deltaTime, 0, 0);
        }

        if(axeState == AxeState.Returning)
        {
            RecallAxe();
        }
    }

    void ThrownAxeWithPhysics()
    {
        axe.transform.parent = null;
        axeState = AxeState.Traveling;
        axeRb.isKinematic = false;
        axeRb.useGravity = true;

        axeRb.AddForce(axe.transform.forward * axeThrowPower);
    }

    void RecallAxe()
    {
        float distCovered = (Time.time - startTime) / axeFlightSpeed;
        float fracJourney = distCovered / journeyLength;
        
        axe.transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

        if(fracJourney >= 1.0f)
        {
            RecalledAxe();
        }
    }

    void RecalledAxe()
    {
        axeState = AxeState.Static;

        axeCollision.RemoveConstraints();

        axe.transform.position = axePosHolder.transform.position;
        axe.transform.rotation = axePosHolder.transform.rotation;

        axeRb.isKinematic = true;
        axeRb.useGravity = false;

        axe.transform.parent = this.transform;
    }

    public void CollisionOccured()
    {
        axeState = AxeState.Static;
    }
}
