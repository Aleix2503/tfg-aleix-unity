using System.Collections.Generic;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class StateDefinition
    {
        public string id;
        public bool is_entry_point;
        public bool is_any_state;
        public bool is_global_state;
        public List<ActionDefinition> enter;
        public List<ActionDefinition> tick;
        public List<ActionDefinition> exit;
    }
}