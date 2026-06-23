using Unity.Entities;
using Unity.NetCode;

namespace RunstarSystems.ECS.Groups
{
    /*
    *   Final level of abstraction for the predicted simulation group.
    *   Allows predicted simulation connections with:
    *
    *       [ECSPredictedGroupOrder(100)]
    *
    *   @NOTE   Sorting is disabled because Runstar preserves numeric
    *           group order manually.
    */
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class RunstarPredictedPipelineGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            base.EnableSystemSorting = false;
        }
    }
}
