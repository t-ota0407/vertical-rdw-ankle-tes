using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private const LanguageType languageType = LanguageType.EN;

    private const int MINIMUM_LIKERT_SCORE = 1;
    private const int MAXIMUM_LIKERT_SCORE = 7;

    private readonly Dictionary<ExperimentalStatus, string> INSTRUCTION_TEXTS = new Dictionary<ExperimentalStatus, string>()
    {
        { ExperimentalStatus.ASC_ENTERING_IN, (languageType == LanguageType.JP) ? "���s�o�H��ɓ����Ă��������B" : "Please enter on the walking path." },
        { ExperimentalStatus.ASC_TURNING_TO_FLAT_PATH, (languageType == LanguageType.JP) ? "�i�s�����������Ă��������B" : "Please face the direction of travel." },
        { ExperimentalStatus.ASC_WALKING_FLATLAND, "" },
        { ExperimentalStatus.ASC_GETTING_ON, (languageType == LanguageType.JP) ? "�X���[�v�ɏ���Ă��������B" : "Please get on the slope." },
        { ExperimentalStatus.ASC_TURNING_TO_SLOPED_PATH, (languageType == LanguageType.JP) ? "�i�s�����������Ă��������B" : "Please face the direction of travel." },
        { ExperimentalStatus.ASC_WALKING_SLOPE, "" },
        { ExperimentalStatus.ASC_ANSERING_QUESTIONNAIRE, "" },
        { ExperimentalStatus.ASC_GETTING_OFF, (languageType == LanguageType.JP) ? "�X���[�v����~��Ă��������B" : "Please get off the slope." },
        { ExperimentalStatus.ASC_RESTING, (languageType == LanguageType.JP) ? "�x�e���Ă��������B" : "Please take a break."},
        { ExperimentalStatus.DES_ENTERING_IN, (languageType == LanguageType.JP) ? "���s�o�H��ɓ����Ă��������B" : "Please enter on the walking path." },
        { ExperimentalStatus.DES_TURNING_TO_FLAT_PATH, (languageType == LanguageType.JP) ? "�i�s�����������Ă��������B" : "Please face the direction of travel." },
        { ExperimentalStatus.DES_WALKING_FLATLAND, "" },
        { ExperimentalStatus.DES_GETTING_ON, (languageType == LanguageType.JP) ? "�X���[�v�ɏ���Ă��������B" : "Please get on the slope." },
        { ExperimentalStatus.DES_TURNING_TO_SLOPED_PATH, (languageType == LanguageType.JP) ? "�i�s�����������Ă��������B" : "Please face the direction of travel." },
        { ExperimentalStatus.DES_ANSERING_QUESTIONNAIRE, "" },
        { ExperimentalStatus.DES_GETTING_OFF, (languageType == LanguageType.JP) ? "�X���[�v����~��Ă��������B" : "Please get off the slope." },
        { ExperimentalStatus.DES_RESTING, (languageType == LanguageType.JP) ? "�x�e���Ă��������B" : "Please take a break."},
        { ExperimentalStatus.FINISHED, (languageType == LanguageType.JP) ? "�������I�����܂����B" : "The experiment has been completed." },
    };

    private readonly Dictionary<QuestionType, string> QUESTION_TEXTS = new Dictionary<QuestionType, string>()
    {
        { QuestionType.QUESTION_1, (languageType == LanguageType.JP) ? "�O���̕��s�̌��œ���ꂽ�X�Ί��o�ƌ㔼�̕��s�̌��œ���ꂽ�X�Ί��o�͂ǂ��炪�傫�������ł����H" : "Which half of the walking experience had a greater sensation of inclination?" },
        { QuestionType.QUESTION_2, (languageType == LanguageType.JP) ? "�O���̍⓹�̕��s�͎��R�Ɋ�����ꂽ�B�i1:�S�����ӂ��Ȃ�, 7:���S�ɓ��ӂ���j" : "Walking a slope in the first half felt natural. (1: totally disagree, 7: completely agree)" },
        { QuestionType.QUESTION_3, (languageType == LanguageType.JP) ? "���o�Ƒ̐����o�͈�v���Ă���悤�Ɋ�����ꂽ�B�i1:�S�����ӂ��Ȃ�, 7:���S�ɓ��ӂ���j" : "The visual and somatosensory perceptions in the first half was coincide. (1: totally disagree, 7: completely agree)" },
    };

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject questionnairePanel;

    [SerializeField] private TextMeshProUGUI instructionText;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject twoAfcElements;
    [SerializeField] private GameObject likertScaleElements;
    [SerializeField] private Button twoAfcLeftButton;
    [SerializeField] private Button twoAfcRightButton;
    [SerializeField] private TextMeshProUGUI twoAfcLeftButtonText;
    [SerializeField] private TextMeshProUGUI twoAfcRightButtonText;
    [SerializeField] private Button likertScaleButton1;
    [SerializeField] private Button likertScaleButton2;
    [SerializeField] private Button likertScaleButton3;
    [SerializeField] private Button likertScaleButton4;
    [SerializeField] private Button likertScaleButton5;
    [SerializeField] private Button likertScaleButton6;
    [SerializeField] private Button likertScaleButton7;

    private QuestionnaireUIState questionnaireUIState = new(QuestionType.QUESTION_1);

    // Start is called before the first frame update
    void Start()
    {
        questionText.text = QUESTION_TEXTS[QuestionType.QUESTION_1];
        twoAfcLeftButtonText.text = (languageType == LanguageType.JP) ? "�O��" : "First half";
        twoAfcRightButtonText.text = (languageType == LanguageType.JP) ? "�㔼" : "Second half";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateInstructionPanel()
    {
        DeactivateAllPanels();
        instructionPanel.SetActive(true);
    }

    public void ActivateQuestionnairePanel()
    {
        DeactivateAllPanels();
        questionnairePanel.SetActive(true);
    }

    public void UpdateInstructionUI(ExperimentalStatus experimentalStatus)
    {
        if (INSTRUCTION_TEXTS.TryGetValue(experimentalStatus, out string instructionTextString))
        {
            instructionText.text = instructionTextString;
        }
    }

    public (QuestionType QuestionType, string QuestionAnswer) ConfirmCurrentQuestion()
    {
        QuestionType currentQuestionType = questionnaireUIState.QuestionType;
        string currentQuestionAnswer = "";
        QuestionType nextQuestionType = QuestionType.QUESTION_1;
        switch (currentQuestionType)
        {
            case QuestionType.QUESTION_1:
                if (questionnaireUIState.IsTwoAfcLeft == null)
                {
                    currentQuestionAnswer = "NoAnswer";
                }
                else
                {
                    currentQuestionAnswer = (bool)questionnaireUIState.IsTwoAfcLeft ? "FirstHalf" : "SecondHalf";

                }
                nextQuestionType = QuestionType.QUESTION_2;
                break;
            case QuestionType.QUESTION_2:
                currentQuestionAnswer = questionnaireUIState.LikertScaleScore.ToString();
                nextQuestionType = QuestionType.QUESTION_3;
                break;
            case QuestionType.QUESTION_3:
                currentQuestionAnswer = questionnaireUIState.LikertScaleScore.ToString();
                nextQuestionType = QuestionType.QUESTION_1;
                break;
        }

        if (QUESTION_TEXTS.TryGetValue(nextQuestionType, out string questionTextString))
        {
            questionnaireUIState = new(nextQuestionType);
            questionText.text = questionTextString;

            switch (questionnaireUIState.QuestionType)
            {
                // 2AFC Questionnaire
                case QuestionType.QUESTION_1:
                    twoAfcElements.SetActive(true);
                    likertScaleElements.SetActive(false);
                    SelectTwoAfcButton(questionnaireUIState.IsTwoAfcLeft);
                    break;

                // Likert Scale Questionnaire
                case QuestionType.QUESTION_2:
                case QuestionType.QUESTION_3:
                    twoAfcElements.SetActive(false);
                    likertScaleElements.SetActive(true);
                    SelectLikertScaleButton(questionnaireUIState.LikertScaleScore);
                    break;
            }
        }

        return (currentQuestionType, currentQuestionAnswer);
    }

    public void UpdateQuestionnaireUI(bool isLeftSelected)
    {
        switch (questionnaireUIState.QuestionType)
        {
            // 2AFC Questionnaire
            case QuestionType.QUESTION_1:
                questionnaireUIState.IsTwoAfcLeft = isLeftSelected;
                SelectTwoAfcButton(questionnaireUIState.IsTwoAfcLeft);
                break;

            // Likert Scale Questionnaire
            case QuestionType.QUESTION_2:
            case QuestionType.QUESTION_3:
                questionnaireUIState.LikertScaleScore = (isLeftSelected)
                    ? Mathf.Clamp(questionnaireUIState.LikertScaleScore - 1, MINIMUM_LIKERT_SCORE, MAXIMUM_LIKERT_SCORE)
                    : Mathf.Clamp(questionnaireUIState.LikertScaleScore + 1, MINIMUM_LIKERT_SCORE, MAXIMUM_LIKERT_SCORE);
                SelectLikertScaleButton(questionnaireUIState.LikertScaleScore);
                break;
        }
    }

    public void DeactivateAllPanels()
    {
        instructionPanel.SetActive(false);
        questionnairePanel.SetActive(false);
    }

    public void SelectTwoAfcButton(bool? isLeftSelected)
    {
        switch (isLeftSelected)
        {
            case null: EventSystem.current.SetSelectedGameObject(null); break;
            case true: twoAfcLeftButton.Select(); break;
            case false: twoAfcRightButton.Select(); break;
        }
    }

    public void SelectLikertScaleButton(int selectedScaleNumber)
    {
        switch (selectedScaleNumber)
        {
            case 1: likertScaleButton1.Select(); break;
            case 2: likertScaleButton2.Select(); break;
            case 3: likertScaleButton3.Select(); break;
            case 4: likertScaleButton4.Select(); break;
            case 5: likertScaleButton5.Select(); break;
            case 6: likertScaleButton6.Select(); break;
            case 7: likertScaleButton7.Select(); break;
        }
    }
}
