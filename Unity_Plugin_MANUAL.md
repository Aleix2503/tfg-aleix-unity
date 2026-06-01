# FSM Runtime Plugin for Unity - User Manual

## Table of Contents

1. [Part I: Installation and Setup](#part-i-installation-and-setup)
2. [Part II: Using the FSM Plugin](#part-ii-using-the-fsm-plugin)

---

# PART I: Installation and Setup

## 1. System Requirements

Before you begin, ensure you have:

- **Unity 2021.3 LTS or higher** (recommended)
- **C# 9.0 or higher** support
- **Git installed** on your system (for package installation)
- A **GitHub account** (recommended)
- Approximately **500 MB** free disk space

## 2. Understanding the Plugin Package

The FSM Runtime Plugin is distributed as a **Unity Package Manager (UPM) package** via GitHub.

**Package Name:** `com.tfg-aleix.runtimefsm`

**What's Included:**
- Core FSM runtime engine
- 95 predefined actions (DefaultActionExecutor)
- Editor tools for code generation
- Condition evaluation system
- Error handling utilities
- Example scenes and templates

## 3. Installation Methods

### 3.1 Installation via Git URL (Recommended)

This method pulls the package directly from GitHub.

#### Step 1: Open Package Manager

1. In Unity, go to **Window → TextEditor and IDE → Package Manager**
   - Or use the menu: **Window → Package Manager**

#### Step 2: Add Package from Git URL

1. Click the **"+"** button in the top-left corner of Package Manager
2. Select **"Add package from git URL..."**
3. Enter the GitHub repository URL:
   ```
   https://github.com/yourusername/fsm-runtime-plugin.git
   ```
4. Click **Add**
5. Unity will download and install the package (may take a moment)

#### Step 3: Verify Installation

Once installed, you should see `com.tfg-aleix.runtimefsm` in the Package Manager list.

### 3.2 Installation via Local Path

If you've cloned the repository locally:

#### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/fsm-runtime-plugin.git
cd fsm-runtime-plugin
```

#### Step 2: Add to Your Project

1. Open Package Manager in Unity
2. Click **"+"** → **"Add package from disk..."**
3. Navigate to the cloned repository folder
4. Select the **`package.json`** file
5. Click **Open**

### 3.3 Manual Installation (Manual Folder)

If Git is not available:

1. Download the plugin as a ZIP file from GitHub
2. Extract to your project's `Packages` folder
3. Rename the folder to match the package name pattern
4. Restart Unity

**Note:** We recommend using Git URL for easier updates.

## 4. First-Time Setup

### 4.1 Verify the Installation

After installation, verify everything is set up correctly:

1. Create a new scene in your project
2. Create an empty GameObject
3. Add the **FSMBehaviour** component (Attach script and search for "FSMBehaviour")
4. The component should appear without errors

### 4.2 Load Sample Scenes

The plugin includes example scenes:

1. In the Project window, navigate to:
   ```
   Packages > FSM Runtime Plugin > Samples > Example > Scenes
   ```
2. Open **"ExampleScene.unity"** or **"TestFSMScene.unity"**
3. Press **Play** to see the FSM in action

### 4.3 Accessing the FSM Generator Tool

The plugin includes an editor tool for generating code:

1. Go to **Tools → FSM Generator** in the menu bar
2. The FSM Generator Window will open
3. This tool helps create ActionExecutor and ConditionEvaluator templates

## 5. Project Structure

Understanding the plugin structure:

```
Packages/com.tfg-aleix.runtimefsm/
├── Runtime/                           # Runtime code
│   ├── Core/                         # Core FSM engine
│   │   ├── FSM.cs                   # Main FSM class
│   │   ├── FSMState.cs              # State definition
│   │   ├── FSMTransition.cs         # Transition logic
│   │   └── FSMEvent.cs              # Event system
│   ├── Data/                        # Data structures
│   │   ├── FSMDefinition.cs         # FSM JSON structure
│   │   ├── StateDefinition.cs       # State definition
│   │   ├── TransitionDefinition.cs  # Transition definition
│   │   ├── ActionDefinition.cs      # Action structure
│   │   ├── ConditionDefinition.cs   # Condition structure
│   │   └── *Operator.cs             # Operator enums
│   ├── Interfaces/                  # Interfaces to implement
│   │   ├── IActionExecutor.cs       # Interface for actions
│   │   └── IConditionEvaluator.cs   # Interface for conditions
│   ├── Implementations/             # Default implementations
│   │   ├── DefaultActionExecutor.cs # 95 predefined actions
│   │   └── ConditionEvaluatorBase.cs # Condition logic
│   ├── UnityIntegration/            # Unity-specific code
│   │   ├── FSMBehaviour.cs          # MonoBehaviour for FSM
│   │   └── FSMInstaller.cs          # Dependency injection
│   └── Utils/                       # Utility classes
│       ├── ErrorHandlers.cs         # Error handling
│       └── ConditionParser.cs       # Condition parsing
├── Editor/                          # Editor tools
│   ├── Tools/                       # Editor windows
│   │   └── FSMGeneratorWindow.cs   # Code generation tool
│   └── Inspectors/                  # Custom inspectors
├── Samples/                         # Example content
│   ├── Example/                     # Example scenes
│   │   ├── Scenes/                 # Sample scenes
│   │   ├── Scripts/                # Example scripts
│   │   └── FSMs/                   # Sample FSM files
│   └── Documentation/              # Additional docs
├── package.json                     # Package manifest
└── README.md                        # Package readme
```

## 6. Updating the Plugin

### 6.1 Update via Git URL

If installed via Git URL:

1. Open **Package Manager**
2. Find **FSM Runtime Plugin** in the list
3. Click on it to select it
4. Click the **Update** button (if available)

### 6.2 Manual Update

If installed via local path:

1. Navigate to your cloned repository folder
2. Run: `git pull origin main`
3. The changes will be reflected in your Unity project automatically

## 7. Troubleshooting Installation

### 7.1 "Cannot Add Package" Error

**Cause:** Git is not installed or the URL is incorrect

**Solution:**
- Install Git from https://git-scm.com/
- Verify the GitHub repository URL is correct
- Ensure your internet connection is working
- Try using the manual local path method

### 7.2 "Missing Package" or "Errors in Package"

**Cause:** Incomplete installation or missing dependencies

**Solution:**
- Remove the package: right-click in Package Manager → Remove
- Restart Unity
- Reinstall the package
- Ensure you have C# 9.0 support in your Unity version

### 7.3 "FSMBehaviour Not Found"

**Cause:** Package not fully imported

**Solution:**
- Wait for Unity to finish importing (check bottom-right progress indicator)
- Go to **Assets → Reimport All**
- Restart Unity if needed

### 7.4 File Association Errors

**Cause:** `.fsmproj` files not recognized

**Solution:**
- This is expected; FSMPROJ files are for the Visual Editor
- The plugin loads FSM data from JSON format
- Use the Visual Editor to create `.fsmproj` files, then export as JSON

---

# PART II: Using the FSM Plugin

## Overview

The FSM Runtime Plugin for Unity provides a complete finite state machine system for your games and applications. It integrates seamlessly with Unity's component system through the **FSMBehaviour** MonoBehaviour.

## 1. Understanding the Architecture

### 1.1 Core Components

**FSM (Finite State Machine):**
- Container for all states and transitions
- Manages state changes and transitions
- Executes actions based on conditions

**State:**
- Individual states with Enter, Tick, and Exit actions
- Can be a regular state, entry point, global state, or any_state
- Contains action lists for different phases

**Transition:**
- Connection between states
- Defined by conditions that must be met
- Triggers state changes

**Condition:**
- Logic that determines if a transition should execute
- Can be simple (single variable check) or logical (AND/OR/NOT)
- Evaluated every frame for active transitions

**Action:**
- Code executed during state lifecycle
- Happens during Enter, Tick, or Exit phase
- Can have parameters

### 1.2 Execution Flow

```
┌─────────────────────────────────────┐
│  FSMBehaviour.Update() called       │
├─────────────────────────────────────┤
│  1. Evaluate all transition         │
│     conditions from current state   │
├─────────────────────────────────────┤
│  2. If condition is true,           │
│     execute transition              │
├─────────────────────────────────────┤
│  3. If transitioning:               │
│     - Call current state Exit()     │
│     - Call new state Enter()        │
├─────────────────────────────────────┤
│  4. Call current state Tick()       │
│     (every frame)                   │
└─────────────────────────────────────┘
```

## 2. Setting Up FSM in Your Scene

### 2.1 Create a Game Object

1. In the Hierarchy, **right-click → Create Empty**
2. Name it something meaningful (e.g., "Enemy", "Player", "UIManager")
3. Keep it at position (0, 0, 0) or position as needed

### 2.2 Add FSMBehaviour Component

1. Select the GameObject
2. In the Inspector, click **Add Component**
3. Search for **"FSMBehaviour"**
4. Click to add it

### 2.3 Configure FSM Data

1. In the FSMBehaviour component, find the **"FSM Definition"** field
2. Assign a JSON file containing your FSM structure
   - Or create it programmatically (see Section 5)

### 2.4 Create Custom Implementation Classes

You need two classes:

**ActionExecutor.cs** - Executes actions
```csharp
public class MyActionExecutor : IActionExecutor
{
    public void Execute(ActionDefinition action)
    {
        // Implement action execution
    }
}
```

**ConditionEvaluator.cs** - Evaluates conditions
```csharp
public class MyConditionEvaluator : IConditionEvaluator
{
    public bool Evaluate(ConditionDefinition condition)
    {
        // Implement condition evaluation
    }
}
```

### 2.5 Assign Implementation Classes

1. In FSMBehaviour Inspector, find **"Action Executor"** and **"Condition Evaluator"**
2. Assign your custom classes
3. The FSM is now ready to run

## 3. Creating an Action Executor

An ActionExecutor is responsible for executing all actions in your FSM.

### 3.1 Using DefaultActionExecutor

The plugin includes 95 predefined actions:

```csharp
public class MyActionExecutor : DefaultActionExecutor
{
    private MyCharacter character;
    
    public MyActionExecutor(MyCharacter character)
    {
        this.character = character;
    }
    
    // Optional: Override specific actions or add custom ones
    public override void Move(Vector3 direction, float speed)
    {
        character.rigidbody.velocity = direction * speed;
    }
}
```

### 3.2 Implementing Custom ActionExecutor

```csharp
using UnityEngine;
using FSM.Interfaces;
using FSM.Data;
using System.Reflection;

public class CustomActionExecutor : IActionExecutor
{
    private MyCharacter character;
    
    public CustomActionExecutor(MyCharacter character)
    {
        this.character = character;
    }
    
    public void Execute(ActionDefinition action)
    {
        // Get the method matching the action name
        MethodInfo method = GetType().GetMethod(
            action.actionName,
            BindingFlags.Public | BindingFlags.Instance
        );
        
        if (method == null)
        {
            Debug.LogError($"Action '{action.actionName}' not found");
            return;
        }
        
        // Parse parameters
        object[] parameters = ParseParameters(action.parameters, method);
        
        // Execute the method
        try
        {
            method.Invoke(this, parameters);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error executing action '{action.actionName}': {e.Message}");
        }
    }
    
    // Action implementations
    public void PlayAnimation(string animationName)
    {
        character.animator.SetTrigger(animationName);
    }
    
    public void SetSpeed(float speed)
    {
        character.movementSpeed = speed;
    }
    
    public void PlaySound(string soundName)
    {
        AudioManager.PlaySound(soundName);
    }
    
    private object[] ParseParameters(ActionParameter[] parameters, MethodInfo method)
    {
        // Implementation for parameter parsing
        // Converts string values to appropriate types
        // ...
        return new object[parameters.Length];
    }
}
```

### 3.3 Available Default Actions (95 Total)

**Movement Actions:**
- `Move(Vector3 direction, float speed)`
- `MoveTo(Vector3 position, float speed)`
- `Stop()`
- `Rotate(Vector3 direction, float speed)`
- `Jump(float force)`

**Animation Actions:**
- `PlayAnimation(string animationName)`
- `StopAnimation()`
- `SetAnimationParameter(string paramName, object value)`

**Audio Actions:**
- `PlaySound(string soundName)`
- `StopSound()`
- `SetVolume(float volume)`

**Particle Effects:**
- `PlayParticles(string effectName)`
- `StopParticles()`

**Game State:**
- `SetGamePause(bool isPaused)`
- `SetActive(bool active)`
- `Destroy()`

**UI Actions:**
- `ShowUI(string uiName)`
- `HideUI(string uiName)`
- `UpdateUIText(string elementName, string text)`

**And many more... see DefaultActionExecutor.cs for complete list**

## 4. Creating a Condition Evaluator

A ConditionEvaluator determines when transitions should execute.

### 4.1 Basic Condition Evaluator

```csharp
using UnityEngine;
using FSM.Interfaces;
using FSM.Data;

public class CharacterConditionEvaluator : ConditionEvaluatorBase
{
    private MyCharacter character;
    
    public CharacterConditionEvaluator(MyCharacter character)
    {
        this.character = character;
    }
    
    public override bool Evaluate(ConditionDefinition condition)
    {
        // Handle simple conditions
        if (condition.type == ConditionType.Simple)
        {
            return EvaluateSimple(condition as SimpleConditionDefinition);
        }
        
        // Handle logical conditions
        if (condition.type == ConditionType.Logical)
        {
            return EvaluateLogical(condition as LogicalConditionDefinition);
        }
        
        return false;
    }
    
    private bool EvaluateSimple(SimpleConditionDefinition condition)
    {
        // Get variable value
        object variableValue = GetVariableValue(condition.variableName);
        object compareValue = condition.value;
        
        // Compare based on operator
        return condition.op switch
        {
            ConditionOperator.Equal => variableValue.Equals(compareValue),
            ConditionOperator.NotEqual => !variableValue.Equals(compareValue),
            ConditionOperator.GreaterThan => 
                float.Parse(variableValue.ToString()) > float.Parse(compareValue.ToString()),
            ConditionOperator.LessThan => 
                float.Parse(variableValue.ToString()) < float.Parse(compareValue.ToString()),
            _ => false
        };
    }
    
    private bool EvaluateLogical(LogicalConditionDefinition condition)
    {
        return condition.op switch
        {
            LogicalOperator.AND => 
                condition.conditions.All(c => Evaluate(c)),
            LogicalOperator.OR => 
                condition.conditions.Any(c => Evaluate(c)),
            LogicalOperator.NOT => 
                !Evaluate(condition.conditions[0]),
            _ => false
        };
    }
    
    private object GetVariableValue(string variableName)
    {
        // Map variable names to character properties
        return variableName switch
        {
            "health" => character.health,
            "hasTarget" => character.currentTarget != null,
            "distanceToTarget" => 
                character.currentTarget != null ? 
                Vector3.Distance(character.transform.position, character.currentTarget.position) : 
                float.MaxValue,
            "isAlive" => character.health > 0,
            "speed" => character.currentSpeed,
            _ => null
        };
    }
}
```

## 5. Loading FSM Data

FSM data can be loaded from multiple sources.

### 5.1 Loading from JSON File

Create a JSON file with your FSM structure (exported from the Visual Editor):

**MyFSM.json:**
```json
{
  "version": "1.0",
  "name": "EnemyAI",
  "states": [
    {
      "id": "Idle",
      "type": "Normal",
      "isEntryPoint": true,
      "enter": [
        {
          "action": "PlayAnimation",
          "params": [{"key": "animationName", "value": "idle"}]
        }
      ],
      "tick": [],
      "exit": []
    },
    {
      "id": "Chase",
      "type": "Normal",
      "enter": [
        {
          "action": "PlayAnimation",
          "params": [{"key": "animationName", "value": "run"}]
        }
      ],
      "tick": [],
      "exit": []
    }
  ],
  "transitions": [
    {
      "from": "Idle",
      "to": "Chase",
      "condition": {
        "type": "Simple",
        "variable": "hasTarget",
        "operator": "Equal",
        "value": "true"
      }
    }
  ]
}
```

### 5.2 Loading in Code

```csharp
using UnityEngine;
using FSM.Core;
using FSM.Data;

public class EnemyController : MonoBehaviour
{
    private FSM fsmEngine;
    private FSMBehaviour fsmBehaviour;
    
    private void Start()
    {
        // Load FSM data from JSON
        TextAsset fsmFile = Resources.Load<TextAsset>("FSMs/EnemyAI");
        FSMDefinition definition = JsonUtility.FromJson<FSMDefinition>(fsmFile.text);
        
        // Get FSMBehaviour component
        fsmBehaviour = GetComponent<FSMBehaviour>();
        
        // Create implementation classes
        var actionExecutor = new MyActionExecutor(this);
        var conditionEvaluator = new MyConditionEvaluator(this);
        
        // Initialize FSM
        fsmBehaviour.Initialize(definition, actionExecutor, conditionEvaluator);
    }
}
```

## 6. FSMBehaviour Component

FSMBehaviour is the main MonoBehaviour that runs your FSM.

### 6.1 Inspector Properties

- **FSM Definition** (TextAsset): JSON file with FSM structure
- **Action Executor** (IActionExecutor): Component that executes actions
- **Condition Evaluator** (IConditionEvaluator): Component that evaluates conditions
- **Start On Awake** (bool): Automatically start FSM when scene loads

### 6.2 Programmatic Control

```csharp
FSMBehaviour fsm = GetComponent<FSMBehaviour>();

// Start the FSM
fsm.StartFSM();

// Stop the FSM
fsm.StopFSM();

// Get current state
string currentState = fsm.GetCurrentStateName();

// Listen for state changes
fsm.OnStateChanged += (oldState, newState) => 
{
    Debug.Log($"Changed from {oldState} to {newState}");
};

// Force transition to a specific state
fsm.TransitionToState("Chase");
```

## 7. State Types

The plugin supports different state types for different behaviors.

### 7.1 Normal State

Regular states with full state machine behavior.

**Characteristics:**
- Can have incoming and outgoing transitions
- Executes Enter, Tick, and Exit actions
- Can be the Entry Point

**When to use:**
- Most of your FSM states
- Character behaviors (Idle, Run, Attack, etc.)

### 7.2 Entry Point State

The initial state when FSM starts.

**Characteristics:**
- Exactly one per FSM
- Automatically entered at startup
- Otherwise works like a normal state

**When to use:**
- Define starting behavior of your FSM

### 7.3 Global State

Executes every frame regardless of current state.

**Characteristics:**
- Only executes Tick actions
- No Enter or Exit actions
- No transitions
- Runs parallel to normal states

**When to use:**
- Health monitoring
- Input processing
- Always-active logic

### 7.4 ANY_STATE

Virtual state for global transitions.

**Characteristics:**
- Auto-created, cannot be deleted
- Transitions FROM ANY_STATE are global
- Not visible in state list
- Useful for emergency states

**When to use:**
- "If health reaches 0, go to Dead state" (from any state)
- "If player presses escape, go to Menu" (from any gameplay state)

## 8. Transitions and Conditions

### 8.1 Transition Execution

Transitions are checked every frame:

1. Current state's outgoing transitions are evaluated
2. Conditions are checked left-to-right
3. First true condition triggers transition
4. Any_State transitions checked last

### 8.2 Condition Types

**Simple Condition:**
```
variable operator value
```
Example: `health < 30`

**Logical Condition:**
```
condition1 AND condition2
condition1 OR condition2
NOT condition
```
Example: `(health < 30) AND (hasPotion == true)`

### 8.3 Supported Operators

| Operator | Type | Example |
|----------|------|---------|
| == | All | `health == 100` |
| != | All | `state != Attacking` |
| > | Numeric | `distance > 5.0` |
| >= | Numeric | `health >= 30` |
| < | Numeric | `ammo < 1` |
| <= | Numeric | `stamina <= 0` |

## 9. Best Practices

### 9.1 Code Organization

**Structure your implementation:**
```
Assets/
├── Scripts/
│   ├── FSM/
│   │   ├── ActionExecutor.cs
│   │   ├── ConditionEvaluator.cs
│   │   └── FSMController.cs
│   ├── Character/
│   │   ├── CharacterController.cs
│   │   └── CharacterStats.cs
│   └── Managers/
│       └── GameManager.cs
└── Resources/
    └── FSMs/
        ├── EnemyAI.json
        ├── PlayerFSM.json
        └── UIStateMachine.json
```

### 9.2 Naming Conventions

- **States:** PascalCase, descriptive (Idle, Chase, Attack, Dead)
- **Actions:** PascalCase, verb-based (PlayAnimation, SetSpeed)
- **Conditions:** Describe the check (hasTarget, healthBelowThreshold)
- **Variables:** camelCase (currentHealth, distanceToPlayer)

### 9.3 Performance Tips

- **Minimize condition complexity:** Simple conditions execute faster
- **Cache calculations:** Store frequently accessed values
- **Use Global States sparingly:** They run every frame
- **Avoid nested logical conditions:** Keep logic simple and readable
- **Profile your FSM:** Use Unity Profiler to find bottlenecks

### 9.4 Debugging

**Enable Debug Logging:**
```csharp
fsmBehaviour.enableDebugLogging = true;
```

**Listen for state changes:**
```csharp
fsmBehaviour.OnStateChanged += (from, to) => 
{
    Debug.Log($"State: {from} → {to}");
};
```

**Check current state:**
```csharp
Debug.Log($"Current State: {fsmBehaviour.GetCurrentStateName()}");
```

## 10. Common Patterns

### 10.1 Simple AI Controller

```csharp
public class SimpleEnemyAI : MonoBehaviour
{
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 2f;
    
    public Transform playerTarget;
    private FSMBehaviour fsm;
    
    private void Start()
    {
        fsm = GetComponent<FSMBehaviour>();
        fsm.OnStateChanged += OnStateChanged;
    }
    
    private void Update()
    {
        // Update variables for condition checking
        playerTarget = FindPlayer();
    }
    
    private Transform FindPlayer()
    {
        // Find player in sight range
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
                return col.transform;
        }
        return null;
    }
    
    private void OnStateChanged(string from, string to)
    {
        Debug.Log($"Enemy: {from} → {to}");
    }
}
```

### 10.2 UI State Machine

```csharp
public class UIManager : MonoBehaviour
{
    private FSMBehaviour uiFSM;
    
    public void ShowMainMenu()
    {
        uiFSM.TransitionToState("MainMenu");
    }
    
    public void StartGame()
    {
        uiFSM.TransitionToState("InGame");
    }
    
    public void PauseGame()
    {
        uiFSM.TransitionToState("Paused");
    }
    
    public void ShowGameOver()
    {
        uiFSM.TransitionToState("GameOver");
    }
}
```

## 11. Troubleshooting

### 11.1 "Action Not Found" Error

**Cause:** The action method doesn't exist in your ActionExecutor

**Solution:**
- Check the action name matches the method name exactly
- Ensure the method is public
- Verify parameter types match

### 11.2 Transitions Not Working

**Cause:** Conditions never evaluate to true

**Solution:**
- Add debug logging to condition evaluator
- Check variable values are being returned correctly
- Verify operator logic is correct
- Test conditions individually

### 11.3 Actions Not Executing

**Cause:** ActionExecutor is not properly assigned

**Solution:**
- Verify ActionExecutor is assigned in FSMBehaviour
- Check for error logs when action executes
- Ensure ActionExecutor class implements IActionExecutor

### 11.4 FSM Stays in Starting State

**Cause:** Entry Point not found or conditions are false

**Solution:**
- Verify you have exactly one state marked as Entry Point
- Check conditions with debug logging
- Ensure current state has outgoing transitions

### 11.5 Memory Leaks or Performance Issues

**Cause:** FSM not properly cleaned up

**Solution:**
- Call `fsm.StopFSM()` before destroying GameObject
- Unsubscribe from FSM events: `fsm.OnStateChanged -= ...`
- Dispose of resources in Exit actions

## 12. Keyboard Shortcuts in Editor

| Shortcut | Action |
|----------|--------|
| Ctrl+K | Open FSM Generator Tool |
| Ctrl+Shift+I | Inspect FSM Data (when selected) |

## 13. Support and Additional Resources

### 13.1 Documentation

- **Code Documentation:** See XML comments in source files
- **Example Scenes:** Check Samples folder for working examples
- **API Reference:** Use IntelliSense in your IDE for method signatures

### 13.2 Getting Help

If you encounter issues:

1. Check this manual's troubleshooting section
2. Review example scenes for implementation patterns
3. Check the plugin's GitHub issues page
4. Inspect console logs for error messages
5. Enable debug logging in FSMBehaviour

### 13.3 Extending the Plugin

The plugin is designed to be extended:

- **Custom Actions:** Inherit from `DefaultActionExecutor` or implement `IActionExecutor`
- **Custom Conditions:** Extend `ConditionEvaluatorBase` or implement `IConditionEvaluator`
- **Custom State Types:** Modify FSMState to add new state type handling

---

## Quick Reference

### FSMBehaviour Methods

```csharp
void StartFSM()                                    // Start the FSM
void StopFSM()                                     // Stop the FSM
void TransitionToState(string stateName)           // Force transition
string GetCurrentStateName()                       // Get current state name
void Initialize(FSMDefinition def, ...)           // Initialize with data
```

### Key Interfaces

```csharp
public interface IActionExecutor
{
    void Execute(ActionDefinition action);
}

public interface IConditionEvaluator
{
    bool Evaluate(ConditionDefinition condition);
}
```

### Loading FSM Data

```csharp
// From JSON file
TextAsset fsmFile = Resources.Load<TextAsset>("FSMs/MyFSM");
FSMDefinition def = JsonUtility.FromJson<FSMDefinition>(fsmFile.text);

// From Visual Editor export
// Use the exported JSON file directly
```

---

## Next Steps

1. **Follow the examples:** Open and study the sample scenes
2. **Create your FSM:** Use the Visual Editor to design your FSM visually
3. **Implement actions:** Write your ActionExecutor
4. **Implement conditions:** Write your ConditionEvaluator
5. **Integrate with your game:** Add FSMBehaviour to your GameObjects
6. **Test and iterate:** Use debug logging and profiling

Enjoy building powerful state machines with the FSM Runtime Plugin! 🚀
