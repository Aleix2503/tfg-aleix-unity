using UnityEngine;
using System;
using System.Collections.Generic;

namespace RuntimeFSM.Utils
{
    /// <summary>
    /// Gestor centralizado de errores para ActionExecutor.
    /// Proporciona métodos para logging consistente y manejo de excepciones.
    /// </summary>
    public static class ActionExecutorErrorHandler
    {
        private const string PREFIX = "[ActionExecutor Error]";

        /// <summary>
        /// Registra un error cuando no se encuentra el método de acción
        /// </summary>
        public static void LogMethodNotFound(string actionName, string methodName, GameObject gameObject)
        {
                $"{PREFIX} Acción '{actionName}' → Método '{methodName}' NO ENCONTRADO.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Script: {gameObject.GetComponent<MonoBehaviour>().GetType().Name}\n" +
                $"Posible solución: Implementa protected virtual bool {methodName}(Dictionary<string, string> parameters)",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando hay una excepción al invocar el método
        /// </summary>
        public static void LogInvocationException(string actionName, string methodName, Exception ex, GameObject gameObject, Dictionary<string, string> parameters)
        {
            string paramString = parameters != null && parameters.Count > 0
                ? string.Join(", ", parameters.Keys)
                : "(sin parámetros)";

                $"{PREFIX} Error al ejecutar '{actionName}'.\n" +
                $"Método: {methodName}\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Parámetros: {paramString}\n" +
                $"Excepción: {ex.GetType().Name}\n" +
                $"Mensaje: {ex.Message}\n" +
                $"Stack Trace: {ex.StackTrace}",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando falta un parámetro requerido
        /// </summary>
        public static void LogMissingParameter(string actionName, string parameterName, GameObject gameObject)
        {
                $"{PREFIX} Parámetro requerido '{parameterName}' no encontrado en acción '{actionName}'.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Verifica que el JSON de la acción incluya este parámetro.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando un parámetro no se puede parsear
        /// </summary>
        public static void LogParameterParsingError(string actionName, string parameterName, string parameterValue, string expectedType, GameObject gameObject)
        {
                $"{PREFIX} No se pudo convertir parámetro en acción '{actionName}'.\n" +
                $"Parámetro: {parameterName}\n" +
                $"Valor recibido: '{parameterValue}'\n" +
                $"Tipo esperado: {expectedType}\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Verifica que el valor sea válido para el tipo esperado.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando no se encuentra un componente requerido
        /// </summary>
        public static void LogMissingComponent(string actionName, string componentName, GameObject gameObject)
        {
                $"{PREFIX} Componente '{componentName}' requerido NO ENCONTRADO para acción '{actionName}'.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Añade el componente '{componentName}' a este GameObject.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando no se encuentra un GameObject
        /// </summary>
        public static void LogGameObjectNotFound(string actionName, string targetName, GameObject gameObject)
        {
                $"{PREFIX} GameObject '{targetName}' no encontrado durante la ejecución de '{actionName}'.\n" +
                $"Origen: {gameObject.name}\n" +
                $"Asegúrate de que '{targetName}' existe en la escena.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando una operación falla
        /// </summary>
        public static void LogOperationFailed(string actionName, string operation, string reason, GameObject gameObject)
        {
                $"{PREFIX} Operación fallida en acción '{actionName}'.\n" +
                $"Operación: {operation}\n" +
                $"Razón: {reason}\n" +
                $"GameObject: {gameObject.name}",
                gameObject
            );
        }

        /// <summary>
        /// Registra un aviso cuando un componente es null pero la acción continúa
        /// </summary>
        public static void LogComponentNullSilent(string actionName, string componentName, GameObject gameObject)
        {
                $"{PREFIX} Componente '{componentName}' es null en '{actionName}', acción ignorada.\n" +
                $"GameObject: {gameObject.name}",
                gameObject
            );
        }
    }
}
