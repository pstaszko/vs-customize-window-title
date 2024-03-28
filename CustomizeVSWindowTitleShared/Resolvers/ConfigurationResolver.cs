using EnvDTE;
using EnvDTE80;
using System.ComponentModel;


namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class ConfigurationNameResolver : SimpleTagResolver {
        public ConfigurationNameResolver() : base(tagName: "configurationName") { }

        public override string Resolve(AvailableInfo info) {
            return GetActiveConfigurationNameOrEmpty(info.Solution);
        }

        public static string GetActiveConfigurationNameOrEmpty(Solution solution) {
            if (string.IsNullOrEmpty(solution?.FullName)) return string.Empty;
            var activeConfig = (SolutionConfiguration2)solution.SolutionBuild.ActiveConfiguration;
            return activeConfig != null ? activeConfig.Name ?? string.Empty : string.Empty;
        }
    }

    public class PlatformNameResolver : SimpleTagResolver {
        public PlatformNameResolver() : base(tagName: "platformName") { }

        public override string Resolve(AvailableInfo info) {
            return GetPlatformNameOrEmpty(info.Solution);
        }

        public static string GetPlatformNameOrEmpty(Solution solution) {
            if (string.IsNullOrEmpty(solution?.FullName)) return string.Empty;
            var activeConfig = (SolutionConfiguration2)solution.SolutionBuild.ActiveConfiguration;
            return activeConfig != null ? activeConfig.PlatformName ?? string.Empty : string.Empty;
        }
    }
}