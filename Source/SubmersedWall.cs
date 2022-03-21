using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SubWall
{
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

        private void Surface()
        {
            var building = (Building)ThingMaker.MakeThing(SurfacedWall.Def, Stuff);
            building.SetFaction(Faction);
            GenSpawn.Spawn(building, Position, Map, Rotation);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            var Rise = new Command_Action
            {
                defaultLabel = "SubWall_RiseAction".Translate(),
                defaultDesc = "SubWall_RiseActionDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/SubWallRise_MenuIcon", false),
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
}