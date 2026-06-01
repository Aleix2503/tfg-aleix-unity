using System.Collections.Generic;
using UnityEngine;
using RuntimeFSM.Data;
using RuntimeFSM.Interfaces;
using RuntimeFSM.Implementations;
using RuntimeFSM.Utils;

namespace RuntimeFSM.Examples
{
    /// <summary>
    /// Ejemplo completo de cómo usar el RuntimeFSM plugin.
    ///
    /// Este ejemplo demuestra:
    /// 1. Crear un ActionExecutor personalizado (NPCActionExecutor)
    /// 2. Crear un ConditionEvaluator personalizado (NPCConditions)
    /// 3. Inicializar y ejecutar un FSM
    /// 4. Actualizar variables que usa el FSM
    /// </summary>
    public class ExampleNPCController : MonoBehaviour
    {
        [SerializeField] private TextAsset _fsmDefinition;
        private FSM _fsm;
        private NPCActionExecutor _actionExecutor;
        private NPCConditions _conditions;

        private void Start()
        {
            // 1. Crear componentes necesarios
            _actionExecutor = gameObject.AddComponent<NPCActionExecutor>();
            _conditions = gameObject.AddComponent<NPCConditions>();

            // 2. Cargar definición del FSM
            if (_fsmDefinition == null)
            {
                return;
            }

            FSMDefinition fsmDef = JsonUtility.FromJson<FSMDefinition>(_fsmDefinition.text);

            // 3. Inicializar FSM
            _fsm = new FSM(fsmDef, _actionExecutor, _conditions);

        }

        private void Update()
        {
            // Actualizar variables que usa el FSM
            UpdateNPCVariables();

            // Ejecutar un tick del FSM
            _fsm.Tick();
        }

        private void UpdateNPCVariables()
        {
            // Calcular distancia al jugador
            if (GameObject.FindGameObjectWithTag("Player") is GameObject player)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                _conditions.SetDistanceToPlayer(distance);
            }

            // Aquí podrías actualizar otras variables
            // _conditions.SetHealth(_currentHealth);
            // _conditions.SetIsAlerted(_alerted);
        }
    }

    /// <summary>
    /// ActionExecutor personalizado para NPCs.
    /// Implementa todas las acciones específicas del NPC.
    /// </summary>
    public class NPCActionExecutor : DefaultActionExecutor
    {
        private Animator _animator;
        private AudioSource _audioSource;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        // Personalizar cómo se reproducen animaciones
        protected override void OnPlayAnimation(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("animationName", out string animationName))
                return;

            if (_animator != null)
            {
                _animator.SetTrigger(animationName);
            }
        }

        // Personalizar cómo se reproducen sonidos
        protected override void OnPlaySound(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("soundName", out string soundName))
                return;

            if (_audioSource != null)
            {
                // Aquí podrías cargar un clip por nombre desde Resources
                // AudioClip clip = Resources.Load<AudioClip>($"Sounds/{soundName}");
                // _audioSource.PlayOneShot(clip);
            }
        }

        // Personalizar cómo se causa daño
        protected override void OnDealDamage(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("damageAmount", out string damageStr))
                return;

            if (!int.TryParse(damageStr, out int damage))
                return;


            // Encontrar al jugador y causarle daño
            if (GameObject.FindGameObjectWithTag("Player") is GameObject player)
            {
                // var playerHealth = player.GetComponent<PlayerHealth>();
                // playerHealth.TakeDamage(damage);
            }
        }

        // Personalizar loot
        protected override void OnDropLoot(Dictionary<string, string> parameters)
        {
            // Aquí generarías items de loot en la posición actual
        }

        // Personalizar monitoreo de salud
        protected override void OnMonitorHealth(Dictionary<string, string> parameters)
        {
            // Aquí podrías aplicar daño over-time, regeneración, etc.
        }
    }

    /// <summary>
    /// ConditionEvaluator personalizado para NPCs.
    /// Define qué variables usa el FSM y cómo se evalúan.
    /// </summary>
    public class NPCConditions : ConditionEvaluatorBase
    {
        [SerializeField] private float _distanceToPlayer = 999f;
        [SerializeField] private int _health = 100;
        [SerializeField] private bool _isAlerted = false;

        // Métodos públicos para actualizar variables desde el controlador
        public void SetDistanceToPlayer(float distance) => _distanceToPlayer = distance;
        public void SetHealth(int health) => _health = health;
        public void SetIsAlerted(bool alerted) => _isAlerted = alerted;

        protected override bool EvaluateSimpleCondition(ConditionDefinition condition)
        {
            var operatorType = ConditionParser.ParseConditionOperator(condition.@operator);

            // Obtener valor actual de la variable
            float currentValue = GetVariableValue(condition.name);
            float targetValue = float.Parse(condition.value);

            // Evaluar basado en el operador
            bool result = operatorType switch
            {
                ConditionOperator.GreaterThan => currentValue > targetValue,
                ConditionOperator.LessThan => currentValue < targetValue,
                ConditionOperator.GreaterThanOrEqual => currentValue >= targetValue,
                ConditionOperator.LessThanOrEqual => currentValue <= targetValue,
                ConditionOperator.Equals => Mathf.Approximately(currentValue, targetValue),
                ConditionOperator.NotEquals => !Mathf.Approximately(currentValue, targetValue),
                _ => false
            };

            // Debug: mostrar evaluaciones interesantes
            if (condition.name == "distanceToPlayer")
            {
            }

            return result;
        }

        private float GetVariableValue(string variableName)
        {
            return variableName.ToLower() switch
            {
                "distancetoplayer" => _distanceToPlayer,
                "health" => _health,
                "isalerted" => _isAlerted ? 1f : 0f,
                _ => 0f
            };
        }
    }
}
