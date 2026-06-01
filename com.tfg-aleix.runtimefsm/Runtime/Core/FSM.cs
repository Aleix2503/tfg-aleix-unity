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
            // Paso 1: Crear todos los estados
            foreach (var stateDef in definition.states)
            {
                var state = new FSMState(stateDef);

                if (_states.ContainsKey(state.Id))
                    throw new Exception($"Duplicate state id: {state.Id}");

                _states[state.Id] = state;

                // Registrar estados especiales
                if (state.IsAnyState)
                    _anyState = state;

                if (state.IsGlobalState)
                    _globalStates.Add(state);
            }

            // Paso 2: Validar entry point
            var entryPoints = _states.Values.Where(s => s.IsEntryPoint).ToList();

            if (entryPoints.Count != 1)
                throw new Exception(
                    $"FSM debe tener exactamente 1 entry point. Se encontraron: {entryPoints.Count}");

            var entryPointState = entryPoints[0];

            if (entryPointState.Id != definition.initial_state)
                throw new Exception(
                    $"El campo 'initial_state' ({definition.initial_state}) debe apuntar al entry point ({entryPointState.Id})");

            // Paso 3: Crear todas las transiciones
            foreach (var transitionDef in definition.transitions)
            {
                _transitions.Add(new FSMTransition(transitionDef));
            }

            // Paso 4: Validar que transiciones no apunten A ANY_STATE
            foreach (var transition in _transitions)
            {
                if (_anyState != null && transition.To == _anyState.Id)
                    throw new Exception(
                        "No se pueden crear transiciones HACIA ANY_STATE. " +
                        "Solo se pueden crear transiciones DESDE ANY_STATE.");
            }

            // Paso 5: Establecer estado inicial y ejecutar enter actions
            _currentState = entryPointState;
            ExecuteActions(_currentState.EnterActions);

            if (EnableLogging)
                Log($"FSM initialized with entry point: {_currentState.Id}");
        }

        public void Tick()
        {
            if (_currentState == null)
                return;

            // Paso 1: Ejecutar acciones de global states PRIMERO
            foreach (var globalState in _globalStates)
            {
                ExecuteActions(globalState.TickActions);
            }

            // Paso 2: Evaluar transiciones del estado actual
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

            // Paso 3: Si no hay transición del estado actual, evaluar ANY_STATE
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

            // Paso 4: Si ninguna transición fue válida, ejecutar acciones tick del estado actual
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

            // No permitir cambiar hacia ANY_STATE
            if (newState.IsAnyState)
                throw new Exception("No se puede cambiar hacia ANY_STATE.");

            if (EnableLogging)
                Log($"State transition: {_currentState.Id} -> {newStateId}");

            // Ejecutar exit actions del estado actual
            ExecuteActions(_currentState.ExitActions);

            _currentState = newState;

            OnStateChanged?.Invoke(newStateId);

            // Ejecutar enter actions del nuevo estado
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
            }
        }
    }
}