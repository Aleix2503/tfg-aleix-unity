using RuntimeFSM.Data;

namespace RuntimeFSM.Core
{
    internal class FSMTransition
    {
        public string From { get; }
        public string To { get; }
        public ConditionDefinition Condition { get; }

        public FSMTransition(TransitionDefinition definition)
        {
            From = definition.from;
            To = definition.to;
            Condition = definition.condition;
        }
    }
}