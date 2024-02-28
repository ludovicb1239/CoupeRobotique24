using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Vector2 robotSpeed = new Vector2(0.5f, 0.5f);
    public float angularSpeed = 0f;
    public float wheelAngleDeg = 45f;
    private const float wheelRad = 0.03f;
    private const float wheelDistance = 0.162f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float circum = 2.0f * Mathf.PI * wheelRad; // meters
        float wheelSpeed = (- Mathf.Cos(Mathf.Deg2Rad * wheelAngleDeg) * robotSpeed.x + Mathf.Sin(Mathf.Deg2Rad * wheelAngleDeg) * robotSpeed.y) / circum; // tr

        float distCircum = 2.0f * Mathf.PI * wheelDistance; // meters
        wheelSpeed -= distCircum * angularSpeed / circum; // tr

        // Calculate rotation angle based on input
        float rotationAngle = wheelSpeed * Time.deltaTime * 360f;

        // Rotate the wheel around its forward axis
        transform.Rotate(Vector3.forward, rotationAngle);
    }

}
