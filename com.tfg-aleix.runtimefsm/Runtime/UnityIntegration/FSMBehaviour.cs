using UnityEngine;
using RuntimeFSM.Data;
using RuntimeFSM.Core;
using RuntimeFSM.Interfaces;

namespace RuntimeFSM.UnityIntegration
{
    public class FSMBehaviour : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        private FSM _fsm;

        public string CurrentState => _fsm?.CurrentState;

        public void Initialize(
            FSMDefinition definition,
            IActionExecutor actionExecutor,
            IConditionEvaluator conditionEvaluator)
        {
            if (definition == null)
            {
                return;
            }

            _fsm = new FSM(definition, actionExecutor, conditionEvaluator);
            _fsm.EnableLogging = enableLogging;
            _fsm.OnStateChanged += HandleStateChanged;
        }

        private void Update()
        {
            _fsm?.Tick();
        }

        private void HandleStateChanged(string newState)
        {
            if (enableLogging)
            {
            }
        }
    }
}
