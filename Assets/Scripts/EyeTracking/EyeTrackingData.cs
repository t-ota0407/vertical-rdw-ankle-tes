using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTrackingData
{
    private static readonly List<string> CSV_COLUMNS = new() {
        "Timestamp",
        "LeftEyeOpenness", "RightEyeOpenness", "CombinedEyeOpenness",
        "LeftGazeDirection.x", "LeftGazeDirection.y", "LeftGazeDirection.z",
        "RightGazeDirection.x", "RightGazeDirection.y", "RightGazeDirection.z",
        "CombinedGazeDirection.x", "CombinedGazeDirection.y", "CombinedGazeDirection.z",
        "LeftGazeOrigin.x", "LeftGazeOrigin.y", "LeftGazeOrigin.z",
        "RightGazeOrigin.x", "RightGazeOrigin.y", "RightGazeOrigin.z",
        "CombinedGazeOrigin.x", "CombinedGazeOrigin.y", "CombinedGazeOrigin.z",
        "LeftPupilDiameter", "RightPupilDiameter", "CombinedPupilDiameter" };

    private readonly DateTime timestamp;

    private readonly float leftEyeOpenness;
    private readonly float rightEyeOpenness;
    private readonly float combinedEyeOpenness;

    private readonly Vector3 leftGazeDirection;
    private readonly Vector3 rightGazeDirection;
    private readonly Vector3 combinedGazeDirection;

    private readonly Vector3 leftGazeOrigin;
    private readonly Vector3 rightGazeOrigin;
    private readonly Vector3 combinedGazeOrigin;

    private readonly float leftPupilDiameter;
    private readonly float rightPupilDiameter;
    private readonly float combinedPupilDiameter;

    public EyeTrackingData(float leftEyeOpenness, float rightEyeOpenness, float combinedEyeOpenness,
        Vector3 leftGazeDirection, Vector3 rightGazeDirection, Vector3 combinedGazeDirection,
        Vector3 leftGazeOrigin, Vector3 rightGazeOrigin, Vector3 combinedGazeOrigin,
        float leftPupilDiameter, float rightPupilDiameter, float combinedPupilDiameter)
    {
        this.timestamp = DateTime.Now;
        this.leftEyeOpenness = leftEyeOpenness;
        this.rightEyeOpenness = rightEyeOpenness;
        this.combinedEyeOpenness = combinedEyeOpenness;
        this.leftGazeDirection = leftGazeDirection;
        this.rightGazeDirection = rightGazeDirection;
        this.combinedGazeDirection = combinedGazeDirection;
        this.leftGazeOrigin = leftGazeOrigin;
        this.rightGazeOrigin = rightGazeOrigin;
        this.combinedGazeOrigin = combinedGazeOrigin;
        this.leftPupilDiameter = leftPupilDiameter;
        this.rightPupilDiameter = rightPupilDiameter;
        this.combinedPupilDiameter = combinedPupilDiameter;
    }

    public static string CsvHeader()
    {
        return string.Join(",", CSV_COLUMNS);
    }

    public string ToCsvRowString()
    {
        List<string> rowData = new() {
            timestamp.ToString("yyyyMMddHHmmssfff"),
            leftEyeOpenness.ToString(), rightEyeOpenness.ToString(), combinedEyeOpenness.ToString(),
            leftGazeDirection.x.ToString(), leftGazeDirection.y.ToString(), leftGazeDirection.z.ToString(),
            rightGazeDirection.x.ToString(), rightGazeDirection.y.ToString(), rightGazeDirection.z.ToString(),
            combinedGazeDirection.x.ToString(), combinedGazeDirection.y.ToString(), combinedGazeDirection.z.ToString(),
            leftGazeOrigin.x.ToString(), leftGazeOrigin.y.ToString(), leftGazeOrigin.z.ToString(),
            rightGazeOrigin.x.ToString(), rightGazeOrigin.y.ToString(), rightGazeOrigin.z.ToString(),
            combinedGazeOrigin.x.ToString(), combinedGazeOrigin.y.ToString(), combinedGazeOrigin.z.ToString(),
            leftPupilDiameter.ToString(), rightPupilDiameter.ToString(), combinedPupilDiameter.ToString()};
        return string.Join(",", rowData);
    }
}
