namespace NMolecules.Analyzers.Test
{
    public static class ServiceRoleShims
    {
        public static string AppendIfNeeded(string code, string type)
        {
            return type switch
            {
                ElementNames.DomainService => code + DomainServiceShim,
                ElementNames.ApplicationService => code + ApplicationServiceShim,
                _ => code
            };
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
