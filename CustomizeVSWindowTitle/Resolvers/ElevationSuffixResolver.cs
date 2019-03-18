namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class ElevationSuffixResolver : SimpleTagResolver {
        public ElevationSuffixResolver() : base(tagName: "elevationSuffix") { }

        public override string Resolve(AvailableInfo info) {
            return info.ElevationSuffix ?? string.Empty;
        }
    }
}