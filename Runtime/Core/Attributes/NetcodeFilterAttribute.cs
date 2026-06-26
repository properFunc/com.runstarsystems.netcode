using System;
using RunstarSystems.ECS.Data;

/*
*   Allows systems to filter into different worlds
*   these traits can be passed down to their children
*   via InheritMode and other attribute [InheritFromGroup()]
*/
namespace RunstarSystems.ECS.Attributes
{
    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class LocalFilterTraitAttribute
            : RunstarInheritableAttribute
    {
        public LocalFilterTraitAttribute(
                InheritMode inherit_mode =
                        InheritMode.DirectOnly)
                : base(inherit_mode)
        {
        }
    }

    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ClientFilterTraitAttribute
            : RunstarInheritableAttribute
    {
        public ClientFilterTraitAttribute(
                InheritMode inherit_mode =
                        InheritMode.DirectOnly)
                : base(inherit_mode)
        {
        }
    }

    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ThinClientFilterTraitAttribute
            : RunstarInheritableAttribute
    {
        public ThinClientFilterTraitAttribute(
                InheritMode inherit_mode =
                        InheritMode.DirectOnly)
                : base(inherit_mode)
        {
        }
    }

    [AttributeUsage(
            AttributeTargets.Class,
            AllowMultiple = false,
            Inherited = true)]
    public sealed class ServerFilterTraitAttribute
            : RunstarInheritableAttribute
    {
        public ServerFilterTraitAttribute(
                InheritMode inherit_mode =
                        InheritMode.DirectOnly)
                : base(inherit_mode)
        {
        }
    }
}
