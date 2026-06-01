using UnityEngine;
using RuntimeFSM.Data;
using RuntimeFSM.Utils;
using RuntimeFSM.Utilities;

namespace RuntimeFSM.Examples
{
    /// <summary>
    /// Ejemplo de evaluador de condiciones que implementa la lógica de negocio.
    /// Hereda de ConditionEvaluatorBase que maneja automáticamente:
    /// - Condiciones simples (delegadas a EvaluateSimpleCondition)
    /// - Condiciones lógicas recursivas (AND, OR, NOT)
    /// </summary>
    public class ExampleConditionEvaluator : MonoBehaviour, IConditionEvaluator
    {
        [Header("Runtime Variables")]
        [SerializeField] private float distanceToPlayer = 10f;
        [SerializeField] private bool isAlerted = false;
        [SerializeField] private int health = 100;

        private ConditionEvaluatorImpl _evaluator;

        private void OnEnable()
        {
            _evaluator = new ConditionEvaluatorImpl(this);
        }

        public bool Evaluate(ConditionDefinition condition)
        {
            return _evaluator.Evaluate(condition);
        }

        public bool EvaluateSimple(
            string conditionType,
            string conditionName,
            string conditionOperator,
            string value)
        {
            return _evaluator.EvaluateSimple(conditionType, conditionName, conditionOperator, value);
        }

        /// <summary>
        /// Implementación interna que hereda de ConditionEvaluatorBase
        /// </summary>
        private class ConditionEvaluatorImpl : ConditionEvaluatorBase
        {
            private readonly ExampleConditionEvaluator _parent;

            public ConditionEvaluatorImpl(ExampleConditionEvaluator parent)
            {
                _parent = parent;
            }

            /// <summary>
            /// Implementa la lógica específica para evaluar condiciones simples
            /// </summary>
            protected override bool EvaluateSimpleCondition(ConditionDefinition condition)
            {
                // Parsear el operador
                var operatorType = ConditionParser.ParseConditionOperator(condition.@operator);

                // Obtener el valor actual de la variable
                float currentValue = GetVariableValue(condition.name);

                // Si el valor actual es string (para Contains/NotContains)
                string stringValue = GetStringVariableValue(condition.name);

                // Parsear el valor objetivo
                if (!float.TryParse(condition.value, out float targetValue) && stringValue == null)
                {
                    return false;
                }

                // Evaluar según el operador
                switch (operatorType)
                {
                    case ConditionOperator.Equals:
                        // Para valores numéricos
                        if (!string.IsNullOrEmpty(stringValue))
                            return stringValue == condition.value;
                        return Mathf.Approximately(currentValue, targetValue);

                    case ConditionOperator.NotEquals:
                        if (!string.IsNullOrEmpty(stringValue))
                            return stringValue != condition.value;
                        return !Mathf.Approximately(currentValue, targetValue);

                    case ConditionOperator.GreaterThan:
                        return currentValue > targetValue;

                    case ConditionOperator.GreaterThanOrEqual:
                        return currentValue >= targetValue;

                    case ConditionOperator.LessThan:
                        return currentValue < targetValue;

                    case ConditionOperator.LessThanOrEqual:
                        return currentValue <= targetValue;

                    case ConditionOperator.Contains:
                        return !string.IsNullOrEmpty(stringValue) &&
                               stringValue.Contains(condition.value);

                    case ConditionOperator.NotContains:
                        return string.IsNullOrEmpty(stringValue) ||
                               !stringValue.Contains(condition.value);

                    default:
                        return false;
                }
            }

            /// <summary>
            /// Obtiene el valor numérico de una variable
            /// </summary>
            private float GetVariableValue(string variableName)
            {
                switch (variableName)
                {
                    case "distanceToPlayer":
                        return _parent.distanceToPlayer;

                    case "health":
                        return _parent.health;

                    default:
                        return 0f;
                }
            }

            /// <summary>
            /// Obtiene el valor string de una variable
            /// </summary>
            private string GetStringVariableValue(string variableName)
            {
                switch (variableName)
                {
                    case "isAlerted":
                        return _parent.isAlerted ? "true" : "false";

                    default:
                        return null;
                }
            }
        }

        // Métodos públicos para simular cambios en runtime (para testing)
        public void SetDistanceToPlayer(float distance)
        {
            distanceToPlayer = distance;
        }

        public void SetAlerted(bool alerted)
        {
            isAlerted = alerted;
        }

        public void SetHealth(int newHealth)
        {
            health = newHealth;
        }
    }
}