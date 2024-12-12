using Terraria.DataStructures;

namespace SpiritReforged.Content.Desert.GildedScarab;

internal class ScarabFrontLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ProjectileOverArm);
	protected override void Draw(ref PlayerDrawSet drawInfo) //reference sharkbonesExplosion and use DrawAnimationVerticalRect or reference how its done in inferno potion
	{
		if (drawInfo.shadow != 0f)
			return;
		Player drawPlayer = drawInfo.drawPlayer;

		if (drawPlayer.HasBuff(ModContent.BuffType<GildedScarab_buff>()) && (drawPlayer.GetModPlayer<GildedScarabPlayer>().scarabTimer <= 12 || drawPlayer.GetModPlayer<GildedScarabPlayer>().scarabTimer >= 24))
		{
			Texture2D texture = ModContent.Request<Texture2D>("SpiritReforged/Content/Desert/GildedScarab/GildedScarab_player").Value;
			var size = new Vector2(68, 54);
			Vector2 origin = size / 2;
			Vector2 drawPos = drawPlayer.Center;
			Point tileLocation = Main.LocalPlayer.Center.ToTileCoordinates();

			var drawData = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle(0, 56 * (drawPlayer.GetModPlayer<GildedScarabPlayer>().scarabTimer / 4), 68, 54), Lighting.GetColor(tileLocation), 0, origin, 1, SpriteEffects.None, 0);
			drawInfo.DrawDataCache.Add(drawData);
		}
	}
}

internal class ScarabBackLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ForbiddenSetRing);
	protected override void Draw(ref PlayerDrawSet drawInfo) //reference sharkbonesExplosion and use DrawAnimationVerticalRect or reference how its done in inferno potion
	{
		if (drawInfo.shadow != 0f)
			return;

		Player drawPlayer = drawInfo.drawPlayer;
		if (drawPlayer.HasBuff(ModContent.BuffType<GildedScarab_buff>()) && drawPlayer.GetModPlayer<GildedScarabPlayer>().scarabTimer >= 13 && drawPlayer.GetModPlayer<GildedScarabPlayer>().scarabTimer <= 23)
		{
			Texture2D texture = ModContent.Request<Texture2D>("SpiritReforged/Content/Desert/GildedScarab/GildedScarab_player").Value;
			var size = new Vector2(68, 54); //frame size
			Vector2 origin = size / 2;
			Vector2 drawPos = drawPlayer.Center;
			Point tileLocation = Main.LocalPlayer.Center.ToTileCoordinates();

			var drawData = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle(0, 56 * (drawPlayer.GetModPlayer<GildedScarabPlayer>().scarabTimer / 4), 68, 54), Lighting.GetColor(tileLocation), 0, origin, 1, SpriteEffects.None, 0);
			drawInfo.DrawDataCache.Add(drawData);
		}
	}
}