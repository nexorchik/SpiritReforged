namespace SpiritReforged.Content.Savanna.Tiles.AcaciaTree;

/// <summary> Controls player interaction with platforms (<see cref="CustomPlatform"/>). </summary>
public class AcaciaTreePlayer : ModPlayer
{
	private bool wasOnPlatform;
	private float oldRotation;

	public override void PreUpdateMovement()
	{
		//The lower half of Player's hitbox
		Rectangle GetLowRect() => Player.getRect() with { Height = Player.height / 2, Y = (int)Player.position.Y + Player.height / 2 }; 
		bool onPlatform = false;

		foreach (var p in AcaciaTreeSystem.Instance.platforms)
		{
			if (GetLowRect().Intersects(p.GetRect()) && Player.velocity.Y >= 0 && !Player.controlDown)
			{
				//The server can't actually use this value because it's originally intended for client visuals
				float rotation = (!Main.dedServ) ? AcaciaTreeSystem.GetAcaciaSway((int)(p.origin.X / 16), (int)(p.origin.Y / 16)) : 0f;

				//Check the validity of this platform before using it. The client controls platform origin so don't check on server
				if (!Main.dedServ && Framing.GetTileSafely(p.origin).TileType != TileID.PalmTree)
				{
					AcaciaTreeSystem.Instance.platforms.Remove(p);

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						ModPacket packet = SpiritReforgedMod.Instance.GetPacket(Common.Misc.ReforgedMultiplayer.MessageType.SendPlatform, 3);
						packet.WriteVector2(p.center);
						packet.Write(p.width);
						packet.Write(true);
						packet.Send();
					}

					break;
				}

				StandOnPlatform(p, rotation);
				oldRotation = rotation;
				onPlatform = wasOnPlatform = true;
				break; //It would be redundant to check for other platforms when the player is on one
			}
		}

		if (!onPlatform && wasOnPlatform) //Reset rotation when the player just leaves a platform
		{
			Player.fullRotation = 0;
			wasOnPlatform = false;
		}
	}

	private void StandOnPlatform(CustomPlatform platform, float rotation = 0f)
	{
		float diff = rotation - oldRotation; //The difference in rotation from last tick, used to control how much the player displaces horizontally
		float strength = (Player.Center.X - platform.center.X) / (platform.width * .5f); //Scalar based on the player's distance from platform center
		const float disp = 10f; //How much the player is displaced by the previous factors

		Player.velocity.Y = 0;
		Player.position = new Vector2(Player.position.X + diff * disp, platform.GetRect().Top + 10 - Player.height + rotation * strength * disp);

		Player.fullRotation = rotation * .07f;
		Player.fullRotationOrigin = new Vector2(Player.width * .5f, Player.height);
	}
}
