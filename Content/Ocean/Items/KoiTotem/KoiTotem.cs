using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Content.Ocean.Items.KoiTotem;

public class KoiTotem : FloatingItem
{
	public override float SpawnWeight => 0.005f;
	public override float Weight => base.Weight * 0.9f;
	public override float Bouyancy => base.Bouyancy * 1.07f;
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Koi Totem");
		// Tooltip.SetDefault("Increases fishing skill when worn or when placed nearby\nTotem occasionally spits out the bait that was used for reusability\n");
	}

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 36;
		Item.value = Item.sellPrice(gold: 1);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		Item.consumable = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 10;
		Item.useAnimation = 15;
		Item.useTurn = true;
		Item.createTile = ModContent.TileType<KoiTotem_Tile>();
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<OceanPlayer>().KoiTotem = true;
		player.AddBuff(ModContent.BuffType<KoiTotemBuff>(), 2);
	}
	public override bool AllowPrefix(int pre) => false;
}

public class KoiTotem_Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.newTile.StyleWrapLimit = 2; 
		TileObjectData.newTile.StyleMultiplier = 2; 
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft; 
		TileObjectData.addAlternate(1); 
		TileObjectData.addTile(Type);

		DustType = DustID.Ash;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(107, 90, 64), name);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Player player = Main.LocalPlayer;
		if (closer)
		{
			player.GetModPlayer<OceanPlayer>().KoiTotem = true;
			Main.LocalPlayer.AddBuff(ModContent.BuffType<KoiTotemBuff>(), 12);
		}
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}
