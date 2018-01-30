using Eco.Core.Serialization;
using Eco.Gameplay.Plants;
using Eco.Shared.Math;
using Eco.Simulation;
using Eco.Simulation.Agents;
using Eco.World;
using Eco.World.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    public class WorldEditUserData
    {
        public Vector3i? FirstPos;
        public Vector3i? SecondPos;

        //     private bool mUndoDone = false;

        //multiple undos with circular buffer?
        private Stack<WorldEditBlock> mLastCommandBlocks = null;

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

            higher.X = Math.Max(pos1.X, pos2.X) + 1;
            higher.Y = Math.Max(pos1.Y, pos2.Y) + 1;
            higher.Z = Math.Max(pos1.Z, pos2.Z) + 1;

            return new SortedVectorPair(lower, higher);
        }

        public bool ExpandSelection(Vector3i pDirection)
        {
            if (!FirstPos.HasValue || !SecondPos.HasValue)
                return false;

            var firstResult = SumAllAxis(pDirection * FirstPos.Value);
            var secondResult = SumAllAxis(pDirection * SecondPos.Value);

            if (firstResult > secondResult)
                FirstPos = FirstPos + pDirection;
            else
                SecondPos = SecondPos + pDirection;

            return true;
        }

        // Is there a correct name for this operation?
        public int SumAllAxis(Vector3i pVector)
        {
            return pVector.X + pVector.Y + pVector.Z;
        }

        public void StartEditingBlocks()
        {
            mLastCommandBlocks = new Stack<WorldEditBlock>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void addBlockChangedEntry(Block pBlock, Vector3i pPosition)
        {
            mLastCommandBlocks.Push(new WorldEditBlock(pBlock, pPosition));
        }

        public bool Undo()
        {
            if (mLastCommandBlocks == null)
                return false;

            foreach (var entry in mLastCommandBlocks)
            {
                WorldEditManager.SetBlock(entry);
            }
            return true;
        }
    }
}
