using RimWorld;
using Verse;

namespace SubWall
{
    public class JobDriver_OpenGate : JobDriver_AffectGate
    {
        protected override void AffectGate()
        {
            MoteMaker.ThrowText(TargetThingA.TrueCenter(), Map, "MoteOpen".Translate(), 1f);
            var building = (Building)ThingMaker.MakeThing(ThingDef.Named("OpenGate"), TargetThingA.Stuff);
            building.SetFaction(TargetThingA.Faction);
            GenSpawn.Spawn(building, TargetThingA.Position, TargetThingA.Map, TargetThingA.Rotation);
        }
    }
}