using Eco.Core.Serialization;
using Eco.Gameplay.Plants;
using Eco.Gameplay.Players;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.Simulation;
using Eco.Simulation.Agents;
using Eco.World;
using Eco.World.Blocks;
using EcoWorldEdit;
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
        public const string mSchematicPath = "./Schematics/";
        public Vector3i? FirstPos;
        public Vector3i? SecondPos;

        //     private bool mUndoDone = false;

        //multiple undos with circular buffer?
        private Stack<WorldEditBlock> mLastCommandBlocks = null;

        private List<WorldEditBlock> mClipboard = new List<WorldEditBlock>(); //Maybe "better" type?

        private Vector3i mUserClipboardPosition;

        public UserSession GetNewSession()
        {
            return new UserSession();
        }

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

            if (Math.Abs(firstResult) > Math.Abs(secondResult))
                FirstPos = FirstPos + pDirection;
            else
                SecondPos = SecondPos + pDirection;

            return true;
        }

        public bool ShiftSelection(Vector3i pDirection)
        {
            if (!FirstPos.HasValue || !SecondPos.HasValue)
                return false;

            FirstPos = FirstPos + pDirection;
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

        public bool SaveSelectionToClipboard(User pUser)
        {
            var vectors = GetSortedVectors();

            if (vectors == null)
                return false;

            mUserClipboardPosition = pUser.Player.Position.Round;

            mClipboard.Clear();

            for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                    {
                        var pos = new Vector3i(x, y, z);

                        //pos - mUserClipboardPosition: "Spitze minus Anfang"
                        mClipboard.Add(new WorldEditBlock(Eco.World.World.GetBlock(pos), pos - mUserClipboardPosition));
                    }
            return true;
        }

        public bool LoadSelectionFromClipboard(User pUser, WorldEditUserData pWeud)
        {
            if (mClipboard == null)
                return false;

            StartEditingBlocks();
            var currentPos = pUser.Player.Position.Round;

            UserSession session = pWeud.GetNewSession();

            foreach (var entry in mClipboard)
            {
                var web = entry.Clone();
                web.Position += currentPos;

                addBlockChangedEntry(Eco.World.World.GetBlock(web.Position), web.Position);
                WorldEditManager.SetBlock(web.Type, web.Position, session, null, web.Data);
            }
            return true;
        }

        public bool RotateClipboard(float pAngle)
        {
            if (mClipboard == null)
                return false;

            AffineTransform transform = new AffineTransform();

            pAngle = (float)(Math.PI * MathUtil.NormalizeAngle0to360(pAngle) / 180.0);
            transform = transform.RotateY(pAngle);

            for (int i = 0; i < mClipboard.Count; i++)
            {
                var block = mClipboard[i].Clone();
                block.Position = transform.Apply(block.Position);
                mClipboard[i] = block;
            }

            return true;
        }

        public bool SaveClipboard(string pFileName)
        {
            if (mClipboard == null)
                return false;

            var stream = EcoSerializer.Serialize(mClipboard.ToArray());

            Directory.CreateDirectory(mSchematicPath);
            pFileName = new string(pFileName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());

            File.WriteAllBytes(Path.Combine(mSchematicPath, pFileName + ".ecoschematic"), stream.ToArray());

            return true;
        }

        public bool LoadClipboard(string pFileName)
        {
            pFileName = new string(pFileName.Where(x => !Path.GetInvalidFileNameChars().Contains(x)).ToArray());

            pFileName = Path.Combine(mSchematicPath, pFileName + ".ecoschematic");

            if (!File.Exists(pFileName))
                return false;

            mClipboard = EcoSerializer.Deserialize<List<WorldEditBlock>>(File.OpenRead(pFileName));

            return true;
        }

    }
}
