namespace RuntimeFSM.Data
{
    /// <summary>
    /// Operadores lógicos para combinar condiciones
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>AND: Todas las condiciones deben ser verdaderas</summary>
        And,

        /// <summary>OR: Al menos una condición debe ser verdadera</summary>
        Or,

        /// <summary>NOT: Niega el resultado de la condición</summary>
        Not
    }
}
