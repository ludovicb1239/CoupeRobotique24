using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Recorder : MonoBehaviour
{
    List<RecordingObject> objects;
    [SerializeField] int time;
    [SerializeField] int lenght;
    public bool isRec;
    public bool isPlay;

    public List<Transform> transforms;

    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] string statusPrefix = "Status Enr. : ";
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        lenght = 0;
        isRec = false;
        isPlay = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isRec)
            {
                isRec = false;
                statusText.SetText(statusPrefix + "Aucun");
                lenght = time;
            }
            else if (isPlay)
            {
                isPlay = false;
                time = 0;

                foreach (Transform tr in transforms)
                    tr.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                statusText.SetText(statusPrefix + "Aucun");
            }
            else if (time == 0)
            {
                isRec = true;

                objects = new();
                foreach (Transform tr in transforms)
                {
                    RecordingObject newRec = new();
                    objects.Add(newRec);
                    newRec.Instantiate(tr);
                    tr.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                }

                statusText.SetText(statusPrefix + "Enregistrement");
            }
            else
            {
                isPlay = true;
                time = 0;
                foreach (Transform tr in transforms)
                    tr.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                statusText.SetText(statusPrefix + "Lecture");
            }
        }
    }

    private void FixedUpdate()
    {
        if (isRec)
        {
            time++;
            foreach (RecordingObject obj in objects)
            {
                obj.SaveData(time);
            }
        }
        else if (isPlay)
        {
            foreach (RecordingObject obj in objects)
            {
                obj.ReplayData(time);
            }
            time++;
            if (lenght == time)
            {
                time = 0;
            }
        }
    }
    public void timeChanged(float t)
    {
        Time.timeScale = t;
    }
}
public class RecordingObject
{
    public Transform transform;

    public List<Vector3> pos;
    public List<Quaternion> rot;
    public List<int> timeSteps;
    public void Instantiate(Transform tr)
    {
        transform = tr;
        pos = new();
        rot = new();
        timeSteps = new();

        pos.Add(transform.position);
        rot.Add(transform.rotation);
        timeSteps.Add(0);
    }
    public void SaveData(int time)
    {
        if (pos[pos.Count-1] != transform.position || rot[rot.Count - 1] != transform.rotation)
        {
            pos.Add(transform.position);
            rot.Add(transform.rotation);
            timeSteps.Add(time);
        }
    }
    public void ReplayData(int time)
    {
        if (timeSteps.Count == 0) return;

        int id = 0;

        if (timeSteps.Count != 1)
            id = timeSteps.BinarySearch(time);

        if (id >= 0 && id < timeSteps.Count)
        {
            transform.position = pos[id];
            transform.rotation = rot[id];
        }

    }
}