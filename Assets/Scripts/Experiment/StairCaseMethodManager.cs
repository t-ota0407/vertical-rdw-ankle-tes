using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCaseMethodManager
{
    private readonly GradientGain MAXIMUM_GRADIENT_GAIN = new(0f);
    private readonly GradientGain MINIMUM_GRADIENT_GAIN = new(0.24f);
    private readonly List<GradientGain> DEFINED_UPDATE_STEP_SIZES = new List<GradientGain>() {
        new(0.04f), new(0.04f), new(0.02f), new(0.02f), new(0.02f), new(0.01f), new(0.01f), new(0.01f), new(0.01f), new(0.01f), new(0.01f)
    };

    private readonly int iterationNumber;

    private readonly List<StairCaseMethodTrial> ascendingSeries = new();
    private readonly List<StairCaseMethodTrial> descendingSeries = new();

    private bool isCurrentTrialAscent = true;

    public bool IsFinished { get { return isFinished; } }
    private bool isFinished = false;

    public int TrialNum { get { return trialNum; } }
    private int trialNum = 1;

    public StairCaseMethodManager(int iterationNumber)
    {
        this.iterationNumber = iterationNumber;
    }

    public string GetCurrentTrialsSeries()
    {
        return (isCurrentTrialAscent) ? "Ascending" : "Descending";
    }

    public GradientGain GetTrialsGradientGain()
    {
        var currentSeries = (isCurrentTrialAscent) ? ascendingSeries : descendingSeries;

        if (currentSeries.Count == 0)
            return (isCurrentTrialAscent) ? MINIMUM_GRADIENT_GAIN.Clone() : MAXIMUM_GRADIENT_GAIN.Clone();

        StairCaseMethodTrial lastTrial = currentSeries[currentSeries.Count - 1];
        float updateStepSize = DEFINED_UPDATE_STEP_SIZES[currentSeries.Count - 1].Value;

        if (lastTrial.IsDescendingResponse)
        {
            return ClampGradientGain(new(lastTrial.GradientGain.Value - updateStepSize));
        }
        else
        {
            return ClampGradientGain(new(lastTrial.GradientGain.Value + updateStepSize));
        }
    }

    public void Next(bool isDescendingResponse)
    {
        GradientGain currentGradientGain = GetTrialsGradientGain();
        StairCaseMethodTrial stairCaseMethodTrial = new(currentGradientGain, isDescendingResponse);
        if (isCurrentTrialAscent)
        {
            ascendingSeries.Add(stairCaseMethodTrial);
        }
        else
        {
            descendingSeries.Add(stairCaseMethodTrial);
        }

        if (ascendingSeries.Count == iterationNumber && descendingSeries.Count == iterationNumber)
        {
            isFinished = true;
        }

        isCurrentTrialAscent = (isCurrentTrialAscent) ? false : true;

        trialNum++;
    }

    private GradientGain ClampGradientGain(GradientGain gradientGain)
    {
        if (gradientGain.Value < MINIMUM_GRADIENT_GAIN.Value)
            return MINIMUM_GRADIENT_GAIN.Clone();

        if (gradientGain.Value > MAXIMUM_GRADIENT_GAIN.Value)
            return MAXIMUM_GRADIENT_GAIN.Clone();

        return gradientGain;
    }
}
