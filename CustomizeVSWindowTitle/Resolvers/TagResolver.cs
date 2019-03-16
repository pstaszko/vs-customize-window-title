using System;
using System.Collections.Generic;

namespace ErwinMayerLabs.CustomizeVSWindowTitleExtension.Resolvers {
    public interface ITagResolver {
        IEnumerable<string> TagNames { get; }
        bool TryResolve(string tag, AvailableInfo info, out string s);
    }

    public interface ISimpleTagResolver : ITagResolver {
        string TagName { get; }
        string Resolve(AvailableInfo info);
    }

    public abstract class TagResolver : ITagResolver {
        public IEnumerable<string> TagNames { get; }

        protected TagResolver(IEnumerable<string> tagNames) {
            this.TagNames = tagNames;
        }

        public abstract bool TryResolve(string tag, AvailableInfo availableInfo, out string s);
    }

    public abstract class SimpleTagResolver : ISimpleTagResolver {
        public string TagName { get; }
        public IEnumerable<string> TagNames { get; }

        protected SimpleTagResolver(string tagName) {
            this.TagName = tagName;
            this.TagNames = new[] { this.TagName };
        }

        public abstract string Resolve(AvailableInfo info);

        public bool TryResolve(string tag, AvailableInfo info, out string s) {
            s = null;
            if (tag != this.TagName) return false;
            s = this.Resolve(info: info);
            return true;
        }
    }
}
