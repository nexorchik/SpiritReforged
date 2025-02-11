using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Multiplayer;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Ocean.Items;
using SpiritReforged.Content.Particles;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Hydrothermal.Tiles;

public class HydrothermalVent : ModTile
{
	/// <summary> Cooldowns for all <see cref="HydrothermalVent"/> tiles in the world. Never read on multiplayer clients. </summary>
	private static readonly Dictionary<Point16, int> cooldowns = [];
	private const int cooldownMax = (int)(Main.dayLength / 2);
	internal const int eruptDuration = 600;

	/// <summary> Precise texture top positions for all tile styles, used for visuals. </summary>
	private static readonly Point[] tops = [new Point(16, 16), new Point(16, 16), new Point(16, 24), new Point(12, 4), new Point(20, 4), new Point(16, 16), new Point(16, 16), new Point(16, 16)];

	public override void Load() => On_Wiring.UpdateMech += UpdateCooldowns;

	private void UpdateCooldowns(On_Wiring.orig_UpdateMech orig)
	{
		orig();

		foreach (var entry in cooldowns)
		{
			var coords = entry.Key;

			if (!IsValid(coords.X, coords.Y))
			{
				cooldowns.Remove(coords);
				break;
			}

			if (cooldowns[coords] > 0)
				cooldowns[coords]--;
		}
	}

	/// <summary> Checks whether the given position is valid for a <see cref="cooldowns"/> entry to exist. </summary>
	/// <param name="i"> The X coordinate. </param>
	/// <param name="j"> The Y Coordinate.</param>
	/// <returns> Whether the given position is valid. </returns>
	private bool IsValid(int i, int j) => TileObjectData.IsTopLeft(i, j) && Framing.GetTileSafely(i, j).TileType == Type;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileSpelunker[Type] = true;
		Main.tileLighted[Type] = true;

		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
		TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
		TileObjectData.newTile.Origin = new(0, 3);
		TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<Gravel>(), ModContent.TileType<Magmastone>(), TileID.Sand];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = 8;
		TileObjectData.addTile(Type);

		DustType = DustID.Stone;
		MinPick = 50;
		AddMapEntry(new Color(64, 54, 66), CreateMapEntryName());
	}

	public override void NearbyEffects(int i, int j, bool closer) //Client effects
	{
		if (!TileObjectData.IsTopLeft(i, j) || Main.gamePaused)
			return;

		var t = Framing.GetTileSafely(i, j);
		int fullWidth = TileObjectData.GetTileData(t).CoordinateFullWidth;
		var position = new Vector2(i, j) * 16 + tops[t.TileFrameX / fullWidth].ToVector2();

		if (Main.rand.NextBool(5)) //Passive smoke effects
		{
			var velocity = new Vector2(0, -Main.rand.NextFloat(2f, 2.5f));

			ParticleHandler.SpawnParticle(new DissipatingSmoke(position, velocity,
				new Color(40, 40, 50), Color.SlateGray, Main.rand.NextFloat(.005f, .06f), 150)
			{ squash = true });
		}

		if (Main.rand.NextBool()) //Passive ash effects
		{
			float range = Main.rand.NextFloat();
			var velocity = new Vector2(0, -Main.rand.NextFloat(range * 8f)).RotatedByRandom((1f - range) * 1.5f);

			var dust = Dust.NewDustPerfect(position, DustID.Ash, velocity, Alpha: 180);
			dust.noGravity = true;
		}

		BubbleSoundPlayer.StartSound(new Vector2(i, j) * 16);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0)
		{
			float pulse = Main.rand.Next(28, 42) * .001f;
			var col = Color.Orange.ToVector3() / 3.5f;

			(r, g, b) = (col.X + pulse, col.Y + pulse, col.Z + pulse);
		}
	}

	public override void MouseOver(int i, int j)
	{
		var player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<MineralSlag>();
	}

	public override void RandomUpdate(int i, int j) => TryErupt(i, j);

	private bool TryErupt(int i, int j)
	{
		TileExtensions.GetTopLeft(ref i, ref j);

		var pt = new Point16(i, j);
		if (IsValid(i, j) && !cooldowns.ContainsKey(pt))
			cooldowns.Add(pt, cooldownMax); //Initialize cooldown counters on the server/singleplayer

		if (cooldowns[pt] == 0 && WorldGen.PlayerLOS(i, j))
		{
			Erupt(i, j);
			cooldowns[pt] = cooldownMax;

			if (Main.netMode != NetmodeID.SinglePlayer) //Sync vent eruption in multiplayer
				new EruptionData(new Point16(i, j)).Send();

			return true;
		}

		return false;
	}

	public static void Erupt(int i, int j)
	{
		var t = Framing.GetTileSafely(i, j);
		int fullWidth = TileObjectData.GetTileData(t).CoordinateFullWidth;
		var position = new Vector2(i, j) * 16 + tops[t.TileFrameX / fullWidth].ToVector2();

		if (Main.netMode != NetmodeID.MultiplayerClient)
			Projectile.NewProjectile(new EntitySource_Wiring(i, j), position, Vector2.UnitY * -4f, ModContent.ProjectileType<HydrothermalVentPlume>(), 5, 0f);

		if (!Main.dedServ)
		{
			var player = Main.LocalPlayer;

			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(position, ModContent.DustType<Dusts.BoneDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));
			for (int k = 0; k <= 20; k++)
				Dust.NewDustPerfect(position, ModContent.DustType<Dusts.FireClubDust>(), new Vector2(0, 6).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1));

			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Tile/StoneCrack" + Main.rand.Next(1, 3)) { PitchVariance = .6f }, position);
			SoundEngine.PlaySound(SoundID.Drown with { Pitch = -.5f, PitchVariance = .25f, Volume = 1.5f }, position);

			ParticleHandler.SpawnParticle(new TexturedPulseCircle(position, Color.Yellow, 0.75f, 200, 20, "supPerlin",
				new Vector2(4, 0.75f), EaseFunction.EaseCubicOut).WithSkew(0.75f, MathHelper.Pi - MathHelper.PiOver2));

			ParticleHandler.SpawnParticle(new TexturedPulseCircle(position, Color.Red, 0.75f, 200, 20, "supPerlin",
				new Vector2(4, 0.75f), EaseFunction.EaseCubicOut).WithSkew(0.75f, MathHelper.Pi - MathHelper.PiOver2));

			for (int x = 0; x < 5; x++) //Large initial smoke plume
				ParticleHandler.SpawnParticle(new DissipatingSmoke(position + Main.rand.NextVector2Unit() * 25f, -Vector2.UnitY,
					new Color(40, 40, 50), Color.Black, Main.rand.NextFloat(.05f, .3f), 150));

			if (Collision.WetCollision(player.position, player.width, player.height))
				player.SimpleShakeScreen(2, 3, 90, 16 * 10);

			Magmastone.AddGlowPoint(i, j);
		}
	}
}

internal class EruptionData : PacketData
{
	private readonly Point16 _point;

	public EruptionData() { }
	public EruptionData(Point16 point) => _point = point;

	public override void OnReceive(BinaryReader reader, int whoAmI)
	{
		var point = reader.ReadPoint16();

		if (Main.netMode == NetmodeID.Server) //If received by the server, send to all clients
			new EruptionData(point).Send(ignoreClient: whoAmI);

		HydrothermalVent.Erupt(point.X, point.Y);
	}

	public override void OnSend(ModPacket modPacket) => modPacket.WritePoint16(_point);
}
