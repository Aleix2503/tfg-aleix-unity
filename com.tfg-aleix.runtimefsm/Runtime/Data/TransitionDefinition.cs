namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class TransitionDefinition
    {
        public string from;
        public string to;
        public ConditionDefinition condition;
    }
}