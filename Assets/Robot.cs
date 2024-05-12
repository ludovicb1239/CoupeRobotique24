using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private Vector2 robotSpeed = new Vector2(0f, 0f); // m/s
    private float angularSpeed = 0f; // t/s
    public List<Wheel> wheels = new List<Wheel>();
    public float speed = 0.5f;
    public float angSpeed = 0.5f;
    Rigidbody rb;

    public SerialCommunication comm;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!rb.isKinematic)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            Vector3 localAngVelocity = transform.InverseTransformDirection(rb.angularVelocity);
            foreach (Wheel wheel in wheels)
            {
                wheel.robotSpeed = new Vector2(localVelocity.x, localVelocity.z);
                wheel.angularSpeed = localAngVelocity.y * Mathf.Rad2Deg / 360f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            angularSpeed = comm.keys.Joystick2_X * angSpeed;
            robotSpeed.y = comm.keys.Joystick1_Y;
            robotSpeed.x = comm.keys.Joystick1_X;
            float sped = Mathf.Clamp01(robotSpeed.sqrMagnitude) * speed;
            float ang = Mathf.Atan2(robotSpeed.y, robotSpeed.x);

            //rb.velocity = sped * (transform.right * Mathf.Cos(ang) + transform.forward * Mathf.Sin(ang));
            //rb.angularVelocity = Vector3.up * angularSpeed * 2 * Mathf.PI;
            rb.AddForce(sped * (transform.right * Mathf.Cos(ang) + transform.forward * Mathf.Sin(ang)), ForceMode.Force);
            rb.angularVelocity = 2 * angularSpeed * Mathf.PI * Vector3.up;
        }
    }
}
