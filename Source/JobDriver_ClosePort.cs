using RimWorld;
using Verse;

namespace SubWall
{
    public class JobDriver_ClosePort : JobDriver_AffectGate
    {
        public JobDriver_ClosePort()
        {
            BaseWorkAmount = 1000;
        }

        protected override void AffectGate()
        {
            MoteMaker.ThrowText(TargetThingA.TrueCenter(), Map, "MoteShut".Translate(), 1f);
            var building = (Building)ThingMaker.MakeThing(ThingDef.Named("ClosedPortcullis"), TargetThingA.Stuff);
            building.SetFaction(TargetThingA.Faction);
            GenSpawn.Spawn(building, TargetThingA.Position, TargetThingA.Map, TargetThingA.Rotation);
        }
    }
}