using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TrackingManager : MonoBehaviour
{
    private bool useConstantTrackingData = true;

    private static readonly Dictionary<SteamVR_Input_Sources, TrackerType> INPUT_SOURCE_TO_TRACKER_TYPE = new()
    {
        { SteamVR_Input_Sources.Camera, TrackerType.PLATFORM_H },
        { SteamVR_Input_Sources.Keyboard, TrackerType.PLATFORM_C },
        { SteamVR_Input_Sources.LeftFoot, TrackerType.PLATFORM_A },
        { SteamVR_Input_Sources.RightFoot, TrackerType.PLATFORM_B },
        { SteamVR_Input_Sources.LeftShoulder, TrackerType.PLATFORM_D },
        { SteamVR_Input_Sources.RightShoulder, TrackerType.PLATFORM_E },
        { SteamVR_Input_Sources.Waist, TrackerType.PLATFORM_F },
        { SteamVR_Input_Sources.Chest, TrackerType.PLATFORM_G },
    };

    private Dictionary<TrackerType, Posture> trackingData = new Dictionary<TrackerType, Posture>();

    private readonly Dictionary<TrackerType, Posture> constantTrackingData = new()
    {
        { TrackerType.PLATFORM_A, new Posture(new Vector3(-0.55f, 0.05f, 1.55f), Vector3.zero) },
        { TrackerType.PLATFORM_B, new Posture(new Vector3(-1.30f, 0.05f, 1.55f), Vector3.zero) },
        { TrackerType.PLATFORM_C, new Posture(new Vector3(0.5f, 0.1f, -1.1f), Vector3.zero) },
        { TrackerType.PLATFORM_D, new Posture(new Vector3(1.4f, 0.1f, -1.1f), Vector3.zero) },
        { TrackerType.PLATFORM_E, new Posture(new Vector3(0.5f, 0.4f, 1.6f), Vector3.zero) },
        { TrackerType.PLATFORM_F, new Posture(new Vector3(1.4f, 0.4f, 1.6f), Vector3.zero) },
        { TrackerType.PLATFORM_G, new Posture(new Vector3(0.1f, 0.1f, 1.0f), Vector3.zero) },
        { TrackerType.PLATFORM_H, new Posture(new Vector3(0.1f, 0.1f, 1.5f), Vector3.zero) },
    };

    private SteamVR_Action_Pose steamvrActionPose = SteamVR_Actions.default_Pose;

    void Awake()
    {
        foreach (TrackerType trackerType in INPUT_SOURCE_TO_TRACKER_TYPE.Values)
        {
            trackingData.Add(trackerType, Posture.Zero);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (SteamVR_Input_Sources inputSource in INPUT_SOURCE_TO_TRACKER_TYPE.Keys)
        {
            Vector3 position = steamvrActionPose.GetLocalPosition(inputSource);
            Vector3 rotation = steamvrActionPose.GetLocalRotation(inputSource).eulerAngles;

            if (!IsZeroVector(position) && !IsZeroVector(rotation))
            {
                TrackerType trackerType = INPUT_SOURCE_TO_TRACKER_TYPE[inputSource];
                Posture posture = new Posture(position, rotation);
                trackingData[trackerType] = posture;
            }
        }
    }

    public Posture GetPosture(TrackerType trackerType)
    {
        return (useConstantTrackingData) ? constantTrackingData[trackerType] : trackingData[trackerType];
    }

    private bool IsZeroVector(Vector3 vector)
    {
        float epsilon = 0.0001f;
        return (-epsilon < vector.x && vector.x < epsilon) && (-epsilon < vector.y && vector.y < epsilon) && (-epsilon < vector.z && vector.z < epsilon);
    }
}
