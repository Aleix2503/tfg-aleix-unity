namespace RuntimeFSM.Data
{
    /// <summary>
    /// Tipos de condiciones soportadas
    /// </summary>
    public enum ConditionType
    {
        /// <summary>Condición simple: variable, operador, valor</summary>
        Simple,

        /// <summary>Condición lógica: combinación de otras condiciones con AND/OR/NOT</summary>
        Logical
    }
}
