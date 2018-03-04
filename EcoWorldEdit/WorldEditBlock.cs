using Eco.Core.Serialization;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Plants;
using Eco.Shared.Math;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    [Serialized]
    public struct WorldEditBlock
    {
        public static WorldEditBlock CreateNew(Block pBlock, Vector3i pPosition, Vector3i? pSourcePosition)
        {
            WorldEditBlock web = new WorldEditBlock();
            web.Position = pPosition;
            web.Type = pBlock.GetType();

            var constuctor = web.Type.GetConstructor(Type.EmptyTypes);

            if (constuctor == null)
            {
                if (pBlock is PlantBlock)
                    web.Data = EcoSerializer.Serialize(((PlantBlock)pBlock).Plant).ToArray();

                if (pBlock is WorldObjectBlock)
                {
                    WorldObjectBlock wob = (WorldObjectBlock)pBlock;

                    if (wob.WorldObjectHandle.Object.Position3i == pSourcePosition)
                    {
                        web.Data = EcoSerializer.Serialize(wob.WorldObjectHandle.Object).ToArray();
                    }
                }
            }

            return web;
        }

        [Serialized] public Vector3i Position { get; set; }

        [Serialized] public byte[] Data { get; set; }

        [Serialized] public Type Type;

        public WorldEditBlock(Type pType, Vector3i pPosition, byte[] pData) : this()
        {
            Type = pType;
            Position = pPosition;
            Data = pData;
        }

        public WorldEditBlock Clone()
        {
            return new WorldEditBlock(Type, Position, Data);
        }
    }

}
