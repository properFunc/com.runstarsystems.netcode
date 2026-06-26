using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;

/*
*   Similar to the offline input print system
*   However this one makes sure to sync up with the network time
*
*   Allowing a client to send data over before printing
*/
namespace RunstarSystems.ECS.Systems
{
    [InheritFromGroup(typeof(RunstarServerInputPrintGroup))]
    public partial class InputPrintSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<NetworkTime>();
        }

        protected override void OnUpdate()
        {
            /*
            *   Takes in the clients inputs sent to them
            *   via the buffer
            */
            foreach (DynamicBuffer<components.DuelStickClientInputCommand>
                    input_buffer
                    in SystemAPI.Query
                            <DynamicBuffer
                                    <components.DuelStickClientInputCommand>>())
            {
                if (input_buffer.Length == 0)
                {
                    continue;
                }

                components.DuelStickClientInputCommand client_input =
                        input_buffer[input_buffer.Length - 1];

                PrintPlayerInput0(client_input);

                if (client_input.input_count > 1)
                {
                    PrintPlayerInput1(client_input);
                }
            }
        }

        private static void PrintPlayerInput0(
                components.DuelStickClientInputCommand client_input)
        {
            Debug.Log("Frame: " + client_input.frame);
            Debug.Log(
                    "Server Player "
                    + client_input.input_0_local_player_index
                    + " Move "
                    + client_input.input_0_move_x
                    + ", "
                    + client_input.input_0_move_y
                    + " Target "
                    + client_input.input_0_target_x
                    + ", "
                    + client_input.input_0_target_y
                    + " Held "
                    + client_input.input_0_held_actions
                    + " Pressed "
                    + client_input.input_0_pressed_actions
                    + " Released "
                    + client_input.input_0_released_actions);
        }

        private static void PrintPlayerInput1(
                components.DuelStickClientInputCommand client_input)
        {
            Debug.Log(
                    "Server Player "
                    + client_input.input_1_local_player_index
                    + " Move "
                    + client_input.input_1_move_x
                    + ", "
                    + client_input.input_1_move_y
                    + " Target "
                    + client_input.input_1_target_x
                    + ", "
                    + client_input.input_1_target_y
                    + " Held "
                    + client_input.input_1_held_actions
                    + " Pressed "
                    + client_input.input_1_pressed_actions
                    + " Released "
                    + client_input.input_1_released_actions);
        }
    }
}
