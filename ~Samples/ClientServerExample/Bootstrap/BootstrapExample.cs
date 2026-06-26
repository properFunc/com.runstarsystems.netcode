using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

using metadata = RunstarSystems.ECS.Data;

namespace RunstarSystems.ECS.Admin
{
    public static class RunstarClientServerRuntimeBootstrap
    {
        private const ushort LOCAL_HOST_PORT = 7979;

        private static bool has_initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeClientServerWorlds()
        {
            if (has_initialized)
            {
                return;
            }

            has_initialized = true;

            Application.runInBackground = true;

            Debug.Log("Runstar client/server runtime bootstrap started.");

            KillFillerWorld();

            RunstarWorldManager world_manager =
                    new RunstarWorldManager();

            world_manager.Init();

            metadata.RunstarOrganizerContext server_context =
                    world_manager.CreateServerContext(
                            "Runstar Server World");

            if (server_context == null)
            {
                Debug.LogError(
                        "Runstar runtime bootstrap failed. " +
                        "Could not create Runstar Server World.");

                return;
            }

            metadata.RunstarOrganizerContext client_context =
                    world_manager.CreateClientContext(
                            "Runstar Client World");

            if (client_context == null)
            {
                Debug.LogError(
                        "Runstar runtime bootstrap failed. " +
                        "Could not create Runstar Client World.");

                return;
            }

            world_manager.RunServerWorld(
                    server_context);

            world_manager.RunClientWorld(
                    client_context);

            StartLocalHostConnection(
                    server_context.World,
                    client_context.World,
                    LOCAL_HOST_PORT);

            World.DefaultGameObjectInjectionWorld =
                    client_context.World;

            world_manager.Complete();

            Debug.Log(
                    "Runstar Server World and Client World are running on port " +
                    LOCAL_HOST_PORT +
                    ".");
        }

        private static void StartLocalHostConnection(
                World server_world,
                World client_world,
                ushort port)
        {
            if (server_world == null || !server_world.IsCreated)
            {
                Debug.LogError(
                        "Runstar connection failed. Server world is missing.");

                return;
            }

            if (client_world == null || !client_world.IsCreated)
            {
                Debug.LogError(
                        "Runstar connection failed. Client world is missing.");

                return;
            }

            EntityManager server_entity_manager =
                    server_world.EntityManager;

            EntityManager client_entity_manager =
                    client_world.EntityManager;


            NetworkEndpoint listen_endpoint =
                    NetworkEndpoint.AnyIpv4.WithPort(
                            port);

            NetworkEndpoint connect_endpoint =
                    NetworkEndpoint.LoopbackIpv4.WithPort(
                            port);

            Entity listen_request =
                    server_entity_manager.CreateEntity(
                            typeof(NetworkStreamRequestListen));

            server_entity_manager.SetComponentData(
                    listen_request,
                    new NetworkStreamRequestListen
                    {
                        Endpoint = listen_endpoint
                    });

            Entity connect_request =
                    client_entity_manager.CreateEntity(
                            typeof(NetworkStreamRequestConnect));

            client_entity_manager.SetComponentData(
                    connect_request,
                    new NetworkStreamRequestConnect
                    {
                        Endpoint = connect_endpoint
                    });

            Debug.Log(
                    "Runstar server listen request created: " +
                    listen_endpoint);

            Debug.Log(
                    "Runstar client connect request created: " +
                    connect_endpoint);
        }

        private static void KillFillerWorld()
        {
            World filler_world =
                    World.DefaultGameObjectInjectionWorld;

            if (filler_world == null)
            {
                return;
            }

            Debug.Log(
                    "Killing Runstar filler world: " +
                    filler_world.Name);

            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(
                    filler_world);

            filler_world.Dispose();

            World.DefaultGameObjectInjectionWorld = null;
        }
    }
}
