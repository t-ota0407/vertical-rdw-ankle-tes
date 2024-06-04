using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeTrackingManager : MonoBehaviour
{
    private static EyeData_v2 eyeData;
    private static EyeParameter parameter;

    public EyeTrackingData EyeTrackingData { get { return eyeTrackingData; }}
    private EyeTrackingData eyeTrackingData;

    // Start is called before the first frame update
    void Start()
    {
        SRanipal_Eye_API.GetEyeParameter(ref parameter);

    }

    // Update is called once per frame
    void Update()
    {
        SRanipal_Eye_API.GetEyeData_v2(ref eyeData);

        float leftEyeOpenness = eyeData.verbose_data.left.eye_openness;
        float rightEyeOpenness = eyeData.verbose_data.right.eye_openness;
        float combinedEyeOpenness = eyeData.verbose_data.combined.eye_data.eye_openness;

        Vector3 leftGazeDirection = eyeData.verbose_data.left.gaze_direction_normalized;
        Vector3 rightGazeDirection = eyeData.verbose_data.right.gaze_direction_normalized;
        Vector3 combinedGazeDirection = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;

        Vector3 leftGazeOrigin = eyeData.verbose_data.left.gaze_origin_mm;
        Vector3 rightGazeOrigin = eyeData.verbose_data.right.gaze_origin_mm;
        Vector3 combinedGazeOrigin = eyeData.verbose_data.combined.eye_data.gaze_origin_mm;

        float leftPupilDiameter = eyeData.verbose_data.left.pupil_diameter_mm;
        float rightPupilDiameter = eyeData.verbose_data.right.pupil_diameter_mm;
        float combinedPupilDiameter = eyeData.verbose_data.combined.eye_data.pupil_diameter_mm;

        eyeTrackingData = new(leftEyeOpenness, rightEyeOpenness, combinedEyeOpenness,
            leftGazeDirection, rightGazeDirection, combinedGazeDirection,
            leftGazeOrigin, rightGazeOrigin, combinedGazeOrigin,
            leftPupilDiameter, rightPupilDiameter, combinedPupilDiameter);
    }

    public void LogEyeTrackingDataLog(DataLogger eyeTrackingDataLogger)
    {
        if (eyeTrackingData is not null)
        {
            eyeTrackingDataLogger.AppendLine(eyeTrackingData.ToCsvRowString());
        }
    }
}
