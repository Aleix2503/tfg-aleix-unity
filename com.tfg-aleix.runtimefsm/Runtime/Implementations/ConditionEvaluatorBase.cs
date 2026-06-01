using System;
using RuntimeFSM.Data;
using RuntimeFSM.Interfaces;
using RuntimeFSM.Utilities;
using UnityEngine;

namespace RuntimeFSM.Utils
{
    /// <summary>
    /// Clase base abstracta para implementadores de IConditionEvaluator.
    /// Maneja automáticamente las condiciones lógicas (AND, OR, NOT) de forma recursiva.
    /// Los subclases solo necesitan implementar EvaluateSimpleCondition() para condiciones simples.
    /// </summary>
    public abstract class ConditionEvaluatorBase : MonoBehaviour, IConditionEvaluator
    {
        /// <summary>
        /// Evaluates a condition definition (simple, logical, or new types like VariableCompare, BoolIsTrue, BoolIsFalse)
        /// Handles recursion automatically for logical conditions
        /// Includes robust error handling
        /// </summary>
        public bool Evaluate(ConditionDefinition condition)
        {
            try
            {
                if (condition == null)
                    return true;

                // Check for new condition types first
                if (!string.IsNullOrEmpty(condition.type))
                {
                    var type = condition.type.ToLower();
                    if (type == "variablecompare" || type == "boolistrue" || type == "boolisfalse")
                    {
                        return EvaluateNewCondition(condition);
                    }
                }

                var conditionType = ConditionParser.ParseConditionType(condition.type);

                switch (conditionType)
                {
                    case ConditionType.Simple:
                        return EvaluateSimpleCondition(condition);

                    case ConditionType.Logical:
                        return EvaluateLogicalCondition(condition);

                    case ConditionType.New:
                        return EvaluateNewCondition(condition);

                    default:
                        ConditionEvaluatorErrorHandler.LogInvalidConditionStructure($"Unknown condition type: {condition.type}", gameObject);
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                ConditionEvaluatorErrorHandler.LogEvaluationException(condition?.name ?? "Unknown", ex, gameObject);
                return false;
            }
        }

        /// <summary>
        /// Evaluates new condition types: VariableCompare, BoolIsTrue, BoolIsFalse
        /// </summary>
        private bool EvaluateNewCondition(ConditionDefinition condition)
        {
            if (condition.@params == null)
            {
                ConditionEvaluatorErrorHandler.LogInvalidConditionStructure($"Condition type '{condition.type}' missing params", gameObject);
                return false;
            }

            var type = condition.type.ToLower();

            switch (type)
            {
                case "variablecompare":
                    return EvaluateVariableCompare(condition);

                case "boolistrue":
                    return EvaluateBoolIsTrue(condition);

                case "boolisfalse":
                    return EvaluateBoolIsFalse(condition);

                default:
                    ConditionEvaluatorErrorHandler.LogInvalidConditionStructure($"Unknown new condition type: {condition.type}", gameObject);
                    return false;
            }
        }

        /// <summary>
        /// Evaluates VariableCompare condition
        /// </summary>
        protected virtual bool EvaluateVariableCompare(ConditionDefinition condition)
        {
            var variableName = condition.@params.variableName;
            var operatorStr = condition.@params.@operator;
            var targetValue = condition.@params.value;

            return EvaluateSimpleCondition(new ConditionDefinition
            {
                name = variableName,
                @operator = operatorStr,
                value = targetValue
            });
        }

        /// <summary>
        /// Evaluates BoolIsTrue condition
        /// </summary>
        protected virtual bool EvaluateBoolIsTrue(ConditionDefinition condition)
        {
            var variableName = condition.@params.variableName;
            float value = GetBoolVariableValue(variableName) ? 1f : 0f;
            return value > 0.5f;
        }

        /// <summary>
        /// Evaluates BoolIsFalse condition
        /// </summary>
        protected virtual bool EvaluateBoolIsFalse(ConditionDefinition condition)
        {
            var variableName = condition.@params.variableName;
            float value = GetBoolVariableValue(variableName) ? 1f : 0f;
            return value < 0.5f;
        }

        /// <summary>
        /// Gets a boolean variable value. Override in subclasses.
        /// </summary>
        protected virtual bool GetBoolVariableValue(string variableName)
        {
            // Default implementation - subclasses should override
            throw new System.NotImplementedException($"GetBoolVariableValue not implemented for '{variableName}'");
        }

        /// <summary>
        /// Evalúa una condición lógica (AND, OR, NOT) de forma recursiva
        /// </summary>
        private bool EvaluateLogicalCondition(ConditionDefinition condition)
        {
            try
            {
                if (condition.conditions == null || condition.conditions.Count == 0)
                {
                    ConditionEvaluatorErrorHandler.LogInvalidConditionStructure("Condición lógica sin sub-condiciones", gameObject);
                    return false;
                }

                var logicalOp = ConditionParser.ParseLogicalOperator(condition.logicalOperator);

                switch (logicalOp)
                {
                    case LogicalOperator.And:
                        return EvaluateAnd(condition.conditions);

                    case LogicalOperator.Or:
                        return EvaluateOr(condition.conditions);

                    case LogicalOperator.Not:
                        return EvaluateNot(condition.conditions);

                    default:
                        ConditionEvaluatorErrorHandler.LogInvalidConditionStructure($"Operador lógico desconocido: {condition.logicalOperator}", gameObject);
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                ConditionEvaluatorErrorHandler.LogEvaluationException("LogicalCondition", ex, gameObject);
                return false;
            }
        }

        /// <summary>
        /// Evalúa AND: todas las condiciones deben ser verdaderas
        /// </summary>
        private bool EvaluateAnd(System.Collections.Generic.List<ConditionDefinition> conditions)
        {
            foreach (var condition in conditions)
            {
                if (!Evaluate(condition))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Evalúa OR: al menos una condición debe ser verdadera
        /// </summary>
        private bool EvaluateOr(System.Collections.Generic.List<ConditionDefinition> conditions)
        {
            foreach (var condition in conditions)
            {
                if (Evaluate(condition))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Evalúa NOT: niega el resultado de la única sub-condición
        /// </summary>
        private bool EvaluateNot(System.Collections.Generic.List<ConditionDefinition> conditions)
        {
            if (conditions.Count != 1)
            {
                ConditionEvaluatorErrorHandler.LogInvalidConditionStructure($"NOT debe tener exactamente 1 sub-condición, pero tiene {conditions.Count}", gameObject);
                return false;
            }

            return !Evaluate(conditions[0]);
        }

        /// <summary>
        /// Evalúa una condición simple.
        /// DEBE SER IMPLEMENTADO por las subclases.
        /// </summary>
        /// <param name="condition">Definición de condición simple</param>
        /// <returns>Resultado de la evaluación</returns>
        protected abstract bool EvaluateSimpleCondition(ConditionDefinition condition);

        /// <summary>
        /// Método legacy para compatibilidad.
        /// Delega a Evaluate creando una ConditionDefinition simple.
        /// </summary>
        public bool EvaluateSimple(
            string conditionType,
            string conditionName,
            string conditionOperator,
            string value)
        {
            var condition = new ConditionDefinition
            {
                type = "simple",
                name = conditionName,
                @operator = conditionOperator,
                value = value
            };

            return EvaluateSimpleCondition(condition);
        }
    }
}
