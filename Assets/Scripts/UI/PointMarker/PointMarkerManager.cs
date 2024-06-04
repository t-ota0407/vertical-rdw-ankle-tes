using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMarkerManager : MonoBehaviour
{
    [SerializeField] private GameObject triangles;
    [SerializeField] private GameObject sphere;

    private const float SPHERE_HEIGHT = 1.1f;

    private int counter = 0;
    private float phase = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 trianglesRotation = triangles.transform.localEulerAngles;
        triangles.transform.localRotation = Quaternion.Euler(trianglesRotation.x, 0.5f * counter, trianglesRotation.z);

        sphere.transform.localPosition = new Vector3(0, SPHERE_HEIGHT + 0.04f * Mathf.Sin(phase * 0.9f), 0);
    }

    private void FixedUpdate()
    {
        counter++;
        phase += 0.03f;
    }
}
