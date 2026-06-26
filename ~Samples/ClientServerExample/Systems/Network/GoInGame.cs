using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Groups;

namespace RunstarSystems.ECS.Systems
{
    /*
    *   Temporary localhost bootstrap helper.
    *
    *   Marks fully connected Netcode connection entities as "in game".
    */
    [InheritFromGroup(typeof(RunstarNetworkConnectionGroup))]
    public partial class RunstarGoInGameSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            /*
            *   NetworkStreamConnection alone is too early.
            *   Wait until Netcode has assigned NetworkId too.
            */
            EntityQuery connection_query =
                    GetEntityQuery(
                            ComponentType.ReadOnly<NetworkStreamConnection>(),
                            ComponentType.ReadOnly<NetworkId>(),
                            ComponentType.Exclude<NetworkStreamInGame>());

            RequireForUpdate(
                    connection_query);
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer entity_command_buffer =
                    new EntityCommandBuffer(
                            Allocator.Temp);

            int added_count =
                    0;

            foreach ((RefRO<NetworkStreamConnection> connection,
                      RefRO<NetworkId> network_id,
                      Entity entity)
                     in SystemAPI.Query
                            <RefRO<NetworkStreamConnection>,
                             RefRO<NetworkId>>()
                            .WithNone<NetworkStreamInGame>()
                            .WithEntityAccess())
            {
                entity_command_buffer.AddComponent<NetworkStreamInGame>(
                        entity);

                added_count++;

                Debug.Log(
                        "Runstar marked connection in game in world " +
                        World.Name +
                        " entity " +
                        entity +
                        " network id " +
                        network_id.ValueRO.Value);
            }

            entity_command_buffer.Playback(
                    EntityManager);

            entity_command_buffer.Dispose();

            /*
            *   Fine for your current one-client localhost test.
            *   Later, server should stay enabled if more clients can join.
            */
            if (added_count > 0)
            {
                Enabled =
                        false;
            }
        }
    }
}
