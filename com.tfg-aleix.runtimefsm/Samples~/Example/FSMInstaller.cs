using UnityEngine;
using Newtonsoft.Json;
using RuntimeFSM.Data;
using RuntimeFSM.UnityIntegration;
using RuntimeFSM.Examples;

public class FSMInstaller : MonoBehaviour
{
    [Header("FSM JSON")]
    [TextArea(10, 30)]
    [SerializeField] private string jsonDefinition;

    private void Start()
    {
        if (string.IsNullOrEmpty(jsonDefinition))
        {
            return;
        }

        var fsmBehaviour = GetComponent<FSMBehaviour>();
        var executor = GetComponent<ExampleActionExecutor>();
        var evaluator = GetComponent<ExampleConditionEvaluator>();

        if (fsmBehaviour == null || executor == null || evaluator == null)
        {
            return;
        }

        try
        {
            var definition = JsonConvert.DeserializeObject<FSMDefinition>(jsonDefinition);

            if (definition == null)
            {
                return;
            }

            fsmBehaviour.Initialize(definition, executor, evaluator);
        }
        catch (System.Exception ex)
        {
        }
    }
}