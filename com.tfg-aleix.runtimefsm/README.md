# 🎮 RuntimeFSM - Finite State Machine para Unity

Un plugin flexible y extensible para implementar máquinas de estado finitas en Unity. Carga FSMs desde archivos JSON generados por cualquier editor visual.

## 🌟 Características

- ✅ **Cargar FSMs desde JSON** - Compatibilidad total con JSONs del framework visual Python
- ✅ **4 Tipos de Estados** - Normal, Entry Point, Any State, Global State
- ✅ **Condiciones Complejas** - Lógica AND/OR/NOT recursiva
- ✅ **Acciones Predefinidas** - 11 acciones listas para usar
- ✅ **ActionExecutor por Defecto** - DefaultActionExecutor implementa todas las acciones
- ✅ **Generador de Código** - Crea ActionExecutors y ConditionEvaluators personalizados sin tocar el plugin
- ✅ **Totalmente Extensible** - Interfaz limpia para crear tus propias implementaciones
- ✅ **Evaluación Eficiente** - Short-circuit evaluation en lógica AND/OR

## 📦 Instalación

1. Copia la carpeta `com.tfg-aleix.runtimefsm` a `Assets/` o `Packages/`
2. ¡Listo! El plugin está listo para usar

## 🚀 Inicio Rápido

### 1. Crear un FSM (JSON)

```json
{
  "version": "1.0",
  "name": "EnemyFSM",
  "initial_state": "Patrol",
  "states": [
    {
      "id": "Patrol",
      "is_entry_point": true,
      "is_any_state": false,
      "is_global_state": false,
      "enter": [{"action": "PlayAnimation", "params": {"animationName": "Patrol"}}],
      "tick": [],
      "exit": []
    },
    {
      "id": "ANY_STATE",
      "is_entry_point": false,
      "is_any_state": true,
      "is_global_state": false,
      "enter": [],
      "tick": [],
      "exit": []
    }
  ],
  "transitions": [
    {
      "from": "ANY_STATE",
      "to": "Dead",
      "condition": {"type": "simple", "name": "health", "operator": "<=", "value": "0"}
    }
  ]
}
```

### 2. Crear ConditionEvaluator

```
Tools > RuntimeFSM > Generate Custom Components > Condition Evaluator
```

O crear manualmente:

```csharp
public class EnemyConditions : ConditionEvaluatorBase
{
    [SerializeField] private int _health = 100;

    protected override bool EvaluateSimpleCondition(ConditionDefinition condition)
    {
        var op = ConditionParser.ParseConditionOperator(condition.@operator);
        float current = GetVariableValue(condition.name);
        float target = float.Parse(condition.value);

        return op switch
        {
            ConditionOperator.LessThanOrEqual => current <= target,
            // ... más operadores
        };
    }

    private float GetVariableValue(string name)
    {
        return name.ToLower() switch { "health" => _health, _ => 0f };
    }
}
```

### 3. Usar en tu GameObject

```csharp
public class EnemyController : MonoBehaviour
{
    private FSM _fsm;

    private void Start()
    {
        var actionExecutor = gameObject.AddComponent<DefaultActionExecutor>();
        var conditions = gameObject.AddComponent<EnemyConditions>();

        var fsmDef = JsonUtility.FromJson<FSMDefinition>(
            Resources.Load<TextAsset>("FSMs/EnemyFSM").text
        );

        _fsm = new FSM(fsmDef, actionExecutor, conditions);
    }

    private void Update()
    {
        _fsm.Tick();
    }
}
```

## 🎯 Componentes Principales

### FSM (Motor)
```csharp
var fsm = new FSM(definition, actionExecutor, conditionEvaluator);
fsm.Tick(); // Ejecutar un frame del FSM
```

### DefaultActionExecutor
Implementa estas 11 acciones:
- `PlayAnimation` - Reproducir animación
- `PlaySound` - Reproducir sonido
- `DealDamage` - Causar daño
- `DropLoot` - Soltar loot
- `InitializeHealthSystem` - Inicializar salud
- `UpdateHealthUI` - Actualizar UI
- `ApplyHealthRegen` - Regenerar salud
- `ShutdownHealthSystem` - Apagar salud
- `InitializeAudioSystem` - Inicializar audio
- `UpdateAmbientSound` - Actualizar volumen
- `MonitorHealth` - Monitorear salud

### ConditionEvaluatorBase
Base class que maneja automáticamente lógica AND/OR/NOT:

```csharp
public class MyEvaluator : ConditionEvaluatorBase
{
    // Solo implementa esto
    protected override bool EvaluateSimpleCondition(ConditionDefinition condition)
    {
        // Tu lógica aquí
    }
}
```

## 🔄 Flujo de Ejecución

Cada `FSM.Tick()` ejecuta en este orden:

```
1. Ejecutar tick de Global States
2. Evaluar transiciones del estado actual
3. Si no hay transición, evaluar ANY_STATE
4. Si aún no hay transición, ejecutar tick del estado actual
```

## 📋 Tipos de Estados

### Normal
Estado regular que transiciona basado en condiciones.

### Entry Point
Estado inicial obligatorio. Solo uno permitido.
```json
{"is_entry_point": true}
```

### ANY_STATE
Transiciones globales desde cualquier estado.
```json
{"is_any_state": true}
```

### Global State
Ejecuta tick cada frame sin cambiar el estado principal.
```json
{"is_global_state": true, "tick": [...]}
```

## 🔀 Tipos de Condiciones

### Simple
```json
{
  "type": "simple",
  "name": "distance",
  "operator": "<",
  "value": "15"
}
```

**Operadores**: `==`, `!=`, `>`, `>=`, `<`, `<=`, `contains`, `not_contains`

### Lógica AND
```json
{
  "type": "logical",
  "logicalOperator": "AND",
  "conditions": [
    {"type": "simple", "name": "distance", "operator": "<", "value": "15"},
    {"type": "simple", "name": "health", "operator": ">", "value": "50"}
  ]
}
```

### Lógica OR
```json
{
  "type": "logical",
  "logicalOperator": "OR",
  "conditions": [
    {"type": "simple", "name": "health", "operator": "<=", "value": "0"},
    {"type": "simple", "name": "outOfBounds", "operator": "==", "value": "true"}
  ]
}
```

### Lógica NOT
```json
{
  "type": "logical",
  "logicalOperator": "NOT",
  "conditions": [
    {"type": "simple", "name": "hasWeapon", "operator": "==", "value": "true"}
  ]
}
```

## 🛠️ Generar Componentes Personalizados

El plugin incluye un generador automático para crear ActionExecutors y ConditionEvaluators sin modificar el código del plugin.

```
Tools > RuntimeFSM > Generate Custom Components
```

**Pestaña Action Executor**
- Genera un ActionExecutor que hereda de `DefaultActionExecutor`
- Personaliza solo las acciones que necesites
- El resto usa la implementación por defecto

**Pestaña Condition Evaluator**
- Genera un ConditionEvaluator que hereda de `ConditionEvaluatorBase`
- Define tus variables
- Implementa `EvaluateSimpleCondition()`
- La lógica AND/OR/NOT se maneja automáticamente

## 📚 Documentación Adicional

- **[QUICK_START.md](QUICK_START.md)** - Guía rápida para empezar
- **[CUSTOM_IMPLEMENTATIONS.md](CUSTOM_IMPLEMENTATIONS.md)** - Crear ActionExecutors y ConditionEvaluators personalizados
- **[JSON_STRUCTURE_REFERENCE.md](JSON_STRUCTURE_REFERENCE.md)** - Referencia completa de estructura JSON
- **[RUNTIME_IMPLEMENTATION.md](RUNTIME_IMPLEMENTATION.md)** - Detalles internos de ejecución
- **[STRUCTURE_CHANGES.md](STRUCTURE_CHANGES.md)** - Cambios realizados desde la fase de estructura

## 📂 Estructura de Carpetas

```
com.tfg-aleix.runtimefsm/
├── Runtime/
│   ├── Core/                    # Motor FSM (FSM.cs, FSMState.cs)
│   ├── Data/                    # Definiciones serializables (StateDefinition, etc)
│   ├── Implementations/         # DefaultActionExecutor
│   ├── Interfaces/              # IActionExecutor, IConditionEvaluator
│   ├── Utilities/               # Helpers (ConditionParser, etc)
│   ├── UnityIntegration/        # FSMBehaviour para inspector
│   └── RuntimeFSM.asmdef
├── Editor/
│   ├── Tools/                   # FSMGeneratorWindow
│   └── RuntimeFSM.Editor.asmdef
├── Samples~/
│   └── Example/                 # Ejemplos de uso
├── Documentation/               # Docs en Markdown
└── package.json
```

## 🔧 Interfaz Personalizada

### IActionExecutor
```csharp
public interface IActionExecutor
{
    void Execute(string actionName, Dictionary<string, string> parameters);
}
```

### IConditionEvaluator
```csharp
public interface IConditionEvaluator
{
    bool Evaluate(ConditionDefinition condition);
    bool EvaluateSimple(string type, string name, string op, string value);
}
```

## 💡 Mejores Prácticas

1. **Una implementación por entidad**
   ```csharp
   // ✅ BIEN
   public class PlayerActionExecutor : DefaultActionExecutor { }
   public class EnemyActionExecutor : DefaultActionExecutor { }
   
   // ❌ MAL
   public class UniversalActionExecutor : DefaultActionExecutor { } // Con 100 if/else
   ```

2. **Personaliza solo lo necesario**
   ```csharp
   // ✅ BIEN
   protected override void OnDealDamage(...) { /* tu lógica */ }
   
   // ❌ MAL
   public override void Execute(...) { /* reinventar la rueda */ }
   ```

3. **Usa variables serializables**
   ```csharp
   [SerializeField] private float _detectionRange = 20f;
   // Ajustable en el inspector sin código
   ```

4. **Actualiza variables antes de Tick()**
   ```csharp
   private void Update()
   {
       UpdateVariables(); // Actualizar
       _fsm.Tick();       // Ejecutar
   }
   ```

## 🐛 Debugging

### Ver qué acción se ejecuta
```csharp
protected override void OnPlayAnimation(Dictionary<string, string> parameters)
{
    Debug.Log($"Ejecutando: PlayAnimation {parameters["animationName"]}");
    base.OnPlayAnimation(parameters);
}
```

### Ver evaluación de condiciones
```csharp
protected override bool EvaluateSimpleCondition(ConditionDefinition condition)
{
    bool result = /* tu lógica */;
    Debug.Log($"Condición {condition.name} {condition.@operator} {condition.value} = {result}");
    return result;
}
```

## 🎮 Ejemplos

### Ejemplo Simple: Patrulla y Muerte
Ver `Samples~/Example/TestFSM_AnyState.json`

### Ejemplo Completo: NPC con Lógica Compleja
Ver `Samples~/Example/ExampleNPCController.cs`

## 📊 Estadísticas

- **11** acciones predefinidas
- **8** operadores de condición
- **3** operadores lógicos (AND, OR, NOT)
- **4** tipos de estados
- **2** tipos de condiciones (simple, lógica)
- **0** dependencias externas

## 🤝 Contribuir

Este plugin fue creado como parte del Trabajo Fin de Grado.

Para reportar bugs o sugerir mejoras, contacta con el autor.

## 📄 Licencia

[Especificar licencia según sea necesario]

---

**Versión**: 1.0  
**Autor**: Aleix [Apellido]  
**Última Actualización**: 2026-05-17
