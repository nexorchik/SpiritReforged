namespace SpiritReforged.Content.Visuals.FrostBreath;

public class FrostBreathPlayer : ModPlayer
{
	public override void PreUpdate()
	{
		if ((Player.ZoneSnow || Player.ZoneSkyHeight) && !Player.behindBackWall && Main.rand.NextBool(27))
		{
			Vector2 spawnPos = new Vector2(Player.Center.X + 8 * Player.direction, Player.Center.Y - 2f);
			if (Player.sleeping.isSleeping)
				spawnPos = new Vector2(Player.Center.X + ((Player.direction == -1) ? 20 : -14), Player.Center.Y - 6f);

			int d = Dust.NewDust(spawnPos, Player.width, 10, ModContent.DustType<Dusts.FrostBreath>(), 1.5f * Player.direction, 0f, 100, default, Main.rand.NextFloat(.20f, 0.75f));
			Main.dust[d].velocity.Y *= 0f;
		}
	}
}