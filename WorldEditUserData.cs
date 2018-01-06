using Eco.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    public class WorldEditUserData
    {
        public Vector3 LowerPos;
        public Vector3 HigherPos;

        public void sortVectors()
        {
            if (LowerPos == null || HigherPos == null)
                return;

            Vector3 lower = new Vector3();
            Vector3 higher = new Vector3();

            lower.X = Math.Min(LowerPos.X, HigherPos.X);
            lower.Y = Math.Min(LowerPos.Y, HigherPos.Y);
            lower.Z = Math.Min(LowerPos.Z, HigherPos.Z);

            higher.X = Math.Max(LowerPos.X, HigherPos.X);
            higher.Y = Math.Max(LowerPos.Y, HigherPos.Y);
            higher.Z = Math.Max(LowerPos.Z, HigherPos.Z);

            LowerPos = lower;
            HigherPos = higher;
        }
    }
}
