using Unity.Entities;
using Unity.NetCode;

namespace RunstarSystems.ECS.Groups
{
    /*
    *   Final level of abstraction for the predicted fixed simulation group.
    *   Allows predicted fixed connections with:
    *
    *       [ECSPredictedFixedGroupOrder(100)]
    *
    *   @NOTE   Sorting is disabled because Runstar preserves numeric
    *           group order manually.
    */
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial class RunstarPredictedFixedPipelineGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            base.EnableSystemSorting = false;
        }
    }
}
