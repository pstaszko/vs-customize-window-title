// Guids.cs
// MUST match guids.h
using System;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    static class GuidList {

        public const string guidCustomizeVSWindowTitlePkgString = "5126c493-138a-46d7-a04d-ad772f6be158";
        public const string guidCustomizeVSWindowTitleCmdSetString = "939a4ccc-55d2-4f90-8858-b7fce11bb09b";

        public static readonly Guid guidCustomizeVSWindowTitleCmdSet = new Guid(guidCustomizeVSWindowTitleCmdSetString);
    };
}