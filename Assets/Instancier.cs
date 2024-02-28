using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class Instancier : MonoBehaviour
{
    public GameObject PotPrefab;
    public List<Transform> PotPosList;
    public List<GameObject> Pots = new List<GameObject>();

    public GameObject PlantPrefab;
    public List<Transform> PlantPosList;
    public float distanceFromCenter = 0.6f;
    public float subRadius = 0.12f;
    public List<GameObject> Plants = new List<GameObject>();
    //[SerializeField] Material PlantDefaultMat;
    [SerializeField] Material PlantReinfMat;

    public Recorder rec;
    // Start is called before the first frame update
    void Start()
    {
        setupScene();
    }
    public void setupScene()
    {
        foreach (Transform t in PotPosList)
        {
            GameObject go1 = Instantiate(PotPrefab, t.position, Quaternion.Euler(0, 0, 0), this.transform);

            Vector3 mirrorPos = t.position;
            mirrorPos.z *= -1;
            GameObject go2 = Instantiate(PotPrefab, mirrorPos, Quaternion.Euler(0, 0, 0), this.transform);

            Pots.Add(go1);
            Pots.Add(go2);
        }
        Material[] materialsCopy = PlantPrefab.GetComponent<MeshRenderer>().sharedMaterials;
        materialsCopy[1] = PlantReinfMat;

        foreach (Transform t in PlantPosList)
        {
            List<Vector3> subverts = GeneratePolygonVertices(0.07f, subRadius, 5).ToList();
            subverts.Add(GeneratePolygonVertices(-.02f, 0.02f, 1)[0]);
            List<int> randomNumbers = GenerateRandomNumbers(2, 0, 6);
            int i = 0;
            foreach (Vector3 subvert in subverts)
            {
                GameObject go = Instantiate(PlantPrefab, t.position + subvert, Quaternion.Euler(0, UnityEngine.Random.value * 360f, 0), this.transform);
                Plants.Add(go);
                if (randomNumbers.Contains(i))
                    go.GetComponent<MeshRenderer>().materials = materialsCopy;
                i++;
            }
        }

        if (rec != null)
            AddTransToRec();
    }
    static List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
    {
        List<int> randomNumbers = new();
        Random random = new();

        // Generate unique random numbers
        while (randomNumbers.Count < count)
        {
            int randomNumber = random.Next(minValue, maxValue);
            // Check if the generated number is not already in the list
            if (!randomNumbers.Contains(randomNumber))
                randomNumbers.Add(randomNumber);
        }
        return randomNumbers;
    }
    void AddTransToRec()
    {
        foreach(GameObject go in Pots)
            rec.transforms.Add(go.transform);
        foreach (GameObject go in Plants)
            rec.transforms.Add(go.transform);
    }
    void RemoveTransToRec()
    {
        foreach (GameObject go in Pots)
            rec.transforms.Remove(go.transform);
        foreach (GameObject go in Plants)
            rec.transforms.Remove(go.transform);
    }

    // Update is called once per frame
    void Update()
    {
    }
    Vector3[] GeneratePolygonVertices(float radius, int points)
    {
        Vector3[] vertices = new Vector3[points];

        for (int i = 0; i < points; i++)
        {
            float angle = 2 * Mathf.PI / points * i;
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            vertices[i] = new Vector3(x, 0f, z);
        }

        return vertices;
    }
    Vector3[] GeneratePolygonVertices(float minRadius, float maxRadius, int points)
    {
        Vector3[] vertices = new Vector3[points];

        for (int i = 0; i < points; i++)
        {
            float angle = 2 * Mathf.PI / points * i;
            float rad = UnityEngine.Random.Range(minRadius, maxRadius);
            float x = rad * Mathf.Cos(angle);
            float z = rad * Mathf.Sin(angle);

            vertices[i] = new Vector3(x, 0f, z);
        }

        return vertices;
    }
    public void deleteObjects()
    {
        RemoveTransToRec();

        foreach (GameObject obj in Pots)
            Destroy(obj);
        Pots.Clear();

        foreach (GameObject obj in Plants)
            Destroy(obj);
        Plants.Clear();

    }
}
