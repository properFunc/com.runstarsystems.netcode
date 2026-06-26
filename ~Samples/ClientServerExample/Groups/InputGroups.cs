using Unity.Entities;
using RunstarSystems.ECS.Data;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Groups
{
    /*
    *   Input is collected before simulation
    */
    [ECSFixedGroupOrder(200)]
    [ClientFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarInputGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(300)]
    [ServerFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarServerInputPrintGroup : ComponentSystemGroup
    {
    }

    /*
    *   Input is gathered from update instead of fixed update
    *   That is where unity collects input so to put it into
    *   the ecs system we also want our collector to be in update
    */
    [ECSUpdateGroupOrder(100)]
    [ClientFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarInputPullGroup : ComponentSystemGroup
    {
    }
}
