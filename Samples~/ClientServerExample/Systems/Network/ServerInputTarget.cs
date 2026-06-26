using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;

namespace RunstarSystems.ECS.Systems
{
    [InheritFromGroup(typeof(RunstarServerNetworkReceiveGroup))]
    public partial class ServerInputCommandTargetSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<NetworkStreamInGame>();
            RequireForUpdate<CommandTarget>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer entity_command_buffer =
                    new EntityCommandBuffer(
                            Allocator.Temp);

            foreach ((RefRO<CommandTarget> command_target,
                      RefRO<NetworkId> network_id,
                      Entity connection_entity)
                     in SystemAPI.Query
                            <RefRO<CommandTarget>,
                             RefRO<NetworkId>>()
                            .WithAll<NetworkStreamInGame>()
                            .WithEntityAccess())
            {
                if (command_target.ValueRO.targetEntity != Entity.Null &&
                        EntityManager.Exists(
                                command_target.ValueRO.targetEntity) &&
                        EntityManager.HasBuffer
                                <components.DuelStickClientInputCommand>(
                                        command_target.ValueRO.targetEntity))
                {
                    continue;
                }

                Entity command_entity =
                        entity_command_buffer.CreateEntity();

                entity_command_buffer.AddBuffer
                        <components.DuelStickClientInputCommand>(
                                command_entity);

                CommandTarget updated_command_target =
                        command_target.ValueRO;

                updated_command_target.targetEntity =
                        command_entity;

                entity_command_buffer.SetComponent(
                        connection_entity,
                        updated_command_target);

                Debug.Log(
                        "SERVER command target queued. Connection " +
                        connection_entity +
                        " network id " +
                        network_id.ValueRO.Value);
            }

            entity_command_buffer.Playback(
                    EntityManager);

            entity_command_buffer.Dispose();
        }
    }
}
