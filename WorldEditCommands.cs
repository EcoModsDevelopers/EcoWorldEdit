using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Chat;
using Eco.Mods.TechTree;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.World;
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
                Item item = Item.Get("WandAxeItem");
                ItemStack itemStack = new ItemStack(item, 1);
                user.Inventory.AddItems(itemStack);
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
                WorldEditUserData weud = WorldEditManager.getUserData(user.Name);

                if (weud.LowerPos == null || weud.HigherPos == null)
                {
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockType = WorldEditManager.findBlockTypeFromBlockName(pTypeName);

                if (blockType == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + pTypeName + " found!");
                    return;
                }

                for (int x = (int)weud.LowerPos.X; x <= weud.HigherPos.X; x++)
                    for (int y = (int)weud.HigherPos.Y; y >= weud.LowerPos.Y; y--)
                        for (int z = (int)weud.LowerPos.Z; z <= weud.HigherPos.Z; z++)
                            Eco.World.World.SetBlock(blockType, new Vector3i(x, y, z));

                int changedBlocks = (int)((weud.HigherPos.X - weud.LowerPos.X + 1) * (weud.HigherPos.Y - weud.LowerPos.Y + 1) * (weud.HigherPos.Z - weud.LowerPos.Z + 1));

                user.Player.SendTemporaryMessage("Around " + changedBlocks + " blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/replace", "", ChatAuthorizationLevel.Admin)]
        public static void Replace(User user, string pTypeNames)
        {
            try
            {
                string toFind = pTypeNames.Split(' ')[0].ToLower();
                string toReplace = pTypeNames.Split(' ')[1].ToLower();

                WorldEditUserData weud = WorldEditManager.getUserData(user.Name);

                if (weud.LowerPos == null || weud.HigherPos == null)
                {
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockTypeToFind = WorldEditManager.findBlockTypeFromBlockName(toFind);
                if (blockTypeToFind == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + toFind + " found!");
                    return;
                }

                Type blockTypeToReplace = WorldEditManager.findBlockTypeFromBlockName(toReplace);
                if (blockTypeToReplace == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + toReplace + " found!");
                    return;
                }

                long changedBlocks = 0;

                for (int x = (int)weud.LowerPos.X; x <= weud.HigherPos.X; x++)
                    for (int y = (int)weud.HigherPos.Y; y >= weud.LowerPos.Y; y--)
                        for (int z = (int)weud.LowerPos.Z; z <= weud.HigherPos.Z; z++)
                        {
                            var pos = new Vector3i(x, y, z);
                            var block = Eco.World.World.GetBlock(pos);

                            if (block != null && block.GetType() == blockTypeToFind)
                            {
                                Eco.World.World.SetBlock(blockTypeToReplace, pos);
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
                WorldEditUserData weud = WorldEditManager.getUserData(user.Name);

                if (weud.LowerPos == null || weud.HigherPos == null)
                {
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockType = WorldEditManager.findBlockTypeFromBlockName(pTypeName);

                if (blockType == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + pTypeName + " found!");
                    return;
                }

                for (int x = (int)weud.LowerPos.X; x <= weud.HigherPos.X; x++)
                    for (int y = (int)weud.HigherPos.Y; y >= weud.LowerPos.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i(x, y, (int)weud.LowerPos.Z));

                for (int x = (int)weud.LowerPos.X; x <= weud.HigherPos.X; x++)
                    for (int y = (int)weud.HigherPos.Y; y >= weud.LowerPos.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i(x, y, (int)weud.HigherPos.Z));

                for (int z = (int)weud.LowerPos.Z; z <= weud.HigherPos.Z; z++)
                    for (int y = (int)weud.HigherPos.Y; y >= weud.LowerPos.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i((int)weud.LowerPos.X, y, z));

                for (int z = (int)weud.LowerPos.Z; z <= weud.HigherPos.Z; z++)
                    for (int y = (int)weud.HigherPos.Y; y >= weud.LowerPos.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i((int)weud.HigherPos.X, y, z));

                int changedBlocks = (int)((weud.HigherPos.X + 1 * weud.LowerPos.Y + 1) * 2 + (weud.HigherPos.Z + 1 * weud.LowerPos.Y + 1) * 2);

                user.Player.SendTemporaryMessage("Around " + changedBlocks + " blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/expand", "", ChatAuthorizationLevel.Admin)]
        public static void Expand(User user, string pDirectionAndCount = "1 up")
        {
            try
            {
                int count = int.Parse(pDirectionAndCount.Split(' ')[0]);
                string direction = pDirectionAndCount.Split(' ')[1].ToLower();

                WorldEditUserData weud = WorldEditManager.getUserData(user.Name);

                if (direction == "up"  || direction == "u")
                {
                    if (weud.HigherPos != null)
                        weud.HigherPos.Y = weud.HigherPos.Y + count;
                }
                else if (direction == "down" || direction == "d")
                {
                    if (weud.LowerPos != null)
                        weud.LowerPos.Y = weud.LowerPos.Y - count;
                }
                else if (direction == "x")
                {
                    if (count > 0)
                    {
                        if (weud.HigherPos != null)
                            weud.HigherPos.X = weud.HigherPos.X + count;
                    }
                    else
                    {
                        if (weud.LowerPos != null)
                            weud.LowerPos.X = weud.LowerPos.X + count;
                    }
                }
                else if (direction == "z")
                {
                    if (count > 0)
                    {
                        if (weud.HigherPos != null)
                            weud.HigherPos.Z = weud.HigherPos.Z + count;
                    }
                    else
                    {
                        if (weud.LowerPos != null)
                            weud.LowerPos.Z = weud.LowerPos.Z + count;
                    }
                }
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
                Eco.World.World.SetBlock(typeof(StoneBlock), newpos);
                newpos.Y += 2;
                user.Player.SetPosition(newpos);
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

    }
}