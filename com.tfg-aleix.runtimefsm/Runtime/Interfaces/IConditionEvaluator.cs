using RuntimeFSM.Data;

namespace RuntimeFSM.Interfaces
{
    /// <summary>
    /// Evalúa condiciones definidas en el JSON.
    /// Soporta:
    /// - Condiciones simples (ej: variable > 5)
    /// - Condiciones lógicas (AND, OR, NOT) con sub-condiciones anidadas
    /// </summary>
    public interface IConditionEvaluator
    {
        /// <summary>
        /// Evalúa una definición de condición completa (simple o lógica)
        /// </summary>
        bool Evaluate(ConditionDefinition condition);

        /// <summary>
        /// Evalúa una condición simple
        /// (Método legacy para compatibilidad)
        /// </summary>
        bool EvaluateSimple(
            string conditionType,
            string conditionName,
            string conditionOperator,
            string value);
    }
}