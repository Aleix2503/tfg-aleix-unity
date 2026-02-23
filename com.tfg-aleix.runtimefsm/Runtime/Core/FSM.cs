using System;
using System.Collections.Generic;
using RuntimeFSM.Data;
using RuntimeFSM.Interfaces;

namespace RuntimeFSM.Core
{
    public class FSM
    {
        public event Action<string> OnStateChanged;

        private readonly Dictionary<string, FSMState> _states;
        private readonly List<FSMTransition> _transitions;

        private readonly IActionExecutor _actionExecutor;
        private readonly IConditionEvaluator _conditionEvaluator;

        private FSMState _currentState;

        public string CurrentState => _currentState?.Id;

        public bool EnableLogging { get; set; }

        public FSM(
            FSMDefinition definition,
            IActionExecutor actionExecutor,
            IConditionEvaluator conditionEvaluator)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            _actionExecutor = actionExecutor
                ?? throw new ArgumentNullException(nameof(actionExecutor));

            _conditionEvaluator = conditionEvaluator
                ?? throw new ArgumentNullException(nameof(conditionEvaluator));

            _states = new Dictionary<string, FSMState>();
            _transitions = new List<FSMTransition>();

            Build(definition);
        }

        private void Build(FSMDefinition definition)
        {
            foreach (var stateDef in definition.states)
            {
                var state = new FSMState(stateDef);

                if (_states.ContainsKey(state.Id))
                    throw new Exception($"Duplicate state id: {state.Id}");

                _states[state.Id] = state;
            }

            foreach (var transitionDef in definition.transitions)
            {
                _transitions.Add(new FSMTransition(transitionDef));
            }

            if (!_states.TryGetValue(definition.initial_state, out _currentState))
                throw new Exception("Initial state not found in FSM.");

            ExecuteActions(_currentState.EnterActions);
        }

        public void Tick()
        {
            if (_currentState == null)
                return;

            foreach (var transition in _transitions)
            {
                if (transition.From != _currentState.Id)
                    continue;

                if (IsTransitionValid(transition))
                {
                    ChangeState(transition.To);
                    return;
                }
            }

            ExecuteActions(_currentState.TickActions);
        }

        private bool IsTransitionValid(FSMTransition transition)
        {
            if (transition.Condition == null)
                return true;

            return _conditionEvaluator.Evaluate(
                transition.Condition.type,
                transition.Condition.name,
                transition.Condition.@operator,
                transition.Condition.value);
        }

        private void ChangeState(string newStateId)
        {
            if (!_states.TryGetValue(newStateId, out var newState))
                throw new Exception($"State {newStateId} not found.");

            ExecuteActions(_currentState.ExitActions);

            _currentState = newState;

            OnStateChanged?.Invoke(newStateId);

            ExecuteActions(_currentState.EnterActions);
        }

        private void ExecuteActions(List<ActionDefinition> actions)
        {
            foreach (var action in actions)
            {
                _actionExecutor.Execute(action.action, action.@params);
            }
        }
    }
}