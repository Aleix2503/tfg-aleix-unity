using System;
using RuntimeFSM.Data;
using RuntimeFSM.Interfaces;
using RuntimeFSM.Utilities;

namespace RuntimeFSM.Utils
{
    /// <summary>
    /// Clase base abstracta para implementadores de IConditionEvaluator.
    /// Maneja automáticamente las condiciones lógicas (AND, OR, NOT) de forma recursiva.
    /// Los subclases solo necesitan implementar EvaluateSimpleCondition() para condiciones simples.
    /// </summary>
    public abstract class ConditionEvaluatorBase : IConditionEvaluator
    {
        /// <summary>
        /// Evalúa una definición de condición (simple o lógica)
        /// Maneja la recursión automáticamente para condiciones lógicas
        /// </summary>
        public bool Evaluate(ConditionDefinition condition)
        {
            if (condition == null)
                return true;

            var conditionType = ConditionParser.ParseConditionType(condition.type);

            switch (conditionType)
            {
                case ConditionType.Simple:
                    return EvaluateSimpleCondition(condition);

                case ConditionType.Logical:
                    return EvaluateLogicalCondition(condition);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Evalúa una condición lógica (AND, OR, NOT) de forma recursiva
        /// </summary>
        private bool EvaluateLogicalCondition(ConditionDefinition condition)
        {
            if (condition.conditions == null || condition.conditions.Count == 0)
                return false;

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
                return false;

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
