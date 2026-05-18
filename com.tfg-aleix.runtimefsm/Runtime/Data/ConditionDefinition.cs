using System.Collections.Generic;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class ConditionDefinition
    {
        /// <summary>
        /// "simple" para condiciones simples, "logical" para condiciones lógicas (AND, OR, NOT)
        /// </summary>
        public string type;

        // Propiedades para condiciones simples
        public string name;
        public string @operator;
        public string value;

        // Propiedades para condiciones lógicas
        /// <summary>
        /// "AND", "OR", "NOT" (solo para type="logical")
        /// </summary>
        public string logicalOperator;

        /// <summary>
        /// Sub-condiciones (solo para type="logical")
        /// </summary>
        public List<ConditionDefinition> conditions;
    }
}