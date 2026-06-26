using Unity.NetCode;
using Unity.Entities;

namespace RunstarSystems.ECS.Components
{
    public struct DuelStickClientInputCommand : ICommandData
    {
        public NetworkTick Tick { get; set; }

        public uint frame;
        public int input_count;

        public int input_0_local_player_index;
        public int input_0_move_x;
        public int input_0_move_y;
        public int input_0_target_x;
        public int input_0_target_y;
        public uint input_0_held_actions;
        public uint input_0_pressed_actions;
        public uint input_0_released_actions;

        public int input_1_local_player_index;
        public int input_1_move_x;
        public int input_1_move_y;
        public int input_1_target_x;
        public int input_1_target_y;
        public uint input_1_held_actions;
        public uint input_1_pressed_actions;
        public uint input_1_released_actions;
    }

    public struct DuelStickPolledInput : IComponentData
        {
            public uint frame;

            public int input_count;

            public int input_0_local_player_index;
            public int input_0_move_x;
            public int input_0_move_y;
            public int input_0_target_x;
            public int input_0_target_y;
            public uint input_0_held_actions;
            public uint input_0_pressed_actions;
            public uint input_0_released_actions;

            public int input_1_local_player_index;
            public int input_1_move_x;
            public int input_1_move_y;
            public int input_1_target_x;
            public int input_1_target_y;
            public uint input_1_held_actions;
            public uint input_1_pressed_actions;
            public uint input_1_released_actions;
        }
}
