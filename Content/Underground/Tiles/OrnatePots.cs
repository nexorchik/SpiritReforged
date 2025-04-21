using RubbleAutoloader;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.Tiles;

public class OrnatePots : PotTile
{
	public override Dictionary<string, int[]> TileStyles => new() { { string.Empty, [0, 1, 2] } };

	public override void AddRecord(int type, StyleDatabase.StyleGroup group)
	{
		var record = new TileRecord(group.name, type, group.styles);
		RecordHandler.Records.Add(record.AddRating(5).AddDescription(Language.GetText(TileRecord.DescKey + ".CoinPortal")));
	}

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		Main.tileOreFinderPriority[Type] = 575;
		DustType = DustID.Gold;
	}

	public override void AddMapData() => AddMapEntry(new Color(180, 180, 77), Language.GetText("Mods.SpiritReforged.Items.OrnatePotsItem.DisplayName"));

	public override bool KillSound(int i, int j, bool fail)
	{
		if (!fail && !Autoloader.IsRubble(Type))
		{
			var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);
			SoundEngine.PlaySound(SoundID.Shatter, pos);

			return false;
		}

		return true;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (!Autoloader.IsRubble(Type) && Main.netMode != NetmodeID.MultiplayerClient)
		{
			var source = new EntitySource_TileBreak(i, j);
			Projectile.NewProjectile(source, new Vector2(i, j).ToWorldCoordinates(16, 16), Vector2.UnitY * -4f, ProjectileID.CoinPortal, 0, 0);
		}

		base.KillMultiTile(i, j, frameX, frameY);
	}

	public override void DeathEffects(int i, int j, int frameX, int frameY)
	{
		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j) * 16;

		for (int g = 1; g < 4; g++)
		{
			int goreType = Mod.Find<ModGore>("PotGold" + g).Type;
			Gore.NewGore(source, position, Vector2.Zero, goreType);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (TileObjectData.IsTopLeft(i, j))
			GlowTileHandler.AddGlowPoint(new Rectangle(i, j, 32, 32), Color.Goldenrod * .5f, 200);

		return true;
	}
}