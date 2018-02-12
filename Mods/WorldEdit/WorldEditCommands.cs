using Eco.Core.Plugins;
using Eco.Gameplay;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Mods.TechTree;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.World;
using Eco.World.Blocks;
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
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockType = WorldEditManager.FindBlockTypeFromBlockName(pTypeName);

                if (blockType == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + pTypeName + " found!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                weud.StartEditingBlocks();

                for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                        for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                        {
                            var pos = new Vector3i(x, y, z);
                            weud.addBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                            WorldEditManager.SetBlock(blockType, pos);
                        }

                int changedBlocks = (int)((vectors.Higher.X - vectors.Lower.X) * (vectors.Higher.Y - vectors.Lower.Y) * (vectors.Higher.Z - vectors.Lower.Z));

                user.Player.SendTemporaryMessage("Around " + changedBlocks + " blocks changed.");
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
                    user.Player.SendTemporaryMessage("Please set both points with the Wand Tool first!");
                    return;
                }

                Type blockTypeToFind = WorldEditManager.FindBlockTypeFromBlockName(toFind);
                if (blockTypeToFind == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + toFind + " found!");
                    return;
                }

                Type blockTypeToReplace = null;

                if (toReplace != string.Empty)
                {
                    blockTypeToReplace = WorldEditManager.FindBlockTypeFromBlockName(toReplace);
                    if (blockTypeToReplace == null)
                    {
                        user.Player.SendTemporaryMessage("No BlockType with name " + toReplace + " found!");
                        return;
                    }
                }

                var vectors = weud.GetSortedVectors();

                long changedBlocks = 0;


                //if toReplace is string empty we will replace everything except empty with toFind type

                weud.StartEditingBlocks();

                if (toReplace != string.Empty)
                    for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                            {
                                var pos = new Vector3i(x, y, z);
                                var block = Eco.World.World.GetBlock(pos);

                                if (block != null && block.GetType() == blockTypeToFind)
                                {
                                    weud.addBlockChangedEntry(block, pos);
                                    WorldEditManager.SetBlock(blockTypeToReplace, pos);
                                    changedBlocks++;
                                }
                            }
                else
                    for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                            {
                                var pos = new Vector3i(x, y, z);
                                var block = Eco.World.World.GetBlock(pos);

                                if (block != null && block.GetType() != typeof(EmptyBlock))
                                {
                                    weud.addBlockChangedEntry(block, pos);
                                    WorldEditManager.SetBlock(blockTypeToFind, pos);
                                    changedBlocks++;
                                }
                            }

                user.Player.SendTemporaryMessage(changedBlocks + " blocks changed.");
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
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockType = WorldEditManager.FindBlockTypeFromBlockName(pTypeName);

                if (blockType == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + pTypeName + " found!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                weud.StartEditingBlocks();
                for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(x, y, vectors.Lower.Z);
                        weud.addBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(x, y, vectors.Higher.Z - 1);
                        weud.addBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(vectors.Lower.X, y, z);
                        weud.addBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                    for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                    {
                        var pos = new Vector3i(vectors.Higher.X - 1, y, z);
                        weud.addBlockChangedEntry(Eco.World.World.GetBlock(pos), pos);
                        WorldEditManager.SetBlock(blockType, pos);
                    }

                int changedBlocks = (((vectors.Higher.X - vectors.Lower.X) * 2 + (vectors.Higher.Z - vectors.Lower.Z) * 2) - 4) * (vectors.Higher.Y - vectors.Lower.Y);

                if (changedBlocks == 0) //maybe better math?
                    changedBlocks = 1;

                user.Player.SendTemporaryMessage("Around " + changedBlocks + " blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/stack", "", ChatAuthorizationLevel.Admin)]
        public static void Stack(User user, int pAmount = 1)
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

                Direction dir = WorldEditManager.getLookingDirection(user);

                weud.StartEditingBlocks();

                for (int i = 1; i <= pAmount; i++)
                {
                    Vector3i offset = dir.ToVec() * (vectors.Higher - vectors.Lower) * i;

                    for (int x = vectors.Lower.X; x < vectors.Higher.X; x++)
                        for (int y = vectors.Lower.Y; y < vectors.Higher.Y; y++)
                            for (int z = vectors.Lower.Z; z < vectors.Higher.Z; z++)
                            {
                                var pos = new Vector3i(x, y, z);

                                weud.addBlockChangedEntry(Eco.World.World.GetBlock(pos + offset), pos + offset);
                                WorldEditManager.SetBlock(new WorldEditBlock(Eco.World.World.GetBlock(pos), pos + offset));
                            }
                }

                int changedBlocks = (int)((vectors.Higher.X - vectors.Lower.X) * (vectors.Higher.Y - vectors.Lower.Y) * (vectors.Higher.Z - vectors.Lower.Z)) * pAmount;

                user.Player.SendTemporaryMessage("Around " + changedBlocks + " blocks changed.");
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
                int amount = 1;
                // Vector3i direction = user.FacingDir.ToVec();
                Direction direction = Direction.Unknown;

                if (pDirectionAndAmount != null)
                {
                    string[] splitted = pDirectionAndAmount.Split(' ');

                    if (!int.TryParse(splitted[0], out amount))
                    {
                        user.Player.SendTemporaryError("Please provide an amount first");
                        return;
                    }

                    if (splitted.Length >= 2)
                    {
                        switch (splitted[1])
                        {
                            case "up":
                            case "u":
                                direction = Direction.Up;
                                break;
                            case "down":
                            case "d":
                                direction = Direction.Down;
                                break;
                            default:
                                user.Player.SendTemporaryError("Unknown direction!");
                                return;
                        }
                    }
                    else
                    {
                        direction = WorldEditManager.getLookingDirection(user);
                    }
                }

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                weud.ExpandSelection(direction.ToVec() * amount);

                user.Player.SendTemporaryMessage("Expanded " + amount + " " + direction);
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
                    user.Player.SendTemporaryMessage("Undo done.");
                else
                    user.Player.SendTemporaryMessage("You can't use undo right now!");
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
                    user.Player.SendTemporaryMessage("Copy done.");
                else
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
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

                if (weud.LoadSelectionFromClipboard(user))
                    user.Player.SendTemporaryMessage("Paste done.");
                else
                    user.Player.SendTemporaryMessage("Please copy a selection first!");
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
                    user.Player.SendTemporaryMessage("Rotation in clipboard done.");
                else
                    user.Player.SendTemporaryMessage("Please copy a selection first!");
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
                    user.Player.SendTemporaryMessage("Export done.");
                else
                    user.Player.SendTemporaryMessage("Please copy a selection first!");
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
                    user.Player.SendTemporaryMessage("Import done. Use //paste");
                else
                    user.Player.SendTemporaryMessage("Schematic file not found! ");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

    }
}