using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.TileSway;
using System.IO;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Forest.Botanist.Tiles;

public class Scarecrow : ModTile, IAutoloadTileItem, ISwayInWind
{
	private static bool IsTop(int i, int j, out ScarecrowTileEntity entity)
	{
		entity = ScarecrowTileEntity.GetMe(i, j);
		return Framing.GetTileSafely(i, j).TileFrameY == 0 && entity is not null;
	}

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileFrameImportant[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateWidth = 46;
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 22];
		TileObjectData.newTile.DrawYOffset = -4;
		TileObjectData.newTile.Origin = new(0, 2);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 1, 0);
		var entity = ModContent.GetInstance<ScarecrowTileEntity>();
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(21, 92, 19));
		DustType = DustID.Hay;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 3;

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => ModContent.GetInstance<ScarecrowTileEntity>().Kill(i, j);

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		var drops = base.GetItemDrops(i, j);

		var entity = ScarecrowTileEntity.GetMe(i, j);
		if (entity is not null && entity.Hat is not null)
			drops = drops.Concat([entity.Hat.Clone()]);

		return drops;
	}

	public override bool RightClick(int i, int j)
	{
		if (!IsTop(i, j, out var entity))
			return false;

		entity.OnInteract(Main.LocalPlayer);
		return true;
	}

	public override void MouseOver(int i, int j)
	{
		if (!IsTop(i, j, out var entity))
			return;

		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = (entity.Hat is null) ? Mod.Find<ModItem>(Name + "Item").Type : entity.Hat.type;
	}

	public void DrawInWind(int i, int j, SpriteBatch spriteBatch, Vector2 offset, float rotation, Vector2 origin)
	{
		var tile = Framing.GetTileSafely(i, j);
		var data = TileObjectData.GetTileData(tile);
		var dataOffset = new Vector2(data.DrawXOffset - 15, data.DrawYOffset);
		var drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + dataOffset;
		var source = new Rectangle(tile.TileFrameX, tile.TileFrameY, data.CoordinateWidth, data.CoordinateHeights[tile.TileFrameY / 18]);

		spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawPos + offset, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0f);

		if (tile.TileFrameY == 0 && tile.TileFrameX == 0)
		{
			var entity = ScarecrowTileEntity.GetMe(i, j);
			if (entity is not null && entity.Hat is not null)
			{
				var hatPos = new Vector2(i + rotation * .75f, j) * 16 + dataOffset + offset + new Vector2(2, -12);
				entity.DrawHat(hatPos, origin, spriteBatch, rotation);
			}
		}
	}

	public void ModifyRotation(int i, int j, ref float rotation) => rotation *= .5f;

	public float SetWindSway(Point16 topLeft)
	{
		var data = TileObjectData.GetTileData(Framing.GetTileSafely(topLeft));
		float rotation = Main.instance.TilesRenderer.GetWindCycle(topLeft.X, topLeft.Y, TileSwaySystem.Instance.TreeWindCounter);

		if (!WorldGen.InAPlaceWithWind(topLeft.X, topLeft.Y, data.Width, data.Height))
			rotation = 0f;

		return rotation + TileSwayHelper.GetHighestWindGridPushComplex(topLeft.X, topLeft.Y, data.Width, data.Height, 20, 3f, 1, true);
	}
}

public class ScarecrowTileEntity : ModTileEntity
{
	public Item Hat { get; private set; } = null;

	public static ScarecrowTileEntity GetMe(int i, int j)
	{
		TileExtensions.GetTopLeft(ref i, ref j);
		if (ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ScarecrowTileEntity sgaregrow)
			return sgaregrow;

		return null;
	}

	public void OnInteract(Player player)
	{
		var item = player.HeldItem;

		bool TryPlaceHat()
		{
			if (item.headSlot > -1)
			{
				Hat = item.Clone();
				item.TurnToAir();
				return true;
			}

			return false;
		}

		if (Hat is null)
		{
			if (TryPlaceHat())
				NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
		}
		else
		{
			int id = Item.NewItem(player.GetSource_TileInteraction(Position.X, Position.Y), Position.ToVector2() * 16, Hat);
			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendData(MessageID.SyncItem, number: id, number2: 1f);

			Hat = null;
			TryPlaceHat();

			NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);

			for (int i = 0; i < 10; i++)
			{
				var dust = Dust.NewDustDirect(Position.ToVector2() * 16 - new Vector2(0, 8), 16, 16, DustID.Smoke, Alpha: 150, Scale: Main.rand.NextFloat(.5f, 1.5f));
				dust.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(.25f);
			}
		}
	}

	public void DrawHat(Vector2 worldPos, Vector2 origin, SpriteBatch spriteBatch, float rotation = 0)
	{
		if (Hat.headSlot < 0)
			return;

		var texture = TextureAssets.ArmorHead[Hat.headSlot].Value;
		var source = texture.Frame(1, 20);
		var shadowColor = Lighting.GetColor((worldPos / 16).ToPoint()).MultiplyRGB(Color.Black) * .3f;

		//Draw a dropshadow
		spriteBatch.Draw(texture, worldPos - Main.screenPosition + new Vector2(0, 2), source, shadowColor, rotation, origin, 1, SpriteEffects.None, 0);
		//Draw the hat
		spriteBatch.Draw(texture, worldPos - Main.screenPosition, source, Lighting.GetColor((worldPos / 16).ToPoint()),
			rotation, origin, 1, SpriteEffects.None, 0);
	}

	public override bool IsTileValidForEntity(int x, int y)
	{
		Tile tile = Framing.GetTileSafely(x, y);
		return tile.HasTile && tile.TileType == ModContent.TileType<Scarecrow>();
	}

	public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
	{
		var tile = Framing.GetTileSafely(i, j);
		(i, j) = (i - tile.TileFrameX % TileObjectData.GetTileData(tile).CoordinateFullWidth / 18, 
			j - tile.TileFrameY % TileObjectData.GetTileData(tile).CoordinateFullHeight / 18);

		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			NetMessage.SendTileSquare(Main.myPlayer, i, j);
			NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
			return -1;
		}

		return Place(i, j);
	}

	public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);

	public override void NetSend(BinaryWriter writer) => ItemIO.Send(Hat, writer);

	public override void NetReceive(BinaryReader reader) => Hat = ItemIO.Receive(reader);

	public override void SaveData(TagCompound tag)
	{
		if (Hat != null)
			tag[nameof(Hat)] = Hat;
	}

	public override void LoadData(TagCompound tag) => Hat = tag.Get<Item>(nameof(Hat));
}
