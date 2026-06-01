# FSM Runtime Plugin for Unity - Quick Start Guide

Get your first FSM up and running in 10 minutes!

## Installation (2 minutes)

### Step 1: Open Package Manager
1. In Unity, go to **Window → Package Manager**
2. Click the **"+"** button in the top-left

### Step 2: Add from Git URL
1. Select **"Add package from git URL..."**
2. Paste: `https://github.com/yourusername/fsm-runtime-plugin.git`
3. Click **Add**
4. Wait for import to complete

### Step 3: Verify Installation
1. Go to **Tools** menu
2. You should see **"FSM Generator"** option
3. The plugin is ready!

---

## Create Your First FSM (8 minutes)

### Step 1: Create a Scene (1 minute)

1. Create a new Scene: **File → New Scene**
2. Save it as "FSMDemo"

### Step 2: Add Game Object (1 minute)

1. In Hierarchy, **Right-click → Create Empty**
2. Name it "Agent"
3. Add a Rigidbody for physics (optional)

### Step 3: Add FSMBehaviour (1 minute)

1. Select the "Agent" GameObject
2. In Inspector, click **Add Component**
3. Search for **"FSMBehaviour"**
4. Add it

### Step 4: Create ActionExecutor (2 minutes)

Create a new C# script named `MyActionExecutor.cs`:

```csharp
using UnityEngine;
using FSM.Implementations;

public class MyActionExecutor : DefaultActionExecutor
{
    private Transform transform;
    private Animator animator;
    
    public MyActionExecutor(Transform t, Animator a)
    {
        transform = t;
        animator = a;
    }
    
    // Override or add custom actions
    public void Move(Vector3 direction, float speed)
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    
    public void PlayAnimation(string animName)
    {
        if (animator != null)
            animator.SetTrigger(animName);
    }
}
```

### Step 5: Create ConditionEvaluator (2 minutes)

Create a new C# script named `MyConditionEvaluator.cs`:

```csharp
using UnityEngine;
using FSM.Interfaces;
using FSM.Data;
using FSM.Implementations;

public class MyConditionEvaluator : ConditionEvaluatorBase
{
    private Transform transform;
    public Transform targetPosition;
    public float distanceThreshold = 5f;
    
    public MyConditionEvaluator(Transform t)
    {
        transform = t;
    }
    
    public override bool Evaluate(ConditionDefinition condition)
    {
        if (condition.type == ConditionType.Simple)
        {
            var simple = condition as SimpleConditionDefinition;
            return EvaluateCondition(simple.variableName, simple.op, simple.value);
        }
        
        return base.Evaluate(condition);
    }
    
    private bool EvaluateCondition(string variable, ConditionOperator op, object value)
    {
        bool result = variable switch
        {
            "hasTarget" => targetPosition != null,
            "distanceToTarget" => targetPosition != null && 
                Vector3.Distance(transform.position, targetPosition.position) < distanceThreshold,
            _ => false
        };
        
        return result;
    }
}
```

### Step 6: Create FSM JSON File (1 minute)

Create `Assets/Resources/FSMs/SimpleFSM.json`:

```json
{
  "version": "1.0",
  "name": "SimpleFSM",
  "states": [
    {
      "id": "Idle",
      "type": "Normal",
      "isEntryPoint": true,
      "enter": [
        {
          "action": "PlayAnimation",
          "params": [{"key": "animName", "value": "idle"}]
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
          "params": [{"key": "animName", "value": "run"}]
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
        "variableName": "hasTarget",
        "operator": "Equal",
        "value": "true"
      }
    },
    {
      "from": "Chase",
      "to": "Idle",
      "condition": {
        "type": "Simple",
        "variableName": "hasTarget",
        "operator": "Equal",
        "value": "false"
      }
    }
  ]
}
```

### Step 7: Connect Everything (1 minute)

Create `AgentController.cs`:

```csharp
using UnityEngine;
using FSM.Data;

public class AgentController : MonoBehaviour
{
    public Transform target;
    private FSMBehaviour fsm;
    
    private void Start()
    {
        fsm = GetComponent<FSMBehaviour>();
        
        // Load FSM data
        TextAsset fsmFile = Resources.Load<TextAsset>("FSMs/SimpleFSM");
        var definition = JsonUtility.FromJson<FSMDefinition>(fsmFile.text);
        
        // Create implementations
        var actionExec = new MyActionExecutor(transform, GetComponent<Animator>());
        var condEval = new MyConditionEvaluator(transform);
        condEval.targetPosition = target;
        
        // Initialize and start
        fsm.Initialize(definition, actionExec, condEval);
        fsm.StartFSM();
    }
    
    private void Update()
    {
        // Update target for condition checking
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                target = hit.transform;
            }
        }
    }
}
```

### Step 8: Assign and Test (1 minute)

1. Select the "Agent" GameObject
2. Add **AgentController** component (drag script to Inspector)
3. In FSMBehaviour:
   - Assign the action executor: Click in Action Executor field
   - Assign the condition evaluator: Click in Condition Evaluator field
4. Press **Play**
5. Click in the scene to set target
6. Watch the FSM work! 🎉

---

## What You Just Created

✅ A working Finite State Machine!

**The flow:**
```
Idle State (playing idle animation)
    ↓ (when hasTarget == true)
Chase State (playing run animation)
    ↓ (when hasTarget == false)
Idle State
```

Your agent will:
- Start in Idle state
- When you click (target set), transition to Chase
- When target is removed, return to Idle

---

## Next Steps

### Learn More About:

- **State Types:** Read about Normal, Entry Point, Global States
  - See: [Manual Section 7](Unity_Plugin_MANUAL.md#7-state-types)

- **Actions:** Use the 95 predefined actions or create your own
  - See: [Manual Section 3](Unity_Plugin_MANUAL.md#3-creating-an-action-executor)

- **Conditions:** Build complex condition logic
  - See: [Manual Section 4](Unity_Plugin_MANUAL.md#4-creating-a-condition-evaluator)

- **Advanced Features:** Error handling, debugging, patterns
  - See: [Complete Manual](Unity_Plugin_MANUAL.md)

### Common Additions:

**Add movement:**
```csharp
public void Move(Vector3 direction, float speed)
{
    transform.Translate(direction * speed * Time.deltaTime);
}
```

**Add audio:**
```csharp
public void PlaySound(string soundName)
{
    AudioManager.PlaySound(soundName);
}
```

**Add complex conditions:**
```csharp
"health < 30" => currentHealth < 30,
"canAttack" => attackCooldown <= 0,
"nearTarget" => Vector3.Distance(transform.position, target) < 3f,
```

---

## Keyboard Tips

- Use **Ctrl+K** to open FSM Generator Tool
- Use **Inspector context menu** for quick FSM data view

---

## Common Issues & Quick Fixes

### "FSMBehaviour not found"
- Wait for Unity to finish importing
- Go to **Assets → Reimport All**

### "Action not executing"
- Check method name matches exactly in ActionExecutor
- Ensure method is **public**
- Look for error logs in Console

### "State won't change"
- Add logging: Print condition values
- Verify variable conditions are true
- Check transition is defined in JSON

### "Component not assigning"
- Drag components from Hierarchy
- Or create instances manually in code

---

## Project Structure

Keep it organized:

```
Assets/
├── Scripts/
│   ├── FSM/
│   │   ├── MyActionExecutor.cs
│   │   └── MyConditionEvaluator.cs
│   └── Controllers/
│       └── AgentController.cs
└── Resources/
    └── FSMs/
        └── SimpleFSM.json
```

---

## Tips for Success

✅ **DO:**
- Start simple (2-3 states)
- Test each transition
- Use meaningful state names
- Check Console for errors
- Expand features gradually

❌ **DON'T:**
- Try to create complex FSM first
- Forget to assign implementations
- Use identical state names
- Skip error logging
- Ignore condition logic

---

## Ready to Build More?

Now that you understand the basics:

1. **AI Controller:** Build an enemy with multiple behaviors
2. **UI Manager:** Create menu state machines
3. **Game States:** Design menu → playing → paused → gameover
4. **Character States:** Idle → Run → Jump → Fall → Land

Each one follows the same pattern you just learned!

---

## Need Help?

1. **First Time?** You're reading it! ✓
2. **Want More Details?** See [Complete Manual](Unity_Plugin_MANUAL.md)
3. **Specific Problem?** Check [Troubleshooting](Unity_Plugin_MANUAL.md#11-troubleshooting)

---

## Complete Workflow Summary

```
1. Install Plugin (2 min)
        ↓
2. Create GameObject + FSMBehaviour (2 min)
        ↓
3. Write ActionExecutor (2 min)
        ↓
4. Write ConditionEvaluator (2 min)
        ↓
5. Create FSM JSON (1 min)
        ↓
6. Connect Everything (1 min)
        ↓
7. Press Play (0 min)
        ↓
8. See it Work! 🎉
```

---

Happy FSM building with Unity! 🚀
