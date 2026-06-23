using System;
using RunstarSystems.ECS.Data;

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
