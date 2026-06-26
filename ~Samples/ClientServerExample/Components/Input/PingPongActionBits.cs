using Unity.Entities;

namespace RunstarSystems.ECS.Components
{
    public static class InputSourceTypes
    {
        public const int keyboard = 0;
        public const int gamepad = 1;
    }

    public static class PingPongActionBits
    {
        public const ushort top_spin = 1 << 0;
        public const ushort back_spin = 1 << 1;
        public const ushort dash = 1 << 2;
        public const ushort super = 1 << 3;
    }
}
