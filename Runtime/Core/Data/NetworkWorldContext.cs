using registry = RunstarSystems.ECS.Registry;

namespace RunstarSystems.ECS.Data
{
    public sealed class NetworkWorldContext
    {
        public registry.TypeRegistry Local { get; }
        public registry.TypeRegistry Client { get; }
        public registry.TypeRegistry ThinClient { get; }
        public registry.TypeRegistry Server { get; }

        public NetworkWorldContext(
                registry.TypeRegistry local,
                registry.TypeRegistry client,
                registry.TypeRegistry thin_client,
                registry.TypeRegistry server)
        {
            Local = local;
            Client = client;
            ThinClient = thin_client;
            Server = server;
        }
    }
}
