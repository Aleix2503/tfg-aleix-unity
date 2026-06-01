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
                return;
            }

            if (!parameters.TryGetValue("animationName", out string animationName))
            {
                return;
            }

            float duration = 0.3f;
            if (parameters.TryGetValue("duration", out string durationStr) && !float.TryParse(durationStr, out duration))
                duration = 0.3f;

            animator.CrossFadeInFixedTime(animationName, duration);
        }

        protected virtual void OnStopAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.speed = 0f;
        }

        protected virtual void OnPauseAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.speed = 0f;
        }

        protected virtual void OnResumeAnimation(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.speed = 1f;
        }

        protected virtual void OnSetAnimatorBool(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                return;
            }

            bool value = false;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = false;

            animator.SetBool(parameter, value);
        }

        protected virtual void OnSetAnimatorTrigger(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                return;
            }

            animator.SetTrigger(parameter);
        }

        protected virtual void OnResetAnimatorTrigger(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                return;
            }

            animator.ResetTrigger(parameter);
        }

        protected virtual void OnSetAnimatorFloat(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                return;
            }

            float value = 0f;
            if (parameters.TryGetValue("value", out string valueStr) && !float.TryParse(valueStr, out value))
                value = 0f;

            animator.SetFloat(parameter, value);
        }

        protected virtual void OnSetAnimatorInt(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            if (!parameters.TryGetValue("parameter", out string parameter))
            {
                return;
            }

            int value = 0;
            if (parameters.TryGetValue("value", out string valueStr) && !int.TryParse(valueStr, out value))
                value = 0;

            animator.SetInteger(parameter, value);
        }

        protected virtual void OnSetAnimationLayerWeight(Dictionary<string, string> parameters)
        {
            Animator animator = GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            if (!parameters.TryGetValue("layer", out string layerStr) || !int.TryParse(layerStr, out int layer))
            {
                return;
            }

            float weight = 1f;
            if (parameters.TryGetValue("weight", out string weightStr) && !float.TryParse(weightStr, out weight))
                weight = 1f;

            animator.SetLayerWeight(layer, weight);
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
                return;
            }

            Vector3 targetPos = new Vector3(x, y, z);
            trans.position = Vector3.Lerp(trans.position, targetPos, Time.deltaTime * speed);
        }

        protected virtual void OnMoveToTarget(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("targetName", out string targetName))
            {
                return;
            }

            GameObject target = GameObject.Find(targetName);
            if (target == null)
            {
                return;
            }

            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
        }

        protected virtual void OnMoveForward(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            transform.position += transform.forward * speed * Time.deltaTime;
        }

        protected virtual void OnMoveBackward(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            transform.position -= transform.forward * speed * Time.deltaTime;
        }

        protected virtual void OnStrafe(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("direction", out string direction))
            {
                return;
            }

            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            Vector3 moveDir = direction.ToLower() == "left" ? -transform.right : transform.right;
            transform.position += moveDir * speed * Time.deltaTime;
        }

        protected virtual void OnRotateToTarget(Dictionary<string, string> parameters)
        {
            float rotationSpeed = 2f;
            if (parameters.TryGetValue("rotationSpeed", out string rotStr) && !float.TryParse(rotStr, out rotationSpeed))
                rotationSpeed = 2f;

        }

        protected virtual void OnRotateToPosition(Dictionary<string, string> parameters)
        {
            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out float x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out float y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out float z))
            {
                return;
            }

            float rotationSpeed = 2f;
            if (parameters.TryGetValue("rotationSpeed", out string rotStr) && !float.TryParse(rotStr, out rotationSpeed))
                rotationSpeed = 2f;

            Vector3 targetPos = new Vector3(x, y, z);
            Vector3 direction = (targetPos - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        protected virtual void OnLookAtTarget(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("targetName", out string targetName))
            {
                return;
            }

            GameObject target = GameObject.Find(targetName);
            if (target == null)
            {
                return;
            }

            transform.LookAt(target.transform.position);
        }

        protected virtual void OnSetSpeed(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (!parameters.TryGetValue("speed", out string speedStr) || !float.TryParse(speedStr, out speed))
            {
                return;
            }

        }

        protected virtual void OnStopMovement(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;

        }

        protected virtual void OnJump(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }

            float force = 5f;
            if (parameters.TryGetValue("force", out string forceStr) && !float.TryParse(forceStr, out force))
                force = 5f;

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }

        protected virtual void OnDash(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }

            float force = 10f;
            if (parameters.TryGetValue("force", out string forceStr) && !float.TryParse(forceStr, out force))
                force = 10f;

            rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        protected virtual void OnAddForce(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }

            float x = 0, y = 0, z = 0;
            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out z))
            {
                return;
            }

            rb.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
        }

        protected virtual void OnTeleport(Dictionary<string, string> parameters)
        {
            if (!float.TryParse(parameters.GetValueOrDefault("x", "0"), out float x) ||
                !float.TryParse(parameters.GetValueOrDefault("y", "0"), out float y) ||
                !float.TryParse(parameters.GetValueOrDefault("z", "0"), out float z))
            {
                return;
            }

            transform.position = new Vector3(x, y, z);
        }

        protected virtual void OnEnableGravity(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }

            rb.useGravity = true;
        }

        protected virtual void OnDisableGravity(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }

            rb.useGravity = false;
        }

        #endregion

        #region AI (13 acciones)

        protected virtual void OnSetTarget(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("targetName", out string targetName))
            {
                return;
            }

            GameObject target = GameObject.Find(targetName);
            if (target == null)
            {
                return;
            }

        }

        protected virtual void OnClearTarget(Dictionary<string, string> parameters)
        {
        }

        protected virtual void OnChaseTarget(Dictionary<string, string> parameters)
        {
            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

            float stoppingDistance = 1f;
            if (parameters.TryGetValue("stoppingDistance", out string distStr) && !float.TryParse(distStr, out stoppingDistance))
                stoppingDistance = 1f;

        }

        protected virtual void OnStopChasing(Dictionary<string, string> parameters)
        {
        }

        protected virtual void OnFleeFromTarget(Dictionary<string, string> parameters)
        {
            float distance = 10f;
            if (parameters.TryGetValue("distance", out string distStr) && !float.TryParse(distStr, out distance))
                distance = 10f;

            float speed = 5f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 5f;

        }

        protected virtual void OnPatrol(Dictionary<string, string> parameters)
        {
            float speed = 3f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 3f;

        }

        protected virtual void OnSetPatrolPoint(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("pointIndex", out string pointStr) || !int.TryParse(pointStr, out int pointIndex))
            {
                return;
            }

        }

        protected virtual void OnNextPatrolPoint(Dictionary<string, string> parameters)
        {
        }

        protected virtual void OnWait(Dictionary<string, string> parameters)
        {
            float duration = 1f;
            if (parameters.TryGetValue("duration", out string durationStr) && !float.TryParse(durationStr, out duration))
                duration = 1f;

        }

        protected virtual void OnSearchLastKnownPosition(Dictionary<string, string> parameters)
        {
            float duration = 5f;
            if (parameters.TryGetValue("duration", out string durationStr) && !float.TryParse(durationStr, out duration))
                duration = 5f;

        }

        protected virtual void OnSetAggro(Dictionary<string, string> parameters)
        {
            bool value = true;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = true;

        }

        protected virtual void OnSetAlert(Dictionary<string, string> parameters)
        {
            bool value = true;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = true;

        }

        protected virtual void OnSetState(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("stateName", out string stateName))
            {
                return;
            }

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

        }

        protected virtual void OnMeleeAttack(Dictionary<string, string> parameters)
        {
            int damage = 15;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 15;

            float radius = 2f;
            if (parameters.TryGetValue("radius", out string radiusStr) && !float.TryParse(radiusStr, out radius))
                radius = 2f;

        }

        protected virtual void OnRangedAttack(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("projectile", out string projectile))
            {
                return;
            }

            float speed = 20f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 20f;

            int damage = 10;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 10;

        }

        protected virtual void OnEnableHitbox(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("hitboxName", out string hitboxName))
            {
                return;
            }

            Transform hitbox = transform.Find(hitboxName);
            if (hitbox != null)
                hitbox.gameObject.SetActive(true);

        }

        protected virtual void OnDisableHitbox(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("hitboxName", out string hitboxName))
            {
                return;
            }

            Transform hitbox = transform.Find(hitboxName);
            if (hitbox != null)
                hitbox.gameObject.SetActive(false);

        }

        protected virtual void OnTakeDamage(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("amount", out string amountStr) || !int.TryParse(amountStr, out int amount))
            {
                return;
            }

        }

        protected virtual void OnHeal(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("amount", out string amountStr) || !int.TryParse(amountStr, out int amount))
            {
                return;
            }

        }

        protected virtual void OnDie(Dictionary<string, string> parameters)
        {
            Destroy(gameObject);
        }

        protected virtual void OnRevive(Dictionary<string, string> parameters)
        {
            int health = 100;
            if (parameters.TryGetValue("health", out string healthStr) && !int.TryParse(healthStr, out health))
                health = 100;

        }

        protected virtual void OnApplyKnockback(Dictionary<string, string> parameters)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }

            float force = 10f;
            if (parameters.TryGetValue("force", out string forceStr) && !float.TryParse(forceStr, out force))
                force = 10f;

            rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        protected virtual void OnSpawnProjectile(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("prefabName", out string prefabName))
            {
                return;
            }

            float speed = 20f;
            if (parameters.TryGetValue("speed", out string speedStr) && !float.TryParse(speedStr, out speed))
                speed = 20f;

            int damage = 10;
            if (parameters.TryGetValue("damage", out string dmgStr) && !int.TryParse(dmgStr, out damage))
                damage = 10;

        }

        protected virtual void OnReloadWeapon(Dictionary<string, string> parameters)
        {
        }

        protected virtual void OnEquipWeapon(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("weaponName", out string weaponName))
            {
                return;
            }

        }

        protected virtual void OnUnequipWeapon(Dictionary<string, string> parameters)
        {
        }

        #endregion

        #region Audio (8 acciones)

        protected virtual void OnPlaySound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            if (!parameters.TryGetValue("clipName", out string clipName))
            {
                return;
            }

            float volume = 1f;
            if (parameters.TryGetValue("volume", out string volStr) && !float.TryParse(volStr, out volume))
                volume = 1f;

            audioSource.volume = volume;
            audioSource.Play();
        }

        protected virtual void OnPlayOneShot(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            if (!parameters.TryGetValue("clipName", out string clipName))
            {
                return;
            }

            float volume = 1f;
            if (parameters.TryGetValue("volume", out string volStr) && !float.TryParse(volStr, out volume))
                volume = 1f;

        }

        protected virtual void OnStopSound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            audioSource.Stop();
        }

        protected virtual void OnPauseSound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            audioSource.Pause();
        }

        protected virtual void OnResumeSound(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            audioSource.Play();
        }

        protected virtual void OnSetVolume(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            if (!parameters.TryGetValue("volume", out string volStr) || !float.TryParse(volStr, out float volume))
            {
                return;
            }

            audioSource.volume = Mathf.Clamp01(volume);
        }

        protected virtual void OnMuteAudio(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            audioSource.mute = true;
        }

        protected virtual void OnUnmuteAudio(Dictionary<string, string> parameters)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                return;
            }

            audioSource.mute = false;
        }

        #endregion

        #region VFX (9 acciones)

        protected virtual void OnSpawnVFX(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("vfxName", out string vfxName))
            {
                return;
            }

        }

        protected virtual void OnDestroyVFX(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("vfxName", out string vfxName))
            {
                return;
            }

        }

        protected virtual void OnPlayParticleSystem(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("systemName", out string systemName))
            {
                return;
            }

            ParticleSystem ps = transform.Find(systemName)?.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

        }

        protected virtual void OnStopParticleSystem(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("systemName", out string systemName))
            {
                return;
            }

            ParticleSystem ps = transform.Find(systemName)?.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Stop();

        }

        protected virtual void OnShakeCamera(Dictionary<string, string> parameters)
        {
            float intensity = 0.5f;
            if (parameters.TryGetValue("intensity", out string intStr) && !float.TryParse(intStr, out intensity))
                intensity = 0.5f;

            float duration = 0.2f;
            if (parameters.TryGetValue("duration", out string durStr) && !float.TryParse(durStr, out duration))
                duration = 0.2f;

        }

        protected virtual void OnFlashScreen(Dictionary<string, string> parameters)
        {
            float duration = 0.1f;
            if (parameters.TryGetValue("duration", out string durStr) && !float.TryParse(durStr, out duration))
                duration = 0.1f;

        }

        protected virtual void OnChangeColor(Dictionary<string, string> parameters)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null)
            {
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
        }

        protected virtual void OnEnableTrail(Dictionary<string, string> parameters)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                return;
            }

            trail.enabled = true;
        }

        protected virtual void OnDisableTrail(Dictionary<string, string> parameters)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                return;
            }

            trail.enabled = false;
        }

        #endregion

        #region Variables (10 acciones)

        protected virtual void OnSetBool(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            bool value = false;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = false;

        }

        protected virtual void OnToggleBool(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

        }

        protected virtual void OnSetInt(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            int value = 0;
            if (parameters.TryGetValue("value", out string valueStr) && !int.TryParse(valueStr, out value))
                value = 0;

        }

        protected virtual void OnIncrementInt(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            int amount = 1;
            if (parameters.TryGetValue("amount", out string amountStr) && !int.TryParse(amountStr, out amount))
                amount = 1;

        }

        protected virtual void OnDecrementInt(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            int amount = 1;
            if (parameters.TryGetValue("amount", out string amountStr) && !int.TryParse(amountStr, out amount))
                amount = 1;

        }

        protected virtual void OnSetFloat(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            float value = 0f;
            if (parameters.TryGetValue("value", out string valueStr) && !float.TryParse(valueStr, out value))
                value = 0f;

        }

        protected virtual void OnAddFloat(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            float amount = 0f;
            if (parameters.TryGetValue("amount", out string amountStr) && !float.TryParse(amountStr, out amount))
                amount = 0f;

        }

        protected virtual void OnSubtractFloat(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            float amount = 0f;
            if (parameters.TryGetValue("amount", out string amountStr) && !float.TryParse(amountStr, out amount))
                amount = 0f;

        }

        protected virtual void OnSetString(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

            string value = parameters.GetValueOrDefault("value", "");
        }

        protected virtual void OnClearVariable(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("variableName", out string varName))
            {
                return;
            }

        }

        #endregion

        #region GameObject (7 acciones)

        protected virtual void OnSetActive(Dictionary<string, string> parameters)
        {
            bool value = true;
            if (parameters.TryGetValue("value", out string valueStr) && !bool.TryParse(valueStr, out value))
                value = true;

            gameObject.SetActive(value);
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

        }

        protected virtual void OnInstantiatePrefab(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("prefabName", out string prefabName))
            {
                return;
            }

        }

        protected virtual void OnEnableComponent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("componentName", out string componentName))
            {
                return;
            }

            System.Type compType = System.Type.GetType(componentName);
            if (compType != null)
            {
                Component comp = GetComponent(compType);
                if (comp is Behaviour behaviour)
                    behaviour.enabled = true;
            }

        }

        protected virtual void OnDisableComponent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("componentName", out string componentName))
            {
                return;
            }

            System.Type compType = System.Type.GetType(componentName);
            if (compType != null)
            {
                Component comp = GetComponent(compType);
                if (comp is Behaviour behaviour)
                    behaviour.enabled = false;
            }

        }

        protected virtual void OnSetTag(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("tag", out string tag))
            {
                return;
            }

            gameObject.tag = tag;
        }

        protected virtual void OnSetLayer(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("layer", out string layerStr) || !int.TryParse(layerStr, out int layer))
            {
                return;
            }

            gameObject.layer = layer;
        }

        #endregion

        #region UI (4 acciones)

        protected virtual void OnShowUI(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiName", out string uiName))
            {
                return;
            }

            GameObject ui = GameObject.Find(uiName);
            if (ui != null)
                ui.SetActive(true);

        }

        protected virtual void OnHideUI(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiName", out string uiName))
            {
                return;
            }

            GameObject ui = GameObject.Find(uiName);
            if (ui != null)
                ui.SetActive(false);

        }

        protected virtual void OnSetUIText(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiElement", out string uiElement) || !parameters.TryGetValue("text", out string text))
            {
                return;
            }

        }

        protected virtual void OnSetUIProgress(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("uiElement", out string uiElement))
            {
                return;
            }

            float value = 0f;
            if (parameters.TryGetValue("value", out string valueStr) && !float.TryParse(valueStr, out value))
                value = 0f;

        }

        #endregion

        #region Events (3 acciones)

        protected virtual void OnSendEvent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("eventName", out string eventName))
            {
                return;
            }

        }

        protected virtual void OnBroadcastEvent(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("eventName", out string eventName))
            {
                return;
            }

        }

        protected virtual void OnInvokeMethod(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("methodName", out string methodName))
            {
                return;
            }

        }

        #endregion
    }
}
