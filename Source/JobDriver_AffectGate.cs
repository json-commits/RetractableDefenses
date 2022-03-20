using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace SubWall
{
    public abstract class JobDriver_AffectGate : JobDriver
    {
        protected int BaseWorkAmount = 4000;
        private float workLeft = -1000f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }


        protected override IEnumerable<Toil> MakeNewToils()
        {
            //this.FailOn(() => (!jobDriver_CloseGate.job.ignoreDesignations && jobDriver_CloseGate.Map.designationManager.DesignationAt(jobDriver_CloseGate.TargetLocA, jobDriver_CloseGate.DesDef) == null) ? true : false);
            //this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);
            var doWork = new Toil
            {
                initAction = delegate { workLeft = BaseWorkAmount; }
            };
            doWork.tickAction = delegate
            {
                var num = doWork.actor.GetStatValue(StatDefOf.GeneralLaborSpeed) * 1.7f;
                workLeft -= num;
                if (doWork.actor.skills != null)
                {
                    //doWork.actor.skills.Learn(SkillDefOf.Construction, 0.1f);
                }

                if (!(workLeft <= 0f))
                {
                    return;
                }

                AffectGate();
                //jobDriver_CloseGate.Map.designationManager.DesignationAt(jobDriver_CloseGate.TargetLocA, jobDriver_CloseGate.DesDef)?.Delete();
                ReadyForNextToil();
            };
            //doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            doWork.WithProgressBar(TargetIndex.A, () => 1f - (workLeft / BaseWorkAmount));
            doWork.defaultCompleteMode = ToilCompleteMode.Never;
            //doWork.activeSkill = (() => SkillDefOf.Construction);
            yield return doWork;
        }

        protected abstract void AffectGate();
    }
}