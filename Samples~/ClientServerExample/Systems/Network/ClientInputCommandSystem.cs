using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;

namespace RunstarSystems.ECS.Systems
{
    /*
    *   Takes in the polling from the singelton
    *   And delivers it through the network to the server
    */
    [InheritFromGroup(typeof(RunstarClientNetworkSendGroup))]
    public partial class ClientInputCommandSystem : SystemBase
    {
        // Sending data requires and entity
        private Entity command_entity;

        protected override void OnCreate()
        {
            base.OnCreate();

            command_entity =
                    Entity.Null;

            RequireForUpdate<NetworkTime>();
            RequireForUpdate<CommandTarget>();
            RequireForUpdate<NetworkStreamInGame>();
            RequireForUpdate<components.DuelStickPolledInput>();
        }

        protected override void OnUpdate()
        {
            EnsureCommandTarget();

            if (command_entity == Entity.Null)
            {
                return;
            }

            NetworkTime network_time =
                    SystemAPI.GetSingleton<NetworkTime>();

            components.DuelStickPolledInput polled_input =
                    SystemAPI.GetSingleton<components.DuelStickPolledInput>();

            components.DuelStickClientInputCommand command =
                    new components.DuelStickClientInputCommand
                    {
                        Tick =
                                network_time.ServerTick,

                        frame =
                                polled_input.frame,

                        input_count =
                                polled_input.input_count,

                        input_0_local_player_index =
                                polled_input.input_0_local_player_index,

                        input_0_move_x =
                                polled_input.input_0_move_x,

                        input_0_move_y =
                                polled_input.input_0_move_y,

                        input_0_target_x =
                                polled_input.input_0_target_x,

                        input_0_target_y =
                                polled_input.input_0_target_y,

                        input_0_held_actions =
                                polled_input.input_0_held_actions,

                        input_0_pressed_actions =
                                polled_input.input_0_pressed_actions,

                        input_0_released_actions =
                                polled_input.input_0_released_actions,

                        input_1_local_player_index =
                                polled_input.input_1_local_player_index,

                        input_1_move_x =
                                polled_input.input_1_move_x,

                        input_1_move_y =
                                polled_input.input_1_move_y,

                        input_1_target_x =
                                polled_input.input_1_target_x,

                        input_1_target_y =
                                polled_input.input_1_target_y,

                        input_1_held_actions =
                                polled_input.input_1_held_actions,

                        input_1_pressed_actions =
                                polled_input.input_1_pressed_actions,

                        input_1_released_actions =
                                polled_input.input_1_released_actions
                    };

            DynamicBuffer<components.DuelStickClientInputCommand> command_buffer =
                    EntityManager.GetBuffer
                            <components.DuelStickClientInputCommand>(
                                    command_entity);

            CommandDataUtility.AddCommandData(
                    command_buffer,
                    command);
        }

        private void EnsureCommandTarget()
        {
            CommandTarget command_target =
                    SystemAPI.GetSingleton<CommandTarget>();

            if (command_entity != Entity.Null &&
                    EntityManager.Exists(command_entity) &&
                    command_target.targetEntity == command_entity)
            {
                return;
            }

            if (command_target.targetEntity != Entity.Null &&
                    EntityManager.Exists(command_target.targetEntity) &&
                    EntityManager.HasBuffer
                            <components.DuelStickClientInputCommand>(
                                    command_target.targetEntity))
            {
                command_entity =
                        command_target.targetEntity;

                return;
            }

            command_entity =
                    EntityManager.CreateEntity();

            EntityManager.AddBuffer
                    <components.DuelStickClientInputCommand>(
                            command_entity);

            command_target.targetEntity =
                    command_entity;

            SystemAPI.SetSingleton(
                    command_target);

            Debug.Log(
                    "CLIENT command target set to entity " +
                    command_entity);
        }
    }
}
