using Asphalt.Util;
using Eco.Core.Plugins;
using Eco.Core.Serialization;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Plants;
using Eco.Gameplay.Players;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.Simulation;
using Eco.Simulation.Agents;
using Eco.Simulation.Settings;
using Eco.Simulation.Types;
using Eco.World;
using Eco.World.Blocks;
using EcoWorldEdit;
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

        public static void SetBlock(WorldEditBlock pBlock, UserSession pSession)
        {
            SetBlock(pBlock.Type, pBlock.Position, pSession, null, null, pBlock.Data);
        }

        public static void SetBlock(Type pType, Vector3i pPosition, UserSession pSession = null, Vector3i? pSourcePosition = null, Block pSourceBlock = null, byte[] pData = null)
        {
            if (pType == null || pType.DerivesFrom<PickupableBlock>())
                pType = typeof(EmptyBlock);

            WorldObjectBlock worldObjectBlock = Eco.World.World.GetBlock(pPosition) as WorldObjectBlock;

            if (worldObjectBlock != null)
            {
                if (worldObjectBlock.WorldObjectHandle.Object.Position3i == pPosition)
                {
                    worldObjectBlock.WorldObjectHandle.Object.Destroy();
                }
            }

            if (pType == typeof(EmptyBlock))
            {
                Eco.World.World.DeleteBlock(pPosition);
                return;
            }

            var constuctor = pType.GetConstructor(Type.EmptyTypes);

            if (constuctor != null)
            {
                Eco.World.World.SetBlock(pType, pPosition);
                return;
            }

            Type[] types = new Type[1];
            types[0] = typeof(WorldPosition3i);

            constuctor = pType.GetConstructor(types);

            if (constuctor != null)
            {
                object obj = null;

                if (pData != null)
                {
                    MemoryStream ms = new MemoryStream(pData);
                    obj = EcoSerializer.Deserialize(ms);
                }

                if (pType.DerivesFrom<PlantBlock>())
                {
                    PlantSpecies ps = null;
                    Plant pb = null;

                    if (obj != null)
                    {
                        pb = (Plant)obj;
                        ps = pb.Species;
                    }
                    else
                    {
                        ps = WorldUtils.GetPlantSpecies(pType);

                        if (pSourceBlock != null)
                            pb = PlantBlock.GetPlant(pPosition);
                    }

                    Plant newplant = EcoSim.PlantSim.SpawnPlant(ps, pPosition);

                    if (pb != null)
                    {
                        newplant.YieldPercent = pb.YieldPercent;
                        newplant.Dead = pb.Dead;
                        newplant.DeadType = pb.DeadType;
                        newplant.GrowthPercent = pb.GrowthPercent;
                        newplant.DeathTime = pb.DeathTime;
                        newplant.Tended = pb.Tended;
                    }

                    return;
                }

                AsphaltLog.WriteLine("Unknown Type: " + pType);
            }

            types[0] = typeof(WorldObject);
            constuctor = pType.GetConstructor(types);

            if (constuctor != null)
            {
                WorldObject wObject = null;

                if (pSourceBlock != null)
                {
                    wObject = ((WorldObjectBlock)pSourceBlock).WorldObjectHandle.Object;

                    //if this is not the "main block" of an object do nothing
                    if (wObject.Position3i != pSourcePosition.Value)
                        return;

                }
                else if (pData != null)
                {
                    MemoryStream ms = new MemoryStream(pData);
                    var obj = EcoSerializer.Deserialize(ms);

                    if (obj is WorldObject)
                    {
                        wObject = obj as WorldObject;
                    }
                    else
                        throw new InvalidOperationException("obj is not WorldObjectBlock");
                }

                if (wObject == null)
                    return;


                //    WorldObject newObject = WorldObjectUtil.Spawn(wObject.GetType().Name, wObject.Creator.User, pPosition);
                //    newObject.Rotation = wObject.Rotation;


                WorldObject newObject = (WorldObject)Activator.CreateInstance(wObject.GetType(), true);
                newObject = WorldObjectManager.Add(newObject, wObject.Creator.User, pPosition, wObject.Rotation);


                {
                    StorageComponent newSC = newObject.GetComponent<StorageComponent>();

                    if (newSC != null)
                    {
                        StorageComponent oldPSC = wObject.GetComponent<StorageComponent>();
                        newSC.Inventory.AddItems(oldPSC.Inventory.Stacks);
                        //    newSC.Inventory.OnChanged.Invoke(null);
                    }

                }

                {
                    CustomTextComponent newTC = newObject.GetComponent<CustomTextComponent>();

                    if (newTC != null)
                    {
                        CustomTextComponent oldTC = wObject.GetComponent<CustomTextComponent>();

                        typeof(CustomTextComponent).GetProperty("Text", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(newTC, oldTC.Text);
                    }
                }

                return;
            }

            AsphaltLog.WriteLine("Unknown Type: " + pType);
        }

        public static Direction GetDirectionAndAmount(User pUser, string pDirectionAndAmount, out int pAmount)
        {
            pAmount = 1;
            string[] splitted = pDirectionAndAmount.Split(' ', ',');

            if (!int.TryParse(splitted[0], out pAmount))
            {
                pUser.Player.SendTemporaryError($"Please provide an amount first");
                return Direction.Unknown;
            }

            return DirectionUtils.GetDirectionOrLooking(pUser, splitted.Length >= 2 ? splitted[1] : null);
        }
    }
}
