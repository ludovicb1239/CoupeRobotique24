using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SerialPortDialog : MonoBehaviour
{
    public TMP_Dropdown portDropdown;
    private string selectedPort;

    private System.Action<string> callback;

    private void Start()
    {
        gameObject.SetActive(false); // Initially hide the dialog
        LoadAvailablePorts();
    }

    private bool LoadAvailablePorts()
    {
        try
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            portDropdown.ClearOptions();
            portDropdown.AddOptions(new System.Collections.Generic.List<string>(ports));
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    public void ShowDialog(System.Action<string> onPortSelected)
    {
        callback = onPortSelected;
        bool success = LoadAvailablePorts();
        gameObject.SetActive(success);
    }

    public void OnOKButtonClicked()
    {
        selectedPort = portDropdown.captionText.text;
        if (!string.IsNullOrEmpty(selectedPort))
        {
            Debug.Log($"Selected Serial Port: {selectedPort}");
            callback?.Invoke(selectedPort);
        }
        else
        {
            Debug.Log("No serial port selected.");
            callback?.Invoke(null);
        }

        // Close or hide the dialog
        gameObject.SetActive(false);
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Serial port selection canceled.");
        callback?.Invoke(null);

        // Close or hide the dialog
        gameObject.SetActive(false);
    }
}