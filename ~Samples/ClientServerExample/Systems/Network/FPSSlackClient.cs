using Unity.Entities;
using Unity.NetCode;

namespace RunstarSystems.ECS.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct SetClientTickRateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            Entity entity;

            if (SystemAPI.HasSingleton<ClientTickRate>())
            {
                entity =
                        SystemAPI.GetSingletonEntity<ClientTickRate>();
            }
            else
            {
                entity =
                        state.EntityManager.CreateEntity();
            }

            ClientTickRate tick_rate =
                    new ClientTickRate
                    {
                        TargetCommandSlack = 5,
                        MaxCommandSlack = 15
                    };

            tick_rate.ResolveDefaults();

            state.EntityManager.SetComponentData(
                    entity,
                    tick_rate);

            state.Enabled = false;
        }
    }
}
