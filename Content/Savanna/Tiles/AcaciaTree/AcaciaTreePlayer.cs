using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

public class AcaciaTreePlayer : ModPlayer
{
	public override void PreUpdateMovement()
	{
		static Rectangle GetPlatform(Point16 tilePosition)
		{
			var position = tilePosition.ToVector2() * 16;
			var dimensions = new Vector2(288, 96); //Should correspond to one frame of the acacia tree top texture
			int offsetX = Framing.GetTileSafely(tilePosition).TileFrameY; //Palm trees use this for drawing a horizontal offset

			return new Rectangle((int)position.X - (int)(dimensions.X / 2) + 18 + offsetX, (int)position.Y - (int)dimensions.Y - 8, (int)dimensions.X, 8);
		}

		var lowerHitbox = Player.getRect() with { Height = Player.height / 2, Y = (int)Player.position.Y + Player.height / 2 }; //The lower half of Player's hitbox
		foreach (var p in AcaciaTreeSystem.Instance.treeTopPoints)
		{
			var platform = GetPlatform(p);
			if (lowerHitbox.Intersects(platform) && Player.velocity.Y >= 0 && !Player.controlDown)
			{
				StandOnPlatform(platform, AcaciaTreeSystem.GetAcaciaSway(p.X, p.Y));
				break;
			}
		}
	}

	private void StandOnPlatform(Rectangle platform, float rotation)
	{
		float strength = (Player.Center.X - platform.Center.X) / (platform.Width * .5f);

		Player.velocity.Y = 0;
		Player.position.Y = platform.Top + 10 + rotation * strength * 12 - Player.height;

		Player.fullRotationOrigin = new Vector2(Player.width / 2, Player.height);
		Player.fullRotation = rotation * .16f * Math.Abs(strength);
	}
}
