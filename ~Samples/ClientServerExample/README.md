# Priority Groups Sample

This sample shows how to use Runstar priority groups.

Priority groups are ordered ECS groups. Systems can be added to them the same way they are added to normal Unity ECS groups.

## What this sample shows

- How to define ordered groups.
- How to place systems inside those groups.
- How group order controls system update order.

## Basic usage

### Group

```csharp
using Unity.Entities;
using RunstarSystems.ECS.Attributes;

// Builds the group
[ECSUpdateGroupOrder(100)]
public partial class RunstarInputPullGroup : ComponentSystemGroup
{
    // Nothing needs to be in here.
}
```

### System

```csharp
using Unity.Entities;

// Builds the system
[UpdateInGroup(typeof(RunstarInputPullGroup))]
public partial class ExampleInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Runs inside RunstarInputPullGroup.
    }
}
```

## Group types

Use the order attribute that matches the pipeline you want the group added to.

```csharp
[ECSFixedGroupOrder(100)]  // Fixed pipeline
[ECSUpdateGroupOrder(100)] // Update pipeline
[ECSLateGroupOrder(100)]   // Late/presentation pipeline
```

Lower order values run earlier.

Example:

```csharp
[ECSUpdateGroupOrder(100)]
public partial class RunstarInputGroup : ComponentSystemGroup {}

[ECSUpdateGroupOrder(200)]
public partial class RunstarGameplayGroup : ComponentSystemGroup {}

[ECSUpdateGroupOrder(300)]
public partial class RunstarOutputGroup : ComponentSystemGroup {}
```

Runs as:

```text
RunstarInputGroup
RunstarGameplayGroup
RunstarOutputGroup
```

## Notes

- Systems still use normal Unity ECS `UpdateInGroup`.
- Ordered groups are discovered through the Runstar registry.
- If two groups use the same order value, their type names are used as a stable tie-breaker.
- Do not manually sort the Runstar pipeline groups after ordered groups are inserted.
