using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

using metadata = RunstarSystems.ECS.Data;
using registry = RunstarSystems.ECS.Registry;

namespace RunstarSystems.ECS.Admin
{
    public sealed class RunstarWorldManager
    {
        /*
        *   Holds all registry types for each network world
        */
        public metadata.NetworkWorldContext NetworkWorldContext { get; private set; }

        public IReadOnlyList<IRunstarOrganizer> Organizers { get; private set; }

        public IReadOnlyList<Type> AssemblyTypes { get; private set; }


        /*
        *   These are global registries that are meant to build the world
        *   I dont have them seperated like I probably should be for now
        *   that is okay
        */
        public IReadOnlyList<Type> LocalSystemTypes { get; private set; }

        public IReadOnlyList<Type> ClientSystemTypes { get; private set; }

        public IReadOnlyList<Type> ThinClientSystemTypes { get; private set; }

        public IReadOnlyList<Type> ServerSystemTypes { get; private set; }

        public const int PREFILTER_PRIORITY = 100; // Used for organizers before client/server filter

        public RunstarWorldManager()
        {
        }

        /*
        *   Creates the systems, organizer and types list
        *   Then uses that to build the global type registry and cache
        *   Allows organizers that want to target all types to do so here
        *
        *   Examples include inheritance and type registries
        */
        public metadata.RunstarOrganizerContext Init()
        {
            // Since Runstar cannot use Unity attributes
            // All of its data will be here
            List<Type> local_system_types =
                    DefaultWorldInitialization
                            .GetAllSystems(WorldSystemFilterFlags.Default)
                            .ToList();

            List<Type> client_system_types =
                    DefaultWorldInitialization
                            .GetAllSystems(
                                    WorldSystemFilterFlags.ClientSimulation |
                                    WorldSystemFilterFlags.Presentation)
                            .ToList();

            List<Type> thin_client_system_types =
                    DefaultWorldInitialization
                            .GetAllSystems(
                                    WorldSystemFilterFlags.ThinClientSimulation)
                            .ToList();

            List<Type> server_system_types =
                    DefaultWorldInitialization
                            .GetAllSystems(
                                    WorldSystemFilterFlags.ServerSimulation)
                            .ToList();

            // This grabs all the type required for Runstar including metadata
            List<Type> assembly_types =
                    AssemblyScanner.GetAllAssemblyTypes();

            registry.TypeRegistry global_type_registry =
                    new registry.TypeRegistry();

            registry.InheritCache inherit_cache =
                    new registry.InheritCache();


            List<IRunstarOrganizer> organizers =
                    RunstarBootStrapPipeline.CreateOrganizers(
                            assembly_types);

            // We dont need a world with global
            // Ass global gets removed after the other registries are built
            metadata.RunstarOrganizerContext context =
                    RunstarBootStrapPipeline.RegisterAttributes(
                            organizers,
                            null,
                            global_type_registry,
                            inherit_cache,
                            local_system_types);


            RunstarBootStrapPipeline.RunOrganizers(
                    organizers,
                    context,
                    int.MinValue,
                    PREFILTER_PRIORITY);

            metadata.NetworkWorldContext network_world_context =
                    NetcodeBootStrapPipeline.BuildWorldRegistries(
                            context.TypeRegistry);

            /*
            *   This deletes all registry types used by runstar
            *   from the type list in each default world
            *
            *
            *   This allows use to seperate Runstar Attributes
            *   from Unity attributes automatically
            */
            RunstarBootStrapPipeline.RemoveRegistryTypes(
                    local_system_types,
                    context.TypeRegistry);

            RunstarBootStrapPipeline.RemoveRegistryTypes(
                    client_system_types,
                    context.TypeRegistry);

            RunstarBootStrapPipeline.RemoveRegistryTypes(
                    thin_client_system_types,
                    context.TypeRegistry);

            RunstarBootStrapPipeline.RemoveRegistryTypes(
                    server_system_types,
                    context.TypeRegistry);

            LocalSystemTypes = local_system_types;
            ClientSystemTypes = client_system_types;
            ThinClientSystemTypes = thin_client_system_types;
            ServerSystemTypes = server_system_types;

            AssemblyTypes = assembly_types;
            NetworkWorldContext = network_world_context;
            Organizers = organizers;

            return context;
        }

        /*
        *   These are meant to create the worlds and build
        *   the context around the worlds
        *
        *   I reuse the same context struct so inherit is just null
        *   since it is only required for global
        */
        public metadata.RunstarOrganizerContext CreateLocalContext(
                string default_world_name = "Runstar Local World",
                registry.TypeRegistry type_registry = null)
        {
            return CreateWorldContext(
                    default_world_name,
                    type_registry,
                    context => context.Local,
                    WorldFlags.Game,
                    world => World.DefaultGameObjectInjectionWorld = world,
                    "Runstar Local");
        }

        public metadata.RunstarOrganizerContext CreateClientContext(
                string world_name = "Runstar Client World",
                registry.TypeRegistry type_registry = null)
        {
            return CreateWorldContext(
                    world_name,
                    type_registry,
                    context => context.Client,
                    WorldFlags.GameClient,
                    world => ClientServerBootstrap.ClientWorlds.Add(world),
                    "Runstar Client");
        }

        public metadata.RunstarOrganizerContext CreateThinClientContext(
                string world_name = "Runstar Thin Client World",
                registry.TypeRegistry type_registry = null)
        {
            return CreateWorldContext(
                    world_name,
                    type_registry,
                    context => context.ThinClient,
                    WorldFlags.GameThinClient,
                    world => ClientServerBootstrap.ThinClientWorlds.Add(world),
                    "Runstar Thin Client");
        }

        public metadata.RunstarOrganizerContext CreateServerContext(
                string world_name = "Runstar Server World",
                registry.TypeRegistry type_registry = null)
        {
            return CreateWorldContext(
                    world_name,
                    type_registry,
                    context => context.Server,
                    WorldFlags.GameServer,
                    world => ClientServerBootstrap.ServerWorlds.Add(world),
                    "Runstar Server");
        }

        private metadata.RunstarOrganizerContext CreateWorldContext(
                string world_name,
                registry.TypeRegistry type_registry,
                Func<metadata.NetworkWorldContext, registry.TypeRegistry> fallback_registry_selector,
                WorldFlags world_flags,
                Action<World> after_world_created = null,
                string world_label = "world")
        {
            registry.TypeRegistry selected_registry =
                    type_registry;

            if (selected_registry == null &&
                    NetworkWorldContext != null &&
                    fallback_registry_selector != null)
            {
                selected_registry =
                        fallback_registry_selector(NetworkWorldContext);
            }

            if (selected_registry == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not create " + world_label + " context. " +
                        "No TypeRegistry was provided and no " + world_label + " registry exists.");

                return null;
            }

            World world =
                    new World(
                            world_name,
                            world_flags);

            after_world_created?.Invoke(world);

            return new metadata.RunstarOrganizerContext(
                    world,
                    selected_registry,
                    null);
        }

        /*
        *   Little delegation wrappers
        *   to run the organizers for each world
        */
        public void RunLocalWorld(
                metadata.RunstarOrganizerContext context,
                IReadOnlyList<IRunstarOrganizer> organizers = null,
                IReadOnlyList<Type> system_types = null)
        {
            RunWorld(
                    context,
                    organizers,
                    system_types,
                    LocalSystemTypes);
        }

        public void RunClientWorld(
                metadata.RunstarOrganizerContext context,
                IReadOnlyList<IRunstarOrganizer> organizers = null,
                IReadOnlyList<Type> system_types = null)
        {
            RunWorld(
                    context,
                    organizers,
                    system_types,
                    ClientSystemTypes);
        }

        public void RunThinClientWorld(
                metadata.RunstarOrganizerContext context,
                IReadOnlyList<IRunstarOrganizer> organizers = null,
                IReadOnlyList<Type> system_types = null)
        {
            RunWorld(
                    context,
                    organizers,
                    system_types,
                    ThinClientSystemTypes);
        }

        public void RunServerWorld(
                metadata.RunstarOrganizerContext context,
                IReadOnlyList<IRunstarOrganizer> organizers = null,
                IReadOnlyList<Type> system_types = null)
        {
            RunWorld(
                    context,
                    organizers,
                    system_types,
                    ServerSystemTypes);
        }

        /*
        *   Builds the world given wrapper function call
        *
        */
        private void RunWorld(
                metadata.RunstarOrganizerContext context,
                IReadOnlyList<IRunstarOrganizer> organizers,
                IReadOnlyList<Type> system_types,
                IReadOnlyList<Type> default_system_types)
        {
            if (context == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not run world. Context was null.");

                return;
            }

            if (context.World == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not run world. Context world was null.");

                return;
            }

            IReadOnlyList<IRunstarOrganizer> selected_organizers =
                    organizers;

            if (selected_organizers == null ||
                    selected_organizers.Count == 0)
            {
                selected_organizers =
                        Organizers;
            }

            if (selected_organizers == null)
            {
                selected_organizers =
                        Array.Empty<IRunstarOrganizer>();
            }

            IReadOnlyList<Type> selected_system_types =
                    system_types;

            if (selected_system_types == null ||
                    selected_system_types.Count == 0)
            {
                selected_system_types =
                        default_system_types;
            }

            if (selected_system_types == null)
            {
                selected_system_types =
                        Array.Empty<Type>();
            }

            RunstarBootStrapPipeline.RunOrganizers(
                    selected_organizers,
                    context,
                    PREFILTER_PRIORITY,
                    RunstarBootStrapPipeline.UNITY_DEFAULT_INSERT_PRIORITY);

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(
                    context.World,
                    selected_system_types);

            RunstarBootStrapPipeline.RunOrganizers(
                    selected_organizers,
                    context,
                    RunstarBootStrapPipeline.UNITY_DEFAULT_INSERT_PRIORITY,
                    int.MaxValue);

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(
                    context.World);
        }

        public void Complete()
        {
            NetworkWorldContext = null;
            Organizers = null;
            AssemblyTypes = null;

            LocalSystemTypes = null;
            ClientSystemTypes = null;
            ThinClientSystemTypes = null;
            ServerSystemTypes = null;
        }
    }
}
