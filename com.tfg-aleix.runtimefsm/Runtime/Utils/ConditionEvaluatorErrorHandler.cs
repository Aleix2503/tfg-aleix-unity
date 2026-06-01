using UnityEngine;
using System;
using RuntimeFSM.Data;

namespace RuntimeFSM.Utils
{
    /// <summary>
    /// Gestor centralizado de errores para ConditionEvaluator.
    /// Proporciona logging consistente para errores de evaluación de condiciones.
    /// </summary>
    public static class ConditionEvaluatorErrorHandler
    {
        private const string PREFIX = "[ConditionEvaluator Error]";

        /// <summary>
        /// Registra un error cuando una variable no está disponible
        /// </summary>
        public static void LogVariableNotFound(string variableName, GameObject gameObject)
        {
                $"{PREFIX} Variable '{variableName}' NO ESTÁ DISPONIBLE.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Script: {gameObject.GetComponent<MonoBehaviour>().GetType().Name}\n" +
                $"Posible solución:\n" +
                $"  1. Asegúrate de que la variable '{variableName}' existe en el evaluador\n" +
                $"  2. La variable debe estar inicializada en Awake() o Start()\n" +
                $"  3. Verifica que esté declarada como campo público o implementada en EvaluateSimpleCondition()",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando no se puede parsear un valor de condición
        /// </summary>
        public static void LogValueParsingError(string conditionName, string fieldName, string value, string expectedType, GameObject gameObject)
        {
                $"{PREFIX} No se pudo parsear valor en condición '{conditionName}'.\n" +
                $"Campo: {fieldName}\n" +
                $"Valor: '{value}'\n" +
                $"Tipo esperado: {expectedType}\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Verifica que el valor en el JSON sea válido.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando una condición tiene estructura inválida
        /// </summary>
        public static void LogInvalidConditionStructure(string reason, GameObject gameObject)
        {
                $"{PREFIX} Estructura de condición inválida.\n" +
                $"Razón: {reason}\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Verifica la estructura JSON de la condición.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando falta un parámetro requerido en la condición
        /// </summary>
        public static void LogMissingConditionParameter(string conditionName, string parameterName, GameObject gameObject)
        {
                $"{PREFIX} Parámetro requerido '{parameterName}' NO ENCONTRADO en condición '{conditionName}'.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Verifica que el JSON incluya este parámetro.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando la evaluación de una condición lanza excepción
        /// </summary>
        public static void LogEvaluationException(string conditionName, Exception ex, GameObject gameObject)
        {
                $"{PREFIX} Error al evaluar condición '{conditionName}'.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Excepción: {ex.GetType().Name}\n" +
                $"Mensaje: {ex.Message}\n" +
                $"Stack Trace: {ex.StackTrace}",
                gameObject
            );
        }

        /// <summary>
        /// Registra un aviso cuando una variable es nula pero se intenta usar
        /// </summary>
        public static void LogVariableIsNull(string variableName, GameObject gameObject)
        {
                $"{PREFIX} Variable '{variableName}' es NULL.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Asegúrate de que está inicializada correctamente.",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error cuando el operador de condición es inválido
        /// </summary>
        public static void LogInvalidOperator(string operatorStr, string variableType, GameObject gameObject)
        {
                $"{PREFIX} Operador '{operatorStr}' no es válido para tipo '{variableType}'.\n" +
                $"GameObject: {gameObject.name}\n" +
                $"Variables bool: solo '==' y '!='\n" +
                $"Variables numéricas: '==', '!=', '<', '>', '<=', '>='",
                gameObject
            );
        }

        /// <summary>
        /// Registra un error genérico en la evaluación
        /// </summary>
        public static void LogEvaluationFailed(string reason, GameObject gameObject)
        {
                $"{PREFIX} Evaluación fallida: {reason}\n" +
                $"GameObject: {gameObject.name}",
                gameObject
            );
        }
    }
}
