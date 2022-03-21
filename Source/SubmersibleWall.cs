using System.Linq;
using RimWorld;
using SubWall.Settings;
using Verse;

namespace SubWall
{
    //Industrial Tech
    public class SubmersibleWall : Building
    {
        public bool actionWaiting;
        public int powerAction = 150;
        public CompPowerTrader powerComp;
        public int progressTick;
        public int ticksToAction = 360;
        public bool IsPowered => powerComp.PowerOn;

        public UtilityConsole MannedConsole => PowerComp?.PowerNet?.powerComps?.Select(x => x.parent)
            .OfType<UtilityConsole>().FirstOrDefault(x => x.Manned);

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
            ticksToAction = SubWall_Mod.Settings.ticksToAction;
            powerAction = SubWall_Mod.Settings.powerAction;
        }

        // for debug inspect
        /*
       public override string GetInspectString()
       {
           StringBuilder stringBuilder = new StringBuilder();
           string baseString = base.GetInspectString();
           if (!baseString.NullOrEmpty())
           {
               stringBuilder.Append(baseString);
               stringBuilder.AppendLine();
           }
           stringBuilder.Append("ticksToAction: " + ticksToAction.ToString());
           stringBuilder.AppendLine();
           stringBuilder.Append("powerAction: " + powerAction.ToString());
           stringBuilder.AppendLine();
           //stringBuilder.Append("PowerOutput: " + powerComp.PowerOutput);
           //stringBuilder.AppendLine();
           //stringBuilder.Append("basePowerConsumption: " + powerComp.PowerOutput);
           //stringBuilder.AppendLine();
           stringBuilder.Append("powerComp: " + powerComp.PowerOn);

           return stringBuilder.ToString().TrimEndNewlines();
       }
       */
        public void DoProgress(int progress)
        {
            MoteMaker.ThrowText(this.TrueCenter(), Map, (progress + 60).TicksToSeconds().ToString(), 1f);
        }

        public void PendAction()
        {
            actionWaiting = true;
            powerComp.PowerOutput = -powerAction;
        }
    }


    //Medieval Tech
}

//*/