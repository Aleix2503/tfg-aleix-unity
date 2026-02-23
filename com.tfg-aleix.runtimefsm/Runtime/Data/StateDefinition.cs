using System.Collections.Generic;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class StateDefinition
    {
        public string id;
        public List<ActionDefinition> enter;
        public List<ActionDefinition> tick;
        public List<ActionDefinition> exit;
    }
}