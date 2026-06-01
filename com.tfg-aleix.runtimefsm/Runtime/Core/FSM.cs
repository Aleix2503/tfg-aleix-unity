using System;
using System.Collections.Generic;
using System.Linq;
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
        private FSMState _anyState;
        private readonly List<FSMState> _globalStates;

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
            _globalStates = new List<FSMState>();

            Build(definition);
        }

        private void Build(FSMDefinition definition)
        {
            // Step 1: Create all states
            foreach (var stateDef in definition.states)
            {
                var state = new FSMState(stateDef);

                if (_states.ContainsKey(state.Id))
                    throw new Exception($"Duplicate state id: {state.Id}");

                _states[state.Id] = state;

                // Register special states
                if (state.IsAnyState)
                    _anyState = state;

                if (state.IsGlobalState)
                    _globalStates.Add(state);
            }

            // Step 2: Validate entry point
            var entryPoints = _states.Values.Where(s => s.IsEntryPoint).ToList();

            if (entryPoints.Count != 1)
                throw new Exception(
                    $"FSM must have exactly 1 entry point. Found: {entryPoints.Count}");

            var entryPointState = entryPoints[0];

            // Step 3: Create all transitions
            foreach (var transitionDef in definition.transitions)
            {
                _transitions.Add(new FSMTransition(transitionDef));
            }

            // Step 4: Validate that transitions do not point TO ANY_STATE
            foreach (var transition in _transitions)
            {
                if (_anyState != null && transition.To == _anyState.Id)
                    throw new Exception(
                        "Cannot create transitions TO ANY_STATE. " +
                        "Only transitions FROM ANY_STATE are allowed.");
            }

            // Step 5: Set initial state and execute enter actions
            _currentState = entryPointState;
            ExecuteActions(_currentState.EnterActions);

            if (EnableLogging)
                Log($"FSM initialized with entry point: {_currentState.Id}");
        }

        public void Tick()
        {
            if (_currentState == null)
                return;

            // Step 1: Execute global states' actions FIRST
            foreach (var globalState in _globalStates)
            {
                ExecuteActions(globalState.TickActions);
            }

            // Step 2: Evaluate transitions from the current state
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

            // Step 3: If no transition from current state, evaluate ANY_STATE
            if (_anyState != null)
            {
                foreach (var transition in _transitions)
                {
                    if (transition.From != _anyState.Id)
                        continue;

                    if (IsTransitionValid(transition))
                    {
                        if (EnableLogging)
                            Log($"ANY_STATE transition triggered: {_currentState.Id} -> {transition.To}");

                        ChangeState(transition.To);
                        return;
                    }
                }
            }

            // Step 4: If no transition was valid, execute tick actions of the current state
            ExecuteActions(_currentState.TickActions);
        }

        private bool IsTransitionValid(FSMTransition transition)
        {
            if (transition.Condition == null)
                return true;

            return _conditionEvaluator.Evaluate(transition.Condition);
        }

        private void ChangeState(string newStateId)
        {
            if (!_states.TryGetValue(newStateId, out var newState))
                throw new Exception($"State {newStateId} not found.");

            // Don't allow changing to ANY_STATE
            if (newState.IsAnyState)
                throw new Exception("Cannot change to ANY_STATE.");

            if (EnableLogging)
                Log($"State transition: {_currentState.Id} -> {newStateId}");

            // Execute exit actions of the current state
            ExecuteActions(_currentState.ExitActions);

            _currentState = newState;

            OnStateChanged?.Invoke(newStateId);

            // Execute enter actions of the new state
            ExecuteActions(_currentState.EnterActions);
        }

        private void ExecuteActions(List<ActionDefinition> actions)
        {
            if (actions == null || actions.Count == 0)
                return;

            foreach (var action in actions)
            {
                if (EnableLogging)
                    Log($"Executing action: {action.action}");

                _actionExecutor.Execute(action.action, action.GetParametersAsDictionary());
            }
        }

        private void Log(string message)
        {
            if (EnableLogging)
            {
                UnityEngine.Debug.Log($"[FSM] {message}");
            }
        }
    }
}