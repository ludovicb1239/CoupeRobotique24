using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Drawing.Printing;
using System;

public struct KeyStruct
{
    public float Joystick1_X;
    public float Joystick1_Y;
    public float Joystick2_X;
    public float Joystick2_Y;
    public bool Button1;
    public bool Button2;
    public bool Button3;
    public bool Button4;
    public bool Joystick1_SW;
    public bool Joystick2_SW;
    public override string ToString()
    {
        return $"Joystick1_X: {Joystick1_X:F2}\nJoystick1_Y: {Joystick1_Y:F2}\nJoystick2_X: {Joystick2_X:F2}\nJoystick2_Y: {Joystick2_Y:F2}\n" +
           $"Bouton1: {Button1}\nBouton2: {Button2}\nBouton3: {Button3}\nBouton4: {Button4}\nJoystick1_SW: {Joystick1_SW}\nJoystick2_SW: {Joystick2_SW}";
    }
    public static KeyStruct FromInput()
    {
        KeyStruct keyboard = new()
        {
            Joystick1_X = Input.GetAxis("Joystick1_X"),
            Joystick1_Y = Input.GetAxis("Joystick1_Y"),
            Joystick2_X = Input.GetAxis("Joystick2_X"),
            Joystick2_Y = Input.GetAxis("Joystick2_Y"),
            Button1 = Input.GetAxis("Button1") != 0,
            Button2 = Input.GetAxis("Button2") != 0,
            Button3 = Input.GetAxis("Button3") != 0,
            Button4 = Input.GetAxis("Button4") != 0,
            Joystick1_SW = Input.GetAxis("Joystick1_SW") != 0,
            Joystick2_SW = Input.GetAxis("Joystick2_SW") != 0
        };
        return keyboard;
    }
}
public class keysDisplay : MonoBehaviour
{
    public SerialCommunication serialComm;
    public TextMeshProUGUI keys;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        keys.text = serialComm.keys.ToString();
    }
}
