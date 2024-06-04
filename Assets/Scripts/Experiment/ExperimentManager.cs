using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [Header("実験中に変更する可能性のある項目")]
    [SerializeField] private List<TESConditions> tesConditions;
    [SerializeField] private ExperimentType experimentType;
    [SerializeField] private int participantID;
    [SerializeField] private bool isCalibrating;

    [Space(20)]
    [Header("実験中に原則変更しない項目")]
    [SerializeField] private int iterationNumber;
    [SerializeField] private bool isManualControllEnabled;

    [Space(20)]
    [Header("リファレンス")]
    [SerializeField] private TESManager tesManager;
    [SerializeField] private TESCalibration tesCalibration;
    [SerializeField] private GradientRedirectionManager gradientRedirectionManager;
    [SerializeField] private EyeTrackingManager eyeTrackingManager;
    [SerializeField] private GameObject playersHead;
    [SerializeField] private TrackingManager trackingManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private EnvironmentManager environmentManager;
    [SerializeField] private GameObject pointMarker;
    [SerializeField] private FadeManager fadeManager;

    private AnsweringQuestionnaireController answeringQuestionnaireController;

    private StairCaseMethodManager stairCaseMethodManager;

    private ExperimentalStatus currentExperimentalStatus;

    private DataLogger questionnaireDataLogger;
    private DataLogger eyeTrackingDataLogger;

    private readonly Vector3 ASC_FIRST_SLOPE_ORIGIN = new Vector3(-1, 0, 1);
    private readonly Vector3 ASC_SECOND_SLOPE_ORIGIN = new Vector3(1, 0, -1);

    private readonly Vector3 DES_FIRST_SLOPE_ORIGIN = new Vector3(-1, 0, 1);
    private readonly Vector3 DES_SECOND_SLOPE_ORIGIN = new Vector3(1, 0, -1);

    private readonly Vector3 STAND_PART_CENTER = new Vector3(0.25f, 0, 1);

    private int currentTESConditionsIndex = 0;

    private DateTime restingStartedAt = new();

    // Start is called before the first frame update
    void Start()
    {
        currentExperimentalStatus = (experimentType == ExperimentType.ASCENDING_EVALUATION)
            ? ExperimentalStatus.ASC_ENTERING_IN
            : ExperimentalStatus.DES_ENTERING_IN;

        stairCaseMethodManager = new(iterationNumber);

        string directoryNameParticipantID = $"Participant{participantID}";
        string directoryNameExperimentType = ExperimentTypeConverter.ToString(experimentType);

        questionnaireDataLogger = new($"questinnaire_result_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv", directoryNameParticipantID, directoryNameExperimentType);
        questionnaireDataLogger.AppendLine("Timestamp,TrialNum,AnkleTES,Series,GradientGain,Question1,Question2,Question3"); 
        
        eyeTrackingDataLogger = new($"eye_tracking_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv", directoryNameParticipantID, directoryNameExperimentType);
        eyeTrackingDataLogger.AppendLine(EyeTrackingData.CsvHeader());

        tesCalibration.InitializeTESCalibrationLogger(directoryNameParticipantID, directoryNameExperimentType);

        uiManager.ActivateInstructionPanel();
        uiManager.UpdateInstructionUI(currentExperimentalStatus);
        answeringQuestionnaireController = new(uiManager, questionnaireDataLogger, stairCaseMethodManager);

        environmentManager.CalibrateObjectsPosition(experimentType);
        switch (currentExperimentalStatus)
        {
            case ExperimentalStatus.ASC_ENTERING_IN: pointMarker.transform.localPosition = ASC_FIRST_SLOPE_ORIGIN + new Vector3(0, 0, 0.2f); break;
            case ExperimentalStatus.DES_ENTERING_IN: pointMarker.transform.localPosition = DES_FIRST_SLOPE_ORIGIN + new Vector3(0, 0.1f, -2.2f * 0.99f); break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        tesCalibration.IsCalibrationFromKeyCode = isCalibrating;
        
        switch (currentExperimentalStatus)
        {
            case ExperimentalStatus.ASC_ENTERING_IN: CheckEndOfASCEnteringIn(); break;
            case ExperimentalStatus.ASC_TURNING_TO_FLAT_PATH: CheckEndOfASCTurningToFlatPath(); break;
            case ExperimentalStatus.ASC_WALKING_FLATLAND: CheckEndOfASCWalkingFlatland(); break;
            case ExperimentalStatus.ASC_FADING: CheckEndOfASCFading(); break;
            case ExperimentalStatus.ASC_GETTING_ON: CheckEndOfASCGettingOn(); break;
            case ExperimentalStatus.ASC_TURNING_TO_SLOPED_PATH: CheckEndOfASCTurningToSlopedPath(); break;
            case ExperimentalStatus.ASC_WALKING_SLOPE: CheckEndOfASCWalkingSlope(); break;
            case ExperimentalStatus.ASC_ANSERING_QUESTIONNAIRE: CheckEndOfASCAnsweringQuestionnaire(); break;
            case ExperimentalStatus.ASC_GETTING_OFF: CheckEndOfASCGettingOff(); break;
            case ExperimentalStatus.ASC_RESTING: CheckEndOfASCResting(); break;
            case ExperimentalStatus.DES_ENTERING_IN: CheckEndOfDESEnteringIn(); break;
            case ExperimentalStatus.DES_TURNING_TO_FLAT_PATH: CheckEndOfDESTurningToFlatPath(); break;
            case ExperimentalStatus.DES_FADING: CheckENDOfDESFading(); break;
            case ExperimentalStatus.DES_WALKING_FLATLAND: CheckEndOfDESWalkingFlatland(); break;
            case ExperimentalStatus.DES_GETTING_ON: CheckEndOfDESGettingOn(); break;
            case ExperimentalStatus.DES_TURNING_TO_SLOPED_PATH: CheckEndOfDESTurningToSlopedPath(); break;
            case ExperimentalStatus.DES_WALKING_SLOPE: CheckEndOfDESWalkingSlope(); break;
            case ExperimentalStatus.DES_ANSERING_QUESTIONNAIRE: CheckEndOfDESAnsweringQuestionnaire(); break;
            case ExperimentalStatus.DES_GETTING_OFF: CheckEndOfDESGettingOff(); break;
            case ExperimentalStatus.DES_RESTING: CheckEndOfDESResting(); break;
            case ExperimentalStatus.FINISHED: break;
        }
    }

    void FixedUpdate()
    {
        eyeTrackingManager.LogEyeTrackingDataLog(eyeTrackingDataLogger);
    }

    void OnDestroy()
    {
        questionnaireDataLogger.Flush();
        eyeTrackingDataLogger.Flush();
    }

    private void CheckEndOfASCEnteringIn()
    {
        Vector3 targetPosition = ASC_FIRST_SLOPE_ORIGIN + new Vector3(0, 0, 0.3f);
        float thresholdDistance = 0.2f;
        bool isNotZero = IsNotZero(playersHead.transform.position);
        bool isNear = IsNearInHorizontalPlane(targetPosition, playersHead.transform.position, thresholdDistance);

        bool isConditionSatisfied = isNotZero && isNear;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha1))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_TURNING_TO_FLAT_PATH;

            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            pointMarker.SetActive(false);
        }
    }

    private void CheckEndOfASCTurningToFlatPath()
    {
        Vector3 targetDirection = new Vector3(0, 0, -1);

        float thresholdAngle = 15f;
        bool isSameDirection = IsSameDirection(playersHead.transform.forward, targetDirection, thresholdAngle);

        bool isConditionSatisfied = isSameDirection;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha2))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_WALKING_FLATLAND;
            GradientGain gradientGain = stairCaseMethodManager.GetTrialsGradientGain();
            gradientRedirectionManager.GradientGain = gradientGain;
            gradientRedirectionManager.RedirectionOrigin = ASC_FIRST_SLOPE_ORIGIN;
            gradientRedirectionManager.WalkingDirection = targetDirection;
            gradientRedirectionManager.IsRedirectionEnabled = true;

            environmentManager.SetFirstSlopePartActive(true);
            environmentManager.SetSecondSlopePartActive(false);
            environmentManager.SetStandPartActive(false);
            environmentManager.SetFirstSlopeGradient(gradientGain.EulerAngle);

            uiManager.DeactivateAllPanels();

            tesManager.StartElectricalStimulation(tesConditions[currentTESConditionsIndex]);
        }
    }

    private void CheckEndOfASCWalkingFlatland()
    {
        float walkingStartPositionZ = ASC_FIRST_SLOPE_ORIGIN.z;
        float walkingDistance = 2.2f;
        float rangeLength = 0.4f;
        bool isWithinRange = IsWithinRange(playersHead.transform.position.z, walkingStartPositionZ - walkingDistance - rangeLength, walkingStartPositionZ - walkingDistance);

        bool isConditionSatisfied = isWithinRange;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha3))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_FADING;

            fadeManager.StartFadeOut();

            tesManager.StopElectricalStimulation();
        }
    }

    private void CheckEndOfASCFading()
    {
        bool isConditionSatisfied = (fadeManager.FadeStatus == FadeStatus.FADED_OUT);

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_GETTING_ON;

            gradientRedirectionManager.IsRedirectionEnabled = false;

            GradientGain physicalSlopeGradientGain = new(0.1f);
            environmentManager.SetFirstSlopePartActive(false);
            environmentManager.SetSecondSlopePartActive(true);
            environmentManager.SetStandPartActive(true);
            environmentManager.SetSecondSlopeGradient(physicalSlopeGradientGain.EulerAngle);

            pointMarker.SetActive(true);
            pointMarker.transform.localPosition = ASC_SECOND_SLOPE_ORIGIN + new Vector3(0, 0.1f, 0);

            uiManager.ActivateInstructionPanel();
            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            fadeManager.StartFadeIn();
        }
    }

    private void CheckEndOfASCGettingOn()
    {
        float thresholdDistance = 0.35f;
        bool isNear = IsNearInHorizontalPlane(playersHead.transform.position, ASC_SECOND_SLOPE_ORIGIN, thresholdDistance);

        bool isConditionSatisfied = isNear;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha4))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_TURNING_TO_SLOPED_PATH;
            
            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            pointMarker.SetActive(false);
        }
    }

    private void CheckEndOfASCTurningToSlopedPath()
    {
        Vector3 targetDirection = new Vector3(0, 0.05f, 1);

        float thresholdAngle = 5f;
        bool isSameDirection = IsSameDirection(playersHead.transform.forward, targetDirection, thresholdAngle);

        bool isConditionSatisfied = isSameDirection;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha5))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_WALKING_SLOPE;

            uiManager.DeactivateAllPanels();
        }
    }

    private void CheckEndOfASCWalkingSlope()
    {
        float walkingFinishedPositionZ = ASC_SECOND_SLOPE_ORIGIN.z + 2.2f * 0.995f;
        float rangeLength = 0.4f;
        bool isWithinRange = IsWithinRange(playersHead.transform.position.z, walkingFinishedPositionZ - rangeLength/2, walkingFinishedPositionZ + rangeLength/2);

        bool isConditionSatisfied = isWithinRange;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha5))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_ANSERING_QUESTIONNAIRE;

            gradientRedirectionManager.IsRedirectionEnabled = false;
            
            uiManager.ActivateQuestionnairePanel();
        }
    }

    private void CheckEndOfASCAnsweringQuestionnaire()
    {
        var questionnaireResult = answeringQuestionnaireController.CheckUIControl(stairCaseMethodManager.TrialNum, TESConditionsConverter.ToString(tesConditions[currentTESConditionsIndex]));

        bool isConditionSatisfied = false;
        if (questionnaireResult.TryGetValue(QuestionType.QUESTION_3, out string question3Answer))
        {
            isConditionSatisfied = question3Answer != "";
        }

        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha6))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_GETTING_OFF;

            answeringQuestionnaireController.InitializeQuestionnaireResult();

            stairCaseMethodManager.Next(questionnaireResult[QuestionType.QUESTION_1] == "SecondHalf");
            
            uiManager.ActivateInstructionPanel();
            uiManager.UpdateInstructionUI(currentExperimentalStatus);
        }
    }

    private void CheckEndOfASCGettingOff()
    {
        Vector3 standPartLeftEdge1 = STAND_PART_CENTER + new Vector3(-0.3f, 0, -0.3f);
        Vector3 standPartLeftEdge2 = STAND_PART_CENTER + new Vector3(-0.3f, 0, 0.3f);

        bool isRightSide = !IsLeftSide(playersHead.transform.position, standPartLeftEdge1, standPartLeftEdge2);

        bool isConditionSatisfied = isRightSide;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha7))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            if (stairCaseMethodManager.IsFinished)
            {
                if (currentTESConditionsIndex == tesConditions.Count - 1)
                {
                    currentExperimentalStatus = ExperimentalStatus.FINISHED;
                    uiManager.UpdateInstructionUI(currentExperimentalStatus);
                }
                else
                {
                    currentExperimentalStatus = ExperimentalStatus.ASC_RESTING;
                    uiManager.UpdateInstructionUI(currentExperimentalStatus);

                    currentTESConditionsIndex++;
                    restingStartedAt = DateTime.Now;
                    stairCaseMethodManager = new(iterationNumber);
                }
            }
            else
            {
                currentExperimentalStatus = ExperimentalStatus.ASC_ENTERING_IN;
                uiManager.UpdateInstructionUI(currentExperimentalStatus);

                environmentManager.SetFirstSlopePartActive(true);
                environmentManager.SetSecondSlopePartActive(false);
                environmentManager.SetStandPartActive(false);

                uiManager.UpdateInstructionUI(currentExperimentalStatus);

                pointMarker.SetActive(true);
                pointMarker.transform.localPosition = ASC_FIRST_SLOPE_ORIGIN + new Vector3(0, 0, 0.3f);
            }
        }
    }

    private void CheckEndOfASCResting()
    {
        if ((DateTime.Now - restingStartedAt).TotalMilliseconds > 60 * 1000)
        {
            currentExperimentalStatus = ExperimentalStatus.ASC_ENTERING_IN;
            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            environmentManager.SetFirstSlopePartActive(true);
            environmentManager.SetSecondSlopePartActive(false);
            environmentManager.SetStandPartActive(false);

            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            pointMarker.SetActive(true);
            pointMarker.transform.localPosition = ASC_FIRST_SLOPE_ORIGIN + new Vector3(0, 0, 0.3f);
        }
    }

    private void CheckEndOfDESEnteringIn()
    {
        Vector3 targetPosition = DES_FIRST_SLOPE_ORIGIN + new Vector3(0, 0.1f, -2.2f * 0.99f);

        float thresholdDistance = 0.3f;
        bool isNear = IsNearInHorizontalPlane(playersHead.transform.position, targetPosition, thresholdDistance);

        bool isConditionSatisfied = isNear;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha1))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_TURNING_TO_FLAT_PATH;

            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            pointMarker.SetActive(false);
        }
    }

    private void CheckEndOfDESTurningToFlatPath()
    {
        Vector3 targetDirection = new Vector3(0, 0, 1);

        float thresholdAngle = 5f;
        bool isSameDirection = IsSameDirection(playersHead.transform.forward, targetDirection, thresholdAngle);

        bool isConditionSatisfied = isSameDirection;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha2))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_FADING;

            fadeManager.StartFadeOut();
        }
    }

    private void CheckENDOfDESFading()
    {
        bool isConditionSatisfied = (fadeManager.FadeStatus == FadeStatus.FADED_OUT);

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_WALKING_FLATLAND;

            Vector3 targetDirection = new Vector3(0, 0, 1);
            GradientGain gradientGain = stairCaseMethodManager.GetTrialsGradientGain();
            gradientRedirectionManager.GradientGain = gradientGain;
            gradientRedirectionManager.RedirectionOrigin = DES_FIRST_SLOPE_ORIGIN;
            gradientRedirectionManager.WalkingDirection = targetDirection;
            gradientRedirectionManager.IsRedirectionEnabled = true;

            environmentManager.SetFirstSlopePartActive(true);
            environmentManager.SetSecondSlopePartActive(false);
            environmentManager.SetStandPartActive(false);
            environmentManager.SetFirstSlopeGradient(gradientGain.EulerAngle);

            uiManager.DeactivateAllPanels();

            fadeManager.StartFadeIn();

            tesManager.StartElectricalStimulation(tesConditions[currentTESConditionsIndex]);
        }
    }

    private void CheckEndOfDESWalkingFlatland()
    {
        float targetPositionZ = DES_FIRST_SLOPE_ORIGIN.z;

        float rangeLength = 0.4f;
        bool isWithinRange = IsWithinRange(playersHead.transform.position.z, targetPositionZ - rangeLength/2, targetPositionZ + rangeLength/2);

        bool isConditionSatisfied = isWithinRange;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha3))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_GETTING_ON;

            gradientRedirectionManager.IsRedirectionEnabled = false;

            uiManager.ActivateInstructionPanel();
            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            GradientGain physicalSlopeGradientGain = new(0.1f);
            environmentManager.SetFirstSlopePartActive(false);
            environmentManager.SetSecondSlopePartActive(true);
            environmentManager.SetStandPartActive(true);
            environmentManager.SetSecondSlopeGradient(physicalSlopeGradientGain.EulerAngle);

            pointMarker.SetActive(true);
            pointMarker.transform.position = DES_SECOND_SLOPE_ORIGIN + new Vector3(0, 2.2f * 0.10f, 2.2f * 0.99f);

            tesManager.StopElectricalStimulation();
        }
    }

    private void CheckEndOfDESGettingOn()
    {
        Vector3 targetPosition = DES_SECOND_SLOPE_ORIGIN + new Vector3(0, 2.2f * 0.10f, 2.2f * 0.99f);
        float thresholdDistance = 0.3f;
        bool isNear = IsNearInHorizontalPlane(playersHead.transform.position, targetPosition, thresholdDistance);
        bool isConditionSatisfied = isNear;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha4))
            isConditionSatisfied = true;
        
        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_TURNING_TO_SLOPED_PATH;

            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            pointMarker.SetActive(false);
        }
    }

    private void CheckEndOfDESTurningToSlopedPath()
    {

        Vector3 targetDirection = new Vector3(0, -0.1f, -1);

        float thresholdAngle = 5f;
        bool isSameDirection = IsSameDirection(playersHead.transform.forward, targetDirection, thresholdAngle);

        bool isConditionSatisfied = isSameDirection;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha5))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_WALKING_SLOPE;

            uiManager.DeactivateAllPanels();
        }
    }

    private void CheckEndOfDESWalkingSlope()
    {
        Vector3 trackerCPosition = trackingManager.GetPosture(TrackerType.PLATFORM_C).Position;
        Vector3 trackerDPosition = trackingManager.GetPosture(TrackerType.PLATFORM_D).Position;
        Vector3 middlePointCD = (trackerCPosition + trackerDPosition) / 2;

        float rangeLength = 0.4f;
        bool isNotZero = IsNotZero(trackerCPosition, trackerDPosition);
        bool isWithinRange = IsWithinRange(playersHead.transform.position.z, middlePointCD.z, middlePointCD.z + rangeLength);

        bool isConditionSatisfied = isNotZero && isWithinRange;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha5))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_ANSERING_QUESTIONNAIRE;

            uiManager.ActivateQuestionnairePanel();
            uiManager.UpdateInstructionUI(currentExperimentalStatus);
        }
    }

    private void CheckEndOfDESAnsweringQuestionnaire()
    {
        var questionnaireResult = answeringQuestionnaireController.CheckUIControl(stairCaseMethodManager.TrialNum, TESConditionsConverter.ToString(tesConditions[currentTESConditionsIndex]));

        bool isConditionSatisfied = false;
        if (questionnaireResult.TryGetValue(QuestionType.QUESTION_3, out string question3Answer))
        {
            isConditionSatisfied = question3Answer != "";
        }

        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha6))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_GETTING_OFF;

            answeringQuestionnaireController.InitializeQuestionnaireResult();

            stairCaseMethodManager.Next(questionnaireResult[QuestionType.QUESTION_1] == "SecondHalf");

            uiManager.ActivateInstructionPanel();
            uiManager.UpdateInstructionUI(currentExperimentalStatus);
        }
    }

    private void CheckEndOfDESGettingOff()
    {
        Vector3 slopeEdge1 = DES_SECOND_SLOPE_ORIGIN + new Vector3(-0.5f, 0, 0);
        Vector3 slopeEdge2 = DES_SECOND_SLOPE_ORIGIN + new Vector3(0.5f, 0, 0);

        bool isLeft = IsLeftSide(playersHead.transform.position, slopeEdge1, slopeEdge2);

        bool isConditionSatisfied = isLeft;
        if (isManualControllEnabled && Input.GetKeyDown(KeyCode.Alpha5))
            isConditionSatisfied = true;

        if (isConditionSatisfied)
        {
            if (stairCaseMethodManager.IsFinished)
            {
                if (currentTESConditionsIndex == tesConditions.Count - 1)
                {
                    currentExperimentalStatus = ExperimentalStatus.FINISHED;
                }
                else
                {
                    currentExperimentalStatus = ExperimentalStatus.DES_RESTING;
                    uiManager.UpdateInstructionUI(currentExperimentalStatus);

                    currentTESConditionsIndex++;
                    restingStartedAt = DateTime.Now;
                    stairCaseMethodManager = new(iterationNumber);
                }
            }
            else
            {
                currentExperimentalStatus = ExperimentalStatus.DES_ENTERING_IN;
                uiManager.UpdateInstructionUI(currentExperimentalStatus);

                environmentManager.SetFirstSlopePartActive(true);
                environmentManager.SetFirstSlopeGradient(0);
                environmentManager.SetSecondSlopePartActive(false);
                environmentManager.SetStandPartActive(false);

                pointMarker.SetActive(true);
                pointMarker.transform.localPosition = DES_FIRST_SLOPE_ORIGIN + new Vector3(0, 0.1f, -2.2f * 0.99f);
            }

            uiManager.UpdateInstructionUI(currentExperimentalStatus);
        }
    }

    private void CheckEndOfDESResting()
    {
        Debug.Log("Resting");
        if ((DateTime.Now - restingStartedAt).TotalMilliseconds > 60 * 1000)
        {
            currentExperimentalStatus = ExperimentalStatus.DES_ENTERING_IN;
            uiManager.UpdateInstructionUI(currentExperimentalStatus);

            environmentManager.SetFirstSlopePartActive(true);
            environmentManager.SetFirstSlopeGradient(0);
            environmentManager.SetSecondSlopePartActive(false);
            environmentManager.SetStandPartActive(false);

            pointMarker.SetActive(true);
            pointMarker.transform.localPosition = DES_FIRST_SLOPE_ORIGIN + new Vector3(0, 0.1f, -2.2f * 0.99f);
        }
    }

    private bool IsNotZero(params Vector3[] vectors)
    {
        float epsilon = 0.00001f;
        foreach (Vector3 vector in vectors)
        {
            if ((-epsilon < vector.x && vector.x < epsilon) && (-epsilon < vector.y && vector.y < epsilon) && (-epsilon < vector.z && vector.z < epsilon))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsNearInHorizontalPlane(Vector3 position1, Vector3 position2, float thresholdDistance)
    {
        Vector2 position1InHorizontalPlane = new(position1.x, position1.z);
        Vector2 position2InHorizontalPlane = new(position2.x, position2.z);

        return (position1InHorizontalPlane - position2InHorizontalPlane).magnitude <= thresholdDistance;
    }

    private bool IsSameDirection(Vector3 direction1, Vector3 direction2, float thresholdAngle)
    {
        return Vector3.Angle(direction1, direction2) <= thresholdAngle;
    }

    private bool IsWithinRange(float value, float lowerLimit, float upperLimit)
    {
        return (lowerLimit <= value) && (value <= upperLimit);
    }

    private bool IsLeftSide(Vector3 userPosition, Vector3 linePointStart, Vector3 linePointGoal)
    {
        Vector3 lineDirection = linePointGoal - linePointStart;
        Vector3 toUser = userPosition - linePointStart;
        Vector3 cross = Vector3.Cross(lineDirection, toUser);
        return cross.y > 0;
    }
}
