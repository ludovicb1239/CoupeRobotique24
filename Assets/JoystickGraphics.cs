using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class JoystickGraphics : MonoBehaviour
{
    public Image image0;
    public Image image1;
    public Image image2;
    public Image image3;
    Color baseColor;
    Joystick joystick;
    // Start is called before the first frame update
    void Start()
    {
        joystick = GetComponent<Joystick>();
        baseColor = image0.color;
        baseColor.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float angleDeg = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
        if (angleDeg < 0)
            angleDeg += 360;
        int zone = Mathf.FloorToInt(angleDeg / 90);
        float magnitude = new Vector2(joystick.Horizontal, joystick.Vertical).sqrMagnitude;

        Color tmpColor = baseColor;
        tmpColor.a = magnitude;
        image0.color = zone == 0 ? tmpColor : baseColor;
        image1.color = zone == 1 ? tmpColor : baseColor;
        image2.color = zone == 2 ? tmpColor : baseColor;
        image3.color = zone == 3 ? tmpColor : baseColor;
    }
}
