using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollower : MonoBehaviour
{
    public Transform toFollow;
    public float heightOffset;
    Camera cam;
    Vector3 followPos;
    public float lookSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        ResetRotation();
    }

    // Update is called once per frame
    void Update()
    {
        followPos = toFollow.position;
        followPos.y += heightOffset;

        Vector3 direction = followPos - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * lookSpeed * Time.timeScale);

        float dist = Vector3.Distance(transform.position, followPos);
        // 70 - 30
        cam.fieldOfView = 80f - dist*16f;
    }
    public void ResetRotation()
    {
        followPos = toFollow.position;
        followPos.y += heightOffset;

        Vector3 direction = followPos - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
