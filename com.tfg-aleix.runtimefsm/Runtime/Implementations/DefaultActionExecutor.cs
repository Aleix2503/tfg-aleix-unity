using System.Collections.Generic;
using UnityEngine;
using RuntimeFSM.Interfaces;

namespace RuntimeFSM.Implementations
{
    /// <summary>
    /// Implementación por defecto del ActionExecutor.
    /// Ejecuta todas las acciones predefinidas del framework.
    ///
    /// Para crear un ejecutor personalizado:
    /// 1. Usa Tools > RuntimeFSM > Generate Action Executor
    /// 2. Hereda de DefaultActionExecutor
    /// 3. Sobrescribe los métodos de acciones que necesites personalizar
    /// </summary>
    public class DefaultActionExecutor : MonoBehaviour, IActionExecutor
    {
        public virtual void Execute(string actionName, Dictionary<string, string> parameters)
        {
            switch (actionName.ToLower())
            {
                case "playanimation":
                    OnPlayAnimation(parameters);
                    break;
                case "playsound":
                    OnPlaySound(parameters);
                    break;
                case "dealdamage":
                    OnDealDamage(parameters);
                    break;
                case "droploot":
                    OnDropLoot(parameters);
                    break;
                case "initializehealthsystem":
                    OnInitializeHealthSystem(parameters);
                    break;
                case "updatehealthui":
                    OnUpdateHealthUI(parameters);
                    break;
                case "applyhealthregen":
                    OnApplyHealthRegen(parameters);
                    break;
                case "shutdownhealthsystem":
                    OnShutdownHealthSystem(parameters);
                    break;
                case "initializeaudiosystem":
                    OnInitializeAudioSystem(parameters);
                    break;
                case "updateambientsound":
                    OnUpdateAmbientSound(parameters);
                    break;
                case "monitorhealth":
                    OnMonitorHealth(parameters);
                    break;
                default:
                    Debug.LogWarning($"[DefaultActionExecutor] Acción desconocida: {actionName}");
                    break;
            }
        }

        #region Acciones de Animación

        /// <summary>
        /// Reproduce una animación
        /// Parámetros esperados: animationName
        /// </summary>
        protected virtual void OnPlayAnimation(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("animationName", out string animationName))
            {
                Debug.LogError("[DefaultActionExecutor] PlayAnimation requiere parámetro 'animationName'");
                return;
            }

            var animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("[DefaultActionExecutor] PlayAnimation: No se encontró Animator en el GameObject");
                return;
            }

            animator.SetTrigger(animationName);
            Debug.Log($"[DefaultActionExecutor] Reproduciendo animación: {animationName}");
        }

        #endregion

        #region Acciones de Audio

        /// <summary>
        /// Reproduce un sonido
        /// Parámetros esperados: soundName
        /// </summary>
        protected virtual void OnPlaySound(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("soundName", out string soundName))
            {
                Debug.LogError("[DefaultActionExecutor] PlaySound requiere parámetro 'soundName'");
                return;
            }

            var audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("[DefaultActionExecutor] PlaySound: No se encontró AudioSource en el GameObject");
                return;
            }

            // Aquí iría la lógica de cargar el clip por nombre y reproducirlo
            Debug.Log($"[DefaultActionExecutor] Reproduciendo sonido: {soundName}");
        }

        /// <summary>
        /// Actualiza el sonido ambiente
        /// Parámetros esperados: volume (0-1)
        /// </summary>
        protected virtual void OnUpdateAmbientSound(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("volume", out string volumeStr))
            {
                Debug.LogError("[DefaultActionExecutor] UpdateAmbientSound requiere parámetro 'volume'");
                return;
            }

            if (!float.TryParse(volumeStr, out float volume))
            {
                Debug.LogError($"[DefaultActionExecutor] UpdateAmbientSound: volumen inválido '{volumeStr}'");
                return;
            }

            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Clamp01(volume);
                Debug.Log($"[DefaultActionExecutor] Volumen ambiente actualizado a: {volume}");
            }
        }

        #endregion

        #region Acciones de Daño

        /// <summary>
        /// Causa daño
        /// Parámetros esperados: damageAmount
        /// </summary>
        protected virtual void OnDealDamage(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("damageAmount", out string damageStr))
            {
                Debug.LogError("[DefaultActionExecutor] DealDamage requiere parámetro 'damageAmount'");
                return;
            }

            if (!int.TryParse(damageStr, out int damageAmount))
            {
                Debug.LogError($"[DefaultActionExecutor] DealDamage: cantidad de daño inválida '{damageStr}'");
                return;
            }

            Debug.Log($"[DefaultActionExecutor] Causando {damageAmount} de daño");
            // Aquí iría la lógica de aplicar daño a una entidad
        }

        #endregion

        #region Acciones de Loot

        /// <summary>
        /// Suelta loot
        /// </summary>
        protected virtual void OnDropLoot(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Soltando loot");
            // Aquí iría la lógica de generar loot en la posición actual
        }

        #endregion

        #region Acciones de Sistema de Salud

        /// <summary>
        /// Inicializa el sistema de salud
        /// </summary>
        protected virtual void OnInitializeHealthSystem(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Inicializando sistema de salud");
            // Aquí iría la lógica de inicializar salud, UI, etc.
        }

        /// <summary>
        /// Actualiza la UI de salud
        /// </summary>
        protected virtual void OnUpdateHealthUI(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Actualizando UI de salud");
            // Aquí iría la lógica de actualizar la UI de salud
        }

        /// <summary>
        /// Aplica regeneración de salud
        /// Parámetros esperados: regenPerSecond
        /// </summary>
        protected virtual void OnApplyHealthRegen(Dictionary<string, string> parameters)
        {
            if (!parameters.TryGetValue("regenPerSecond", out string regenStr))
            {
                Debug.LogError("[DefaultActionExecutor] ApplyHealthRegen requiere parámetro 'regenPerSecond'");
                return;
            }

            if (!float.TryParse(regenStr, out float regenPerSecond))
            {
                Debug.LogError($"[DefaultActionExecutor] ApplyHealthRegen: valor inválido '{regenStr}'");
                return;
            }

            Debug.Log($"[DefaultActionExecutor] Regenerando {regenPerSecond} de salud por segundo");
            // Aquí iría la lógica de aplicar regeneración
        }

        /// <summary>
        /// Apaga el sistema de salud
        /// </summary>
        protected virtual void OnShutdownHealthSystem(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Apagando sistema de salud");
            // Aquí iría la lógica de limpiar el sistema de salud
        }

        /// <summary>
        /// Monitorea la salud
        /// </summary>
        protected virtual void OnMonitorHealth(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Monitoreando salud");
            // Aquí iría la lógica de verificar y monitorear salud
        }

        #endregion

        #region Acciones de Sistema de Audio

        /// <summary>
        /// Inicializa el sistema de audio
        /// </summary>
        protected virtual void OnInitializeAudioSystem(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Inicializando sistema de audio");
            // Aquí iría la lógica de inicializar el sistema de audio
        }

        #endregion
    }
}
