namespace SpiritReforged.Content.Ocean.Items.Driftwood;

public class Driftwood1Item : ModItem
{
	public override string Texture => base.Texture.Replace("1Item", string.Empty);

	public override void SetStaticDefaults() => Main.RegisterItemAnimation(Type, new Terraria.DataStructures.DrawAnimationVertical(2, 3) { NotActuallyAnimating = true, Frame = 0 });

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Driftwood1Tile>());
		Item.width = 30;
		Item.height = 18;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D tex = TextureAssets.Item[Type].Value;
		var frame = Main.itemAnimations[Type].GetFrame(tex);

		spriteBatch.Draw(tex, Item.position - Main.screenPosition, frame, GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 10);
		recipe.Register();
	}
}

public class Driftwood1Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); //facing right will use the second texture style
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 54, 43), name);
		DustType = DustID.Stone;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}

public class Driftwood2Item : ModItem
{
	public override string Texture => base.Texture.Replace("2Item", string.Empty);

	public override void SetStaticDefaults() => Main.RegisterItemAnimation(Type, new Terraria.DataStructures.DrawAnimationVertical(2, 3) { NotActuallyAnimating = true, Frame = 1 });

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Driftwood2Tile>());
		Item.width = 30;
		Item.height = 18;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D tex = TextureAssets.Item[Type].Value;
		var frame = Main.itemAnimations[Type].GetFrame(tex);

		spriteBatch.Draw(tex, Item.position - Main.screenPosition, frame, GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 20);
		recipe.Register();
	}
}

public class Driftwood2Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 4;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); //facing right will use the second texture style
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 54, 43), name);
		DustType = DustID.Stone;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}

public class Driftwood3Item : ModItem
{
	public override string Texture => base.Texture.Replace("3Item", string.Empty);

	public override void SetStaticDefaults() => Main.RegisterItemAnimation(Type, new Terraria.DataStructures.DrawAnimationVertical(2, 3) { NotActuallyAnimating = true, Frame = 2 });

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Driftwood3Tile>());
		Item.width = 30;
		Item.height = 18;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		Texture2D tex = TextureAssets.Item[Type].Value;
		var frame = Main.itemAnimations[Type].GetFrame(tex);

		spriteBatch.Draw(tex, Item.position - Main.screenPosition, frame, GetAlpha(lightColor) ?? lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<DriftwoodTileItem>(), 25);
		recipe.Register();
	}
}

public class Driftwood3Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 4;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1); //facing right will use the second texture style
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 54, 43), name);
		DustType = DustID.Stone;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
}