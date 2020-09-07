using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using System.Text;
using SubWall.Settings;
using System;

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
	//Industrial Tech
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

	//Medieval Tech
	public class Gate : Building
    {
        ///*
		private FloatMenuOption GetFailureReason(Pawn myPawn)
		{
			if (!myPawn.CanReach(this, PathEndMode.ClosestTouch, Danger.Deadly))
			{
				return new FloatMenuOption("CannotUseNoPath".Translate(), null);
			}
			if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				return new FloatMenuOption("CannotUseReason".Translate("IncapableOfCapacity".Translate(PawnCapacityDefOf.Manipulation.label, myPawn.Named("PAWN"))), null);
			}
			return null;
		}
		
		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			FloatMenuOption failureReason = GetFailureReason(myPawn);
			if (failureReason != null)
			{
				yield return failureReason;
				yield break;
			}
            else
            {
				if(def.ToString() == "OpenGate")
                {
					yield return new FloatMenuOption("OrderManThing".Translate(LabelShort, this), delegate
					{
						Job job = JobMaker.MakeJob(SubWall_JobDefOf.RetDef_CloseGate, this);
						myPawn.jobs.TryTakeOrderedJob(job);
					});
				}
				if(def.ToString() == "ClosedGate")
                {
					yield return new FloatMenuOption("OrderManThing".Translate(LabelShort, this), delegate
					{
						Job job = JobMaker.MakeJob(SubWall_JobDefOf.RetDef_OpenGate, this);
						myPawn.jobs.TryTakeOrderedJob(job);
					});
				}
            }
	}
		//*/
        public override string GetInspectString()
        {
			StringBuilder stringBuilder = new StringBuilder();
			string baseString = base.GetInspectString();
			if (!baseString.NullOrEmpty())
			{
				stringBuilder.Append(baseString);
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(this.def.ToString());

			return stringBuilder.ToString().TrimEndNewlines();
		}
    }
	[DefOf]
	class SubWall_JobDefOf
    {
		public static JobDef RetDef_CloseGate;
		public static JobDef RetDef_OpenGate;
	}
	///*
	public abstract class JobDriver_AffectGate : JobDriver
    {
		private float workLeft = -1000f;

		protected int BaseWorkAmount => 4000;

		protected DesignationDef DesDef => DesignationDefOf.SmoothWall;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
			return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
		}


		protected override IEnumerable<Toil> MakeNewToils()
		{
			//this.FailOn(() => (!jobDriver_CloseGate.job.ignoreDesignations && jobDriver_CloseGate.Map.designationManager.DesignationAt(jobDriver_CloseGate.TargetLocA, jobDriver_CloseGate.DesDef) == null) ? true : false);
			//this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
            Toil doWork = new Toil
            {
                initAction = delegate
                {
                    workLeft = BaseWorkAmount;
				}
            };
            doWork.tickAction = delegate
			{
				float num = doWork.actor.GetStatValue(StatDefOf.GeneralLaborSpeed) * 1.7f;
				workLeft -= num;
				if (doWork.actor.skills != null)
				{
					//doWork.actor.skills.Learn(SkillDefOf.Construction, 0.1f);
				}
				if (workLeft <= 0f)
				{
					AffectGate();
					//jobDriver_CloseGate.Map.designationManager.DesignationAt(jobDriver_CloseGate.TargetLocA, jobDriver_CloseGate.DesDef)?.Delete();
					ReadyForNextToil();
				}
			};
			//doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			doWork.WithProgressBar(TargetIndex.A, () => 1f - workLeft / (float)BaseWorkAmount);
			doWork.defaultCompleteMode = ToilCompleteMode.Never;
			//doWork.activeSkill = (() => SkillDefOf.Construction);
			yield return doWork;
		}
		protected abstract void AffectGate();
    }
	public class JobDriver_CloseGate : JobDriver_AffectGate
    {
		protected override void AffectGate()
        {
			MoteMaker.ThrowText(TargetThingA.TrueCenter(), this.Map, "Gate Closed!", 1f);
			Building building = (Building)ThingMaker.MakeThing(ThingDef.Named("ClosedGate"), TargetThingA.Stuff);
			building.SetFaction(TargetThingA.Faction);
			GenSpawn.Spawn(building, TargetThingA.Position, TargetThingA.Map, TargetThingA.Rotation, WipeMode.Vanish);
		}
    }

	public class JobDriver_OpenGate : JobDriver_AffectGate
	{
		protected override void AffectGate()
		{
			MoteMaker.ThrowText(TargetThingA.TrueCenter(), this.Map, "Gate Opened!", 1f);
			Building building = (Building)ThingMaker.MakeThing(ThingDef.Named("OpenGate"), TargetThingA.Stuff);
			building.SetFaction(TargetThingA.Faction);
			GenSpawn.Spawn(building, TargetThingA.Position, TargetThingA.Map, TargetThingA.Rotation, WipeMode.Vanish);
		}
	}
	//*/
}