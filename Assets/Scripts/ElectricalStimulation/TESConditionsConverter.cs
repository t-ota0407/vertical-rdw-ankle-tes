using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESConditionsConverter
{
    public static string ToString(TESConditions tesCondition)
    {
        string stringExpression = "";
        switch (tesCondition)
        {
            case TESConditions.FS:
                stringExpression = "FS";
                break;
            case TESConditions.BS:
                stringExpression = "BS";
                break;
            case TESConditions.NONE:
                stringExpression = "NONE";
                break;
        }
        return stringExpression;
    }
}
