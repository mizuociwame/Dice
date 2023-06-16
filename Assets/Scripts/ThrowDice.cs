using System;
using UnityEngine;

public class ThrowDice : MonoBehaviour
{
    public bool onTable;
    public bool isThrown;

    public float faceCheckCapacityQuarternion = 0.04f;
    public float throwForceMagnitude = 1.5f;
    public float throwForceErrorRange = 0.1f;
    public float throwTorqueMagnitude = 1.5f;
    public float preRotateRange = 90f;
    public float maxAngularVelocity = 50f;

    public Vector3 throwScale = new Vector3(1f, 1f, 1f);

    Vector3 throwPosition = new Vector3(0f, 3f, -1.4f);
    Vector3 throwForceDirection = new Vector3(0, 2.0f, 1.0f);

    static System.Random randomGenerator = new System.Random();
    // Start is called before the first frame update

    static float RandomFloatGenerator()
    {
        Double num = randomGenerator.NextDouble();
        num = num * 2 - 1;
        return (float)num;
    }


    public int CheckFace()
    {
          int face = 0;
        Quaternion quarternion = gameObject.transform.localRotation;

        if (Math.Abs(Math.Pow(quarternion.x, 2) - Math.Pow(quarternion.z, 2)) < faceCheckCapacityQuarternion)
        {
            face = 0;
        }
        else if (Math.Abs(Math.Pow(quarternion.w, 2) - Math.Pow(quarternion.y, 2)) < faceCheckCapacityQuarternion)
        {
            face = 5;
        }
        else if (Math.Pow(quarternion.y + quarternion.z, 2) < faceCheckCapacityQuarternion && Math.Pow(quarternion.w - quarternion.x, 2) < faceCheckCapacityQuarternion)
        {
            face = 1;
        }
        else if (Math.Pow(quarternion.w + quarternion.x, 2) < faceCheckCapacityQuarternion && Math.Pow(quarternion.y - quarternion.z, 2) < faceCheckCapacityQuarternion)
        {
            face = 4;
        }
        else if (Math.Pow(quarternion.w - quarternion.z, 2) < faceCheckCapacityQuarternion && Math.Pow(quarternion.x - quarternion.y, 2) < faceCheckCapacityQuarternion)
        {
            face = 2;
        }
        else if (Math.Pow(quarternion.w + quarternion.z, 2) < faceCheckCapacityQuarternion && Math.Pow(quarternion.x + quarternion.y, 2) < faceCheckCapacityQuarternion)
        {
            face = 3;
        }
        return face;
    }

    public void SetThrowDirection(Vector3 throwPosition, Vector3 throwDirection)
    {
        this.throwPosition = throwPosition;
        this.throwForceDirection = throwDirection;
    }

    void Throw()
    {
        Vector3 forceDirection = throwForceDirection + new Vector3(RandomFloatGenerator(), RandomFloatGenerator(), RandomFloatGenerator()) * throwForceErrorRange;
        Vector3 force = forceDirection * throwForceMagnitude;

        Vector3 torqueDirection = new Vector3(RandomFloatGenerator(), RandomFloatGenerator(), RandomFloatGenerator());
        Vector3 torque = torqueDirection * throwTorqueMagnitude;

        gameObject.transform.position = throwPosition;
        gameObject.transform.localScale = throwScale;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.transform.Rotate(new Vector3(RandomFloatGenerator(), RandomFloatGenerator(), RandomFloatGenerator()) * preRotateRange);
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque);

        onTable = false;
    }

    public bool CheckMoving()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        bool moving = Vector3.SqrMagnitude(rb.velocity) != 0;
        return moving;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "table")
        {
            onTable = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "table")
        {
            onTable = false;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update() 
    {
        if (gameObject.transform.position.y < 0)
        {
            isThrown = true;
        }
    }

    void FixedUpdate()
    {
        if (isThrown)
        {
            Throw();
            isThrown = false;
        }
    }
}
