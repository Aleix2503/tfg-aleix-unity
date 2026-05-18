using System.Collections.Generic;

namespace RuntimeFSM.Data
{
    [System.Serializable]
    public class ActionParameter
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class ActionDefinition
    {
        public string action;
        public ActionParameter[] @params = new ActionParameter[0];

        /// <summary>
        /// Convierte el array de parámetros a Dictionary
        /// </summary>
        public Dictionary<string, string> GetParametersAsDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (@params == null)
                return dict;

            foreach (var param in @params)
            {
                if (param != null && !string.IsNullOrEmpty(param.key))
                {
                    dict[param.key] = param.value ?? "";
                }
            }
            return dict;
        }
    }
}