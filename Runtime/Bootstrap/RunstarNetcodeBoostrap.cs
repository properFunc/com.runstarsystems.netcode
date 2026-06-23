using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace RunstarSystems.ECS.Admin
{
    public sealed class RunstarNetcodeBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            Debug.Log("Runstar filler bootstrap active.");

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
