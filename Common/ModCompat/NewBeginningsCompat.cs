using SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;
using SpiritReforged.Content.Ocean.Items.Vanity.DiverSet;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.ModCompat;

internal class NewBeginningsCompat : ModSystem
{
	public static Asset<Texture2D> GetIcon(string name) => ModContent.Request<Texture2D>("SpiritReforged/Common/ModCompat/NewBeginningsOrigins/" + name);

	public override void Load()
	{
		if (!ModLoader.TryGetMod("NewBeginnings", out Mod beginnings))
			return;

		beginnings.Call("Delay", () =>
		{
			AddDiver();
		});

		void AddDiver()
		{
			object equip = beginnings.Call("EquipData", ModContent.ItemType<DiverHead>(), ModContent.ItemType<DiverBody>(), ModContent.ItemType<DiverLegs>(),
				new int[] { ItemID.Flipper, ModContent.ItemType<OceanPendant>() });
			object misc = beginnings.Call("MiscData", 100, 20, -1);
			object dele = beginnings.Call("DelegateData", () => true, (List<GenPass> list) => { }, () => true, (Func<Point16>)FindBeachSpawnPoint);
			object result = beginnings.Call("ShortAddOrigin", GetIcon("Diver"), "ReforgedDiver",
				"Mods.SpiritReforged.Origins.Diver", Array.Empty<(int, int)>(), equip, misc, dele);
		}
	}

	public static Point16 FindBeachSpawnPoint()
	{
		bool left = WorldGen.genRand.NextBool(2);
		int x = left ? 280 : Main.maxTilesX - 280;
		int y = 80;

		while (Main.tile[x, y].LiquidAmount <= 0 && !Main.tile[x, y].HasTile)
			y++;

		return new Point16(x, y - 8);
	}
}
