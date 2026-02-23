using System.Collections.Generic;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class ActionDefinition
    {
        public string action;
        public Dictionary<string, string> @params;
    }
}