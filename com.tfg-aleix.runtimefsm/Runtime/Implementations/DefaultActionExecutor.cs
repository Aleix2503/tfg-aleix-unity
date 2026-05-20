using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RuntimeFSM.Interfaces;

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
        /// Usa reflection para encontrar el método dinámicamente
        /// </summary>
        public virtual void Execute(string actionName, Dictionary<string, string> parameters)
        {
            string methodName = $"On{actionName}";
            var method = GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase
            );

            if (method != null)
            {
                method.Invoke(this, new object[] { parameters });
            }
            else
            {
                Debug.LogWarning($"[DefaultActionExecutor] Método '{methodName}' no encontrado para la acción '{actionName}'");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // MÉTODOS DE ACCIÓN (95 TOTAL)
        // Organizados por categoría para facilitar la búsqueda
        // ─────────────────────────────────────────────────────────────

        #region Animation (11 acciones)

        protected virtual void OnPlayAnimation(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] PlayAnimation - animationName, speed, loop");
        }

        protected virtual void OnCrossFadeAnimation(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] CrossFadeAnimation - animationName, duration");
        }

        protected virtual void OnStopAnimation(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] StopAnimation");
        }

        protected virtual void OnPauseAnimation(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] PauseAnimation");
        }

        protected virtual void OnResumeAnimation(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ResumeAnimation");
        }

        protected virtual void OnSetAnimatorBool(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAnimatorBool - parameter, value");
        }

        protected virtual void OnSetAnimatorTrigger(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAnimatorTrigger - parameter");
        }

        protected virtual void OnResetAnimatorTrigger(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ResetAnimatorTrigger - parameter");
        }

        protected virtual void OnSetAnimatorFloat(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAnimatorFloat - parameter, value");
        }

        protected virtual void OnSetAnimatorInt(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAnimatorInt - parameter, value");
        }

        protected virtual void OnSetAnimationLayerWeight(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAnimationLayerWeight - layer, weight");
        }

        #endregion

        #region Movement (16 acciones)

        protected virtual void OnMoveToPosition(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] MoveToPosition - x, y, z, speed");
        }

        protected virtual void OnMoveToTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] MoveToTarget - targetName, speed");
        }

        protected virtual void OnMoveForward(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] MoveForward - speed");
        }

        protected virtual void OnMoveBackward(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] MoveBackward - speed");
        }

        protected virtual void OnStrafe(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Strafe - direction, speed");
        }

        protected virtual void OnRotateToTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] RotateToTarget - rotationSpeed");
        }

        protected virtual void OnRotateToPosition(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] RotateToPosition - x, y, z, rotationSpeed");
        }

        protected virtual void OnLookAtTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] LookAtTarget - targetName");
        }

        protected virtual void OnSetSpeed(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetSpeed - speed");
        }

        protected virtual void OnStopMovement(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] StopMovement");
        }

        protected virtual void OnJump(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Jump - force");
        }

        protected virtual void OnDash(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Dash - force, duration");
        }

        protected virtual void OnAddForce(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] AddForce - x, y, z");
        }

        protected virtual void OnTeleport(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Teleport - x, y, z");
        }

        protected virtual void OnEnableGravity(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] EnableGravity");
        }

        protected virtual void OnDisableGravity(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DisableGravity");
        }

        #endregion

        #region AI (13 acciones)

        protected virtual void OnSetTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetTarget - targetName");
        }

        protected virtual void OnClearTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ClearTarget");
        }

        protected virtual void OnChaseTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ChaseTarget - speed, stoppingDistance");
        }

        protected virtual void OnStopChasing(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] StopChasing");
        }

        protected virtual void OnFleeFromTarget(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] FleeFromTarget - distance, speed");
        }

        protected virtual void OnPatrol(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Patrol - speed");
        }

        protected virtual void OnSetPatrolPoint(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetPatrolPoint - pointIndex");
        }

        protected virtual void OnNextPatrolPoint(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] NextPatrolPoint");
        }

        protected virtual void OnWait(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Wait - duration");
        }

        protected virtual void OnSearchLastKnownPosition(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SearchLastKnownPosition - duration");
        }

        protected virtual void OnSetAggro(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAggro - value");
        }

        protected virtual void OnSetAlert(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetAlert - value");
        }

        protected virtual void OnSetState(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetState - stateName");
        }

        #endregion

        #region Combat (14 acciones)

        protected virtual void OnAttack(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Attack - damage, range");
        }

        protected virtual void OnMeleeAttack(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] MeleeAttack - damage, radius");
        }

        protected virtual void OnRangedAttack(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] RangedAttack - projectile, speed, damage");
        }

        protected virtual void OnEnableHitbox(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] EnableHitbox - hitboxName");
        }

        protected virtual void OnDisableHitbox(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DisableHitbox - hitboxName");
        }

        protected virtual void OnTakeDamage(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] TakeDamage - amount");
        }

        protected virtual void OnHeal(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Heal - amount");
        }

        protected virtual void OnDie(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Die");
        }

        protected virtual void OnRevive(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] Revive - health");
        }

        protected virtual void OnApplyKnockback(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ApplyKnockback - force");
        }

        protected virtual void OnSpawnProjectile(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SpawnProjectile - prefabName, speed, damage");
        }

        protected virtual void OnReloadWeapon(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ReloadWeapon");
        }

        protected virtual void OnEquipWeapon(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] EquipWeapon - weaponName");
        }

        protected virtual void OnUnequipWeapon(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] UnequipWeapon");
        }

        #endregion

        #region Audio (8 acciones)

        protected virtual void OnPlaySound(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] PlaySound - clipName, volume");
        }

        protected virtual void OnPlayOneShot(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] PlayOneShot - clipName, volume");
        }

        protected virtual void OnStopSound(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] StopSound - clipName");
        }

        protected virtual void OnPauseSound(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] PauseSound - clipName");
        }

        protected virtual void OnResumeSound(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ResumeSound - clipName");
        }

        protected virtual void OnSetVolume(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetVolume - volume");
        }

        protected virtual void OnMuteAudio(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] MuteAudio");
        }

        protected virtual void OnUnmuteAudio(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] UnmuteAudio");
        }

        #endregion

        #region VFX (9 acciones)

        protected virtual void OnSpawnVFX(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SpawnVFX - vfxName");
        }

        protected virtual void OnDestroyVFX(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DestroyVFX - vfxName");
        }

        protected virtual void OnPlayParticleSystem(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] PlayParticleSystem - systemName");
        }

        protected virtual void OnStopParticleSystem(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] StopParticleSystem - systemName");
        }

        protected virtual void OnShakeCamera(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ShakeCamera - intensity, duration");
        }

        protected virtual void OnFlashScreen(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] FlashScreen - duration");
        }

        protected virtual void OnChangeColor(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ChangeColor - r, g, b");
        }

        protected virtual void OnEnableTrail(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] EnableTrail");
        }

        protected virtual void OnDisableTrail(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DisableTrail");
        }

        #endregion

        #region Variables (10 acciones)

        protected virtual void OnSetBool(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetBool - variableName, value");
        }

        protected virtual void OnToggleBool(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ToggleBool - variableName");
        }

        protected virtual void OnSetInt(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetInt - variableName, value");
        }

        protected virtual void OnIncrementInt(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] IncrementInt - variableName, amount");
        }

        protected virtual void OnDecrementInt(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DecrementInt - variableName, amount");
        }

        protected virtual void OnSetFloat(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetFloat - variableName, value");
        }

        protected virtual void OnAddFloat(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] AddFloat - variableName, amount");
        }

        protected virtual void OnSubtractFloat(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SubtractFloat - variableName, amount");
        }

        protected virtual void OnSetString(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetString - variableName, value");
        }

        protected virtual void OnClearVariable(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ClearVariable - variableName");
        }

        #endregion

        #region GameObject (7 acciones)

        protected virtual void OnSetActive(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetActive - value");
        }

        protected virtual void OnDestroyObject(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DestroyObject - delay");
        }

        protected virtual void OnInstantiatePrefab(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] InstantiatePrefab - prefabName");
        }

        protected virtual void OnEnableComponent(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] EnableComponent - componentName");
        }

        protected virtual void OnDisableComponent(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] DisableComponent - componentName");
        }

        protected virtual void OnSetTag(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetTag - tag");
        }

        protected virtual void OnSetLayer(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetLayer - layer");
        }

        #endregion

        #region UI (4 acciones)

        protected virtual void OnShowUI(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] ShowUI - uiName");
        }

        protected virtual void OnHideUI(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] HideUI - uiName");
        }

        protected virtual void OnSetUIText(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetUIText - uiElement, text");
        }

        protected virtual void OnSetUIProgress(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SetUIProgress - uiElement, value");
        }

        #endregion

        #region Events (3 acciones)

        protected virtual void OnSendEvent(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] SendEvent - eventName");
        }

        protected virtual void OnBroadcastEvent(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] BroadcastEvent - eventName");
        }

        protected virtual void OnInvokeMethod(Dictionary<string, string> parameters)
        {
            Debug.Log("[DefaultActionExecutor] InvokeMethod - methodName");
        }

        #endregion
    }
}
