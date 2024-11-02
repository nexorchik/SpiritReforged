using SpiritReforged.Common.TileCommon;
using SpiritReforged.Content.Vanilla.Items.Food;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Utilities;

namespace SpiritReforged.Content.Savanna.Tiles;

public class RoastCampfire : ModTile
{
	private struct Unit
	{
		int i, j;
		public Unit Target(int i, int j)
		{
			(this.i, this.j) = (i, j);
			return this;
		}

		public readonly bool OnFire => Main.tile[i, j].TileFrameY < fullFrameSize;
		public readonly bool HasItem => Main.tile[i, j].TileFrameX < fullFrameSize;
		public readonly bool IsTop => Main.tile[i, j].TileFrameY % fullFrameSize <= 18;
	}

	private static Asset<Texture2D> glowTexture;
	private static Unit unit;
	private const int itemType = ItemID.Campfire;

	private const int fullFrameSize = 18 * 3;

	public override void Load()
	{
		glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
		unit = new Unit();
	}

	public override void SetStaticDefaults()
	{
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileWaterDeath[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.InteractibleByNPCs[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.DrawYOffset = 2;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(254, 121, 2), Language.GetText("ItemName.Campfire"));
		RegisterItemDrop(itemType);
		DustType = -1;
		AdjTiles = [TileID.Campfire];
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

	public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
	{
		width = 3;
		height = 2;
		extraY = 0;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (unit.Target(i, j).OnFire)
			Main.SceneMetrics.HasCampfire = true;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = (unit.Target(i, j).IsTop && unit.Target(i, j).HasItem) ? ModContent.ItemType<CookedMeat>() : itemType;
	}

	public override bool RightClick(int i, int j)
	{
		if (unit.Target(i, j).IsTop && unit.Target(i, j).HasItem)
		{
			TileExtensions.GetTopLeft(ref i, ref j);

			for (int x = i; x < i + 3; x++)
				for (int y = j; y < j + 3; y++)
					Main.tile[x, y].TileFrameX += fullFrameSize;

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 3, 3);

			int item = Item.NewItem(null, new Vector2(i + 1, j) * 16, ModContent.ItemType<CookedMeat>());
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncItem, number: item);
		}
		else
		{
			SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
			HitWire(i, j);
		}

		return true;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		var drops = base.GetItemDrops(i, j);

		if (unit.Target(i, j).HasItem)
			drops = drops.Concat([new Item(ModContent.ItemType<CookedMeat>())]);

		return drops;
	}

	public override void HitWire(int i, int j)
	{
		TileExtensions.GetTopLeft(ref i, ref j);
		short frameAdjustment = (short)(unit.Target(i, j).OnFire ? fullFrameSize : -fullFrameSize);

		for (int x = i; x < i + 3; x++)
		{
			for (int y = j; y < j + 3; y++)
			{
				Main.tile[x, y].TileFrameY += frameAdjustment;

				if (Wiring.running)
					Wiring.SkipWire(x, y);
			}
		}

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j, 3, 3);
	}

	public override void AnimateTile(ref int frame, ref int frameCounter)
	{
		if (++frameCounter >= 4)
		{
			frameCounter = 0;
			frame = ++frame % 8;
		}
	}

	public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
	{
		if (unit.Target(i, j).OnFire)
			frameYOffset = Main.tileFrame[type] * fullFrameSize;
		else
			frameYOffset = fullFrameSize * 7;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		if (unit.Target(i, j).OnFire)
		{
			float pulse = Main.rand.Next(28, 42) * .005f + (270 - Main.mouseTextColor) / 700f;
			(r, g, b) = (.9f + pulse, .4f + pulse, .1f + pulse);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];

		Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
		int addFrameX = 0;
		int addFrameY = 0;
		TileLoader.SetAnimationFrame(Type, i, j, ref addFrameX, ref addFrameY); // calculates the animation offsets
		var source = new Rectangle(tile.TileFrameX % fullFrameSize, tile.TileFrameY + addFrameY, 16, 16);
		var position = new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(0, 2);

		spriteBatch.Draw(TextureAssets.Tile[Type].Value, position, source, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);

		if (Main.InSmartCursorHighlightArea(i, j, out bool actuallySelected))
			spriteBatch.Draw(TextureAssets.HighlightMask[Type].Value, position, source, actuallySelected ? Color.Yellow : Color.Gray, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

		return false;
	}

	private static void DustEffects(int i, int j)
	{
		if (Main.gamePaused || !Main.instance.IsActive)
			return;

		if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) == 0)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY == 18 && Main.rand.NextBool(3) && (Main.drawToScreen && Main.rand.NextBool(4) || !Main.drawToScreen))
			{
				var dust = Dust.NewDustDirect(new Vector2(i * 16 + 2, j * 16 - 4), 4, 8, DustID.Smoke, 0f, 0f, 100);
				if (tile.TileFrameX == 0)
					dust.position.X += Main.rand.Next(8);

				if (tile.TileFrameX == 36)
					dust.position.X -= Main.rand.Next(8);

				dust.alpha += Main.rand.Next(100);
				dust.velocity *= 0.2f;
				dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
				dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
			}
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (!TileDrawing.IsVisible(tile))
			return;

		DustEffects(i, j);
		if (unit.Target(i, j).HasItem)
		{
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			int frameY = tile.TileFrameY - (!unit.Target(i, j).OnFire ? fullFrameSize : 0) + fullFrameSize * 9;
			var drawRectangle = new Rectangle(tile.TileFrameX % fullFrameSize, frameY, 16, 16);

			spriteBatch.Draw(TextureAssets.Tile[Type].Value, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(0, 2),
				drawRectangle, Lighting.GetColor(i, j), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
		}

		if (unit.Target(i, j).OnFire)
		{
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			int addFrameX = 0;
			int addFrameY = 0;
			TileLoader.SetAnimationFrame(Type, i, j, ref addFrameX, ref addFrameY); // calculates the animation offsets
			var drawRectangle = new Rectangle(tile.TileFrameX % fullFrameSize, tile.TileFrameY + addFrameY, 16, 16);

			spriteBatch.Draw(glowTexture.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(0, 2),
				drawRectangle, Color.White with { A = 0 }, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}
}