using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.IO.Ports;
using System.Drawing;
using UnityEngine.VFX;
using TMPro;

public class SerialCommunication : MonoBehaviour
{
    // Start is called before the first frame update
    int counter;
    public TextMeshProUGUI countText;

    public KeyStruct keys = new();

    public SerialPortDialog portDialog;

    private static System.Threading.Timer timer;

    public SerialPort robotPort;
    public SerialPort remotePort;

    public bool fromKeyboard = true;
    public GameManager gameManager;

    private Thread thread;
    private bool running = true;
    private float loopFrequency = 100f; // Frequency in Hz

    void Start()
    {
        counter = 10;
        updateCountDisplay();

        // Start the thread when the script is initialized

        thread = new Thread(ThreadLoop);
        running = true;
        thread.Start();
    }

    private void ThreadLoop()
    {
        // Calculate the time interval in milliseconds between each loop iteration
        int interval = (int)(1000f / loopFrequency);

        while (running)
        {
            CommunicateWithRemote();
            CheckForBytes();

            // Sleep the thread for the specified interval
            Thread.Sleep(interval);
        }
    }

    private void FixedUpdate()
    {
        fromKeyboard = remotePort == null || !remotePort.IsOpen;
        if (fromKeyboard)
        {
            if (!gameManager.isTouchControls)
            {
                keys = KeyStruct.FromInput();
            }
            else
            {
                keys = gameManager.FromTouchControls();
            }
        }
    }
    private void OnApplicationQuit()
    {
        // Stop the thread when the application quits
        running = false;
        thread.Join(); // Wait for the thread to finish before quitting
    }
    public void AskForRemotePort()
    {
        if (remotePort != null && remotePort.IsOpen) remotePort.Close();
        portDialog.ShowDialog(ConnectToRemote);
    }
    public void AskForRobotPort()
    {
        if (robotPort != null && robotPort.IsOpen) robotPort.Close();
        portDialog.ShowDialog(ConnectToRobot);
    }
    public void ConnectToRemote(string port)
    {
        if (!string.IsNullOrEmpty(port))
        {
            remotePort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            remotePort.Open();
        }
    }
    public void DisconnectToRemote()
    {
        if (remotePort == null) return;
        remotePort.Close();
        remotePort.Dispose();
    }
    public void ConnectToRobot(string port)
    {
        if (!string.IsNullOrEmpty(port))
        {
            robotPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            robotPort.Open();
        } 
    }
    public void DisconnectToRobot()
    {
        if (robotPort == null) return;
        robotPort.Close();
        robotPort.Dispose();
    }
    public bool CommunicateWithRemote()
    {
        if (remotePort == null) return false;
        if (!remotePort.IsOpen) return false;



        if (remotePort.BytesToRead < 13)
        {
            remotePort.Write("A");

            return false;
        }

        // read the incoming bytes
        byte[] Mymessage = new byte[16];
        remotePort.Read(Mymessage, 0, 13);

        keys = new KeyStruct()
        {
            Joystick1_X = ((float)Mymessage[1] - 127.5f) / -127.5f,
            Joystick1_Y = ((float)Mymessage[2] - 127.5f) / -127.5f,

            Joystick2_X = ((float)Mymessage[3] - 127.5f) / -127.5f,
            Joystick2_Y = ((float)Mymessage[4] - 127.5f) / -127.5f,

            Button1 = Mymessage[5] == 0,
            Button2 = Mymessage[6] == 0,
            Button3 = Mymessage[7] == 0,
            Button4 = Mymessage[8] == 0,

            Joystick1_SW = Mymessage[9] == 0,
            Joystick2_SW = Mymessage[10] == 0
        };
        if (MathF.Abs(keys.Joystick1_X) < 0.12f)
            keys.Joystick1_X = 0;
        if (MathF.Abs(keys.Joystick1_Y) < 0.12f)
            keys.Joystick1_Y = 0;
        if (MathF.Abs(keys.Joystick2_X) < 0.12f)
            keys.Joystick2_X = 0;
        if (MathF.Abs(keys.Joystick2_Y) < 0.12f)
            keys.Joystick2_Y = 0;

        if (Mymessage[11] != 0)
        {
            counter += (sbyte)Mymessage[11];
            updateCountDisplay();
        }

        remotePort.Write("A");


        return true;
    }

    void CheckForBytes()
    {
        if (robotPort == null) return;
        if (!robotPort.IsOpen) return;

        if (robotPort.BytesToRead > 0)
        {
            byte[] val = new byte[1];
            robotPort.Read(val, 0, 1);

            if (val[0] == 65) // ASCII value of 'A'
            {
                byte[] myMessage = new byte[13];
                myMessage[0] = 65;
                myMessage[1] = floatToByte(keys.Joystick1_X);
                myMessage[2] = floatToByte(keys.Joystick1_Y);
                myMessage[3] = floatToByte(keys.Joystick2_X);
                myMessage[4] = floatToByte(keys.Joystick2_Y);
                myMessage[5] = keys.Button1 ? (byte)0 : (byte)1;
                myMessage[6] = keys.Button2 ? (byte)0 : (byte)1;
                myMessage[7] = keys.Button3 ? (byte)0 : (byte)1;
                myMessage[8] = keys.Button4 ? (byte)0 : (byte)1;
                myMessage[9] = keys.Joystick1_SW ? (byte)0 : (byte)1;
                myMessage[10] = keys.Joystick2_SW ? (byte)0 : (byte)1;
                myMessage[11] = 0; //Delta Encoder
                myMessage[12] = 0x55;

                robotPort.Write(myMessage, 0, 13); // Write the serial data
            }
            robotPort.DiscardInBuffer();
        }
    }
    static byte floatToByte(float f)
    {
        return Math.Clamp((byte)(f * -127.5f + 127.5f), (byte)0, (byte)255);
    }
    void updateCountDisplay()
    {
        countText.text = FormatToFourDigits(counter);
    }
    public static string FormatToFourDigits(int number)
    {
        number = Mathf.Clamp(number, 0, 9999);
        return number.ToString("D4");
    }
}