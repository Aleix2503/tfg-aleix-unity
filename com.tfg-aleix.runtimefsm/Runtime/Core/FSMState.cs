using System.Collections.Generic;
using RuntimeFSM.Data;

namespace RuntimeFSM.Core
{
    internal class FSMState
    {
        public string Id { get; }

        public List<ActionDefinition> EnterActions { get; }
        public List<ActionDefinition> TickActions { get; }
        public List<ActionDefinition> ExitActions { get; }

        public FSMState(StateDefinition definition)
        {
            Id = definition.id;
            EnterActions = definition.enter ?? new List<ActionDefinition>();
            TickActions = definition.tick ?? new List<ActionDefinition>();
            ExitActions = definition.exit ?? new List<ActionDefinition>();
        }
    }
}