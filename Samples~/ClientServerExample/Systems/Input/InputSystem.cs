using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

using RunstarSystems.ECS.Attributes;
using RunstarSystems.ECS.Groups;
using components = RunstarSystems.ECS.Components;

namespace RunstarSystems.ECS.Systems
{
    [InheritFromGroup(typeof(RunstarInputPullGroup))]
    public partial class InputPollSystem : SystemBase
    {
        private const float KEYBOARD_AXIS_VALUES = 0.8f;
        private const float GAMEPAD_DEADZONE = 0.2f;
        private const float QUANTIZED_AXIS_MAX = 32767.0f;

        private uint frame;

        private uint previous_input_0_held_actions;
        private uint previous_input_1_held_actions;

        protected override void OnCreate()
        {
            base.OnCreate();

            frame = 0;

            previous_input_0_held_actions = 0;
            previous_input_1_held_actions = 0;

            Entity input_entity =
                    EntityManager.CreateEntity(
                            typeof(components.DuelStickPolledInput));

            EntityManager.SetComponentData(
                    input_entity,
                    new components.DuelStickPolledInput());
        }

        protected override void OnUpdate()
        {
            frame++;

            components.DuelStickPolledInput input =
                    new components.DuelStickPolledInput
                    {
                        frame = frame,
                        input_count = 0
                    };

            WriteKeyboardInput0(
                    ref input);

            input.input_count++;

            WriteGamepadInput1(
                    0,
                    ref input);

            input.input_count++;

            SystemAPI.SetSingleton(
                    input);
        }

        private void WriteKeyboardInput0(
                ref components.DuelStickPolledInput input)
        {
            float move_x = 0.0f;
            float move_y = 0.0f;
            float target_x = 0.0f;
            float target_y = 0.0f;
            uint held_actions = 0;

            Keyboard keyboard =
                    Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.leftArrowKey.isPressed)
                {
                    move_x -= KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.rightArrowKey.isPressed)
                {
                    move_x += KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.downArrowKey.isPressed)
                {
                    move_y -= KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.upArrowKey.isPressed)
                {
                    move_y += KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.aKey.isPressed)
                {
                    target_x -= KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.dKey.isPressed)
                {
                    target_x += KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.sKey.isPressed)
                {
                    target_y -= KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.wKey.isPressed)
                {
                    target_y += KEYBOARD_AXIS_VALUES;
                }

                if (keyboard.jKey.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.top_spin;
                }

                if (keyboard.kKey.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.back_spin;
                }

                if (keyboard.spaceKey.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.dash;
                }

                if (keyboard.leftShiftKey.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.super;
                }
            }

            uint pressed_actions =
                    held_actions & ~previous_input_0_held_actions;

            uint released_actions =
                    previous_input_0_held_actions & ~held_actions;

            previous_input_0_held_actions =
                    held_actions;

            input.input_0_local_player_index = 0;
            input.input_0_move_x = QuantizeAxis(move_x);
            input.input_0_move_y = QuantizeAxis(move_y);
            input.input_0_target_x = QuantizeAxis(target_x);
            input.input_0_target_y = QuantizeAxis(target_y);
            input.input_0_held_actions = held_actions;
            input.input_0_pressed_actions = pressed_actions;
            input.input_0_released_actions = released_actions;
        }

        private void WriteGamepadInput1(
                int gamepad_index,
                ref components.DuelStickPolledInput input)
        {
            float move_x = 0.0f;
            float move_y = 0.0f;
            float target_x = 0.0f;
            float target_y = 0.0f;
            uint held_actions = 0;

            if (gamepad_index >= 0 &&
                    gamepad_index < Gamepad.all.Count)
            {
                Gamepad gamepad =
                        Gamepad.all[gamepad_index];

                move_x =
                        ApplyDeadzone(gamepad.leftStick.x.ReadValue());

                move_y =
                        ApplyDeadzone(gamepad.leftStick.y.ReadValue());

                target_x =
                        ApplyDeadzone(gamepad.rightStick.x.ReadValue());

                target_y =
                        ApplyDeadzone(gamepad.rightStick.y.ReadValue());

                if (gamepad.buttonWest.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.top_spin;
                }

                if (gamepad.buttonEast.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.back_spin;
                }

                if (gamepad.buttonSouth.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.dash;
                }

                if (gamepad.buttonNorth.isPressed)
                {
                    held_actions |=
                            components.PingPongActionBits.super;
                }
            }

            uint pressed_actions =
                    held_actions & ~previous_input_1_held_actions;

            uint released_actions =
                    previous_input_1_held_actions & ~held_actions;

            previous_input_1_held_actions =
                    held_actions;

            input.input_1_local_player_index = 1;
            input.input_1_move_x = QuantizeAxis(move_x);
            input.input_1_move_y = QuantizeAxis(move_y);
            input.input_1_target_x = QuantizeAxis(target_x);
            input.input_1_target_y = QuantizeAxis(target_y);
            input.input_1_held_actions = held_actions;
            input.input_1_pressed_actions = pressed_actions;
            input.input_1_released_actions = released_actions;
        }

        private static int QuantizeAxis(
                float axis_value)
        {
            float clamped_value =
                    math.clamp(axis_value, -1.0f, 1.0f);

            float scaled_value =
                    math.round(clamped_value * QUANTIZED_AXIS_MAX);

            return (int)scaled_value;
        }

        private static float ApplyDeadzone(
                float axis_value)
        {
            if (axis_value > -GAMEPAD_DEADZONE &&
                    axis_value < GAMEPAD_DEADZONE)
            {
                return 0.0f;
            }

            return axis_value;
        }
    }
}
