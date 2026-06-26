using Unity.Entities;
using Unity.NetCode;

namespace RunstarSystems.ECS.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct SetClientTickRateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ClientTickRate>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Entity entity =
                    SystemAPI.GetSingletonEntity<ClientTickRate>();

            ClientTickRate tick_rate =
                    SystemAPI.GetSingleton<ClientTickRate>();

            tick_rate.TargetCommandSlack = 5;
            tick_rate.NumAdditionalCommandsToSend = 15;

            state.EntityManager.SetComponentData(
                    entity,
                    tick_rate);

            state.Enabled = false;
        }
    }
}
