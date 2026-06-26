using Unity.Entities;
using Unity.NetCode;

namespace RunstarSystems.ECS.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SetNetcodeTickRateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<ClientServerTickRate>())
            {
                Entity entity =
                        state.EntityManager.CreateEntity();

                ClientServerTickRate tick_rate =
                        new ClientServerTickRate
                        {
                            SimulationTickRate = 90,
                            NetworkTickRate = 90
                        };

                tick_rate.ResolveDefaults();

                state.EntityManager.AddComponentData(
                        entity,
                        tick_rate);
            }

            state.Enabled = false;
        }
    }
}
