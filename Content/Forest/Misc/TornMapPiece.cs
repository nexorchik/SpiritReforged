using SpiritReforged.Common.MapCommon;

namespace SpiritReforged.Content.Forest.Misc;

public class TornMapPiece : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 28;
		Item.maxStack = Item.CommonMaxStack;
		Item.value = 1000;
		Item.rare = ItemRarityID.White;
		Item.useAnimation = Item.useTime = 30;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.consumable = true;
	}

	public override bool? UseItem(Player player)
	{
		const int Radius = 100;

		if (Main.myPlayer == player.whoAmI && !Main.dedServ)
		{
			for (int k = 0; k < 10; k++)
			{
				int dust = Dust.NewDust(player.Center, player.width, player.height, DustID.PortalBolt);
				Vector2 vector2_1 = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * (Main.rand.Next(50, 100) * 0.04f);
				Main.dust[dust].velocity = vector2_1;
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position = player.Center - Vector2.Normalize(vector2_1) * 34f;
			}
		}

		var point = (player.Center / 16).ToPoint16();
		RevealMap.DoReveal(point.X, point.Y, Radius);

		return true;
	}
}