using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posture
{
    public Vector3 Position { get { return position; } }
    private Vector3 position;

    public Vector3 Rotation { get { return rotation; } }
    private Vector3 rotation;

    public Posture(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public static Posture Zero
    {
        get
        {
            return new Posture(Vector3.zero, Vector3.zero);
        }
    }
}
