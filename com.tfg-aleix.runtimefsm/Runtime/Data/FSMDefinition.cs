using System.Collections.Generic;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class FSMDefinition
    {
        public string version;
        public string name;
        public string initial_state;
        public List<StateDefinition> states;
        public List<TransitionDefinition> transitions;
    }
}