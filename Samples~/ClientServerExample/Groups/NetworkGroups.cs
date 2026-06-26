using Unity.Entities;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Groups
{
    [ECSFixedGroupOrder(50)]
    [ClientFilterTrait(InheritMode.Inheritable)]
    [ServerFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarNetworkConnectionGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(100)]
    [ClientFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarClientNetworkReceiveGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(110)]
    [ServerFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarServerNetworkReceiveGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(600)]
    [ClientFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarClientNetworkSendGroup : ComponentSystemGroup
    {
    }

    [ECSFixedGroupOrder(610)]
    [ServerFilterTrait(InheritMode.Inheritable)]
    public partial class RunstarServerNetworkSendGroup : ComponentSystemGroup
    {
    }
}
