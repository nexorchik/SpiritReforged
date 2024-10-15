using Terraria.DataStructures;
using SpiritReforged.Content.Ocean.Boids;
using SpiritReforged.Common.Particle;
using SpiritReforged.Content.Particles;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;

public class OceanPendantTile : ModTile
{
	public static int ItemType => ModContent.ItemType<OceanPendant>();
	private static CircleBoid circleBoid;

	public override void Load() => BoidManager.OnAddBoids += AddCircleBoid;

	private void AddCircleBoid(int seed)
	{
		circleBoid = new CircleBoid(BoidManager.Lookup(seed - 1), 1, 12, spawnWeight: 0);
		BoidManager.boids.Add(circleBoid);
	}

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileSpelunker[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = [18];
		TileObjectData.addTile(Type);

		RegisterItemDrop(ItemType);
		AddMapEntry(new Color(133, 106, 56), ItemLoader.GetItem(ItemType).DisplayName);
		DustType = -1;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ItemType;
	}

	public override bool RightClick(int i, int j)
	{
		WorldGen.KillTile(i, j);

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j);

		return true;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var worldPos = new Vector2(i, j) * 16;
		circleBoid.Populate(worldPos, 15, 120, worldPos); //Populate our custom boid

		if (!Main.gamePaused && Main.rand.NextBool(9) && Main.LocalPlayer.Distance(worldPos) < 16 * 20)
			ParticleHandler.SpawnParticle(new GlowParticle(worldPos + new Vector2(8), -Vector2.UnitY * Main.rand.NextFloat(), Color.Goldenrod, Main.rand.NextFloat(.1f, .2f), 100, 1, delegate (Particle p)
			{
				p.Velocity = p.Velocity.RotatedByRandom(.1f);
				p.Velocity *= .98f;
			}));
	}

	public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) => Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);

	public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		var worldPos = new Vector2(i, j) * 16 + new Vector2(8);
		var drawPos = worldPos - Main.screenPosition + zero;

		float rotation = (float)Main.timeForVisualEffects * .01f;
		float scale = .1f;
		float distanceMult = Math.Clamp(1f - Main.LocalPlayer.Distance(worldPos) / (16 * 25), 0, 1);

		spriteBatch.Draw(AssetLoader.LoadedTextures["Star2"], drawPos, null, (Color.Yellow with { A = 0 }) * .5f * distanceMult,
			rotation, AssetLoader.LoadedTextures["Star2"].Size() / 2, scale, SpriteEffects.None, 0);

		spriteBatch.Draw(AssetLoader.LoadedTextures["Star"], drawPos, null, (Color.White with { A = 0 }) * distanceMult,
			rotation, AssetLoader.LoadedTextures["Star"].Size() / 2, scale, SpriteEffects.None, 0);
	}
}
