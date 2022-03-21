using UnityEngine;
using Verse;

namespace SubWall.Settings
{
    internal class SubWall_Mod : Mod
    {
        public static SubWall_ModSettings Settings;

        public SubWall_Mod(ModContentPack content)
            : base(content)
        {
            Settings = GetSettings<SubWall_ModSettings>();
        }

        public override string SettingsCategory()
        {
            return "SubWall_mod".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}