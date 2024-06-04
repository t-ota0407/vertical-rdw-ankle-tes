using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientRedirectionManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playersHead;

    public GradientGain GradientGain { set { gradientGain = value; } }
    private GradientGain gradientGain = new(0);

    public Vector3 WalkingDirection { set { walkingDirection = value; } }
    private Vector3 walkingDirection = Vector3.zero;

    public Vector3 RedirectionOrigin { set { redirectionOrigin = value; } }
    private Vector3 redirectionOrigin = Vector3.zero;

    public bool IsRedirectionEnabled { set { isRedirectionEnabled = value; } }
    private bool isRedirectionEnabled = false;

    // Update is called once per frame
    void Update()
    {
        if (isRedirectionEnabled)
        {
            float horizontalDisplacement = CalculateHorizontalDisplacement();

            float verticalDisplacement = gradientGain.Value * horizontalDisplacement;

            Vector3 redirectedPosition = new(player.transform.position.x, verticalDisplacement, player.transform.position.z);
            player.transform.position = redirectedPosition;
        }
        else
        {
            Vector3 redirectedPosition = new(player.transform.position.x, 0, player.transform.position.z);
            player.transform.position = redirectedPosition;
        }
    }

    private float CalculateHorizontalDisplacement()
    {
        Vector3 difference = playersHead.transform.position - redirectionOrigin;
        float distanceInDirection = Vector3.Dot(difference, walkingDirection.normalized);
        return Mathf.Abs(distanceInDirection);
    }
}
