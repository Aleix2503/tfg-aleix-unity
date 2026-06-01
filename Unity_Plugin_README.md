# FSM Runtime Plugin for Unity

A powerful, easy-to-use Finite State Machine plugin for Unity with 95 predefined actions, visual editor integration, and complete condition system.

## Features

✨ **95 Predefined Actions**
- Movement, animation, audio, UI, physics, and more
- Easy to extend with custom actions
- DefaultActionExecutor handles all built-in actions

🎮 **Unity Integration**
- FSMBehaviour MonoBehaviour component
- Seamless integration with Unity's component system
- Works with any GameObjects

📊 **Condition System**
- Simple conditions: `variable operator value`
- Logical conditions: AND, OR, NOT
- Full variable support for complex logic

🔧 **Easy Code Generation**
- FSM Generator Tool in Unity Editor
- Auto-generates ActionExecutor and ConditionEvaluator templates
- Customizable for your game logic

💾 **Multiple Data Formats**
- JSON support for FSM definitions
- Compatible with Visual FSM Editor exports
- Serialize to any format

📚 **Documentation & Examples**
- Complete manual with troubleshooting
- Example scenes and scripts
- API documentation and code comments

## Quick Start

### 1. Install from GitHub

**Via Git URL (Recommended):**
```
Window → Package Manager → + → Add package from git URL
https://github.com/yourusername/fsm-runtime-plugin.git
```

**Via Local Path:**
```
Window → Package Manager → + → Add package from disk
Select the plugin folder
```

### 2. Create Your First FSM

```csharp
// 1. Add FSMBehaviour to a GameObject
public class MyCharacter : MonoBehaviour
{
    private FSMBehaviour fsm;
    
    void Start()
    {
        fsm = GetComponent<FSMBehaviour>();
        fsm.StartFSM();
    }
}

// 2. Create your ActionExecutor
public class MyActionExecutor : DefaultActionExecutor
{
    public void PlayAnimation(string animName)
    {
        animator.SetTrigger(animName);
    }
}

// 3. Create your ConditionEvaluator
public class MyConditionEvaluator : ConditionEvaluatorBase
{
    public override bool Evaluate(ConditionDefinition condition)
    {
        // Implement your condition logic
        return true;
    }
}
```

### 3. Define Your FSM

Export from the Visual FSM Editor or create a JSON file:

```json
{
  "version": "1.0",
  "name": "MyFSM",
  "states": [
    {
      "id": "Idle",
      "isEntryPoint": true,
      "enter": [{"action": "PlayAnimation", "params": [{"key": "animName", "value": "idle"}]}],
      "tick": [],
      "exit": []
    }
  ],
  "transitions": []
}
```

## System Requirements

- **Unity 2021.3 LTS or higher**
- **C# 9.0 or higher**
- **300 MB disk space**
- **.NET Standard 2.1 compatibility**

## Project Structure

```
Packages/com.tfg-aleix.runtimefsm/
├── Runtime/
│   ├── Core/              # FSM engine
│   ├── Data/              # Data structures
│   ├── Interfaces/        # IActionExecutor, IConditionEvaluator
│   ├── Implementations/   # DefaultActionExecutor
│   ├── UnityIntegration/  # FSMBehaviour
│   └── Utils/             # Helpers and error handling
├── Editor/
│   └── Tools/             # FSM Generator Window
├── Samples/               # Example scenes and code
└── package.json           # Package manifest
```

## Built-in Actions (95 Total)

**Categories:**
- **Movement:** Move, MoveTo, Stop, Rotate, Jump
- **Animation:** PlayAnimation, StopAnimation, SetParameter
- **Audio:** PlaySound, StopSound, SetVolume
- **Physics:** ApplyForce, SetVelocity, SetGravity
- **UI:** ShowUI, HideUI, UpdateText
- **Game State:** SetPause, SetActive, Destroy
- **And 50+ more...**

See the full list in `DefaultActionExecutor.cs`

## Components

### FSMBehaviour
The main MonoBehaviour that runs your FSM

**Key Methods:**
- `StartFSM()` - Start the state machine
- `StopFSM()` - Stop the state machine
- `TransitionToState(string)` - Force a state transition
- `GetCurrentStateName()` - Get current state

**Events:**
- `OnStateChanged` - Fired when state changes

### State Types
- **Normal:** Regular states with transitions
- **Entry Point:** Starting state
- **Global:** Runs every frame (no transitions)
- **Any_State:** Virtual state for global transitions

## Basic Workflow

1. **Design** your FSM visually using the Visual Editor
2. **Export** as JSON from the Visual Editor
3. **Implement** ActionExecutor and ConditionEvaluator
4. **Assign** to FSMBehaviour component
5. **Test** in Unity Play mode

## Usage Example

```csharp
public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private FSMBehaviour fsm;
    
    void Start()
    {
        fsm = GetComponent<FSMBehaviour>();
        fsm.OnStateChanged += (from, to) => Debug.Log($"{from} → {to}");
        fsm.StartFSM();
    }
}

// Custom ActionExecutor
public class EnemyActionExecutor : DefaultActionExecutor
{
    private Rigidbody rb;
    
    public EnemyActionExecutor(Rigidbody rigidbody) => rb = rigidbody;
    
    public void Chase(float speed)
    {
        // Your chase logic
    }
}

// Custom ConditionEvaluator
public class EnemyConditionEvaluator : ConditionEvaluatorBase
{
    private EnemyAI enemy;
    
    public override bool Evaluate(ConditionDefinition condition)
    {
        if (condition.variableName == "hasTarget")
            return enemy.player != null;
        return false;
    }
}
```

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| Ctrl+K | Open FSM Generator |
| Tools → FSM Generator | Generate code templates |

## Common Tasks

### Add a Custom Action
```csharp
public class MyActionExecutor : DefaultActionExecutor
{
    public void CustomAction(string param)
    {
        // Your code here
    }
}
```

### Evaluate Custom Conditions
```csharp
public override bool Evaluate(ConditionDefinition condition)
{
    return condition.variableName switch
    {
        "health" => currentHealth > 0,
        "hasAmmo" => ammo > 0,
        _ => base.Evaluate(condition)
    };
}
```

### Force State Transition
```csharp
fsm.TransitionToState("Dead");
```

### Listen for State Changes
```csharp
fsm.OnStateChanged += (oldState, newState) =>
{
    Debug.Log($"Changed to {newState}");
};
```

## Documentation

- **Full Manual:** See [Unity_Plugin_MANUAL.md](Unity_Plugin_MANUAL.md)
- **Installation Guide:** [Part I: Installation and Setup](Unity_Plugin_MANUAL.md#part-i-installation-and-setup)
- **Usage Guide:** [Part II: Using the FSM Plugin](Unity_Plugin_MANUAL.md#part-ii-using-the-fsm-plugin)
- **Code Examples:** Check the Samples folder

## Troubleshooting

### Package won't install
- Ensure Git is installed
- Check GitHub URL is correct
- Try using local path installation method

### Actions not executing
- Verify ActionExecutor is assigned in FSMBehaviour
- Check method names match exactly
- Look for error logs in Console

### Transitions not triggering
- Enable debug logging: `fsm.enableDebugLogging = true`
- Check condition logic is correct
- Verify variables are returning expected values

See [Troubleshooting Section](Unity_Plugin_MANUAL.md#11-troubleshooting) for more help

## Support

For detailed documentation:
- 📖 Full manual in [Unity_Plugin_MANUAL.md](Unity_Plugin_MANUAL.md)
- 💡 Examples in `Samples/` folder
- 🔍 API documentation in code comments

## License

Part of a University Thesis Project (TFG - Trabajo de Fin de Grado)

## Next Steps

1. ✅ Install the plugin
2. 📖 Read the [Complete Manual](Unity_Plugin_MANUAL.md)
3. 🎮 Open example scenes
4. 🔧 Create your ActionExecutor
5. ⚙️ Create your ConditionEvaluator
6. 🚀 Build amazing game AI!

Enjoy using the FSM Runtime Plugin! 🎉
