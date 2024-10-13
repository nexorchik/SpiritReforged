using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.Trail_Components;
using System.IO;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Hydrothermal;

internal class MineralPickup : ModProjectile, ITrailProjectile
{
	private int itemType;
	private const int timeLeftMax = 60 * 60 * 3; //3 minutes

	private static Asset<Texture2D> outlineTexture;

	public override void Load() => outlineTexture = ModContent.Request<Texture2D>(Texture + "_Outline");

	public override void SetStaticDefaults() => Main.projFrames[Type] = 3;

	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(16);
		Projectile.tileCollide = false;
		Projectile.hide = true;
		Projectile.timeLeft = timeLeftMax;
	}

	public void DoTrailCreation(TrailManager tm)
	{
		tm.CreateTrail(Projectile, new LightColorTrail(Color.Red * .5f, Color.Transparent), new RoundCap(), new DefaultTrailPosition(), 12, 25);
		tm.CreateTrail(Projectile, new LightColorTrail(Color.Orange * .5f, Color.Transparent), new RoundCap(), new DefaultTrailPosition(), 8, 25);
	}

	public override void AI()
	{
		if (Projectile.timeLeft == timeLeftMax) //On-spawn effects
			Projectile.scale = 0; //Don't do this is SetDefaults because it directly affects projectile dimensions

		if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
		{
			if (Projectile.velocity != Vector2.Zero)
				SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			Projectile.velocity = Vector2.Zero;
			Projectile.scale = 1f;
		}
		else
		{
			Projectile.velocity.Y += .25f;
			Projectile.velocity.X *= .99f;

			Projectile.rotation += Projectile.velocity.X * .05f;
			if (Main.rand.NextBool(5))
			{
				var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 0, default, Main.rand.NextFloat(.5f, 2f));
				dust.noGravity = true;
				dust.velocity = -Projectile.velocity * Main.rand.NextFloat(.5f);
			}
		}

		Projectile.scale = MathHelper.Min(Projectile.scale + .02f, 1f);

		if (Projectile.getRect().Contains(Main.MouseWorld.ToPoint())) //Local client cursor logic
		{
			var player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = itemType;

			Projectile.hide = false;
			if (Main.mouseRight)
			{
				if (itemType != ItemID.None)
				{
					int newItem = Item.NewItem(null, Projectile.Center, new Item(itemType));
					if (Main.netMode != NetmodeID.SinglePlayer)
						NetMessage.SendData(MessageID.SyncItem, number: newItem);
				}

				Projectile.Kill();
				Projectile.netUpdate = true;
			}
		}
		else
			Projectile.hide = true;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/NPCDeath/Squish") { Pitch = -.5f, PitchVariance = .5f }, Projectile.Center);
		for (int i = 0; i < 10; i++)
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Asphalt, Main.rand.NextFloat(-.5f, .5f), Main.rand.NextFloat(-.5f, .5f));
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Rectangle Source(Texture2D texture) => texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);

		var color = Color.Yellow * (1f - Main.MouseWorld.Distance(Projectile.Center) / 50f); //Draw outline
		Main.EntitySpriteDraw(AssetLoader.LoadedTextures["Bloom"], Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null,
			(color with { A = 0 }) * .5f, Projectile.rotation, AssetLoader.LoadedTextures["Bloom"].Size() / 2, Projectile.scale * .25f, SpriteEffects.None);

		Main.EntitySpriteDraw(outlineTexture.Value, Projectile.Center - Main.screenPosition, Source(outlineTexture.Value),
			color, Projectile.rotation, Source(outlineTexture.Value).Size() / 2, Projectile.scale, SpriteEffects.None);

		var texture = TextureAssets.Projectile[Type].Value; //Draw normal
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), Source(texture), 
			lightColor, Projectile.rotation, Source(texture).Size() / 2, Projectile.scale, SpriteEffects.None);

		if (Projectile.timeLeft > timeLeftMax - 60 * 5) //Draw glow
		{
			float duration = 60f * 5;
			float intensity = (Projectile.timeLeft - (timeLeftMax - duration)) / duration;

			for (int i = 0; i < 3; i++)
				Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), Source(texture),
					Color.Orange with { A = 0 } * intensity, Projectile.rotation, Source(texture).Size() / 2, Projectile.scale + .1f * i * intensity, SpriteEffects.None);
		}

		return false;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		=> behindNPCsAndTiles.Add(index);

	public override void SendExtraAI(BinaryWriter writer) => writer.Write7BitEncodedInt(itemType);

	public override void ReceiveExtraAI(BinaryReader reader) => itemType = reader.Read7BitEncodedInt();

	public static void SpawnItemPickup(int itemType, Projectile projectile)
	{
		if (projectile.ModProjectile is MineralPickup mPickup)
		{
			mPickup.itemType = itemType;
			projectile.frame = Main.rand.Next(Main.projFrames[projectile.type]);
			projectile.netUpdate = true;
		}
	}
}
