namespace RuntimeFSM.Data
{
    /// <summary>
    /// Supported condition types
    /// </summary>
    public enum ConditionType
    {
        /// <summary>Simple condition: variable, operator, value</summary>
        Simple,

        /// <summary>Logical condition: combination of other conditions with AND/OR/NOT</summary>
        Logical,

        /// <summary>New condition types: VariableCompare, BoolIsTrue, BoolIsFalse</summary>
        New
    }
}
