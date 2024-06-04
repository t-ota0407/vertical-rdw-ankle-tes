using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientGain
{
    private readonly float gainValue;

    public GradientGain(float gainValue)
    {
        this.gainValue = gainValue;
    }

    public float Value
    {
        get
        {
            return gainValue;
        }
    }

    public float EulerAngle
    {
        get
        {
            return Mathf.Atan(gainValue) * Mathf.Rad2Deg;
        }
    }

    public GradientGain Clone()
    {
        return new(this.Value);
    }
}
