using Eco.Core.Serialization;
using Eco.Gameplay.Plants;
using Eco.Shared.Math;
using Eco.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    public struct WorldEditBlock
    {
        public Vector3i Position { get; set; }

        public byte[] Data { get; set; }

        public Type Type;

        public WorldEditBlock(Type pType, byte[] pData, Vector3i pPosition) : this()
        {
            Type = pType;
            Position = pPosition;
            Data = pData;
        }

        public WorldEditBlock(Block pBlock, Vector3i pPosition) : this()
        {
            Type = pBlock.GetType();
            Position = pPosition;

            var constuctor = Type.GetConstructor(Type.EmptyTypes);

            if (constuctor == null)
            {
                if (pBlock is PlantBlock)
                {
                    var stream = EcoSerializer.Serialize(((PlantBlock)pBlock).Plant);
                    Data = stream.ToArray();
                }
            }
        }

        public WorldEditBlock Clone()
        {
            return new WorldEditBlock(Type, Data, Position);
        }
    }
}
