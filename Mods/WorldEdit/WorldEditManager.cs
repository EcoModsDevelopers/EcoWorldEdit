using Eco.Core.Plugins;
using Eco.Core.Serialization;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.Simulation;
using Eco.Simulation.Agents;
using Eco.World;
using Eco.World.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    public class WorldEditManager
    {
        protected static int mDefaultWorldSaveFrequency;
        protected static bool mWorldSaveStopped = false;
        protected static object mLocker = new object();
        //  internal static SimpleSerializer mSimpleSerializer = new SimpleSerializer();

        protected static Dictionary<string, WorldEditUserData> mUserData = new Dictionary<string, WorldEditUserData>();


        public static ItemStack getWandItemStack()
        {
            Item item = Item.Get("WandAxeItem");
            return new ItemStack(item, 1);
        }

        public static WorldEditUserData GetUserData(string pUsername)
        {
            if (mUserData.Keys.Contains(pUsername))
                return mUserData[pUsername];

            WorldEditUserData weud = new WorldEditUserData();
            mUserData.Add(pUsername, weud);
            return weud;
        }

        public static Type FindBlockTypeFromBlockName(string pBlockName)
        {
            pBlockName = pBlockName.ToLower();

            if (pBlockName == "air")
                return typeof(EmptyBlock);

            Type blockType = BlockManager.BlockTypes.FirstOrDefault(t => t.Name.ToLower() == pBlockName + "floorblock");

            if (blockType == null)
                blockType = BlockManager.BlockTypes.FirstOrDefault(t => t.Name.ToLower() == pBlockName);

            if (blockType == null)
                blockType = BlockManager.BlockTypes.FirstOrDefault(t => t.Name.ToLower() == pBlockName + "block");

            return blockType;
        }

        //an idea was to move this method to WorldEditUserData, but then maybe findBlock must be called twice
        public static Block SetBlock(Type pType, Vector3i pVector, params object[] pArgs)
        {
            if (pType == null || pType == typeof(EmptyBlock))
            {
                Eco.World.World.DeleteBlock(pVector);
                return null;
            }
            else
                return Eco.World.World.SetBlock(pType, pVector, pArgs);
        }

        public static void SetBlock(WorldEditBlock pBlock)
        {
            if (pBlock.Type == null)
                pBlock.Type = typeof(EmptyBlock);

            var constuctor = pBlock.Type.GetConstructor(Type.EmptyTypes);

            if (constuctor != null)
            {
                WorldEditManager.SetBlock(pBlock.Type, pBlock.Position);
                return;
            }

            Type[] types = new Type[1];
            types[0] = typeof(WorldPosition3i);

            constuctor = pBlock.Type.GetConstructor(types);

            if (constuctor != null)
            {
                MemoryStream ms = new MemoryStream(pBlock.Data);
                var obj = EcoSerializer.Deserialize(ms);

                if (obj is Plant) //IsInstanceOfType
                {
                    // ((PlantBlock)block).Plant = ((PlantBlock)obj).Plant;
                    Plant pb = (Plant)obj;

                    var newplant = EcoSim.PlantSim.SpawnPlant(pb.Species, pBlock.Position);

                    PropertyInfo property = newplant.GetType().GetProperty("BornTime");
                    property.SetValue(newplant, pb.BornTime);

                    newplant.YieldPercent = pb.YieldPercent;
                    newplant.Dead = pb.Dead;
                    newplant.DeadType = pb.DeadType;
                    newplant.GrowthPercent = pb.GrowthPercent;
                    newplant.TreeNode = pb.TreeNode;

                    return;
                }

                Log.WriteLine("Unknown Type: " + pBlock.Type);
            }
            /*

            types[0] = typeof(WorldObject);
            constuctor = pBlock.Type.GetConstructor(types);

            if (constuctor != null)
            {
                MemoryStream ms = new MemoryStream(pBlock.Data);
                var obj = EcoSerializer.Deserialize(ms);

                if (obj is WorldObjectBlock)
                {
                    // ((PlantBlock)block).Plant = ((PlantBlock)obj).Plant;
                    WorldObjectBlock pb = (WorldObjectBlock)obj;

                    WorldObject wObject = pb.WorldObjectHandle.Object;

                    //    .ObjectID = Guid.NewGuid();



                    //       wWorldObject newWObject = WorldObjectManager.TryToAdd(WorldObjectManager.GetTypeFromName("StorageChestObject"), wObject.Creator.User,
                    //           pBlock.Position, wObject.Rotation, false, wObject.CreationItem);


                    //       wObject.GetType().InvokeMember("ObjectID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, wObject, new object[] { Guid.NewGuid() });
                    
                    if (WorldObjectManager.GetFromID(wObject.ObjectID) == null)
                    {
                        wObject = WorldObjectManager.Add(wObject, wObject.Creator.User, pBlock.Position, wObject.Rotation);
                    }

                    //      Block block = WorldEditManager.SetBlock(typeof(WorldObjectBlock), pBlock.Position, wObject);
                    //       WorldObjectBlock newBlock = block as WorldObjectBlock;

                    return;
                }

            }*/

            Log.WriteLine("Unknown Type: " + pBlock.Type);
        }




        public static Direction getLookingDirection(User pUser)
        {
            float yDirection = pUser.Rotation.Forward.Y;

            if (yDirection > 0.85)
                return Direction.Up;
            else if (yDirection < -0.85)
                return Direction.Down;

            return pUser.FacingDir;
        }
    }
}
