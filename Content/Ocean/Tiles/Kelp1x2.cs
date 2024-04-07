using SpiritReforged.Content.Ocean.Items;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Ocean.Tiles;

public class SmallKelpItem : ModItem
{
	public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.value = 0;
		Item.rare = ItemRarityID.Blue;
		Item.createTile = ModContent.TileType<Kelp1x2>();
		Item.maxStack = Item.CommonMaxStack;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<Kelp>(), 6);
		recipe.AddTile(TileID.TinkerersWorkbench);
		recipe.Register();
	}
}

public class Kelp1x2 : ModTile
{
	private static Texture2D glowmask;

	public Kelp1x2() => glowmask = ModContent.Request<Texture2D>("SpiritReforged/Content/Ocean/Tiles/Kelp1x2_Glow", AssetRequestMode.ImmediateLoad).Value;
	public override void Unload() => glowmask = null;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileCut[Type] = false;
		Main.tileLighted[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(0, 0);
		TileObjectData.newTile.CoordinateHeights = [16, 18];
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand, TileID.Crimsand, TileID.Ebonsand];
		TileObjectData.newTile.RandomStyleRange = 1;
		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;
		DustType = DustID.Grass;

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(24, 105, 25), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => spriteEffects = (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (Framing.GetTileSafely(i, j).TileFrameY == 0) 
		{
			r = 0.34f * 1.5f;
			g = 0.34f * 1.5f;
			b = 0;
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile t = Framing.GetTileSafely(i, j);
		if (t.TileFrameY == 0)
		{
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			spriteBatch.Draw(glowmask, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16), Color.LightYellow);
		}
	}
}
