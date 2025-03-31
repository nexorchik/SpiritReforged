using SpiritReforged.Common.PlayerCommon;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Buffs;

public class EmpoweredSwim : ModBuff
{
	public override void SetStaticDefaults() => Main.buffNoTimeDisplay[Type] = true;

	public override void Update(Player player, ref int buffIndex)
	{
		player.ignoreWater = true;
		player.accFlipper = true;

		if (player.buffTime[buffIndex] > 2)
		{
			if (player.wet && !Colliding())
			{
				player.fullRotationOrigin = player.Size / 2f;
				player.Rotate(player.velocity.ToRotation() + MathHelper.PiOver2);
			}
			else
				player.Rotate(player.fullRotation * .8f);
		}

		//Check standard solid and platform collision
		bool Colliding() => Collision.SolidCollision(player.BottomLeft, player.width, 4) || Main.tileSolidTop[Framing.GetTileSafely(player.Bottom).TileType];
	}
}
