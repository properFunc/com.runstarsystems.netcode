using System;
using System.Collections.Generic;

using metadata = RunstarSystems.ECS.Data;
using registry = RunstarSystems.ECS.Registry;
using RunstarSystems.ECS.Attributes;

namespace RunstarSystems.ECS.Admin
{
    public static class NetcodeBootStrapPipeline
    {
        public static metadata.NetworkWorldContext BuildWorldRegistries(
                registry.TypeRegistry global_type_registry)
        {
            if (global_type_registry == null)
            {
                return new metadata.NetworkWorldContext(
                        null,
                        null,
                        null,
                        null);
            }

            HashSet<Type> local_types =
                    GetFilteredTypes<LocalFilterTraitAttribute>(
                            global_type_registry);

            HashSet<Type> client_types =
                    GetFilteredTypes<ClientFilterTraitAttribute>(
                            global_type_registry);

            HashSet<Type> thin_client_types =
                    GetFilteredTypes<ThinClientFilterTraitAttribute>(
                            global_type_registry);

            HashSet<Type> server_types =
                    GetFilteredTypes<ServerFilterTraitAttribute>(
                            global_type_registry);

            HashSet<Type> network_filtered_types =
                    new HashSet<Type>();

            AddRange(network_filtered_types, client_types);
            AddRange(network_filtered_types, thin_client_types);
            AddRange(network_filtered_types, server_types);

            AddUnfilteredLocalTypes(
                    local_types,
                    network_filtered_types,
                    global_type_registry);

            registry.TypeRegistry local_registry =
                    CopyRegistryForTypes(
                            global_type_registry,
                            local_types);

            registry.TypeRegistry client_registry =
                    CopyRegistryForTypes(
                            global_type_registry,
                            client_types);

            registry.TypeRegistry thin_client_registry =
                    CopyRegistryForTypes(
                            global_type_registry,
                            thin_client_types);

            registry.TypeRegistry server_registry =
                    CopyRegistryForTypes(
                            global_type_registry,
                            server_types);

            Debug.Log("=== SERVER INHERIT MATCHES AFTER COPY ===");

            IReadOnlyList<metadata.RegistryMetadata<InheritFromGroupAttribute>> inherit_matches =
                    server_registry.GetMatches
                            <InheritFromGroupAttribute,
                                InheritFromGroupAttribute>();

            for (int index = 0; index < inherit_matches.Count; index++)
            {
                metadata.RegistryMetadata<InheritFromGroupAttribute> match =
                        inherit_matches[index];

                Debug.Log(
                        "Server inherit match: child=" +
                        match.MatchedType.FullName +
                        " parent=" +
                        match.Metadata.GroupType.FullName +
                        " inherited=" +
                        match.IsInherited);
            }

            return new metadata.NetworkWorldContext(
                    local_registry,
                    client_registry,
                    thin_client_registry,
                    server_registry);
        }

        private static void AddUnfilteredLocalTypes(
                HashSet<Type> local_types,
                HashSet<Type> network_filtered_types,
                registry.TypeRegistry global_type_registry)
        {
            List<Type> all_types =
                    global_type_registry.GetAllUniqueTypes();

            for (int type_index = 0;
                    type_index < all_types.Count;
                    type_index++)
            {
                Type type = all_types[type_index];

                if (type == null)
                {
                    continue;
                }

                if (network_filtered_types.Contains(type))
                {
                    continue;
                }

                local_types.Add(type);
            }
        }

        private static registry.TypeRegistry CopyRegistryForTypes(
                registry.TypeRegistry source_registry,
                HashSet<Type> allowed_types)
        {
            registry.TypeRegistry target_registry =
                    new registry.TypeRegistry();

            IReadOnlyList<metadata.RegistryMetadata> matches =
                    source_registry.GetAllMatches();

            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata match =
                        matches[match_index];

                Type matched_type = match.MatchedType;

                if (matched_type == null)
                {
                    continue;
                }

                if (!allowed_types.Contains(matched_type))
                {
                    continue;
                }

                target_registry.AddMatch(
                        match.KeyType,
                        match.MatchedType,
                        match.SourceType,
                        match.Metadata,
                        match.IsInherited);
            }

            return target_registry;
        }

        private static HashSet<Type> GetFilteredTypes<TAttribute>(
                registry.TypeRegistry global_type_registry)
                where TAttribute : Attribute
        {
            HashSet<Type> filtered_types =
                    new HashSet<Type>();

            IReadOnlyList<metadata.RegistryMetadata> matches =
                    global_type_registry.GetMatches<TAttribute>();

            for (int match_index = 0;
                    match_index < matches.Count;
                    match_index++)
            {
                metadata.RegistryMetadata match =
                        matches[match_index];

                Type matched_type = match.MatchedType;

                if (matched_type == null)
                {
                    continue;
                }

                filtered_types.Add(matched_type);
            }

            return filtered_types;
        }

        private static void AddRange(
                HashSet<Type> target,
                HashSet<Type> source)
        {
            foreach (Type type in source)
            {
                if (type == null)
                {
                    continue;
                }

                target.Add(type);
            }
        }
    }
}
