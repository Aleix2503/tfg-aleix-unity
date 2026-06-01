using System.Collections.Generic;
using UnityEngine;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class ConditionParams
    {
        public string variableName;
        public string @operator;
        public string value;
    }

    [System.Serializable]
    public class ConditionDefinition
    {
        /// <summary>
        /// Type of condition: "VariableCompare", "BoolIsTrue", "BoolIsFalse", "simple", "logical"
        /// </summary>
        public string type;

        // Properties for new condition structure
        public ConditionParams @params;

        // Properties for simple conditions (legacy)
        public string name;
        public string @operator;
        public string value;

        // Properties for logical conditions (legacy)
        /// <summary>
        /// "AND", "OR", "NOT" (only for type="logical")
        /// </summary>
        public string logicalOperator;

        /// <summary>
        /// Sub-conditions (only for type="logical")
        /// </summary>
        public List<ConditionDefinition> conditions;
    }
}