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
    // Start is called before the first frame update
    void Start()
    {
        parts = Instantiate(particles, this.transform.position + new Vector3(0.1f, -0.1f, 0f), particles.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
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
        transform.rotation = Quaternion.Euler(0, offset, 0);
    }
}
