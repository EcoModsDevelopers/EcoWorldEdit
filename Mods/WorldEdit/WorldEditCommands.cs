using Eco.Gameplay;
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

                for (int x = vectors.Lower.X; x <= vectors.Higher.X; x++)
                    for (int y = vectors.Higher.Y; y >= vectors.Lower.Y; y--)
                        for (int z = vectors.Lower.Z; z <= vectors.Higher.Z; z++)
                            Eco.World.World.SetBlock(blockType, new Vector3i(x, y, z));

                int changedBlocks = (int)((vectors.Higher.X - vectors.Lower.X + 1) * (vectors.Higher.Y - vectors.Lower.Y + 1) * (vectors.Higher.Z - vectors.Lower.Z + 1));

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

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (weud.FirstPos == null || weud.SecondPos == null)
                {
                    user.Player.SendTemporaryMessage("Please set both Points with the Wand Tool first!");
                    return;
                }

                Type blockTypeToFind = WorldEditManager.FindBlockTypeFromBlockName(toFind);
                if (blockTypeToFind == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + toFind + " found!");
                    return;
                }

                Type blockTypeToReplace = WorldEditManager.FindBlockTypeFromBlockName(toReplace);
                if (blockTypeToReplace == null)
                {
                    user.Player.SendTemporaryMessage("No BlockType with name " + toReplace + " found!");
                    return;
                }

                var vectors = weud.GetSortedVectors();

                long changedBlocks = 0;

                for (int x = vectors.Lower.X; x <= vectors.Higher.X; x++)
                    for (int y = vectors.Higher.Y; y >= vectors.Lower.Y; y--)
                        for (int z = vectors.Lower.Z; z <= vectors.Higher.Z; z++)
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

                for (int x = vectors.Lower.X; x <= vectors.Higher.X; x++)
                    for (int y = vectors.Higher.Y; y >= vectors.Lower.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i(x, y, vectors.Lower.Z));

                for (int x = vectors.Lower.X; x <= vectors.Higher.X; x++)
                    for (int y = vectors.Higher.Y; y >= vectors.Lower.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i(x, y, vectors.Higher.Z));

                for (int z = vectors.Lower.Z; z <= vectors.Higher.Z; z++)
                    for (int y = vectors.Higher.Y; y >= vectors.Lower.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i(vectors.Lower.X, y, z));

                for (int z = vectors.Lower.Z; z <= vectors.Higher.Z; z++)
                    for (int y = vectors.Higher.Y; y >= vectors.Lower.Y; y--)
                        Eco.World.World.SetBlock(blockType, new Vector3i(vectors.Higher.X, y, z));

                int changedBlocks = (vectors.Higher.X + 1 * vectors.Lower.Y + 1) * 2 + (vectors.Higher.X + 1 * vectors.Lower.Y + 1) * 2;

                user.Player.SendTemporaryMessage("Around " + changedBlocks + " blocks changed.");
            }
            catch (Exception e)
            {
                Log.WriteError(e.ToStringPretty());
            }
        }

        [ChatCommand("/expand", "", ChatAuthorizationLevel.Admin)]
        public static void Expand(User user, string pDirectionAndAmount = "1 up")
        {
            try
            {
                int amount = int.Parse(pDirectionAndAmount.Split(' ')[0]);
                string direction = pDirectionAndAmount.Split(' ')[1].ToLower();

                WorldEditUserData weud = WorldEditManager.GetUserData(user.Name);

                if (direction == "up" || direction == "u")
                {
                    if (weud.ApplyToHighestVector(v => new Vector3i(v.X, v.Y + amount, v.Z)))
                        user.Player.SendTemporaryMessage("Expanded " + amount + " up");
                }
                else if (direction == "down" || direction == "d")
                {
                    if (weud.ApplyToLowestVector(v => new Vector3i(v.X, v.Y - amount, v.Z)))
                        user.Player.SendTemporaryMessage("Expanded " + amount + " down");
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