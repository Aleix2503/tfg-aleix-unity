using System.Collections.Generic;
using RuntimeFSM.Data;

namespace RuntimeFSM.Core
{
    internal class FSMState
    {
        public string Id { get; }
        public bool IsEntryPoint { get; }
        public bool IsAnyState { get; }
        public bool IsGlobalState { get; }

        public List<ActionDefinition> EnterActions { get; }
        public List<ActionDefinition> TickActions { get; }
        public List<ActionDefinition> ExitActions { get; }

        public FSMState(StateDefinition definition)
        {
            Id = definition.id;
            IsEntryPoint = definition.is_entry_point;
            IsAnyState = definition.is_any_state;
            IsGlobalState = definition.is_global_state;
            EnterActions = definition.enter ?? new List<ActionDefinition>();
            TickActions = definition.tick ?? new List<ActionDefinition>();
            ExitActions = definition.exit ?? new List<ActionDefinition>();
        }
    }
}