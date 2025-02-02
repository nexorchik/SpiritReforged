using SpiritReforged.Common.TileCommon;

namespace SpiritReforged.Content.Ocean.Tiles;

[DrawOrder(DrawOrderAttribute.Layer.NonSolid, DrawOrderAttribute.Layer.OverPlayers)]
internal class OceanKelp : ModTile
{
	private const int ClumpX = 92;

	private static Asset<Texture2D> Clump = null;

	private readonly static int[] ClumpOffsets = [0, -8, 8];

	public override void SetStaticDefaults()
	{
		Clump = ModContent.Request<Texture2D>(Texture + "_Clump");

		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileID.Sets.NotReallySolid[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInFullLiquid;
		TileObjectData.newTile.AnchorBottom = new Terraria.DataStructures.AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Sand];
		TileObjectData.newTile.AnchorAlternateTiles = [Type];
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(21, 92, 19));
		RegisterItemDrop(ModContent.ItemType<Items.Kelp>()); //Reiterate
		DustType = DustID.Grass;
		HitSound = SoundID.Grass;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		var drops = base.GetItemDrops(i, j);

		if (Main.rand.NextBool(100))
			drops = [new Item(ItemID.LimeKelp)];

		return drops;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile tile = Main.tile[i, j];
		Tile above = Main.tile[i, j - 1];
		Tile below = Main.tile[i, j + 1];

		if (!below.HasTile || below.TileType != TileID.Sand && below.TileType != Type)
		{
			WorldGen.KillTile(i, j, false);

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendTileSquare(-1, i, j);
			
			return false;
		}

		short frameX = GetGroupFrameX(i, j);

		tile.TileFrameX = frameX;
		int oldFrameY = tile.TileFrameY;

		if (Main.rand.NextBool(12) && above.HasTile && above.TileType == Type && CanPlaceClump(i, j))
		{
			tile.TileFrameX = ClumpX;
			tile.TileFrameY = (short)Main.rand.Next(4);
			return false;
		}

		SetFrameY(tile, above, below, Type, resetFrame);

		// Set to the same clump status as the old frame
		tile.TileFrameY += (short)(GetClumpNumber(oldFrameY) * 198);
		return false;
	}

	private bool CanPlaceClump(int i, int j)
	{
		int y = j;

		while (Main.tile[i, y].HasTile && Main.tile[i, y].TileType == Type && Main.tile[i, y].TileFrameX != ClumpX)
		{
			y++;
		}

		y--;

		return y - j > 2;
	}

	/// <summary>
	/// Gets the "group" of the kelp's framing.<br/>
	/// The spritesheet is split into two groups - A and B, left and right - giving much needed variety to the sprite.<br/>
	/// This is used for normal framing and for layering clumps.
	/// </summary>
	/// <returns>The frame X of the current kelp.</returns>
	private static short GetGroupFrameX(int i, int j)
	{
		bool aGroup = (i + j) % 2 == 0;
		short frameX = (short)(aGroup ? 2 : 48);
		return frameX;
	}

	/// <summary>
	/// Sets the given tile's frame Y based on the conditions around it.
	/// </summary>
	public static void SetFrameY(Tile tile, Tile above, Tile below, int type, bool resetFrame)
	{
		if (above.TileType != type) // Prioritize using the top texture if the tile above isn't kelp
		{
			if (tile.TileFrameY >= 36 && tile.TileFrameY <= 54)
				return;

			tile.TileFrameY = (short)(Main.rand.Next(2) * 18);
			return;
		}

		if (below.TileType != type) // If the tile above is kelp, use bottom texture
		{
			tile.TileFrameY = (short)((Main.rand.Next(2) + 9) * 18);
			return;
		}

		tile.TileFrameY = (short)((Main.rand.Next(5) + 4) * 18);
	}

	public override void RandomUpdate(int i, int j)
	{
		Tile tile = Main.tile[i, j];

		if (tile.TileFrameY <= 18 && Main.rand.NextBool(10))
		{
			tile.TileFrameY += 36;

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendTileSquare(-1, i, j);
		}
		else if (tile.TileFrameY > 18 && Main.rand.NextBool(60) && !Main.tile[i, j - 1].HasTile)
		{
			WorldGen.PlaceTile(i, j - 1, Type, true);

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendTileSquare(-1, i, j - 1);
		}

		if (Main.rand.NextBool(15))
		{
			bool canAddClump = Main.tile[i, j + 1].TileType != Type || GetClumpNumber(i, j + 1) != GetClumpNumber(i, j);

			if (canAddClump && tile.TileFrameY < 198 * 2)
			{
				tile.TileFrameY += 198;

				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendTileSquare(-1, i, j);
			}
		}
	}

	public static int GetClumpNumber(int i, int j) => GetClumpNumber(Main.tile[i, j]);
	public static int GetClumpNumber(Tile tile) => GetClumpNumber(tile.TileFrameY);
	public static int GetClumpNumber(int frameY) => (int)(frameY / 198f);

	public override void NearbyEffects(int i, int j, bool closer) //Dust effects
	{
		if (Main.rand.Next(1000) <= 1) //Spawns little bubbles
			Dust.NewDustPerfect(new Vector2(i * 16, j * 16) + new Vector2(2 + Main.rand.Next(12), Main.rand.Next(16)), 34, new Vector2(Main.rand.NextFloat(-0.08f, 0.08f), Main.rand.NextFloat(-0.2f, -0.02f)));
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];

		Texture2D tex = TextureAssets.Tile[Type].Value;
		int clumpAmount = GetClumpNumber(tile.TileFrameY) + 1;
		Rectangle frame = new(tile.TileFrameX, tile.TileFrameY % 198, 44, 16);
		Vector2 drawPos = new Vector2(i, j + 1) * 16 - Main.screenPosition + new Vector2(10, 0);

		for (int k = clumpAmount - 1; k >= 0; --k)
		{
			if (DrawOrderSystem.Order == DrawOrderAttribute.Layer.Default && k == 0)
				continue;
			else if (DrawOrderSystem.Order == DrawOrderAttribute.Layer.OverPlayers && k != 0)
				continue;

			bool useClump = (i + j) % 3 + j % 4 + i % 3 == 0; // Deterministic "random" for switching up the clump type
			int clump = k;
			Vector2 realPos = drawPos + new Vector2(ClumpOffsets[clump] + GetOffset(i, j), 0);

			if (useClump)
			{
				if (clump == 1)
					clump = 2;
				else if (clump == 2)
					clump = 1;
			}

			if (tile.TileFrameX == ClumpX)
				DrawClump(i, j, spriteBatch, clumpAmount, frame, realPos, clump);
			else
				DrawSingleKelp(i, j, spriteBatch, tex, clumpAmount, frame, realPos, clump);
		}

		return false;
	}

	private static void DrawClump(int i, int j, SpriteBatch spriteBatch, int clumpAmount, Rectangle frame, Vector2 drawPos, int clump)
	{
		Color color = Lighting.GetColor(i, j, Color.Lerp(Color.White, Color.Black, clump / (float)clumpAmount));
		frame = new Rectangle(GetGroupFrameX(i, j) == 48 ? 76 : 2, frame.Y / 18 * 34, 72, 32);

		spriteBatch.Draw(Clump.Value, drawPos, frame, color, 0f, new Vector2(36, 16), 1f, SpriteEffects.None, 0);
	}

	private static void DrawSingleKelp(int i, int j, SpriteBatch spriteBatch, Texture2D tex, int clumpAmount, Rectangle frame, Vector2 drawPos, int clump)
	{
		Color color = Lighting.GetColor(i, j, Color.Lerp(Color.White, Color.Black, clump / (float)clumpAmount));
		frame.X = GetGroupFrameX(i + clump, j);

		spriteBatch.Draw(tex, drawPos, frame, color, 0f, new Vector2(23, 16), 1f, SpriteEffects.None, 0);
	}

	public float GetOffset(int i, int j, float sOffset = 0f)
	{
		float sin = (float)Math.Sin((Main.GameUpdateCount + i * 24 + j * 19) * (0.04f * (!Lighting.NotRetro ? 0f : 1)) + sOffset) * 2.3f;
		int y = j;

		while (Main.tile[i, y].HasTile && Main.tile[i, y].TileType == Type)
			y++;

		if (y - j < 4)
			sin *= (y - j) / 4f;

		return sin;
	}
}

//public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) //Sets and randomizes tile frame
//{
//	var t = Framing.GetTileSafely(i, j); //this tile :)

//	int totalOffset = t.TileFrameX / ClumpFrameOffset;
//	int realFrameX = t.TileFrameX - ClumpFrameOffset * totalOffset; //Adjusted so its easy to read

//	if (realFrameX < 36 && t.TileFrameY < 108) //Used for adjusting stem/top; does not include grown top or leafy stems
//	{
//		if (!Framing.GetTileSafely(i, j - 1).HasTile) //If top
//			t.TileFrameX = (short)(18 + ClumpFrameOffset * totalOffset);
//		else //If stem
//			t.TileFrameX = (short)(0 + ClumpFrameOffset * totalOffset);
//		realFrameX = 0;
//	}

//	if (realFrameX == 0) //If stem
//		t.TileFrameY = (short)(Main.rand.Next(6) * 18); //Stem
//	else if (realFrameX == 18)
//	{
//		if (t.TileFrameY >= 108) //If grown top
//			t.TileFrameY = (short)(Main.rand.Next(4) * 18 + 108);
//		else //If ungrown top
//			t.TileFrameY = (short)(Main.rand.Next(6) * 18);
//	}
//	else //Leafy stem
//		t.TileFrameY = (short)(18 * Main.rand.Next(8));

//	if (t.TileFrameY is 152 and >= 108)
//		t.TileFrameY = (short)(Main.rand.Next(6) * 18);

//	var below = Framing.GetTileSafely(i, j + 1);
//	if (!below.HasTile || below.IsHalfBlock || below.TopSlope) //KILL ME if there's no ground below me
//		WorldGen.KillTile(i, j);

//	return false;
//}

//public override void RandomUpdate(int i, int j) //Used for growing and "growing"
//{
//	var t = Framing.GetTileSafely(i, j);

//	int totalOffset = t.TileFrameX / ClumpFrameOffset;
//	int realFrameX = t.TileFrameX - ClumpFrameOffset * totalOffset; //Adjusted so its easy to read

//	if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.rand.NextBool(4) && t.LiquidAmount > 155 && t.TileFrameX < 36 && t.TileFrameY < 108) //Grows the kelp
//	{
//		int height = 1;

//		while (Framing.GetTileSafely(i, j + height).HasTile && Framing.GetTileSafely(i, j + height).TileType == Type)
//			height++;

//		int maxHeight = Main.rand.Next(17, 23);
//		if (height < maxHeight && !Main.tile[i, j - 1].HasTile)
//		{
//			WorldGen.PlaceTile(i, j - 1, Type, true, false);
//			if (Main.rand.NextBool(12) && height == maxHeight - 1) //Flower top
//			{
//				Framing.GetTileSafely(i, j - 1).TileFrameX = 18;
//				Framing.GetTileSafely(i, j - 1).TileFrameY = 108;
//			}
//		}
//	}

//	if (realFrameX == 18 && t.TileFrameY < 54 && t.LiquidAmount < 155) //Sprouts top
//		t.TileFrameY = (short)(Main.rand.Next(2) * 18 + 54);

//	if (realFrameX == 0 && Main.rand.NextBool(3)) //"Places" side (just changes frame [we do a LOT of deception])
//	{
//		t.TileFrameY = (short)(18 * Main.rand.Next(8));
//		t.TileFrameX = (short)(44 + totalOffset * ClumpFrameOffset);
//	}

//	bool validGrowthBelow = Framing.GetTileSafely(i, j + 1).TileType != Type || Framing.GetTileSafely(i, j + 1).TileType == Type && Framing.GetTileSafely(i, j + 1).TileFrameX >= ClumpFrameOffset;
//	if (realFrameX == 0 && t.TileFrameX < ClumpFrameOffset * 2 && validGrowthBelow) //grows "clumps"
//	{
//		bool validBelow = Framing.GetTileSafely(i, j + 1).TileFrameX >= ClumpFrameOffset && Framing.GetTileSafely(i, j + 1).TileFrameX < ClumpFrameOffset * 2 && t.TileFrameX < ClumpFrameOffset;
//		if (Framing.GetTileSafely(i, j + 1).TileType != Type && t.TileFrameX < ClumpFrameOffset * 2) //Grows clump if above sand
//			t.TileFrameX += ClumpFrameOffset;
//		else if (validBelow) //grows clump 1
//			t.TileFrameX += ClumpFrameOffset;
//		else if (Framing.GetTileSafely(i, j + 1).TileFrameX >= ClumpFrameOffset * 2 && t.TileFrameX < ClumpFrameOffset * 2) //grows clump 2
//			t.TileFrameX += ClumpFrameOffset;
//	}
//}

//public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) //Drawing woo
//{
//	Tile t = Framing.GetTileSafely(i, j); //ME!
//	Texture2D tile = TextureAssets.Tile[Type].Value; //Associated texture - loaded automatically

//	int totalOffset = t.TileFrameX / ClumpFrameOffset; //Gets offset
//	int realFrameX = t.TileFrameX - ClumpFrameOffset * totalOffset; //Adjusted so its easy to read

//	float xOff = GetOffset(i, j, t.TileFrameX); //Sin offset.

//	var source = new Rectangle(t.TileFrameX, t.TileFrameY, 16, 16); //Source rectangle used for drawing
//	if (realFrameX == 44)
//		source = new Rectangle(t.TileFrameX, t.TileFrameY, 26, 16);

//	Vector2 drawPos = this.DrawPosition(i, j); //Draw position

//	bool[] hasClumps = [GetKelpTile(i, j - 1) >= ClumpFrameOffset, GetKelpTile(i, j - 1) >= ClumpFrameOffset * 2]; //Checks for if there's a grown clump above this clump

//	for (int v = totalOffset; v >= 0; --v)
//	{
//		Rectangle realSource = source;
//		Color col = Color.White; //Lighting colour
//		Vector2 realPos = drawPos;

//		if (v == 0)
//			xOff = GetOffset(i, j, realSource.X); //Grab offset properly
//		else if (v == 1)
//		{
//			realPos.X -= 8f + i % 2; //Plain visual offset

//			if (realSource.Y < 108) //"Randomzies" frame (it's consistent but we do a little more deception)
//			{
//				realSource.Y += 18;
//				if (realSource.Y >= 108)
//					realSource.Y -= 108;
//			}

//			col = new Color(169, 169, 169); //Makes it darker for depth

//			if (!hasClumps[0]) //Adjust frame so it's a kelp top
//			{
//				realSource.X = 18;
//				realSource.Width = 16;
//				if (realSource.Y >= 108)
//					realSource.Y -= 36;
//			}

//			xOff = GetOffset(i, j, realSource.X, -0.75f + i % 4 * 0.4f);
//		}
//		else if (v == 2) //Repeat lines 150-167 but slight offsets
//		{
//			realPos.X += 6f + i % 2;

//			if (realSource.Y < 108)
//			{
//				realSource.Y -= 18;
//				if (realSource.Y < 0)
//					realSource.Y += 108;
//			}

//			col = new Color(140, 140, 140);

//			if (!hasClumps[1]) //Adjust frame to be a kelp top
//			{
//				realSource.X = 18;
//				realSource.Width = 16;
//				if (realSource.Y >= 108)
//					realSource.Y -= 36;
//			}

//			xOff = GetOffset(i, j, realSource.X, -1.55f + i % 4 * 0.4f);
//		}

//		col = Lighting.GetColor(i, j, col);
//		spriteBatch.Draw(tile, realPos - new Vector2(xOff, 0), realSource, new Color(col.R, col.G, col.B, 255), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
//	}

//	return false; //don't draw the BORING, STUPID vanilla tile
//}

//public float GetOffset(int i, int j, int frameX, float sOffset = 0f)
//{
//	float sin = (float)Math.Sin((Main.GameUpdateCount + i * 24 + j * 19) * (0.04f * (!Lighting.NotRetro ? 0f : 1)) + sOffset) * 2.3f;
//	if (Framing.GetTileSafely(i, j + 1).TileType != Type) //Adjusts the sine wave offset to make it look nicer when closer to ground
//		sin *= 0.25f;
//	else if (Framing.GetTileSafely(i, j + 2).TileType != Type)
//		sin *= 0.5f;
//	else if (Framing.GetTileSafely(i, j + 3).TileType != Type)
//		sin *= 0.75f;

//	if (frameX > ClumpFrameOffset) 
//		frameX -= ClumpFrameOffset;

//	if (frameX > ClumpFrameOffset) 
//		frameX -= ClumpFrameOffset; //repeat twice to adjust properly

//	if (frameX == 44)
//		sin += 4; //Adjusts since the source is bigger here

//	return sin;
//}

//public int GetKelpTile(int i, int j)
//{
//	if (Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).TileType == Type)
//		return Framing.GetTileSafely(i, j).TileFrameX;
//	return -1;
//}