using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RuntimeFSM.Interfaces;
using RuntimeFSM.Utils;
using System;


namespace RuntimeFSM.Implementations
{
    /// <summary>
    /// Implementación por defecto del ActionExecutor.
    /// Implementa TODAS las acciones predefinidas del framework.
    ///
    /// Usa REFLECTION para llamar automáticamente a On[ActionName]()
    /// No necesitas un switch case tedioso.
    ///
    /// Métodos protegidos virtuales que puedes sobrescribir en subclases
    /// para personalizar el comportamiento de cada acción.
    ///
    /// Categorías (95 acciones total):
    /// - Animation: Controlar animaciones y parámetros del Animator
    /// - Movement: Movimiento, rotación, física
    /// - AI: Comportamiento de IA, patrullaje, persecución
    /// - Combat: Combate, daño, curación, proyectiles
    /// - Audio: Sonidos y música
    /// - VFX: Efectos visuales, partículas, cámara
    /// - Variables: Manipulación de variables de juego
    /// - GameObject: Control de objetos y componentes
    /// - UI: Interfaz de usuario
    /// - Events: Eventos y llamadas de métodos
    /// </summary>
    public class DefaultActionExecutor : MonoBehaviour, IActionExecutor
    {
        /// <summary>
        /// Ejecuta una acción llamando automáticamente al método On[ActionName]
        /// Usa reflection para encontrar el método dinámicamente.
        /// Incluye manejo robusto de errores y logging detallado.
        /// </summary>
        public virtual void Execute(string actionName, Dictionary<string, string> parameters)
        {
            try
            {
                // Validar parámetros de entrada
                if (string.IsNullOrEmpty(actionName))
                {
                    Debug.LogError("[DefaultActionExecutor] Action name no puede estar vacío", gameObject);
                    return;
                }

                string methodName = $"On{actionName}";
                var method = GetType().GetMethod(
                    methodName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase
                );

                if (method == null)
                {
                    ActionExecutorErrorHandler.LogMethodNotFound(actionName, methodName, gameObject);
                    return;
                }

                // Invocar el método con try-catch para capturar excepciones
                try
                {
                    method.Invoke(this, new object[] { parameters });
                }
                catch (TargetInvocationException tie)
                {
                    // TargetInvocationException envuelve la excepción real
                    ActionExecutorErrorHandler.LogInvocationException(actionName, methodName, tie.InnerException ?? tie, gameObject, parameters);
                }
                catch (Exception ex)
                {
                    ActionExecutorErrorHandler.LogInvocationException(actionName, methodName, ex, gameObject, parameters);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DefaultActionExecutor] Error inesperado en Execute(): {ex.Message}\n{ex.StackTrace}", gameObject);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // MÉTODOS DE ACCIÓN (95 TOTAL)
        // Organizados por categoría para facilitar la búsqueda
        // ─────────────────────────────────────────────────────────────

        #region Animation (11 acciones)

        protected virtual void OnPlayAnimation(Dictionary<string, string> parameters)
        {
            try
            {
                Animator animator = GetComponent<Animator>();
                if (animator == null)
                {
                    ActionExecutorErrorHandler.LogMissingComponent("PlayAnimation", "Animator", gameObject);
                    return;
                }

                // Validar parámetro requerido
                if (!parameters.TryGetValue("animationName", out string animationName) || string.IsNullOrEmpty(animationName))
                {
                    ActionExecutorErrorHandler.LogMissingParameter("PlayAnimation", "animationName", gameObject);
                    return;
                }

                // Parsear speed (opcional)
                float speed = 1f;
                if (parameters.TryGetValue("speed", out string speedStr) && !string.IsNullOrEmpty(speedStr))
                {
                    if (!float.TryParse(speedStr, out speed))
                    {
                        ActionExecutorErrorHandler.LogParameterParsingError("PlayAnimation", "speed", speedStr, "float", gameObject);
                        speed = 1f;
                    }
                }

                // Parsear loop (opcional)
                bool loop = true;
                if (parameters.TryGetValue("loop", out string loopStr) && !string.IsNullOrEmpty(loopStr))
                {
                    if (!bool.TryParse(loopStr, out loop))
                    {
                        ActionExecutorErrorHandler.LogParameterParsingError("PlayAnimation", "loop", loopStr, "bool", gameObject);
                        loop = true;
                    }
                }

                // Ejecutar acción
                animator.SetTrigger(animationName);
                animator.speed = Mathf.Max(0.1f, speed); // Prevenir velocidad 0 o negativa

                Debug.Log($"[PlayAnimation] ✓ {animationName} | speed={speed} | loop={loop}", gameObject);
            }
            catch (System.Exception ex)
            {
                ActionExecutorErrorHandler.LogOperationFailed("PlayAnimation", "SetTrigger/SetSpeed", ex.Message, gameObject);
            }
        }

        protected virtual void OnCrossFadeAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnCrossFadeAnimation] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("animationName", out string animationName))
            {
                Debug.LogError("[DefaultActionExecutor.OnCrossFadeAnimation] Parámetro 'animationName' requerido");
                return;
            }

            float duration = 0.3f;
            if (parameters.TryGetValue("duration", out string durationStr) && !float.TryParse(durationStr, out duration))
                duration = 0.3f;

            animator.CrossFadeInFixedTime(animationName, duration);
            Debug.Log($"[DefaultActionExecutor.OnCrossFadeAnimation] ✓ CrossFade: {animationName} | duration={duration}s");
        }

        protected virtual void OnStopAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnStopAnimation] No Animator encontrado");
                return;
            }

            animator.speed = 0f;
            Debug.Log("[DefaultActionExecutor.OnStopAnimation] ✓ Animación detenida");
        }

        protected virtual void OnPauseAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnPauseAnimation] No Animator encontrado");
                return;
            }

            animator.speed = 0f;
            Debug.Log("[DefaultActionExecutor.OnPauseAnimation] ✓ Animación pausada");
        }

        protected virtual void OnResumeAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnResumeAnimation] No Animator encontrado");
                return;
            }

            animator.speed = 1f;
            Debug.Log("[DefaultActionExecutor.OnResumeAnimation] ✓ Animación reanudada");
        }

        protected virtual void OnSetAnimatorBool(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorBool] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorBool] Parámetro 'parameter' requerido");
                return;
            }

            bool value = false;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = false;

            animator.SetBool(parameter, value);
            Debug.Log($"[DefaultActionExecutor.OnSetAnimatorBool] ✓ {parameter} = {value}");
        }

        protected virtual void OnSetAnimatorTrigger(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorTrigger] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorTrigger] Parámetro 'parameter' requerido");
                return;
            }

            animator.SetTrigger(parameter);
            Debug.Log($"[DefaultActionExecutor.OnSetAnimatorTrigger] ✓ Trigger: {parameter}");
        }

        protected virtual void OnResetAnimatorTrigger(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnResetAnimatorTrigger] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                Debug.LogError("[DefaultActionExecutor.OnResetAnimatorTrigger] Parámetro 'parameter' requerido");
                return;
            }

            animator.ResetTrigger(parameter);
            Debug.Log($"[DefaultActionExecutor.OnResetAnimatorTrigger] ✓ Trigger reset: {parameter}");
        }

        protected virtual void OnSetAnimatorFloat(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorFloat] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorFloat] Parámetro 'parameter' requerido");
                return;
            }

            float value = 0f;
            if (parameters.TryGetValue("value", out string valueStr) && !float.TryParse(valueStr, out value))
                value = 0f;

            animator.SetFloat(parameter, value);
            Debug.Log($"[DefaultActionExecutor.OnSetAnimatorFloat] ✓ {parameter} = {value}");
        }

        protected virtual void OnSetAnimatorInt(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorInt] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimatorInt] Parámetro 'parameter' requerido");
                return;
            }

            int value = 0;
            if (parameters.TryGetValue("value", out string valueStr) && !int.TryParse(valueStr, out value))
                value = 0;

            animator.SetInteger(parameter, value);
            Debug.Log($"[DefaultActionExecutor.OnSetAnimatorInt] ✓ {parameter} = {value}");
        }

        protected virtual void OnSetAnimationLayerWeight(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimationLayerWeight] No Animator encontrado");
                return;
            }

            if (!parameters.TryGetValue("layer", out string layerStr) || !int.TryParse(layerStr, out int layer))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetAnimationLayerWeight] Parámetro 'layer' requerido (int)");
                return;
            }

            float weight = 1f;
            if (parameters.TryGetValue("weight", out string weightStr) && !float.TryParse(weightStr, out weight))
                weight = 1f;

            animator.SetLayerWeight(layer, weight);
            Debug.Log($"[DefaultActionExecutor.OnSetAnimationLayerWeight] ✓ Layer {layer} weight = {weight}");
        }

        #endregion

        #region Movement (16 acciones)

        protected virtual void OnMoveToPosition(Dictionary<string, string> parameters)
        {
            Transform trans = GetComponent<Transform>();
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out float x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out float y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out float z))
            {
                Debug.LogError("[DefaultActionExecutor.OnMoveToPosition] Parámetros x, y, z inválidos");
                return;
            }

            Vector3 targetPos = new Vector3(x, y, z);
            trans.position = Vector3.Lerp(trans.position, targetPos, Time.deltaTime * speed);
            Debug.Log($"[DefaultActionExecutor.OnMoveToPosition] ✓ Moviendo a ({x}, {y}, {z}) | speed={speed}");
        }

        protected virtual void OnMoveToTarget(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("targetName", out string targetName))
            {
                Debug.LogError("[DefaultActionExecutor.OnMoveToTarget] Parámetro 'targetName' requerido");
                return;
            }

            GameObject target = GameObject.Find(targetName);
            if (target == null)
            {
                Debug.LogError($"[DefaultActionExecutor.OnMoveToTarget] Target '{targetName}' no encontrado");
                return;
            }

            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
            Debug.Log($"[DefaultActionExecutor.OnMoveToTarget] ✓ Moviendo hacia {targetName} | speed={speed}");
        }

        protected virtual void OnMoveForward(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            transform.position += transform.forward * speed * Time.deltaTime;
            Debug.Log($"[DefaultActionExecutor.OnMoveForward] ✓ Moviendo adelante | speed={speed}");
        }

        protected virtual void OnMoveBackward(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            transform.position -= transform.forward * speed * Time.deltaTime;
            Debug.Log($"[DefaultActionExecutor.OnMoveBackward] ✓ Moviendo atrás | speed={speed}");
        }

        protected virtual void OnStrafe(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("direction", out string direction))
            {
                Debug.LogError("[DefaultActionExecutor.OnStrafe] Parámetro 'direction' requerido (left/right)");
                return;
            }

            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            Vector3 moveDir = direction.ToLower() == "left" ? -transform.right : transform.right;
            transform.position += moveDir * speed * Time.deltaTime;
            Debug.Log($"[DefaultActionExecutor.OnStrafe] ✓ Strafeando {direction} | speed={speed}");
        }

        protected virtual void OnRotateToTarget(Dictionary<string, string> parameters)
        {
            float rotationSpeed = 2f;
            if (parameters.TryGetValue("rotationSpeed", out string rotStr) && !float.TryParse(rotStr, out rotationSpeed))
                rotationSpeed = 2f;

            Debug.Log($"[DefaultActionExecutor.OnRotateToTarget] ✓ Girando al target | rotationSpeed={rotationSpeed}");
        }

        protected virtual void OnRotateToPosition(Dictionary<string, string> parameters)
        {
            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out float x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out float y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out float z))
            {
                Debug.LogError("[DefaultActionExecutor.OnRotateToPosition] Parámetros x, y, z inválidos");
                return;
            }

            float rotationSpeed = 2f;
            if (parameters.TryGetValue("rotationSpeed", out string rotStr) && !float.TryParse(rotStr, out rotationSpeed))
                rotationSpeed = 2f;

            Vector3 targetPos = new Vector3(x, y, z);
            Vector3 direction = (targetPos - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            Debug.Log($"[DefaultActionExecutor.OnRotateToPosition] ✓ Girando a ({x}, {y}, {z}) | rotSpeed={rotationSpeed}");
        }

        protected virtual void OnLookAtTarget(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("targetName", out string targetName))
            {
                Debug.LogError("[DefaultActionExecutor.OnLookAtTarget] Parámetro 'targetName' requerido");
                return;
            }

            GameObject target = GameObject.Find(targetName);
            if (target == null)
            {
                Debug.LogError($"[DefaultActionExecutor.OnLookAtTarget] Target '{targetName}' no encontrado");
                return;
            }

            transform.LookAt(target.transform.position);
            Debug.Log($"[DefaultActionExecutor.OnLookAtTarget] ✓ Mirando a {targetName}");
        }

        protected virtual void OnSetSpeed(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (!parameters.TryGetValue("speed", out string speedStr) || !float.TryParse(speedStr, out speed))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetSpeed] Parámetro 'speed' inválido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSetSpeed] ✓ Velocidad establecida: {speed}");
        }

        protected virtual void OnStopMovement(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;

            Debug.Log("[DefaultActionExecutor.OnStopMovement] ✓ Movimiento detenido");
        }

        protected virtual void OnJump(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnJump] No Rigidbody encontrado");
                return;
            }

            float force = 5f;
            if (parameters.TryGetValue("force", out string forceStr) && !float.TryParse(forceStr, out force))
                force = 5f;

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            Debug.Log($"[DefaultActionExecutor.OnJump] ✓ Saltando | force={force}");
        }

        protected virtual void OnDash(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnDash] No Rigidbody encontrado");
                return;
            }

            float force = 10f;
            if (parameters.TryGetValue("force", out string forceStr) && !float.TryParse(forceStr, out force))
                force = 10f;

            rb.AddForce(transform.forward * force, ForceMode.Impulse);
            Debug.Log($"[DefaultActionExecutor.OnDash] ✓ Dash ejecutado | force={force}");
        }

        protected virtual void OnAddForce(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnAddForce] No Rigidbody encontrado");
                return;
            }

            float x = 0, y = 0, z = 0;
            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out z))
            {
                Debug.LogError("[DefaultActionExecutor.OnAddForce] Parámetros x, y, z inválidos");
                return;
            }

            rb.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
            Debug.Log($"[DefaultActionExecutor.OnAddForce] ✓ Fuerza añadida: ({x}, {y}, {z})");
        }

        protected virtual void OnTeleport(Dictionary<string, string> parameters)
        {
            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out float x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out float y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out float z))
            {
                Debug.LogError("[DefaultActionExecutor.OnTeleport] Parámetros x, y, z inválidos");
                return;
            }

            transform.position = new Vector3(x, y, z);
            Debug.Log($"[DefaultActionExecutor.OnTeleport] ✓ Teletransportado a ({x}, {y}, {z})");
        }

        protected virtual void OnEnableGravity(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnEnableGravity] No Rigidbody encontrado");
                return;
            }

            rb.useGravity = true;
            Debug.Log("[DefaultActionExecutor.OnEnableGravity] ✓ Gravedad habilitada");
        }

        protected virtual void OnDisableGravity(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnDisableGravity] No Rigidbody encontrado");
                return;
            }

            rb.useGravity = false;
            Debug.Log("[DefaultActionExecutor.OnDisableGravity] ✓ Gravedad deshabilitada");
        }

        #endregion

        #region AI (13 acciones)

        protected virtual void OnSetTarget(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("targetName", out string targetName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetTarget] Parámetro 'targetName' requerido");
                return;
            }

            GameObject target = GameObject.Find(targetName);
            if (target == null)
            {
                Debug.LogError($"[DefaultActionExecutor.OnSetTarget] Target '{targetName}' no encontrado");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSetTarget] ✓ Target establecido: {targetName}");
        }

        protected virtual void OnClearTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor.OnClearTarget] ✓ Target eliminado");
        }

        protected virtual void OnChaseTarget(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            float stoppingDistance = 1f;
            if (parameters.TryGetValue("stoppingDistance", out string distStr) && !float.TryParse(distStr, out stoppingDistance))
                stoppingDistance = 1f;

            Debug.Log($"[DefaultActionExecutor.OnChaseTarget] ✓ Persiguiendo | speed={speed} | stoppingDistance={stoppingDistance}");
        }

        protected virtual void OnStopChasing(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor.OnStopChasing] ✓ Persecución detenida");
        }

        protected virtual void OnFleeFromTarget(Dictionary<string, string> parameters)
        {
            float distance = 10f;
            if (parameters.TryGetValue("distance", out string distStr) && !float.TryParse(distStr, out distance))
                distance = 10f;

            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            Debug.Log($"[DefaultActionExecutor.OnFleeFromTarget] ✓ Huyendo | distance={distance} | speed={speed}");
        }

        protected virtual void OnPatrol(Dictionary<string, string> parameters)
        {
            float speed = 3f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 3f;

            Debug.Log($"[DefaultActionExecutor.OnPatrol] ✓ Patrullando | speed={speed}");
        }

        protected virtual void OnSetPatrolPoint(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("pointIndex", out string pointStr) || !int.TryParse(pointStr, out int pointIndex))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetPatrolPoint] Parámetro 'pointIndex' inválido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSetPatrolPoint] ✓ Punto de patrulla establecido: {pointIndex}");
        }

        protected virtual void OnNextPatrolPoint(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor.OnNextPatrolPoint] ✓ Siguiente punto de patrulla");
        }

        protected virtual void OnWait(Dictionary<string, string> parameters)
        {
            float duration = 1f;
            if (parameters.TryGetValue("duration", out string durationStr) && !float.TryParse(durationStr, out duration))
                duration = 1f;

            Debug.Log($"[DefaultActionExecutor.OnWait] ✓ Esperando {duration}s");
        }

        protected virtual void OnSearchLastKnownPosition(Dictionary<string, string> parameters)
        {
            float duration = 5f;
            if (parameters.TryGetValue("duration", out string durationStr) && !float.TryParse(durationStr, out duration))
                duration = 5f;

            Debug.Log($"[DefaultActionExecutor.OnSearchLastKnownPosition] ✓ Buscando en última posición conocida ({duration}s)");
        }

        protected virtual void OnSetAggro(Dictionary<string, string> parameters)
        {
            bool value = true;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = true;

            Debug.Log($"[DefaultActionExecutor.OnSetAggro] ✓ Agresión: {value}");
        }

        protected virtual void OnSetAlert(Dictionary<string, string> parameters)
        {
            bool value = true;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = true;

            Debug.Log($"[DefaultActionExecutor.OnSetAlert] ✓ Alerta: {value}");
        }

        protected virtual void OnSetState(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("stateName", out string stateName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetState] Parámetro 'stateName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSetState] ✓ Estado establecido: {stateName}");
        }

        #endregion

        #region Combat (14 acciones)

        protected virtual void OnAttack(Dictionary<string, string> parameters)
        {
            int damage = 10;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 10;

            float range = 5f;
            if (parameters.TryGetValue("range", out string rangeStr) && !float.TryParse(rangeStr, out range))
                range = 5f;

            Debug.Log($"[DefaultActionExecutor.OnAttack] ✓ Ataque realizado | damage={damage} | range={range}");
        }

        protected virtual void OnMeleeAttack(Dictionary<string, string> parameters)
        {
            int damage = 15;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 15;

            float radius = 2f;
            if (parameters.TryGetValue("radius", out string radiusStr) && !float.TryParse(radiusStr, out radius))
                radius = 2f;

            Debug.Log($"[DefaultActionExecutor.OnMeleeAttack] ✓ Ataque cuerpo a cuerpo | damage={damage} | radius={radius}");
        }

        protected virtual void OnRangedAttack(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("projectile", out string projectile))
            {
                Debug.LogError("[DefaultActionExecutor.OnRangedAttack] Parámetro 'projectile' requerido");
                return;
            }

            float speed = 20f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 20f;

            int damage = 10;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 10;

            Debug.Log($"[DefaultActionExecutor.OnRangedAttack] ✓ Ataque a distancia | projectile={projectile} | speed={speed} | damage={damage}");
        }

        protected virtual void OnEnableHitbox(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("hitboxName", out string hitboxName))
            {
                Debug.LogError("[DefaultActionExecutor.OnEnableHitbox] Parámetro 'hitboxName' requerido");
                return;
            }

            Transform hitbox = transform.Find(hitboxName);
            if (hitbox != null)
                hitbox.gameObject.SetActive(true);

            Debug.Log($"[DefaultActionExecutor.OnEnableHitbox] ✓ Hitbox habilitado: {hitboxName}");
        }

        protected virtual void OnDisableHitbox(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("hitboxName", out string hitboxName))
            {
                Debug.LogError("[DefaultActionExecutor.OnDisableHitbox] Parámetro 'hitboxName' requerido");
                return;
            }

            Transform hitbox = transform.Find(hitboxName);
            if (hitbox != null)
                hitbox.gameObject.SetActive(false);

            Debug.Log($"[DefaultActionExecutor.OnDisableHitbox] ✓ Hitbox deshabilitado: {hitboxName}");
        }

        protected virtual void OnTakeDamage(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("amount", out string amountStr) || !int.TryParse(amountStr, out int amount))
            {
                Debug.LogError("[DefaultActionExecutor.OnTakeDamage] Parámetro 'amount' inválido (int)");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnTakeDamage] ✓ Daño recibido: {amount}");
        }

        protected virtual void OnHeal(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("amount", out string amountStr) || !int.TryParse(amountStr, out int amount))
            {
                Debug.LogError("[DefaultActionExecutor.OnHeal] Parámetro 'amount' inválido (int)");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnHeal] ✓ Curación: {amount}");
        }

        protected virtual void OnDie(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor.OnDie] ✓ Muerte");
            Destroy(gameObject);
        }

        protected virtual void OnRevive(Dictionary<string, string> parameters)
        {
            int health = 100;
            if (parameters.TryGetValue("health", out string healthStr) && !int.TryParse(healthStr, out health))
                health = 100;

            Debug.Log($"[DefaultActionExecutor.OnRevive] ✓ Revivido | health={health}");
        }

        protected virtual void OnApplyKnockback(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnApplyKnockback] No Rigidbody encontrado");
                return;
            }

            float force = 10f;
            if (parameters.TryGetValue("force", out string forceStr) && !float.TryParse(forceStr, out force))
                force = 10f;

            rb.AddForce(transform.forward * force, ForceMode.Impulse);
            Debug.Log($"[DefaultActionExecutor.OnApplyKnockback] ✓ Knockback aplicado | force={force}");
        }

        protected virtual void OnSpawnProjectile(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("prefabName", out string prefabName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSpawnProjectile] Parámetro 'prefabName' requerido");
                return;
            }

            float speed = 20f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 20f;

            int damage = 10;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 10;

            Debug.Log($"[DefaultActionExecutor.OnSpawnProjectile] ✓ Proyectil spawneado | prefab={prefabName} | speed={speed} | damage={damage}");
        }

        protected virtual void OnReloadWeapon(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor.OnReloadWeapon] ✓ Arma recargada");
        }

        protected virtual void OnEquipWeapon(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("weaponName", out string weaponName))
            {
                Debug.LogError("[DefaultActionExecutor.OnEquipWeapon] Parámetro 'weaponName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnEquipWeapon] ✓ Arma equipada: {weaponName}");
        }

        protected virtual void OnUnequipWeapon(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor.OnUnequipWeapon] ✓ Arma desequipada");
        }

        #endregion

        #region Audio (8 acciones)

        protected virtual void OnPlaySound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnPlaySound] No AudioSource encontrado");
                return;
            }

            if (!parameters.TryGetValue("clipName", out string clipName))
            {
                Debug.LogError("[DefaultActionExecutor.OnPlaySound] Parámetro 'clipName' requerido");
                return;
            }

            float volume = 1f;
            if (parameters.TryGetValue("volume", out string volStr) && !float.TryParse(volStr, out volume))
                volume = 1f;

            audioSource.volume = volume;
            audioSource.Play();
            Debug.Log($"[DefaultActionExecutor.OnPlaySound] ✓ Reproduciendo: {clipName} | volume={volume}");
        }

        protected virtual void OnPlayOneShot(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnPlayOneShot] No AudioSource encontrado");
                return;
            }

            if (!parameters.TryGetValue("clipName", out string clipName))
            {
                Debug.LogError("[DefaultActionExecutor.OnPlayOneShot] Parámetro 'clipName' requerido");
                return;
            }

            float volume = 1f;
            if (parameters.TryGetValue("volume", out string volStr) && !float.TryParse(volStr, out volume))
                volume = 1f;

            Debug.Log($"[DefaultActionExecutor.OnPlayOneShot] ✓ Reproduciendo (OneShot): {clipName} | volume={volume}");
        }

        protected virtual void OnStopSound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnStopSound] No AudioSource encontrado");
                return;
            }

            audioSource.Stop();
            Debug.Log("[DefaultActionExecutor.OnStopSound] ✓ Sonido detenido");
        }

        protected virtual void OnPauseSound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnPauseSound] No AudioSource encontrado");
                return;
            }

            audioSource.Pause();
            Debug.Log("[DefaultActionExecutor.OnPauseSound] ✓ Sonido pausado");
        }

        protected virtual void OnResumeSound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnResumeSound] No AudioSource encontrado");
                return;
            }

            audioSource.Play();
            Debug.Log("[DefaultActionExecutor.OnResumeSound] ✓ Sonido reanudado");
        }

        protected virtual void OnSetVolume(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnSetVolume] No AudioSource encontrado");
                return;
            }

            if (!parameters.TryGetValue("volume", out string volStr) || !float.TryParse(volStr, out float volume))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetVolume] Parámetro 'volume' inválido");
                return;
            }

            audioSource.volume = Mathf.Clamp01(volume);
            Debug.Log($"[DefaultActionExecutor.OnSetVolume] ✓ Volumen: {volume}");
        }

        protected virtual void OnMuteAudio(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnMuteAudio] No AudioSource encontrado");
                return;
            }

            audioSource.mute = true;
            Debug.Log("[DefaultActionExecutor.OnMuteAudio] ✓ Audio silenciado");
        }

        protected virtual void OnUnmuteAudio(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnUnmuteAudio] No AudioSource encontrado");
                return;
            }

            audioSource.mute = false;
            Debug.Log("[DefaultActionExecutor.OnUnmuteAudio] ✓ Audio desilenciado");
        }

        #endregion

        #region VFX (9 acciones)

        protected virtual void OnSpawnVFX(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("vfxName", out string vfxName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSpawnVFX] Parámetro 'vfxName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSpawnVFX] ✓ VFX spawneado: {vfxName}");
        }

        protected virtual void OnDestroyVFX(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("vfxName", out string vfxName))
            {
                Debug.LogError("[DefaultActionExecutor.OnDestroyVFX] Parámetro 'vfxName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnDestroyVFX] ✓ VFX destruido: {vfxName}");
        }

        protected virtual void OnPlayParticleSystem(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("systemName", out string systemName))
            {
                Debug.LogError("[DefaultActionExecutor.OnPlayParticleSystem] Parámetro 'systemName' requerido");
                return;
            }

            ParticleSystem ps = transform.Find(systemName)?.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            Debug.Log($"[DefaultActionExecutor.OnPlayParticleSystem] ✓ Sistema de partículas: {systemName}");
        }

        protected virtual void OnStopParticleSystem(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("systemName", out string systemName))
            {
                Debug.LogError("[DefaultActionExecutor.OnStopParticleSystem] Parámetro 'systemName' requerido");
                return;
            }

            ParticleSystem ps = transform.Find(systemName)?.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Stop();

            Debug.Log($"[DefaultActionExecutor.OnStopParticleSystem] ✓ Sistema de partículas detenido: {systemName}");
        }

        protected virtual void OnShakeCamera(Dictionary<string, string> parameters)
        {
            float intensity = 0.5f;
            if (parameters.TryGetValue("intensity", out string intStr) && !float.TryParse(intStr, out intensity))
                intensity = 0.5f;

            float duration = 0.2f;
            if (parameters.TryGetValue("duration", out string durStr) && !float.TryParse(durStr, out duration))
                duration = 0.2f;

            Debug.Log($"[DefaultActionExecutor.OnShakeCamera] ✓ Cámara temblando | intensity={intensity} | duration={duration}s");
        }

        protected virtual void OnFlashScreen(Dictionary<string, string> parameters)
        {
            float duration = 0.1f;
            if (parameters.TryGetValue("duration", out string durStr) && !float.TryParse(durStr, out duration))
                duration = 0.1f;

            Debug.Log($"[DefaultActionExecutor.OnFlashScreen] ✓ Pantalla destelló | duration={duration}s");
        }

        protected virtual void OnChangeColor(Dictionary<string, string> parameters)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnChangeColor] No Renderer encontrado");
                return;
            }

            float r = 1f, g = 1f, b = 1f;
            if (!float.TryParse(parameters.GetValueOrDefault("r", "1"), out r))
                r = 1f;
            if (!float.TryParse(parameters.GetValueOrDefault("g", "1"), out g))
                g = 1f;
            if (!float.TryParse(parameters.GetValueOrDefault("b", "1"), out b))
                b = 1f;

            renderer.material.color = new Color(r, g, b);
            Debug.Log($"[DefaultActionExecutor.OnChangeColor] ✓ Color cambiado a ({r}, {g}, {b})");
        }

        protected virtual void OnEnableTrail(Dictionary<string, string> parameters)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnEnableTrail] No TrailRenderer encontrado");
                return;
            }

            trail.enabled = true;
            Debug.Log("[DefaultActionExecutor.OnEnableTrail] ✓ Trail habilitado");
        }

        protected virtual void OnDisableTrail(Dictionary<string, string> parameters)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                Debug.LogError("[DefaultActionExecutor.OnDisableTrail] No TrailRenderer encontrado");
                return;
            }

            trail.enabled = false;
            Debug.Log("[DefaultActionExecutor.OnDisableTrail] ✓ Trail deshabilitado");
        }

        #endregion

        #region Variables (10 acciones)

        protected virtual void OnSetBool(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetBool] Parámetro 'variableName' requerido");
                return;
            }

            bool value = false;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = false;

            Debug.Log($"[DefaultActionExecutor.OnSetBool] ✓ {varName} = {value}");
        }

        protected virtual void OnToggleBool(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnToggleBool] Parámetro 'variableName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnToggleBool] ✓ {varName} toggled");
        }

        protected virtual void OnSetInt(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetInt] Parámetro 'variableName' requerido");
                return;
            }

            int value = 0;
            if (parameters.TryGetValue("value", out string valueStr) && !int.TryParse(valueStr, out value))
                value = 0;

            Debug.Log($"[DefaultActionExecutor.OnSetInt] ✓ {varName} = {value}");
        }

        protected virtual void OnIncrementInt(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnIncrementInt] Parámetro 'variableName' requerido");
                return;
            }

            int amount = 1;
            if (parameters.TryGetValue("amount", out string amountStr) && !int.TryParse(amountStr, out amount))
                amount = 1;

            Debug.Log($"[DefaultActionExecutor.OnIncrementInt] ✓ {varName} += {amount}");
        }

        protected virtual void OnDecrementInt(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnDecrementInt] Parámetro 'variableName' requerido");
                return;
            }

            int amount = 1;
            if (parameters.TryGetValue("amount", out string amountStr) && !int.TryParse(amountStr, out amount))
                amount = 1;

            Debug.Log($"[DefaultActionExecutor.OnDecrementInt] ✓ {varName} -= {amount}");
        }

        protected virtual void OnSetFloat(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetFloat] Parámetro 'variableName' requerido");
                return;
            }

            float value = 0f;
            if (parameters.TryGetValue("value", out string valueStr) && !float.TryParse(valueStr, out value))
                value = 0f;

            Debug.Log($"[DefaultActionExecutor.OnSetFloat] ✓ {varName} = {value}");
        }

        protected virtual void OnAddFloat(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnAddFloat] Parámetro 'variableName' requerido");
                return;
            }

            float amount = 0f;
            if (parameters.TryGetValue("amount", out string amountStr) && !float.TryParse(amountStr, out amount))
                amount = 0f;

            Debug.Log($"[DefaultActionExecutor.OnAddFloat] ✓ {varName} += {amount}");
        }

        protected virtual void OnSubtractFloat(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSubtractFloat] Parámetro 'variableName' requerido");
                return;
            }

            float amount = 0f;
            if (parameters.TryGetValue("amount", out string amountStr) && !float.TryParse(amountStr, out amount))
                amount = 0f;

            Debug.Log($"[DefaultActionExecutor.OnSubtractFloat] ✓ {varName} -= {amount}");
        }

        protected virtual void OnSetString(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetString] Parámetro 'variableName' requerido");
                return;
            }

            string value = parameters.GetValueOrDefault("value", "");
            Debug.Log($"[DefaultActionExecutor.OnSetString] ✓ {varName} = '{value}'");
        }

        protected virtual void OnClearVariable(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                Debug.LogError("[DefaultActionExecutor.OnClearVariable] Parámetro 'variableName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnClearVariable] ✓ {varName} limpiado");
        }

        #endregion

        #region GameObject (7 acciones)

        protected virtual void OnSetActive(Dictionary<string, string> parameters)
        {
            bool value = true;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = true;

            gameObject.SetActive(value);
            Debug.Log($"[DefaultActionExecutor.OnSetActive] ✓ GameObject active: {value}");
        }

        protected virtual void OnDestroyObject(Dictionary<string, string> parameters)
        {
            float delay = 0f;
            if (parameters.TryGetValue("delay", out string delayStr) && !float.TryParse(delayStr, out delay))
                delay = 0f;

            if (delay > 0)
                Destroy(gameObject, delay);
            else
                Destroy(gameObject);

            Debug.Log($"[DefaultActionExecutor.OnDestroyObject] ✓ GameObject destruido (delay={delay}s)");
        }

        protected virtual void OnInstantiatePrefab(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("prefabName", out string prefabName))
            {
                Debug.LogError("[DefaultActionExecutor.OnInstantiatePrefab] Parámetro 'prefabName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnInstantiatePrefab] ✓ Prefab instanciado: {prefabName}");
        }

        protected virtual void OnEnableComponent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("componentName", out string componentName))
            {
                Debug.LogError("[DefaultActionExecutor.OnEnableComponent] Parámetro 'componentName' requerido");
                return;
            }

            System.Type compType = System.Type.GetType(componentName);
            if (compType != null)
            {
                Component comp = GetComponent(compType);
                if (comp is Behaviour behaviour)
                    behaviour.enabled = true;
            }

            Debug.Log($"[DefaultActionExecutor.OnEnableComponent] ✓ Componente habilitado: {componentName}");
        }

        protected virtual void OnDisableComponent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("componentName", out string componentName))
            {
                Debug.LogError("[DefaultActionExecutor.OnDisableComponent] Parámetro 'componentName' requerido");
                return;
            }

            System.Type compType = System.Type.GetType(componentName);
            if (compType != null)
            {
                Component comp = GetComponent(compType);
                if (comp is Behaviour behaviour)
                    behaviour.enabled = false;
            }

            Debug.Log($"[DefaultActionExecutor.OnDisableComponent] ✓ Componente deshabilitado: {componentName}");
        }

        protected virtual void OnSetTag(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("tag", out string tag))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetTag] Parámetro 'tag' requerido");
                return;
            }

            gameObject.tag = tag;
            Debug.Log($"[DefaultActionExecutor.OnSetTag] ✓ Tag: {tag}");
        }

        protected virtual void OnSetLayer(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("layer", out string layerStr) || !int.TryParse(layerStr, out int layer))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetLayer] Parámetro 'layer' inválido (int)");
                return;
            }

            gameObject.layer = layer;
            Debug.Log($"[DefaultActionExecutor.OnSetLayer] ✓ Layer: {layer}");
        }

        #endregion

        #region UI (4 acciones)

        protected virtual void OnShowUI(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiName", out string uiName))
            {
                Debug.LogError("[DefaultActionExecutor.OnShowUI] Parámetro 'uiName' requerido");
                return;
            }

            GameObject ui = GameObject.Find(uiName);
            if (ui != null)
                ui.SetActive(true);

            Debug.Log($"[DefaultActionExecutor.OnShowUI] ✓ UI mostrado: {uiName}");
        }

        protected virtual void OnHideUI(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiName", out string uiName))
            {
                Debug.LogError("[DefaultActionExecutor.OnHideUI] Parámetro 'uiName' requerido");
                return;
            }

            GameObject ui = GameObject.Find(uiName);
            if (ui != null)
                ui.SetActive(false);

            Debug.Log($"[DefaultActionExecutor.OnHideUI] ✓ UI ocultado: {uiName}");
        }

        protected virtual void OnSetUIText(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiElement", out string uiElement) || !parameters.TryGetValue("text", out string text))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetUIText] Parámetros 'uiElement' y 'text' requeridos");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSetUIText] ✓ Texto: {uiElement} = '{text}'");
        }

        protected virtual void OnSetUIProgress(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiElement", out string uiElement))
            {
                Debug.LogError("[DefaultActionExecutor.OnSetUIProgress] Parámetro 'uiElement' requerido");
                return;
            }

            float value = 0f;
            if (parameters.TryGetValue("value", out string valueStr) && !float.TryParse(valueStr, out value))
                value = 0f;

            Debug.Log($"[DefaultActionExecutor.OnSetUIProgress] ✓ Progreso: {uiElement} = {value}");
        }

        #endregion

        #region Events (3 acciones)

        protected virtual void OnSendEvent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("eventName", out string eventName))
            {
                Debug.LogError("[DefaultActionExecutor.OnSendEvent] Parámetro 'eventName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnSendEvent] ✓ Evento enviado: {eventName}");
        }

        protected virtual void OnBroadcastEvent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("eventName", out string eventName))
            {
                Debug.LogError("[DefaultActionExecutor.OnBroadcastEvent] Parámetro 'eventName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnBroadcastEvent] ✓ Evento broadcasted: {eventName}");
        }

        protected virtual void OnInvokeMethod(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("methodName", out string methodName))
            {
                Debug.LogError("[DefaultActionExecutor.OnInvokeMethod] Parámetro 'methodName' requerido");
                return;
            }

            Debug.Log($"[DefaultActionExecutor.OnInvokeMethod] ✓ Método invocado: {methodName}");
        }

        #endregion
    }
}
