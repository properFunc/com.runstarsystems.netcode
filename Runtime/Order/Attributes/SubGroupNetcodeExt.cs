using System;

using group = RunstarSystems.ECS.Groups;

namespace RunstarSystems.ECS.Attributes
{
    /*
    *   @EXAMPLE
    *               [ECSPredictedGroupOrder(100)]
    *               public partial class PredictedExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ECSPredictedGroupOrderAttribute :
            ECSGroupOrderAttribute
    {
        public ECSPredictedGroupOrderAttribute(
                int group_order)
                : base(
                        group_order,
                        typeof(group.RunstarPredictedPipelineGroup))
        {
        }
    }

    /*
    *   @EXAMPLE
    *               [ECSPredictedFixedGroupOrder(100)]
    *               public partial class PredictedFixedExampleGroup : ComponentSystemGroup {}
    */
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ECSPredictedFixedGroupOrderAttribute :
            ECSGroupOrderAttribute
    {
        public ECSPredictedFixedGroupOrderAttribute(
                int group_order)
                : base(
                        group_order,
                        typeof(group.RunstarPredictedFixedPipelineGroup))
        {
        }
    }
}
