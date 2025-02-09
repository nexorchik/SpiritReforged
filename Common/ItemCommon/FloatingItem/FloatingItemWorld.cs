using Terraria.Utilities;

namespace SpiritReforged.Common.ItemCommon.FloatingItem;

public class FloatingItemWorld : ModSystem
{
	private static readonly WeightedRandom<int> floatingItemPool = new();

	public override void PostSetupContent()
	{
		foreach (Type type in SpiritReforgedMod.Instance.Code.GetTypes())
			if (type.IsSubclassOf(typeof(FloatingItem)) && !type.IsAbstract)
			{
				var item = Mod.Find<ModItem>(type.Name) as FloatingItem;
				floatingItemPool.Add(item.Type, item.SpawnWeight);
			}
	}

	public override void PreUpdateWorld()
	{
		int floatingItemCount = 0;
		
		foreach (Item item in Main.ActiveItems)
		{
			if (item.ModItem is FloatingItem)
				floatingItemCount++;
		}

		if (floatingItemCount > 12 || !Main.rand.NextBool(3000))
			return;

		int x = Main.rand.Next(600, Main.maxTilesX);
		if (Main.rand.NextBool(2))
			x = Main.rand.Next(Main.maxTilesX * 15, Main.maxTilesX * 16 - 600);

		int y = (int)(Main.worldSurface * 0.35) + 400;

		for (; Framing.GetTileSafely(x / 16, y / 16).LiquidAmount < 200; y += 16)
		{
			if (y / 16 > Main.worldSurface) //If we somehow miss all water, exit
				return;
		}

		y += 40;

		int id = Item.NewItem(Entity.GetSource_NaturalSpawn(), new Vector2(x, y), floatingItemPool);

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.SyncItem, number: id);
	}
}