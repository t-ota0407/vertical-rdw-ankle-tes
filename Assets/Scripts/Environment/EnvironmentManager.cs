using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private GameObject firstSlopePart;
    [SerializeField] private GameObject secondSlopePart;
    [SerializeField] private GameObject standPart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalibrateObjectsPosition(ExperimentType experimentType)
    {
        switch (experimentType)
        {
            case ExperimentType.ASCENDING_EVALUATION:
                firstSlopePart.transform.position = new Vector3(-1, 0, 1);
                firstSlopePart.transform.rotation = Quaternion.Euler(new(0, 180, 0));

                secondSlopePart.transform.position = new Vector3(1, 0, -1);
                secondSlopePart.transform.rotation = Quaternion.Euler(new(0, 0, 0));

                standPart.transform.position = new Vector3(0.25f, 0, 1.1f);

                break;
            case ExperimentType.DESCENDING_EVALUATION:
                firstSlopePart.transform.position = new Vector3(-1, 0, 1);
                firstSlopePart.transform.rotation = Quaternion.Euler(new(0, 180, 0));

                secondSlopePart.transform.position = new Vector3(1, 0, -1);
                secondSlopePart.transform.rotation = Quaternion.Euler(new(0, 0, 0));

                standPart.transform.position = new Vector3(0.25f, 0, 1.1f);
                break;
        }
    }

    public void SetFirstSlopePartActive(bool flag)
    {
        firstSlopePart.SetActive(flag);
    }

    public void SetSecondSlopePartActive(bool flag)
    {
        secondSlopePart.SetActive(flag);
    }

    public void SetStandPartActive(bool flag)
    {
        standPart.SetActive(flag);
    }

    public void SetFirstSlopeGradient(float eulerAngle)
    {
        Vector3 oldRotation = firstSlopePart.transform.rotation.eulerAngles;
        firstSlopePart.transform.rotation = Quaternion.Euler(-eulerAngle, oldRotation.y, oldRotation.z);
    }

    public void SetSecondSlopeGradient(float eulerAngle)
    {
        Vector3 oldRotation = secondSlopePart.transform.rotation.eulerAngles;
        secondSlopePart.transform.rotation = Quaternion.Euler(-eulerAngle, oldRotation.y, oldRotation.z);
    }
}
