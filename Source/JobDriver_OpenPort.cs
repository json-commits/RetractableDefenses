using RimWorld;
using Verse;

namespace SubWall
{
    public class JobDriver_OpenPort : JobDriver_AffectGate
    {
        public JobDriver_OpenPort()
        {
            BaseWorkAmount = 6000;
        }

        protected override void AffectGate()
        {
            MoteMaker.ThrowText(TargetThingA.TrueCenter(), Map, "MoteOpen".Translate(), 1f);
            var building = (Building)ThingMaker.MakeThing(ThingDef.Named("OpenPortcullis"), TargetThingA.Stuff);
            building.SetFaction(TargetThingA.Faction);
            GenSpawn.Spawn(building, TargetThingA.Position, TargetThingA.Map, TargetThingA.Rotation);
        }
    }
}