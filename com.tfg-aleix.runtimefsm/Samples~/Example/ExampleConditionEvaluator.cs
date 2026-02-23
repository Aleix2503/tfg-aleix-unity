using UnityEngine;
using RuntimeFSM.Interfaces;

namespace RuntimeFSM.Examples
{
    public class ExampleConditionEvaluator : MonoBehaviour, IConditionEvaluator
    {
        [Header("Runtime Variables")]
        public float distanceToPlayer;

        public bool Evaluate(string type, string name, string op, string value)
        {
            if (type == "VARIABLE")
            {
                return EvaluateVariable(name, op, value);
            }

            Debug.LogWarning($"Unknown condition type: {type}");
            return false;
        }

        private bool EvaluateVariable(string name, string op, string value)
        {
            float targetValue;

            if (!float.TryParse(value, out targetValue))
                return false;

            float currentValue = GetVariableValue(name);

            switch (op)
            {
                case "<": return currentValue < targetValue;
                case ">": return currentValue > targetValue;
                case "==": return Mathf.Approximately(currentValue, targetValue);
                case "<=": return currentValue <= targetValue;
                case ">=": return currentValue >= targetValue;
                default:
                    Debug.LogWarning($"Unknown operator: {op}");
                    return false;
            }
        }

        private float GetVariableValue(string variableName)
        {
            switch (variableName)
            {
                case "distanceToPlayer":
                    return distanceToPlayer;

                default:
                    Debug.LogWarning($"Unknown variable: {variableName}");
                    return 0f;
            }
        }
    }
}