using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanel : MonoBehaviour
{
    public enum SolarPanelState
    {
        None,
        Blue,
        Yellow,
        Both
    }
    public float offset;
    public SolarPanelState state;
    public ParticleSystem particles;
    ParticleSystem parts;

    public bool resetting;
    HingeJoint joint;
    JointMotor motor;
// Start is called before the first frame update
    void Start()
    {
        parts = Instantiate(particles, this.transform.position + new Vector3(0.1f, -0.1f, 0f), particles.transform.rotation);
        resetting = false;
        joint = this.GetComponent<HingeJoint>();
        motor = joint.motor;
    }

    // Update is called once per frame
    void Update()
    {
        if (resetting)
        {
            float angle = (transform.rotation.eulerAngles.y - offset);
            if (angle < -180f)
            {
                angle += 380f;
            }else if (angle > 180f)
            {
                angle -= 360f;
            }
            if (Mathf.Abs(angle) < 1f)
            {
                resetting = false;
                motor.freeSpin = true;
                motor.force = 0;
                motor.targetVelocity = 0f;
                joint.motor = motor;
            }
            else
            {
                motor.freeSpin = false;
                motor.force = 500;
                motor.targetVelocity = angle * -5f;
                joint.motor = motor;
            }


        }


        float angl = transform.rotation.eulerAngles.y - offset;
        if (angl > 180)
            angl -= 360;
        if (angl < -180)
            angl += 360;

        if (angl > 5f && angl < 155)
            ChangeState(SolarPanelState.Blue);
        else if (angl < -5f && angl > -155)
            ChangeState(SolarPanelState.Yellow);
        else if (angl > 155 || angl < -155)
            ChangeState(SolarPanelState.Both);
        else
            ChangeState(SolarPanelState.None);
        

    }
    private void ChangeState(SolarPanelState newState)
    {
        if (newState != state)
        {
            state = newState;
            if (state == SolarPanelState.Blue || state == SolarPanelState.Yellow)
            {
                var mainModule = parts.main;
                mainModule.startColor = state == SolarPanelState.Blue ? Color.blue : Color.yellow;
                parts.Play();
            }
        }
    }
    public void Restart()
    {
        float angle = (transform.rotation.eulerAngles.y - offset);
        if (angle < -180f)
        {
            angle += 380f;
        }
        else if (angle > 180f)
        {
            angle -= 360f;
        }
        if (Mathf.Abs(angle) > 2f)
            resetting = true;
        /*
        // Get the Rigidbody component attached to this GameObject
        Rigidbody rb = GetComponent<Rigidbody>();

        // Calculate the desired rotation using Euler angles
        Quaternion desiredRotation = Quaternion.Euler(0, offset, 0);

        // Set the rotation of the Rigidbody
        rb.MoveRotation(desiredRotation);
        */

        //transform.rotation = Quaternion.Euler(0, offset, 0);
    }
}
