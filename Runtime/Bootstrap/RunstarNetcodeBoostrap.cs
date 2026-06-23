using Unity.NetCode;

namespace RunstarSystems.ECS.Admin
{
    public sealed class RunstarNetcodeBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            return true;
        }
    }
}
