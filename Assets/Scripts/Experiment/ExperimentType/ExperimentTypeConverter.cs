public class ExperimentTypeConverter
{
    private const string STR_ASCENDING_EVALUATION = "ASCENDING_EVALUATION";
    private const string STR_DESCENDING_EVALUATION = "DESCENDING_EVALUATION";

    public static string ToString(ExperimentType experimentType)
    {
        string stringExpression = "";
        switch (experimentType)
        {
            case ExperimentType.ASCENDING_EVALUATION:
                stringExpression = STR_ASCENDING_EVALUATION;
                break;
            case ExperimentType.DESCENDING_EVALUATION:
                stringExpression = STR_DESCENDING_EVALUATION;
                break;
        }
        return stringExpression;
    }
}
