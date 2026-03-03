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
    }
}
