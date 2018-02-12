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
    [Serializable]
    [Serialized]
    public class WorldEditBlock //TODO change to struct
    {
        [Serialized] public Vector3i Position { get; set; }

        [Serialized] public byte[] Data { get; set; }

        [Serialized] public Type Type;

        public WorldEditBlock()
        {
        }

        public WorldEditBlock(Type pType, byte[] pData, Vector3i pPosition) //: this()  /TODO change to struct
        {
            Type = pType;
            Position = pPosition;
            Data = pData;
        }

        public WorldEditBlock(Block pBlock, Vector3i pPosition) // : this() /TODO change to struct
        {
            Type = pBlock.GetType();
            Position = pPosition;

            var constuctor = Type.GetConstructor(Type.EmptyTypes);

            if (constuctor == null)
            {
                if (pBlock is PlantBlock)
                    Data = EcoSerializer.Serialize(((PlantBlock)pBlock).Plant).ToArray();

                if (pBlock is WorldObjectBlock)
                    Data = EcoSerializer.Serialize(pBlock).ToArray();
            }
        }

        public WorldEditBlock Clone()
        {
            return new WorldEditBlock(Type, Data, Position);
        }
    }

    /*
    public class WorldEditBlockSerializer : ISerializer
    {
        public Type Type => typeof(WorldEditBlock);
        public string SchemaType => "worldeditblock";
        public int ID { get; set; }
        public void Encode(BinaryWriter writer, object instance)
        {
            var vector = (Vector3i)instance;
            writer.EncodeZigZag(vector.x);
            writer.EncodeZigZag(vector.y);
            writer.EncodeZigZag(vector.z);
        }
        public object Decode(BinaryReader reader) => new Vector3i(reader.DecodeIntZigZag(), reader.DecodeIntZigZag(), reader.DecodeIntZigZag());
        public void Skip(BinaryReader reader)
        {
            reader.DecodeIntZigZag(); // x
            reader.DecodeIntZigZag(); // y
            reader.DecodeIntZigZag(); // z
        }
        public override string ToString() => "vector3i";
    }*/
}
