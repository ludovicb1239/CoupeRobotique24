using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public enum Team
{
    blue,
    yellow
}
public struct gamePoints
{
    public short blue;
    public short yellow;
}
public class GameManager : MonoBehaviour
{
    public Instancier instancier;
    public Recorder recorder;
    public Volume volume;
    public TMP_Dropdown qualityDropdown;
    public Toggle visualEffectsToggle;

    public GameObject Camera;
    Vector3 defaultCamPos;

    [SerializeField] public Team team;

    public GameObject Robot;
    Vector3 defaultRobotPos;

    public List<SolarPanel> solarPanels;
    public List<Vector3> blueSquares;
    public List<Vector3> yellowSquares;
    public float squareRadius;
    public float plantRadius;
    public gamePoints points;
    [SerializeField] TextMeshProUGUI bluePointsText;
    [SerializeField] TextMeshProUGUI yellowPointsText;
    [SerializeField] TextMeshProUGUI timerText;
    public float gameTime = 0;

    public bool isTouchControls;
    public GameObject touchControls;
    public Joystick joystick1;
    public Joystick joystick2;


    // Start is called before the first frame update
    void Start()
    {
        defaultCamPos = Camera.transform.position;
        defaultRobotPos = Robot.transform.position;

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        visualEffectsToggle.isOn = true;


        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                Debug.Log("Running on Windows");
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                Debug.Log("Running on macOS");
                break;
            case RuntimePlatform.LinuxPlayer:
                Debug.Log("Running on Linux");
                break;
            case RuntimePlatform.Android:
                Debug.Log("Running on Android");
                isTouchControls = Input.touchSupported;
                break;
            case RuntimePlatform.IPhonePlayer:
                Debug.Log("Running on iOS");
                isTouchControls = Input.touchSupported;
                break;
            default:
                Debug.Log("Running on unknown platform");
                isTouchControls = Input.touchSupported;
                break;
        }
        Application.targetFrameRate = (int)Math.Ceiling(Screen.currentResolution.refreshRateRatio.value) + 1;
        print("trying to go to " + Application.targetFrameRate);

        touchControls.SetActive( isTouchControls );
    }

    // Update is called once per frame
    void Update()
    {
        points = CalculateGamePoints();
        bluePointsText.text = points.blue.ToString();
        yellowPointsText.text = points.yellow.ToString();
        gameTime += Time.deltaTime * Time.timeScale;
        timerText.text = FormatTime(gameTime);
    }
    public KeyStruct FromTouchControls()
    {
        KeyStruct keys = new();
        keys.Joystick1_X = joystick1.Horizontal;
        keys.Joystick1_Y = joystick1.Vertical;
        keys.Joystick2_X = joystick2.Horizontal;
        keys.Joystick2_Y = joystick2.Vertical;
        return keys;
    }
    static string FormatTime(float timeInSeconds)
    {
        // Convert time to TimeSpan
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);

        // Format the time
        string formattedTime = string.Format("{0:0}:{1:00}.{2:0}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds/100);

        return formattedTime;
    }

    public void ResetScene()
    {
        if (!recorder.isRec && !recorder.isPlay)
        {
            instancier.deleteObjects();
            instancier.setupScene();
            foreach(SolarPanel panel in solarPanels)
            {
                panel.Restart();
            }
            gameTime = 0;
        }
    }
    public void PublicSwitchTeam()
    {
        SwitchTeam(team == Team.blue ? Team.yellow : Team.blue);
    }
    void SwitchTeam(Team newTeam)
    {
        team = newTeam;
        float inv = (newTeam == Team.blue) ? 1 : -1;

        Vector3 camVec = defaultCamPos;
        camVec.z *= inv;
        Camera.transform.position = camVec;

        Vector3 robVec = defaultRobotPos;
        robVec.z *= inv;
        Robot.transform.position = robVec;

        Camera.GetComponent<CamFollower>().ResetRotation();
    }
    public gamePoints CalculateGamePoints()
    {
        gamePoints pts = new ()
        {
            blue = 0,
            yellow = 0
        };
        foreach(GameObject plant in instancier.Plants)
        {
            Vector3 forward = plant.transform.up;

            // Define the up vector
            Vector3 up = Vector3.up;

            float angle = Vector3.Angle(up, forward);

            if (angle < 20f)
            {
                foreach (Vector3 pos in blueSquares)
                {
                    Vector3 localPos = plant.transform.position - pos;
                    if (Mathf.Abs(localPos.x) < squareRadius + plantRadius && Mathf.Abs(localPos.z) < squareRadius + plantRadius)
                    {
                        pts.blue += 3;
                    }
                }
                foreach (Vector3 pos in yellowSquares)
                {
                    Vector3 localPos = plant.transform.position - pos;
                    if (Mathf.Abs(localPos.x) < squareRadius + plantRadius && Mathf.Abs(localPos.z) < squareRadius + plantRadius)
                    {
                        pts.yellow += 3;
                    }
                }
            }
        }
        foreach(SolarPanel panel in solarPanels)
        {
            switch(panel.state)
            {
                case SolarPanel.SolarPanelState.Yellow:
                    pts.yellow += 5;
                    break;
                case SolarPanel.SolarPanelState.Blue:
                    pts.blue += 5;
                    break;
                case SolarPanel.SolarPanelState.Both:
                    pts.blue += 5;
                    pts.yellow += 5;
                    break;
            }
        }

        return pts;
    }
    public void VisualEffectsChange(bool isOn)
    {
        volume.enabled = isOn;
    }
    public void QualityChange(int choix)
    {
        QualitySettings.SetQualityLevel(choix);
        Debug.Log("Quality level set to: " + QualitySettings.names[choix]);
    }
}
