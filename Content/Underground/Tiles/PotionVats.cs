using RubbleAutoloader;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.Audio;
using Terraria.DataStructures;
using SpiritReforged.Common.Misc;
using SpiritReforged.Content.Forest.Cloud.Items;

namespace SpiritReforged.Content.Underground.Tiles;

[AutoloadGlowmask("200,200,200")]
public class PotionVats : PotTile, ICutAttempt
{
	private static Asset<Texture2D> FluidTexture;

	public override Dictionary<string, int[]> TileStyles => new()
	{
		{ "Antique", [0, 1, 2, 3] },
		{ "Cloning", [4, 5, 6, 7] }
	};

	public override void AddRecord(int type, StyleDatabase.StyleGroup group) => RecordHandler.Records.Add(new TileRecord(group.name, type, group.styles).AddRating(4));
	public override void AddObjectData()
	{
		Main.tileCut[Type] = !Autoloader.IsRubble(Type);

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
		TileObjectData.newTile.Origin = new(1, 4);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.StyleWrapLimit = 4;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		DustType = DustID.Glass;
		FluidTexture = ModContent.Request<Texture2D>(Texture + "_Fluid");
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (effectOnly || Autoloader.IsRubble(Type))
			return;

		fail = AdjustFrame(i, j);
	}

	public bool OnCutAttempt(int i, int j)
	{
		bool fail = AdjustFrame(i, j);

		var cache = Main.tile[i, j];
		WorldGen.KillTile_MakeTileDust(i, j, cache);
		WorldGen.KillTile_PlaySounds(i, j, true, cache);

		return !fail;
	}

	public override bool KillSound(int i, int j, bool fail)
	{
		if (Autoloader.IsRubble(Type))
			return true;

		var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);

		if (!fail)
		{
			SoundEngine.PlaySound(SoundID.Shatter with { Pitch = .5f }, pos);
			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Tile/PotBreak") with { Volume = .16f, Pitch = .8f, }, pos);
		}
		else
		{
			int phase = Main.tile[i, j].TileFrameX / 54;
			SoundEngine.PlaySound(SoundID.Shatter with { Pitch = phase / 4f }, pos);
		}

		return false;
	}

	private static bool AdjustFrame(int i, int j)
	{
		const int fullWidth = 54;

		TileExtensions.GetTopLeft(ref i, ref j);

		if (Main.tile[i, j].TileFrameX > fullWidth * 2)
			return false; //Frame has already been adjusted to capacity

		for (int x = i; x < i + 3; x++)
		{
			for (int y = j; y < j + 5; y++)
			{
				var t = Main.tile[x, y];
				t.TileFrameX += fullWidth;
			}
		}

		return true;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var texture = FluidTexture.Value;
		var position = new Vector2(i, j) * 16 - Main.screenPosition + TileExtensions.TileOffset + new Vector2(0, 2);

		var t = Main.tile[i, j];
		var frame = new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16);

		var color = Lighting.GetColor(i, j).MultiplyRGBA(Main.DiscoColor.Additive(150));
		spriteBatch.Draw(texture, position, frame, color);

		return true;
	}
}

public class VatSlot : SingleSlotEntity
{
	public static readonly Dictionary<int, Color> BrewColor = new()
	{
		{ ItemID.GravitationPotion, Color.Purple },
		{ ItemID.FeatherfallPotion, Color.White },
		{ ItemID.BattlePotion, Color.White },
		{ ItemID.CalmingPotion, new Color(102, 101, 201) },
		{ ItemID.EndurancePotion, Color.White },
		{ ItemID.TrapsightPotion, Color.White },
		{ ItemID.HunterPotion, Color.White },
		{ ItemID.ShinePotion, Color.White },
		{ ItemID.MiningPotion, Color.White },
		{ ItemID.SpelunkerPotion, Color.Goldenrod },
		{ ItemID.SwiftnessPotion, Color.LightSeaGreen },
		{ ItemID.WrathPotion, Color.White },
		{ ItemID.ObsidianSkinPotion, Color.White },
		{ ModContent.ItemType<DoubleJumpPotion>(), Color.White },
		{ ItemID.LuckPotion, Color.White },
		{ ItemID.IronskinPotion, Color.White },
		{ ItemID.LifeforcePotion, new Color(250, 64, 188) }
	};

	public override bool CanAddItem(Item item) => false;

	public override bool IsTileValidForEntity(int x, int y)
	{
		var t = Framing.GetTileSafely(x, y);
		return t.HasTile && t.TileType == ModContent.TileType<SilverFoodPlatter>() && TileObjectData.IsTopLeft(x, y);
	}
}