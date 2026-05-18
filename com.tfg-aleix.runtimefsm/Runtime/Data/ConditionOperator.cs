namespace RuntimeFSM.Data
{
    /// <summary>
    /// Operadores disponibles para condiciones simples
    /// </summary>
    public enum ConditionOperator
    {
        /// <summary>Igualdad</summary>
        Equals,

        /// <summary>Desigualdad</summary>
        NotEquals,

        /// <summary>Mayor que</summary>
        GreaterThan,

        /// <summary>Mayor o igual que</summary>
        GreaterThanOrEqual,

        /// <summary>Menor que</summary>
        LessThan,

        /// <summary>Menor o igual que</summary>
        LessThanOrEqual,

        /// <summary>Contiene (para strings)</summary>
        Contains,

        /// <summary>No contiene (para strings)</summary>
        NotContains
    }
}
