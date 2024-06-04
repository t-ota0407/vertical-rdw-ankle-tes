using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCaseMethodTrial
{
    public GradientGain GradientGain { get { return gradientGain; } }
    private GradientGain gradientGain;

    public bool IsDescendingResponse { get { return isDescendingResponse; } }
    private bool isDescendingResponse;

    public StairCaseMethodTrial(GradientGain gradientGain, bool isDescendingResponse)
    {
        this.gradientGain = gradientGain;
        this.isDescendingResponse = isDescendingResponse;
    }
}
