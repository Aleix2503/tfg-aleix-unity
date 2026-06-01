using RuntimeFSM.Data;

namespace RuntimeFSM.Utilities
{
    /// <summary>
    /// Utility for parsing and validating condition definitions
    /// </summary>
    public static class ConditionParser
    {
        /// <summary>
        /// Determines condition type based on string
        /// </summary>
        public static ConditionType ParseConditionType(string typeString)
        {
            if (string.IsNullOrEmpty(typeString))
                return ConditionType.Simple;

            var lower = typeString.ToLower();
            return lower switch
            {
                "simple" => ConditionType.Simple,
                "logical" => ConditionType.Logical,
                "variablecompare" or "boolistrue" or "boolisfalse" => ConditionType.New,
                _ => ConditionType.Simple
            };
        }

        /// <summary>
        /// Parses a logical operator from string
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
        /// Parses a comparison operator from string
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
        /// Validates that a condition definition is valid
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
                ConditionType.New => IsValidNewCondition(condition),
                _ => false
            };
        }

        private static bool IsValidSimpleCondition(ConditionDefinition condition)
        {
            // Simple condition needs: name, @operator, value
            return !string.IsNullOrEmpty(condition.name) &&
                   !string.IsNullOrEmpty(condition.@operator) &&
                   !string.IsNullOrEmpty(condition.value);
        }

        private static bool IsValidLogicalCondition(ConditionDefinition condition)
        {
            // Logical condition needs: logicalOperator and conditions
            if (string.IsNullOrEmpty(condition.logicalOperator) || condition.conditions == null)
                return false;

            var logicalOp = ParseLogicalOperator(condition.logicalOperator);

            // NOT must have exactly 1 condition
            if (logicalOp == LogicalOperator.Not)
                return condition.conditions.Count == 1 && IsValid(condition.conditions[0]);

            // AND and OR must have at least 2 conditions
            if (condition.conditions.Count < 2)
                return false;

            // Validate all sub-conditions
            foreach (var subCondition in condition.conditions)
            {
                if (!IsValid(subCondition))
                    return false;
            }

            return true;
        }

        private static bool IsValidNewCondition(ConditionDefinition condition)
        {
            // New condition types need: type and params
            if (condition.@params == null)
                return false;

            var type = condition.type.ToLower();

            return type switch
            {
                "variablecompare" => !string.IsNullOrEmpty(condition.@params.variableName) &&
                                    !string.IsNullOrEmpty(condition.@params.@operator) &&
                                    !string.IsNullOrEmpty(condition.@params.value),
                "boolistrue" or "boolisfalse" => !string.IsNullOrEmpty(condition.@params.variableName),
                _ => false
            };
        }
    }
}
