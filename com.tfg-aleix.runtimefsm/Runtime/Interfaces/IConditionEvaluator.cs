namespace RuntimeFSM.Interfaces
{
    /// <summary>
    /// Evalúa condiciones definidas en el JSON.
    /// </summary>
    public interface IConditionEvaluator
    {
        bool Evaluate(
            string conditionType,
            string conditionName,
            string conditionOperator,
            string value);
    }
}