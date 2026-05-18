using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace RuntimeFSM.Editor
{
    /// <summary>
    /// Ventana del editor para generar ActionExecutors y ConditionEvaluators personalizados.
    ///
    /// Acceso: Tools > RuntimeFSM > Generate Custom Components
    /// </summary>
    public class FSMGeneratorWindow : EditorWindow
    {
        private string _actionExecutorName = "MyActionExecutor";
        private string _conditionEvaluatorName = "MyConditionEvaluator";
        private string _generationPath = "Assets/Scripts";
        private int _selectedTab = 0;

        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem("Tools/RuntimeFSM/Generate Custom Components")]
        public static void ShowWindow()
        {
            GetWindow<FSMGeneratorWindow>("FSM Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("RuntimeFSM Code Generator", EditorStyles.largeLabel);
            EditorGUILayout.HelpBox(
                "Genera implementaciones personalizadas de ActionExecutor y ConditionEvaluator.\n" +
                "Así puedes mantener tus cambios sin ser afectado por actualizaciones del plugin.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            // Path selector
            EditorGUILayout.LabelField("Configuración General", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            _generationPath = EditorGUILayout.TextField("Carpeta de Salida", _generationPath);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.OpenFolderPanel("Selecciona carpeta de salida", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    path = path.Replace(Application.dataPath, "Assets");
                    _generationPath = path;
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(15);

            // Tab selector
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            _selectedTab = GUILayout.Toolbar(_selectedTab, new[] { "Action Executor", "Condition Evaluator" });
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Action Executor Tab
            if (_selectedTab == 0)
            {
                DrawActionExecutorTab();
            }
            // Condition Evaluator Tab
            else
            {
                DrawConditionEvaluatorTab();
            }

            GUILayout.EndScrollView();
        }

        private void DrawActionExecutorTab()
        {
            EditorGUILayout.LabelField("Generar Action Executor Personalizado", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Crea un ActionExecutor personalizado que hereda de DefaultActionExecutor.\n" +
                "Puedes sobrescribir métodos específicos de acciones sin tocar el código del plugin.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Configuración", EditorStyles.boldLabel);
            _actionExecutorName = EditorGUILayout.TextField("Nombre de la Clase", _actionExecutorName);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Acciones Disponibles para Personalizar:", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(
                "• PlayAnimation - Reproducir animaciones\n" +
                "• PlaySound - Reproducir sonidos\n" +
                "• DealDamage - Causar daño\n" +
                "• DropLoot - Soltar loot\n" +
                "• InitializeHealthSystem - Inicializar salud\n" +
                "• UpdateHealthUI - Actualizar UI de salud\n" +
                "• ApplyHealthRegen - Regenerar salud\n" +
                "• ShutdownHealthSystem - Apagar sistema de salud\n" +
                "• InitializeAudioSystem - Inicializar audio\n" +
                "• UpdateAmbientSound - Actualizar sonido ambiente\n" +
                "• MonitorHealth - Monitorear salud",
                EditorStyles.helpBox,
                GUILayout.MinHeight(150)
            );

            EditorGUILayout.Space(15);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generar Action Executor", GUILayout.Height(40)))
            {
                GenerateActionExecutor();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawConditionEvaluatorTab()
        {
            EditorGUILayout.LabelField("Generar Condition Evaluator Personalizado", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Crea un ConditionEvaluator personalizado que hereda de ConditionEvaluatorBase.\n" +
                "Solo necesitas implementar EvaluateSimpleCondition() para tus variables específicas.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Configuración", EditorStyles.boldLabel);
            _conditionEvaluatorName = EditorGUILayout.TextField("Nombre de la Clase", _conditionEvaluatorName);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Operadores Soportados:", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(
                "Simples:\n" +
                "  ==  (igualdad)\n" +
                "  !=  (desigualdad)\n" +
                "  >   (mayor que)\n" +
                "  >=  (mayor o igual)\n" +
                "  <   (menor que)\n" +
                "  <=  (menor o igual)\n" +
                "  contains\n" +
                "  not_contains\n\n" +
                "Lógicos:\n" +
                "  AND (todas las condiciones)\n" +
                "  OR  (cualquier condición)\n" +
                "  NOT (negar condición)",
                EditorStyles.helpBox,
                GUILayout.MinHeight(180)
            );

            EditorGUILayout.Space(15);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generar Condition Evaluator", GUILayout.Height(40)))
            {
                GenerateConditionEvaluator();
            }
            GUILayout.EndHorizontal();
        }

        private void GenerateActionExecutor()
        {
            if (string.IsNullOrWhiteSpace(_actionExecutorName))
            {
                EditorUtility.DisplayDialog("Error", "El nombre de la clase no puede estar vacío", "OK");
                return;
            }

            if (!IsValidClassName(_actionExecutorName))
            {
                EditorUtility.DisplayDialog("Error", "Nombre de clase inválido. Debe empezar con letra y contener solo caracteres alfanuméricos.", "OK");
                return;
            }

            string path = Path.Combine(_generationPath, _actionExecutorName + ".cs");
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog(
                    "Archivo Existe",
                    $"El archivo {_actionExecutorName}.cs ya existe. ¿Sobrescribir?",
                    "Sí",
                    "No"))
                {
                    return;
                }
            }

            string code = GenerateActionExecutorCode(_actionExecutorName);

            try
            {
                Directory.CreateDirectory(_generationPath);
                File.WriteAllText(path, code, Encoding.UTF8);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Éxito", $"Action Executor generado en: {path}", "OK");
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(path));
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"No se pudo generar el archivo: {ex.Message}", "OK");
            }
        }

        private void GenerateConditionEvaluator()
        {
            if (string.IsNullOrWhiteSpace(_conditionEvaluatorName))
            {
                EditorUtility.DisplayDialog("Error", "El nombre de la clase no puede estar vacío", "OK");
                return;
            }

            if (!IsValidClassName(_conditionEvaluatorName))
            {
                EditorUtility.DisplayDialog("Error", "Nombre de clase inválido. Debe empezar con letra y contener solo caracteres alfanuméricos.", "OK");
                return;
            }

            string path = Path.Combine(_generationPath, _conditionEvaluatorName + ".cs");
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog(
                    "Archivo Existe",
                    $"El archivo {_conditionEvaluatorName}.cs ya existe. ¿Sobrescribir?",
                    "Sí",
                    "No"))
                {
                    return;
                }
            }

            string code = GenerateConditionEvaluatorCode(_conditionEvaluatorName);

            try
            {
                Directory.CreateDirectory(_generationPath);
                File.WriteAllText(path, code, Encoding.UTF8);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Éxito", $"Condition Evaluator generado en: {path}", "OK");
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(path));
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"No se pudo generar el archivo: {ex.Message}", "OK");
            }
        }

        private bool IsValidClassName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (!char.IsLetter(name[0])) return false;
            foreach (char c in name)
            {
                if (!char.IsLetterOrDigit(c) && c != '_') return false;
            }
            return true;
        }

        private string GenerateActionExecutorCode(string className)
        {
            return $@"using System.Collections.Generic;
using UnityEngine;
using RuntimeFSM.Implementations;

/// <summary>
/// Action Executor personalizado.
///
/// Este archivo fue generado por FSMGeneratorWindow.
/// Puedes sobrescribir cualquier acción para personalizar su comportamiento.
/// </summary>
public class {className} : DefaultActionExecutor
{{
    // Ejemplo: Personalizar PlayAnimation
    // protected override void OnPlayAnimation(Dictionary<string, string> parameters)
    // {{
    //     if (!parameters.TryGetValue(""animationName"", out string animationName))
    //         return;
    //
    //     // Tu lógica personalizada aquí
    //     Debug.Log($""Animación personalizada: {{animationName}}"");
    //     base.OnPlayAnimation(parameters); // O no llamar a base si quieres reemplazarlo completamente
    // }}

    // Ejemplo: Personalizar DealDamage
    // protected override void OnDealDamage(Dictionary<string, string> parameters)
    // {{
    //     if (!parameters.TryGetValue(""damageAmount"", out string damageStr))
    //         return;
    //
    //     if (!int.TryParse(damageStr, out int damage))
    //         return;
    //
    //     // Tu lógica de daño personalizada
    //     Health -= damage;
    //     Debug.Log($""Daño recibido: {{damage}}"");
    // }}

    // Ejemplo: Personalizar PlaySound
    // protected override void OnPlaySound(Dictionary<string, string> parameters)
    // {{
    //     if (!parameters.TryGetValue(""soundName"", out string soundName))
    //         return;
    //
    //     // Tu lógica de audio personalizada
    //     AudioManager.Instance.PlaySound(soundName);
    // }}
}}
";
        }

        private string GenerateConditionEvaluatorCode(string className)
        {
            return $@"using UnityEngine;
using RuntimeFSM.Data;
using RuntimeFSM.Utils;

/// <summary>
/// Condition Evaluator personalizado.
///
/// Este archivo fue generado por FSMGeneratorWindow.
/// Solo necesitas implementar EvaluateSimpleCondition() para tus variables específicas.
/// Las condiciones lógicas (AND, OR, NOT) se manejan automáticamente en ConditionEvaluatorBase.
/// </summary>
public class {className} : ConditionEvaluatorBase
{{
    // PASO 1: Define tus variables que usarás en las condiciones del FSM
    [SerializeField] private float distanceToPlayer = 0f;
    [SerializeField] private bool isAlerted = false;
    [SerializeField] private int health = 100;

    // PASO 2: Implement EvaluateSimpleCondition para tu lógica específica
    protected override bool EvaluateSimpleCondition(ConditionDefinition condition)
    {{
        var operatorType = ConditionParser.ParseConditionOperator(condition.@operator);

        // Obtén el valor actual de tu variable
        float currentValue = GetVariableValue(condition.name);
        float targetValue = float.Parse(condition.value);

        // Evalúa basado en el operador
        return operatorType switch
        {{
            ConditionOperator.GreaterThan => currentValue > targetValue,
            ConditionOperator.LessThan => currentValue < targetValue,
            ConditionOperator.GreaterThanOrEqual => currentValue >= targetValue,
            ConditionOperator.LessThanOrEqual => currentValue <= targetValue,
            ConditionOperator.Equals => Mathf.Approximately(currentValue, targetValue),
            ConditionOperator.NotEquals => !Mathf.Approximately(currentValue, targetValue),
            ConditionOperator.Contains => condition.value.Contains(GetStringVariableValue(condition.name)),
            ConditionOperator.NotContains => !condition.value.Contains(GetStringVariableValue(condition.name)),
            _ => false
        }};
    }}

    // PASO 3: Implementa GetVariableValue para mapear nombres a tus variables
    private float GetVariableValue(string variableName)
    {{
        return variableName.ToLower() switch
        {{
            ""distancetoplayer"" => distanceToPlayer,
            ""health"" => health,
            _ => 0f
        }};
    }}

    // Para variables de texto (contains, not_contains)
    private string GetStringVariableValue(string variableName)
    {{
        return variableName.ToLower() switch
        {{
            ""name"" => gameObject.name,
            ""tag"" => gameObject.tag,
            _ => """"
        }};
    }}

    // PASO 4 (Opcional): Implementa métodos Update si necesitas actualizar variables cada frame
    private void Update()
    {{
        // Ejemplo: actualizar distancia al jugador
        if (GameObject.FindGameObjectWithTag(""Player"") is GameObject player)
        {{
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        }}
    }}
}}
";
        }
    }
}
