using Eco.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
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
