using System.Linq;
using EnvDTE;

namespace ErwinMayerLabs.RenameVSWindowTitle.Resolvers {
    public class AnythingUnsavedResolver : SimpleTagResolver {
        public AnythingUnsavedResolver() : base(tagName: "anythingUnsaved") { }
        
        private bool IsDirty(AvailableInfo info) {
            if (info.Solution != null && info.Solution.IsOpen && !info.Solution.Saved) return true;
            var hasUnsavedDocument = info.ActiveDocument?.Collection
                .OfType<Document>()
                .Any(document => !document.Saved)
                ?? false;
            return hasUnsavedDocument;
        }

        public override string Resolve(AvailableInfo info) {
            return IsDirty(info) ? "*" : string.Empty;
        }
    }
}