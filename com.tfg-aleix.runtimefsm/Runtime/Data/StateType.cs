namespace RuntimeFSM.Data
{
    /// <summary>
    /// Tipos especiales de estados en la FSM
    /// </summary>
    public enum StateType
    {
        /// <summary>Estado normal</summary>
        Normal,

        /// <summary>Entry Point: Estado inicial de la FSM</summary>
        EntryPoint,

        /// <summary>Any State: Estado especial que puede recibir transiciones desde cualquier estado</summary>
        AnyState,

        /// <summary>Global State: Estado que ejecuta acciones cada tick sin cambiar de estado</summary>
        GlobalState
    }
}
