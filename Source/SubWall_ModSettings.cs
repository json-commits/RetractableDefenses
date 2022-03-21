using UnityEngine;
using Verse;

namespace SubWall.Settings
{
    internal class SubWall_ModSettings : ModSettings
    {
        public int powerAction = 150;
        public int ticksToAction = 360;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToAction, "ticksToAction", 360);
            Scribe_Values.Look(ref powerAction, "powerAction", 15);
        }

        public void DoSettingsWindowContents(Rect canvas)
        {
            var listing_Standard = new Listing_Standard();
            listing_Standard.Begin(canvas);
            listing_Standard.Label("SubWall_Changes".Translate());
            listing_Standard.Label("SubWall_ticksToAction".Translate() + (ticksToAction / 60));
            ticksToAction = (int)(listing_Standard.Slider(ticksToAction / 60, 1, 25) * 60);
            listing_Standard.Label("SubWall_powerAction".Translate() + powerAction);
            powerAction = (int)listing_Standard.Slider(powerAction, 15, 500);
            listing_Standard.End();
            SubWall_Mod.Settings.Write();
        }
    }
}