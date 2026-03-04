using System.Linq;

namespace NMolecules.Analyzers.Test
{
    public static class ServiceRoleShims
    {
        public static string AppendIfNeeded(string code, string type)
        {
            return AppendIfNeeded(code, new[] { type });
        }

        public static string AppendIfNeeded(string code, params string[] types)
        {
            var result = code;

            foreach (var type in types.Distinct())
            {
                result = type switch
                {
                    ElementNames.DomainService when !result.Contains("public class DomainServiceAttribute") => result + DomainServiceShim,
                    ElementNames.ApplicationService when !result.Contains("public class ApplicationServiceAttribute") => result + ApplicationServiceShim,
                    ElementNames.CommandHandler when !result.Contains("public class CommandHandlerAttribute") => result + CommandHandlerShim,
                    ElementNames.CommandDispatcher when !result.Contains("public class CommandDispatcherAttribute") => result + CommandDispatcherShim,
                    ElementNames.ApplicationLayer when !result.Contains("public class ApplicationLayerAttribute") => result + ApplicationLayerShim,
                    ElementNames.DomainLayer when !result.Contains("public class DomainLayerAttribute") => result + DomainLayerShim,
                    ElementNames.InterfaceLayer when !result.Contains("public class InterfaceLayerAttribute") => result + InterfaceLayerShim,
                    ElementNames.InfrastructureLayer when !result.Contains("public class InfrastructureLayerAttribute") => result + InfrastructureLayerShim,
                    ElementNames.QueryModel when !result.Contains("public class QueryModelAttribute") => result + QueryModelShim,
                    ElementNames.UserInterfaceLayer when !result.Contains("public class UserInterfaceLayerAttribute") => result + UserInterfaceLayerShim,
                    _ => result
                };
            }

            return result;
        }

        private const string DomainServiceShim = @"

namespace NMolecules.DDD
{
    using System;

    public class DomainServiceAttribute : Attribute
    {
    }
}";

        private const string ApplicationServiceShim = @"

namespace NMolecules.DDD
{
    using System;

    public class ApplicationServiceAttribute : Attribute
    {
    }
}";

        private const string CommandHandlerShim = @"

namespace NMolecules.Architecture.Cqrs
{
    using System;

    public class CommandHandlerAttribute : Attribute
    {
    }
}";

        private const string CommandDispatcherShim = @"

namespace NMolecules.Architecture.Cqrs
{
    using System;

    public class CommandDispatcherAttribute : Attribute
    {
    }
}";

        private const string ApplicationLayerShim = @"

namespace NMolecules.Architecture.Layered
{
    using System;

    public class ApplicationLayerAttribute : Attribute
    {
    }
}";

        private const string DomainLayerShim = @"

namespace NMolecules.Architecture.Layered
{
    using System;

    public class DomainLayerAttribute : Attribute
    {
    }
}";

        private const string InfrastructureLayerShim = @"

namespace NMolecules.Architecture.Layered
{
    using System;

    public class InfrastructureLayerAttribute : Attribute
    {
    }
}";

        private const string InterfaceLayerShim = @"

namespace NMolecules.Architecture.Layered
{
    using System;

    public class InterfaceLayerAttribute : Attribute
    {
    }
}";

        private const string UserInterfaceLayerShim = @"

namespace NMolecules.Architecture.Layered
{
    using System;

    public class UserInterfaceLayerAttribute : Attribute
    {
    }
}";

        private const string QueryModelShim = @"

namespace NMolecules.Architecture.Cqrs
{
    using System;

    public class QueryModelAttribute : Attribute
    {
    }
}";
    }
}
