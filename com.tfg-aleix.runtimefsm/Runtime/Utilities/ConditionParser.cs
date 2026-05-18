using RuntimeFSM.Data;

namespace RuntimeFSM.Utilities
{
    /// <summary>
    /// Utilidad para parsear y validar definiciones de condiciones
    /// </summary>
    public static class ConditionParser
    {
        /// <summary>
        /// Determina el tipo de condición basado en el string
        /// </summary>
        public static ConditionType ParseConditionType(string typeString)
        {
            if (string.IsNullOrEmpty(typeString))
                return ConditionType.Simple;

            return typeString.ToLower() switch
            {
                "simple" => ConditionType.Simple,
                "logical" => ConditionType.Logical,
                _ => ConditionType.Simple
            };
        }

        /// <summary>
        /// Parsea un operador lógico desde string
        /// </summary>
        public static LogicalOperator ParseLogicalOperator(string operatorString)
        {
            if (string.IsNullOrEmpty(operatorString))
                return LogicalOperator.And;

            return operatorString.ToUpper() switch
            {
                "AND" => LogicalOperator.And,
                "OR" => LogicalOperator.Or,
                "NOT" => LogicalOperator.Not,
                _ => LogicalOperator.And
            };
        }

        /// <summary>
        /// Parsea un operador de comparación desde string
        /// </summary>
        public static ConditionOperator ParseConditionOperator(string operatorString)
        {
            if (string.IsNullOrEmpty(operatorString))
                return ConditionOperator.Equals;

            return operatorString.ToLower() switch
            {
                "==" or "equals" or "equal" => ConditionOperator.Equals,
                "!=" or "not_equals" or "notequals" => ConditionOperator.NotEquals,
                ">" or "greater_than" or "greaterthan" => ConditionOperator.GreaterThan,
                ">=" or "greater_than_or_equal" or "greaterthanorequal" => ConditionOperator.GreaterThanOrEqual,
                "<" or "less_than" or "lessthan" => ConditionOperator.LessThan,
                "<=" or "less_than_or_equal" or "lessthanorequal" => ConditionOperator.LessThanOrEqual,
                "contains" => ConditionOperator.Contains,
                "not_contains" or "notcontains" => ConditionOperator.NotContains,
                _ => ConditionOperator.Equals
            };
        }

        /// <summary>
        /// Valida que una definición de condición sea válida
        /// </summary>
        public static bool IsValid(ConditionDefinition condition)
        {
            if (condition == null)
                return false;

            var conditionType = ParseConditionType(condition.type);

            return conditionType switch
            {
                ConditionType.Simple => IsValidSimpleCondition(condition),
                ConditionType.Logical => IsValidLogicalCondition(condition),
                _ => false
            };
        }

        private static bool IsValidSimpleCondition(ConditionDefinition condition)
        {
            // Una condición simple necesita: name, @operator, value
            return !string.IsNullOrEmpty(condition.name) &&
                   !string.IsNullOrEmpty(condition.@operator) &&
                   !string.IsNullOrEmpty(condition.value);
        }

        private static bool IsValidLogicalCondition(ConditionDefinition condition)
        {
            // Una condición lógica necesita: logicalOperator y conditions
            if (string.IsNullOrEmpty(condition.logicalOperator) || condition.conditions == null)
                return false;

            var logicalOp = ParseLogicalOperator(condition.logicalOperator);

            // NOT debe tener exactamente 1 condición
            if (logicalOp == LogicalOperator.Not)
                return condition.conditions.Count == 1 && IsValid(condition.conditions[0]);

            // AND y OR deben tener al menos 2 condiciones
            if (condition.conditions.Count < 2)
                return false;

            // Validar todas las sub-condiciones
            foreach (var subCondition in condition.conditions)
            {
                if (!IsValid(subCondition))
                    return false;
            }

            return true;
        }
    }
}
