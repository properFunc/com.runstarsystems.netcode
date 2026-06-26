using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace RunstarSystems.ECS.Admin
{
    public sealed class RunstarNetcodeBootstrap : ClientServerBootstrap
    {
        /*
        *   Runs an empty world allowing the manager to
        *   create the worlds anytime during runtime.
        */
        public override bool Initialize(string defaultWorldName)
        {

            World filler_world =
                    new World(defaultWorldName);

            World.DefaultGameObjectInjectionWorld =
                    filler_world;

            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(
                    filler_world);

            return true;
        }
    }
}
