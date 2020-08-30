using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using System.Text;
using SubWall.Settings;

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
	internal class SubWall_ModSettings : ModSettings
	{
		public int ticksToAction = 360;

		public int powerAction = 150;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToAction, "ticksToAction", 360);
			Scribe_Values.Look(ref powerAction, "powerAction", 15);
		}

		public void DoSettingsWindowContents(Rect canvas)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(canvas);
			listing_Standard.Label("SubWall_Changes".Translate());
			listing_Standard.Label("SubWall_ticksToAction".Translate() + ticksToAction / 60);
			ticksToAction =  (int) (listing_Standard.Slider(ticksToAction / 60, 1, 25) * 60);
			listing_Standard.Label("SubWall_powerAction".Translate() + powerAction);
			powerAction = (int) listing_Standard.Slider(powerAction, 15, 500);
			listing_Standard.End();
			SubWall_Mod.Settings.Write();
		}
	}
}

namespace SubWall
{
	public class SubmersibleWall : Building
    {
		public CompPowerTrader powerComp;
		public bool IsPowered => powerComp.PowerOn;
		public int progressTick;
		public bool actionWaiting = false;
		public int ticksToAction = 360;
		public int powerAction = 150;
		public UtilityConsole MannedConsole => PowerComp?.PowerNet?.powerComps?.Select((CompPowerTrader x) => x.parent).OfType<UtilityConsole>().FirstOrDefault((UtilityConsole x) => x.Manned);

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
			powerComp = GetComp<CompPowerTrader>();
			ticksToAction = SubWall_Mod.Settings.ticksToAction; ;
			powerAction = SubWall_Mod.Settings.powerAction; ;

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
			MoteMaker.ThrowText(this.TrueCenter(), this.Map, (progress + 60).TicksToSeconds().ToString(), 1f);
		}
		public void PendAction()
        {
			actionWaiting = true;
			powerComp.PowerOutput = -powerAction;
        }
	}


	public class SurfacedWall : SubmersibleWall
	{
		public static readonly ThingDef Def = ThingDef.Named("SubmersibleWall");

		public override void Tick()
		{
            if (actionWaiting)
            {
				progressTick += 1;
				if(progressTick % 60 == 0)
                {
					DoProgress(ticksToAction - progressTick);
				}
            }
			if (progressTick >= ticksToAction)
            {
				progressTick = 0;
				actionWaiting = false;
				Submerge();
			}
			base.Tick();
		}

		private void Submerge()
		{
			Building building = (Building)ThingMaker.MakeThing(SubmersedWall.Def, this.Stuff);
			building.SetFaction(this.Faction);
			GenSpawn.Spawn(building, this.Position, this.Map, this.Rotation, WipeMode.Vanish);
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			Command_Action Sub = new Command_Action
			{
				defaultLabel = "SubWall_SubAction".Translate(),
				defaultDesc = "SubWall_SubActionDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get("UI/SubWallBricks_MenuIcon", reportFailure: false),
				action = PendAction
			};
			if(MannedConsole != null && IsPowered && !actionWaiting)
            {
				Sub.disabled = false;
            }
			else
			{
				Sub.disabled = true;
				if (MannedConsole == null)
				{
					Sub.disabledReason = "SubWall_MannedError".Translate();
				}
				if (!IsPowered)
				{
					Sub.disabledReason = "SubWall_PowerError".Translate();
				}
				if (actionWaiting)
				{
					Sub.disabledReason = "SubWall_PendError".Translate();
				}
			}
			yield return Sub;
		}
	}
	

	public class SubmersedWall : SubmersibleWall
	{
		public static readonly ThingDef Def = ThingDef.Named("SubmersedWall");
		public override void Tick()
		{
			if (actionWaiting)
			{
				progressTick += 1;
				if (progressTick % 60 == 0)
				{
					DoProgress(ticksToAction - progressTick);
				}
			}
			if (progressTick >= ticksToAction)
			{
				progressTick = 0;
				actionWaiting = false;
				Surface();
			}
			base.Tick();
		}
		private void  Surface()
		{
			Building building = (Building)ThingMaker.MakeThing(SurfacedWall.Def, this.Stuff);
			building.SetFaction(this.Faction);
			GenSpawn.Spawn(building, this.Position, this.Map, this.Rotation, WipeMode.Vanish);
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			Command_Action Rise = new Command_Action
			{
				defaultLabel = "SubWall_RiseAction".Translate(),
				defaultDesc = "SubWall_RiseActionDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get("UI/SubWallRise_MenuIcon", reportFailure: false),
				action = PendAction
			};
			if (MannedConsole != null && IsPowered && !actionWaiting)
			{
				Rise.disabled = false;
			}
            else
            {
                Rise.disabled = true;
				if (MannedConsole == null)
				{
					Rise.disabledReason = "SubWall_MannedError".Translate();
				}
				if (!IsPowered)
				{
					Rise.disabledReason = "SubWall_PowerError".Translate();
				}
				if (actionWaiting)
				{
					Rise.disabledReason = "SubWall_PendError".Translate();
				}
			}
			yield return Rise;
		}
	}

	public class UtilityConsole : Building
    {
		public CompMannable mannableComp;
		public bool Manned => mannableComp.MannedNow;

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
			mannableComp = GetComp<CompMannable>();
		}
	}
}