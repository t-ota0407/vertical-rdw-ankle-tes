using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class AnsweringQuestionnaireController
{
    private readonly UIManager targetUIManager;

    private readonly SteamVR_Action_Boolean INTERACT_UI = SteamVR_Actions.default_InteractUI;
    private readonly SteamVR_Action_Boolean SNAP_TURN_LEFT = SteamVR_Actions.default_SnapTurnLeft;
    private readonly SteamVR_Action_Boolean SNAP_TURN_RIGHT = SteamVR_Actions.default_SnapTurnRight;

    private readonly SteamVR_Input_Sources LEFT_HAND = SteamVR_Input_Sources.LeftHand;
    private readonly SteamVR_Input_Sources RIGHT_HAND = SteamVR_Input_Sources.RightHand;

    private Dictionary<QuestionType, string> questionnaireResult = new();

    private DataLogger questionnaireDataLogger;
    private StairCaseMethodManager stairCaseMethodManager;

    public AnsweringQuestionnaireController(UIManager targetUIManager, DataLogger questionnaireDataLogger, StairCaseMethodManager stairCaseMethodManager)
    {
        this.targetUIManager = targetUIManager;
        this.questionnaireDataLogger = questionnaireDataLogger;
        this.stairCaseMethodManager = stairCaseMethodManager;

        InitializeQuestionnaireResult();
    }

    public void InitializeQuestionnaireResult()
    {
        questionnaireResult.Clear();

        foreach (QuestionType questionType in Enum.GetValues(typeof(QuestionType)))
        {
            questionnaireResult.Add(questionType, "");
        }
    }

    public Dictionary<QuestionType, string> CheckUIControl(int currentTrialNum, string ankleTESCondition)
    {
        if (LeftButtonPressed())
        {
            targetUIManager.UpdateQuestionnaireUI(true);
        }

        if (RightButtonPressed())
        {
            targetUIManager.UpdateQuestionnaireUI(false);
        }

        if (IsTriggered())
        {
            var (questionType, questionAnswer) = targetUIManager.ConfirmCurrentQuestion();
            questionnaireResult[questionType] = questionAnswer;
            if (questionType == QuestionType.QUESTION_3)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string series = stairCaseMethodManager.GetCurrentTrialsSeries();
                string gradientGain = stairCaseMethodManager.GetTrialsGradientGain().Value.ToString("f2");
                string question1Result = questionnaireResult[QuestionType.QUESTION_1];
                string question2Result = questionnaireResult[QuestionType.QUESTION_2];
                string question3Result = questionnaireResult[QuestionType.QUESTION_3];
                questionnaireDataLogger.AppendLine($"{timestamp},{currentTrialNum},{ankleTESCondition},{series},{gradientGain},{question1Result},{question2Result},{question3Result}");
            }
        }

        var copiedQuestionnaireResult = questionnaireResult.ToDictionary(entry => entry.Key, entry => entry.Value);
        return copiedQuestionnaireResult;
    }

    private bool LeftButtonPressed()
    {
        return SNAP_TURN_LEFT.GetStateDown(LEFT_HAND) || SNAP_TURN_LEFT.GetStateDown(RIGHT_HAND);
    }

    private bool RightButtonPressed()
    {
        return SNAP_TURN_RIGHT.GetStateDown(LEFT_HAND) || SNAP_TURN_RIGHT.GetStateDown(RIGHT_HAND);
    }

    private bool IsTriggered()
    {
        return INTERACT_UI.GetStateDown(LEFT_HAND) || INTERACT_UI.GetStateDown(RIGHT_HAND);
    }
}
