using RimWorld;
using Verse;

namespace SubWall
{
    public class JobDriver_CloseGate : JobDriver_AffectGate
    {
        protected override void AffectGate()
        {
            MoteMaker.ThrowText(TargetThingA.TrueCenter(), Map, "MoteShut".Translate(), 1f);
            var building = (Building)ThingMaker.MakeThing(ThingDef.Named("ClosedGate"), TargetThingA.Stuff);
            building.SetFaction(TargetThingA.Faction);
            GenSpawn.Spawn(building, TargetThingA.Position, TargetThingA.Map, TargetThingA.Rotation);
        }
    }
}