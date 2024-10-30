namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Buffs;

public class EmpoweredSwim : ModBuff
{
	public override void SetStaticDefaults() => Main.buffNoTimeDisplay[Type] = true;

	public override void Update(Player player, ref int buffIndex)
	{
		player.ignoreWater = true;
		player.accFlipper = true;

		if (player.buffTime[buffIndex] > 2)
			if (player.wet && !Collision.SolidCollision(player.BottomLeft, player.width, 4))
			{
				player.fullRotationOrigin = player.Size / 2f;
				player.fullRotation = player.velocity.ToRotation() + MathHelper.PiOver2;
			}
			else
				player.fullRotation *= 0.8f;
		else
			player.fullRotation = 0f;
	}
}
