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
        public Vector3i? FirstPos;
        public Vector3i? SecondPos;

        public SortedVectorPair GetSortedVectors()
        {
            if (!FirstPos.HasValue || !SecondPos.HasValue)
                return null;

            Vector3i lower = new Vector3i();
            Vector3i higher = new Vector3i();

            Vector3i pos1 = FirstPos.Value;
            Vector3i pos2 = SecondPos.Value;

            lower.X = Math.Min(pos1.X, pos2.X);
            lower.Y = Math.Min(pos1.Y, pos2.Y);
            lower.Z = Math.Min(pos1.Z, pos2.Z);

            higher.X = Math.Max(pos1.X, pos2.X);
            higher.Y = Math.Max(pos1.Y, pos2.Y);
            higher.Z = Math.Max(pos1.Z, pos2.Z);

            return new SortedVectorPair(lower, higher);
        }

        public bool ApplyToHighestVector(Func<Vector3i, Vector3i> pFunc)
        {
            if (!FirstPos.HasValue || !SecondPos.HasValue)
                return false;

            if (FirstPos.Value.Y > SecondPos.Value.Y)
            {
                FirstPos = pFunc.Invoke(FirstPos.Value);
                return true;
            }

            SecondPos = pFunc.Invoke(SecondPos.Value);
            return true;
        }

        public bool ApplyToLowestVector(Func<Vector3i, Vector3i> pFunc)
        {
            if (!FirstPos.HasValue || !SecondPos.HasValue)
                return false;

            if (FirstPos.Value.Y < SecondPos.Value.Y)
            {
                FirstPos = pFunc.Invoke(FirstPos.Value);
                return true;
            }

            SecondPos = pFunc.Invoke(SecondPos.Value);
            return true;
        }

        public class SortedVectorPair
        {
            public Vector3i Lower { get; protected set; }
            public Vector3i Higher { get; protected set; }

            public SortedVectorPair(Vector3i pLower, Vector3i pHigher)
            {
                Lower = pLower;
                Higher = pHigher;
            }
        }
    }
}
