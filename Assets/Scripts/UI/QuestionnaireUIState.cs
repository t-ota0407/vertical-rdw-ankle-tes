using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireUIState
{
    public QuestionType QuestionType;

    public bool? IsTwoAfcLeft = null;

    public int LikertScaleScore = 4;

    public bool IsAnsweringFinished = false;

    public QuestionnaireUIState(QuestionType questionType)
    {
        this.QuestionType = questionType;
    }
}
