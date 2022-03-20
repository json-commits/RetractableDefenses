using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SubWall
{
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
                return new FloatMenuOption(
                    "CannotUseReason".Translate("IncapableOfCapacity".Translate(PawnCapacityDefOf.Manipulation.label,
                        myPawn.Named("PAWN"))), null);
            }

            return null;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            var failureReason = GetFailureReason(myPawn);
            if (failureReason != null)
            {
                yield return failureReason;
                yield break;
            }

            switch (def.ToString())
            {
                case "OpenGate":
                    yield return new FloatMenuOption("OrderCloseThing".Translate(LabelShort, this), delegate
                    {
                        var job = JobMaker.MakeJob(SubWall_JobDefOf.RetDef_CloseGate, this);
                        myPawn.jobs.TryTakeOrderedJob(job);
                    });
                    break;

                case "ClosedGate":
                    yield return new FloatMenuOption("Open".Translate(this, LabelShort), delegate
                    {
                        var job = JobMaker.MakeJob(SubWall_JobDefOf.RetDef_OpenGate, this);
                        myPawn.jobs.TryTakeOrderedJob(job);
                    });
                    break;

                case "OpenPortcullis":
                    yield return new FloatMenuOption("OrderRaiseThing".Translate(LabelShort, this), delegate
                    {
                        var job = JobMaker.MakeJob(SubWall_JobDefOf.RetDef_ClosePort, this);
                        myPawn.jobs.TryTakeOrderedJob(job);
                    });
                    break;

                case "ClosedPortcullis":
                    yield return new FloatMenuOption("OrderLowerThing".Translate(LabelShort, this), delegate
                    {
                        var job = JobMaker.MakeJob(SubWall_JobDefOf.RetDef_OpenPort, this);
                        myPawn.jobs.TryTakeOrderedJob(job);
                    });
                    break;
            }
        }
        //*/
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
		stringBuilder.Append(this.def.ToString());

		return stringBuilder.ToString().TrimEndNewlines();
	
	*/
    }
}