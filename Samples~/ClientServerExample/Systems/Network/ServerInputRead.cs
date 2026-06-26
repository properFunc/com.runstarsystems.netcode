using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;

namespace RunstarSystems.ECS.Systems
{
    [InheritFromGroup(typeof(RunstarServerNetworkReceiveGroup))]
    public partial class ServerInputCommandReadSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<NetworkTime>();
            RequireForUpdate<NetworkStreamInGame>();
            RequireForUpdate<components.DuelStickClientInputCommand>();
        }

        protected override void OnUpdate()
        {
            NetworkTime network_time =
                    SystemAPI.GetSingleton<NetworkTime>();

            NetworkTick tick =
                    network_time.ServerTick;

            foreach ((DynamicBuffer<components.DuelStickClientInputCommand> input_buffer,
                      Entity command_entity)
                     in SystemAPI.Query
                            <DynamicBuffer
                                    <components.DuelStickClientInputCommand>>()
                            .WithEntityAccess())
            {
                if (input_buffer.Length == 0)
                {
                    continue;
                }

                if (!CommandDataUtility.GetDataAtTick(
                            input_buffer,
                            tick,
                            out components.DuelStickClientInputCommand command))
                {
                    Debug.Log(
                            "SERVER command buffer exists on " +
                            command_entity +
                            " length " +
                            input_buffer.Length +
                            " but no command for tick " +
                            tick);

                    continue;
                }
            }
        }
    }
}
