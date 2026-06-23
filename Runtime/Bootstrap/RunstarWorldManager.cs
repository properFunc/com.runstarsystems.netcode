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
        public metadata.NetworkWorldContext NetworkWorldContext { get; private set; }

        public IReadOnlyList<IRunstarOrganizer> Organizers { get; private set; }

        public IReadOnlyList<Type> AssemblyTypes { get; private set; }

        public IReadOnlyList<Type> LocalSystemTypes { get; private set; }

        public IReadOnlyList<Type> ClientSystemTypes { get; private set; }

        public IReadOnlyList<Type> ThinClientSystemTypes { get; private set; }

        public IReadOnlyList<Type> ServerSystemTypes { get; private set; }

        public const int PREFILTER_PRIORITY = 100;

        public RunstarWorldManager()
        {
        }

        public metadata.RunstarOrganizerContext Init()
        {
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

            List<Type> assembly_types =
                    AssemblyScanner.GetAllAssemblyTypes();

            registry.TypeRegistry global_type_registry =
                    new registry.TypeRegistry();

            registry.InheritCache inherit_cache =
                    new registry.InheritCache();

            /*
            *   Organizers are discovered from all assembly types.
            *   They are tools that operate on the registry, so they should
            *   not be limited to the default system list.
            */
            List<IRunstarOrganizer> organizers =
                    RunstarBootStrapPipeline.CreateOrganizers(
                            assembly_types);

            /*
            *   Runstar only registers metadata from default-world candidates.
            *
            *   This means explicit Netcode/Entities world-filtered systems
            *   remain owned by Unity/Netcode and are not pulled into the
            *   Runstar registry.
            *
            *   Priority:
            *       Netcode for Entities > Runstar > Entities
            */
            metadata.RunstarOrganizerContext context =
                    RunstarBootStrapPipeline.RegisterAttributes(
                            organizers,
                            null,
                            global_type_registry,
                            inherit_cache,
                            local_system_types);

            /*
            *   Run early organizers against the default-only global registry.
            *   This is where inheritance/filter traits are resolved before
            *   the network registry split happens.
            */
            RunstarBootStrapPipeline.RunOrganizers(
                    organizers,
                    context,
                    int.MinValue,
                    PREFILTER_PRIORITY);

            metadata.NetworkWorldContext network_world_context =
                    NetcodeBootStrapPipeline.BuildWorldRegistries(
                            context.TypeRegistry);

            /*
            *   Remove only Runstar-owned default candidates.
            *
            *   Since the registry was built from local_system_types only,
            *   this should not remove explicit ClientSimulation,
            *   ServerSimulation, or ThinClientSimulation systems.
            */
            RunstarBootStrapPipeline.RemoveRegistryTypes(
                    local_system_types,
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

        public metadata.RunstarOrganizerContext CreateLocalContext(
                string default_world_name = "Runstar Local World",
                registry.TypeRegistry type_registry = null)
        {
            registry.TypeRegistry selected_registry =
                    type_registry;

            if (selected_registry == null &&
                    NetworkWorldContext != null)
            {
                selected_registry =
                        NetworkWorldContext.Local;
            }

            if (selected_registry == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not create local context. " +
                        "No TypeRegistry was provided and no local registry exists.");

                return null;
            }

            World world =
                    new World(default_world_name);

            World.DefaultGameObjectInjectionWorld =
                    world;

            return new metadata.RunstarOrganizerContext(
                    world,
                    selected_registry,
                    null);
        }

        public metadata.RunstarOrganizerContext CreateClientContext(
                string world_name = "Runstar Client World",
                registry.TypeRegistry type_registry = null)
        {
            registry.TypeRegistry selected_registry =
                    type_registry;

            if (selected_registry == null &&
                    NetworkWorldContext != null)
            {
                selected_registry =
                        NetworkWorldContext.Client;
            }

            if (selected_registry == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not create client context. " +
                        "No TypeRegistry was provided and no client registry exists.");

                return null;
            }

            World world =
                    new World(
                            world_name,
                            WorldFlags.GameClient);

            ClientServerBootstrap.ClientWorlds.Add(world);

            return new metadata.RunstarOrganizerContext(
                    world,
                    selected_registry,
                    null);
        }

        public metadata.RunstarOrganizerContext CreateThinClientContext(
                string world_name = "Runstar Thin Client World",
                registry.TypeRegistry type_registry = null)
        {
            registry.TypeRegistry selected_registry =
                    type_registry;

            if (selected_registry == null &&
                    NetworkWorldContext != null)
            {
                selected_registry =
                        NetworkWorldContext.ThinClient;
            }

            if (selected_registry == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not create thin client context. " +
                        "No TypeRegistry was provided and no thin client registry exists.");

                return null;
            }

            World world =
                    new World(
                            world_name,
                            WorldFlags.GameThinClient);

            ClientServerBootstrap.ThinClientWorlds.Add(world);

            return new metadata.RunstarOrganizerContext(
                    world,
                    selected_registry,
                    null);
        }

        public metadata.RunstarOrganizerContext CreateServerContext(
                string world_name = "Runstar Server World",
                registry.TypeRegistry type_registry = null)
        {
            registry.TypeRegistry selected_registry =
                    type_registry;

            if (selected_registry == null &&
                    NetworkWorldContext != null)
            {
                selected_registry =
                        NetworkWorldContext.Server;
            }

            if (selected_registry == null)
            {
                Debug.LogWarning(
                        "RunstarWorldManager could not create server context. " +
                        "No TypeRegistry was provided and no server registry exists.");

                return null;
            }

            World world =
                    new World(
                            world_name,
                            WorldFlags.GameServer);

            ClientServerBootstrap.ServerWorlds.Add(world);

            return new metadata.RunstarOrganizerContext(
                    world,
                    selected_registry,
                    null);
        }

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
