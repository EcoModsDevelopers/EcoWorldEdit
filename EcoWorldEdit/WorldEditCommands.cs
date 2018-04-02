using Eco.Core.Plugins;
using Eco.Gameplay;
using Eco.Gameplay.Items;
using Eco.Gameplay.Plants;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Mods.TechTree;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.World;
using Eco.World.Blocks;
using EcoWorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Mods.WorldEdit
{
    public class WorldEditCommands : IChatCommandHandler
    {
        [ChatCommand("/wand", "", ChatAuthorizationLevel.Admin)]
        public static void Wand(User user)
        {
            try
            {
                user.Inventory.AddItems(WorldEditManager.getWandItemStack());
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/rmwand", "", ChatAuthorizationLevel.Admin)]
        public static void RmWand(User user)
        {
            try
            {
                user.Inventory.TryRemoveItems(WorldEditManager.getWandItemStack());
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/set", "", ChatAuthorizationLevel.Admin)]
        public static void Set(User user, string pTypeName)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage($"Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockType = WorldEditManager.FindBlockTypeFromBlockName(pTypeName);

                if (blockType == null)
                {
                    user.Player.SendTemporaryMessage($"No BlockType with name {pTypeName} found!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                weud.StartEditingBlocks();

                for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                        for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                        {
                            var pos = new Vector3i(x, y, z);
                            weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                            WorldEditManager.SetBlock(blockType, pos);
                        }

                int changedBlocks = (int)((vectors.Higher.X - vectors.Lower.X) * (vectors.Higher.Y - vectors.Lower.Y) * (vectors.Higher.Z - vectors.Lower.Z));

                user.Player.SendTemporaryMessage($"{changedBlocks} blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/replace", "", ChatAuthorizationLevel.Admin)]
        public static void Replace(User user, string pTypeNames = "")
        {
            try
            {
                string[] splitted = pTypeNames.Split(' ');
                string toFind = splitted[0].ToLower();

                string toReplace = string.Empty;

                if (splitted.Length >= 2)
                    toReplace = pTypeNames.Split(' ')[1].ToLower();

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
                    return;
                }

                Type blockTypeToFind = WorldEditManager.FindBlockTypeFromBlockName(toFind);
                if (blockTypeToFind == null)
                {
                    user.Player.SendTemporaryMessage($"No BlockType with name {toFind} found!");
                    return;
                }

                Type blockTypeToReplace = null;

                if (toReplace != string.Empty)
                {
                    blockTypeToReplace = WorldEditManager.FindBlockTypeFromBlockName(toReplace);
                    if (blockTypeToReplace == null)
                    {
                        user.Player.SendTemporaryMessage($"No BlockType with name { toReplace } found!");
                        return;
                    }
                }

                var vectors = weud.GetSortedVectors();

                long changedBlocks = 0;


                //if toReplace is string empty we will replace everything except empty with toFind type

                weud.StartEditingBlocks();

                if (toReplace != string.Empty)
                    for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                            {
                                var pos = new Vector3i(x, y, z);
                                var block = Eco.World.World.GetBlock(pos);

                                if (block != null && block.GetType() == blockTypeToFind)
                                {
                                    weud.AddBlockChangedEntry(block, pos);
                                    WorldEditManager.SetBlock(blockTypeToReplace, pos);
                                    changedBlocks++;
                                }
                            }
                else
                    for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                            {
                                var pos = new Vector3i(x, y, z);
                                var block = Eco.World.World.GetBlock(pos);

                                if (block != null && block.GetType() != typeof(EmptyBlock))
                                {
                                    weud.AddBlockChangedEntry(block, pos);
                                    WorldEditManager.SetBlock(blockTypeToFind, pos);
                                    changedBlocks++;
                                }
                            }

                user.Player.SendTemporaryMessage($"{changedBlocks} blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/walls", "", ChatAuthorizationLevel.Admin)]
        public static void Walls(User user, string pTypeName)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage($"Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockType = WorldEditManager.FindBlockTypeFromBlockName(pTypeName);

                if (blockType == null)
                {
                    user.Player.SendTemporaryMessage($"No BlockType with name { pTypeName } found!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                weud.StartEditingBlocks();
                for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(x, y, vectors.Lower.Z);
                        weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(x, y, vectors.Higher.Z - 1);
                        weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(vectors.Lower.X, y, z);
                        weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(vectors.Higher.X - 1, y, z);
                        weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                int changedBlocks = (((vectors.Higher.X - vectors.Lower.X) * 2 + (vectors.Higher.Z - vectors.Lower.Z) * 2) - 4) * (vectors.Higher.Y - vectors.Lower.Y);

                if (changedBlocks == 0) //maybe better math?
                    changedBlocks = 1;

                user.Player.SendTemporaryMessage($"{ changedBlocks } blocks changed.");

            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/stack", "", ChatAuthorizationLevel.Admin)]
        public static void Stack(User user, string pDirectionAndAmount = "1")
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                Direction dir = WorldEditManager.GetDirectionAndAmount(user, pDirectionAndAmount, out int amount);

                weud.StartEditingBlocks();
                UserSession session = weud.GetNewSession();

                for (int i = 1; i <= amount; i++)
                {
                    Vector3i offset = dir.ToVec() * (vectors.Higher - vectors.Lower) * i;

                    for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                            {
                                var pos = new Vector3i(x, y, z);

                                weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos + offset), pos + offset);
                                var sourceBlock = Eco.World.World.GetBlock(pos);
                                WorldEditManager.SetBlock(sourceBlock.GetType(), pos + offset, session, pos, sourceBlock);
                            }
                }

                int changedBlocks = (int)((vectors.Higher.X - vectors.Lower.X) * (vectors.Higher.Y - vectors.Lower.Y) * (vectors.Higher.Z - vectors.Lower.Z)) * amount;

                user.Player.SendTemporaryMessage($"{changedBlocks} blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/move", "", ChatAuthorizationLevel.Admin)]
        public static void Move(User user, string pDirectionAndAmount = "1")
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                Direction dir = WorldEditManager.GetDirectionAndAmount(user, pDirectionAndAmount, out int amount);

                weud.StartEditingBlocks();

                UserSession session = weud.GetNewSession();

                Vector3i offset = dir.ToVec() * amount;

                //     if (dir == Direction.Up)
                //          offset *= vectors.Higher.Y - vectors.Lower.Y;


                Action<int, int, int> action = (int x, int y, int z) =>
                  {
                      var pos = new Vector3i(x, y, z);

                      weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                      weud.AddBlockChangedEntry(Eco.World.World.GetBlock(pos + offset), pos + offset);

                      var sourceBlock = Eco.World.World.GetBlock(pos);
                      WorldEditManager.SetBlock(sourceBlock.GetType(), pos + offset, session, pos, sourceBlock);
                      WorldEditManager.SetBlock(typeof(EmptyBlock), pos, session);
                  };


                if (dir == Direction.Left || dir == Direction.Back || dir == Direction.Down)
                {
                    for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                                action.Invoke(x, y, z);
                }
                else
                {
                    /*                for (int x = vectors.Higher.X - 1; x >= vectors.Lower.X; x--)
                                        for (int y = vectors.Higher.Y - 1; y >= vectors.Lower.Y; y--)
                                            for (int z = vectors.Higher.Z - 1; z >= vectors.Lower.Z; z--)*/

                    int x = vectors.Higher.X - 1;
                    if (x < 0)
                        x = x + Shared.Voxel.World.VoxelSize.X;

                    int startZ = vectors.Higher.Z - 1;
                    if (startZ < 0)
                        startZ = startZ + Shared.Voxel.World.VoxelSize.Z;

                    Console.WriteLine("--------------");
                    Console.WriteLine(vectors.Lower);
                    Console.WriteLine(vectors.Higher);

                    for (; x != (vectors.Lower.X - 1); x--)
                    {
                        if (x < 0)
                            x = x + Shared.Voxel.World.VoxelSize.X;
                        for (int y = vectors.Higher.Y - 1; y >= vectors.Lower.Y; y--)
                            for (int z = startZ; z != (vectors.Lower.Z - 1); z--)
                            {
                                if (z < 0)
                                    z = z + Shared.Voxel.World.VoxelSize.Z;

                                Console.WriteLine($"{x} {y} {z}");
                                action.Invoke(x, y, z);
                            }
                    }
                }


                int changedBlocks = (int)((vectors.Higher.X - vectors.Lower.X) * (vectors.Higher.Y - vectors.Lower.Y) * (vectors.Higher.Z - vectors.Lower.Z)) * amount;

                user.Player.SendTemporaryMessage($"{changedBlocks} blocks moved.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/expand", "", ChatAuthorizationLevel.Admin)]
        public static void Expand(User user, string pDirectionAndAmount = "1")
        {
            try
            {
                Direction direction = WorldEditManager.GetDirectionAndAmount(user, pDirectionAndAmount, out int amount);
                if (direction == Direction.Unknown)
                    return;

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.ExpandSelection(direction.ToVec() * amount))
                    user.Player.SendTemporaryMessage($"Expanded selection {amount} direction");
                else
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/contract", "", ChatAuthorizationLevel.Admin)]
        public static void Contract(User user, string pDirectionAndAmount = "1")
        {
            try
            {
                Direction direction = WorldEditManager.GetDirectionAndAmount(user, pDirectionAndAmount, out int amount);
                if (direction == Direction.Unknown)
                    return;

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.ExpandSelection(direction.ToVec() * -amount, true))
                    user.Player.SendTemporaryMessage($"Contracted selection {amount} {direction}");
                else
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/shift", "", ChatAuthorizationLevel.Admin)]
        public static void Shift(User user, string pDirectionAndAmount = "1")
        {
            try
            {
                Direction direction = WorldEditManager.GetDirectionAndAmount(user, pDirectionAndAmount, out int amount);
                if (direction == Direction.Unknown)
                    return;

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.ShiftSelection(direction.ToVec() * amount))
                    user.Player.SendTemporaryMessage($"Shifted selection {amount} {direction}");
                else
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/up", "", ChatAuthorizationLevel.Admin)]
        public static void Up(User user, int pCount = 1)
        {
            try
            {
                Vector3 pos = user.Player.Position;
                var newpos = new Vector3i((int)pos.X, (int)pos.Y + pCount, (int)pos.Z);
                WorldEditManager.SetBlock(typeof(StoneBlock), newpos);
                newpos.Y += 2;
                user.Player.SetPosition(newpos);
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }


        [ChatCommand("/undo", "", ChatAuthorizationLevel.Admin)]
        public static void Undo(User user)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.Undo())
                    user.Player.SendTemporaryMessage($"Undo done.");
                else
                    user.Player.SendTemporaryMessage($"You can't use undo right now!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }


        [ChatCommand("/copy", "", ChatAuthorizationLevel.Admin)]
        public static void Copy(User user)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.SaveSelectionToClipboard(user))
                    user.Player.SendTemporaryMessage($"Copy done.");
                else
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/paste", "", ChatAuthorizationLevel.Admin)]
        public static void Paste(User user)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.LoadSelectionFromClipboard(user, weud))
                    user.Player.SendTemporaryMessage($"Paste done.");
                else
                    user.Player.SendTemporaryMessage($"Please copy a selection first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/rotate", "", ChatAuthorizationLevel.Admin)]
        public static void Rotate(User user, int pDegree = 90)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.RotateClipboard(pDegree))
                    user.Player.SendTemporaryMessage($"Rotation in clipboard done.");
                else
                    user.Player.SendTemporaryMessage($"Please copy a selection first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/export", "", ChatAuthorizationLevel.Admin)]
        public static void Export(User user, string pFileName)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.SaveClipboard(pFileName))
                    user.Player.SendTemporaryMessage($"Export done.");
                else
                    user.Player.SendTemporaryMessage($"Please copy a selection first!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/import", "", ChatAuthorizationLevel.Admin)]
        public static void Import(User user, string pFileName)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.LoadClipboard(pFileName))
                    user.Player.SendTemporaryMessage($"Import done. Use //paste");
                else
                    user.Player.SendTemporaryMessage($"Schematic file not found!");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/distr", "", ChatAuthorizationLevel.Admin)]
        public static void Distr(User user)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage($"Please set both points with the Wand Tool first!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                Dictionary<string, long> mBlocks = new Dictionary<string, long>();

                for (int x = vectors.Lower.X; x != vectors.Higher.X; x = (x + 1) % Shared.Voxel.World.VoxelSize.X)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                        for (int z = vectors.Lower.Z; z != vectors.Higher.Z; z = (z + 1) % Shared.Voxel.World.VoxelSize.Z)
                        {
                            //                 Console.WriteLine($"{x} {y} {z}");
                            var pos = new Vector3i(x, y, z);
                            var block = Eco.World.World.GetBlock(pos).GetType().ToString();

                            long count;
                            mBlocks.TryGetValue(block, out count);
                            mBlocks[block] = count + 1;
                        }

                double amountBlocks = mBlocks.Values.Sum(); // (vectors.Higher.X - vectors.Lower.X) * (vectors.Higher.Y - vectors.Lower.Y) * (vectors.Higher.Z - vectors.Lower.Z);

                ChatManager.ServerMessageToPlayer($"total blocks: {amountBlocks}", user, false, Shared.Services.DefaultChatTags.Notifications, Shared.Services.ChatCategory.Info);

                foreach (var entry in mBlocks)
                {
                    string percent = (Math.Round((entry.Value / amountBlocks) * 100, 2)).ToString() + "%";
                    string nameOfBlock = entry.Key.Substring(entry.Key.LastIndexOf(".") + 1);
                    ChatManager.ServerMessageToPlayer($"{entry.Value.ToString().PadRight(6)} {percent.PadRight(6)} {nameOfBlock}", user, false, Shared.Services.DefaultChatTags.Notifications, Shared.Services.ChatCategory.Info);
                }
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        /*
        [ChatCommand("/grow", "", ChatAuthorizationLevel.Admin)]
        public static void Grow(User user, string pFileName)
        {
            try
            {
                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                        for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                        {
                            var pos = new Vector3i(x, y, z);
                            var block = Eco.World.World.GetBlock(pos);

                            if (block is PlantBlock)
                            {
                                var pb = block as PlantBlock;
                                pb.Plant.GrowthPercent = 1;
                                pb.Plant.YieldPercent = 1;
                                pb.Plant.Tick();
                            }

                        }

            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }*/

    }
}