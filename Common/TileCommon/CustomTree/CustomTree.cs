using SpiritReforged.Common.TileCommon.TileSway;
using SpiritReforged.Common.WorldGeneration;
using SpiritReforged.Common.WorldGeneration.Noise;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Metadata;

namespace SpiritReforged.Common.TileCommon.CustomTree;

/// <summary> Follows palm tree logic by default. </summary>
public abstract class CustomTree : ModTile
{
	/// <summary> Controls growth height without the need to override <see cref="GenerateTree"/>. </summary>
	public virtual int TreeHeight => WorldGen.genRand.Next(10, 21);

	protected const int FrameSize = 22;
	internal static readonly FastNoiseLite noise = new();

	protected readonly HashSet<Point16> drawPoints = [];
	private readonly HashSet<Point16> treeShakes = [];

	// Textures are set as lookups, keyed by type - this means we can have one static instance (bypassing instancing issues) while keeping data easy to access
	internal readonly static Dictionary<int, Asset<Texture2D>> branchesTextureByType = [];
	internal readonly static Dictionary<int, Asset<Texture2D>> topsTextureByType = [];

	public Asset<Texture2D> TopTexture => topsTextureByType[Type];
	public Asset<Texture2D> BranchTexture => branchesTextureByType[Type];

	public override void Load()
	{
		if (Main.dedServ)
			return;

		On_TileDrawing.DrawTrees += DrawAllFoliage;
		On_TileDrawing.PreDrawTiles += ResetPoints;
	}

	private void DrawAllFoliage(On_TileDrawing.orig_DrawTrees orig, TileDrawing self)
	{
		orig(self);

		foreach (Point16 p in drawPoints)
		{
			if (Main.tile[p.X, p.Y].TileType == Type) //Points aren't cleared before this type becomes invalidated after PreDraw, so double check
				DrawTreeFoliage(p.X, p.Y, Main.spriteBatch);
		}
	}

	private void ResetPoints(On_TileDrawing.orig_PreDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
	{
		orig(self, solidLayer, forRenderTargets, intoRenderTargets);

		if ((intoRenderTargets || Lighting.UpdateEveryFrame) && !solidLayer)
			drawPoints.Clear();
	}

	public override void SetStaticDefaults()
	{
		if (!Main.dedServ)
		{
			if (ModContent.RequestIfExists(Texture + "_Tops", out Asset<Texture2D> tops))
				topsTextureByType.Add(Type, tops);

			if (ModContent.RequestIfExists(Texture + "_Branches", out Asset<Texture2D> branches))
				branchesTextureByType.Add(Type, branches);

			noise.SetFrequency(2f);
		}

		Main.tileSolid[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileAxe[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateWidth = FrameSize - 2;
		TileObjectData.newTile.CoordinateHeights = [FrameSize - 2];
		TileObjectData.newTile.RandomStyleRange = 3;
		TileObjectData.newTile.StyleMultiplier = 3;
		TileObjectData.newTile.StyleWrapLimit = 3 * 4;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, 1, 0);
		TileObjectData.newTile.AnchorValidTiles = [TileID.Grass];
		TileObjectData.newTile.AnchorAlternateTiles = [Type];

		//TileID.Sets.IsATreeTrunk[Type] = true; //If true, allows torches to be placed on trunks regardless of tileNoAttach
		TileID.Sets.IsShakeable[Type] = true;
		TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
		DustType = -1;

		PreAddTileObjectData();
		TileObjectData.addTile(Type);
	}

	/// <summary> Called during SetStaticDefaults. </summary>
	public virtual void PreAddTileObjectData() { }

	/// <summary> Used for pseudo random logic, like branch positions, based on <see cref="noise"/>. </summary>
	protected virtual float Noise(Vector2 position) => noise.GetNoise(position.X, position.Y) * 12;

	/// <returns> Whether the given tile has a treetop. </returns>
	public virtual bool IsTreeTop(int i, int j) => Framing.GetTileSafely(i, j - 1).TileType != Type;

	public void ShakeTree(int i, int j)
	{
		while (Framing.GetTileSafely(i, j - 1).TileType == Type)
			j--; //Move to the top of the tree

		var pt = new Point16(i, j);
		if (!treeShakes.Contains(pt) && IsTreeTop(i, j))
			OnShakeTree(i, j);

		treeShakes.Add(pt); //Prevent this tree from being shook again
	}

	protected virtual void OnShakeTree(int i, int j) => GrowEffects(i, j, true);

	public void GrowEffects(int i, int j, bool shake = false)
	{
		int height = 1;
		while (Framing.GetTileSafely(i, j - height).TileType == Type)
			height++; //Move to the top of the tree

		if (shake)
			height = 1;

		OnGrowEffects(i, j - (height - 1), height);
	}

	/// <summary> Used to create effects when the tree is grown, such as leaves. Doubles for shake effects by default. </summary>
	protected virtual void OnGrowEffects(int i, int j, int height) { }

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		var drops = base.GetItemDrops(i, j);

		if (IsTreeTop(i, j))
			drops = drops.Concat([new Item(ItemID.Acorn)]);

		return drops;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!fail) //Switch to the 'chopped' frame
			Framing.GetTileSafely(i, j + 1).TileFrameX = (short)(WorldGen.genRand.Next(9, 12) * FrameSize);
		else
			ShakeTree(i, j);
	}

	/// <summary> Use this to draw treetops and tree branches based on coordinates resulting from <see cref="Noise"/>. </summary>
	public virtual void DrawTreeFoliage(int i, int j, SpriteBatch spriteBatch)
	{
		var position = new Vector2(i, j) * 16 - Main.screenPosition + new Vector2(10, 0) + TreeHelper.GetPalmTreeOffset(i, j);
		float rotation = Main.instance.TilesRenderer.GetWindCycle(i, j, TileSwaySystem.Instance.TreeWindCounter) * .1f;

		if (IsTreeTop(i, j) && TopTexture != null) //Draw tops
		{
			var source = TopTexture.Frame(3, sizeOffsetX: -2, sizeOffsetY: -2);
			var origin = source.Bottom();

			spriteBatch.Draw(TopTexture.Value, position, source, Lighting.GetColor(i, j), rotation, origin, 1, SpriteEffects.None, 0);
		}
	}

	public sealed override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		DrawTreeBody(i, j, spriteBatch);

		if (IsTreeTop(i, j) || (int)Noise(new Vector2(i, j)) == 0)
			drawPoints.Add(new Point16(i, j));

		return false;
	}

	public virtual bool DrawTreeBody(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Framing.GetTileSafely(i, j);
		var texture = TextureAssets.Tile[Type].Value;

		var source = new Rectangle(tile.TileFrameX % (FrameSize * 12), 0, FrameSize - 2, FrameSize - 2);
		var offset = Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;
		var position = (new Vector2(i, j) + offset) * 16 - Main.screenPosition + TreeHelper.GetPalmTreeOffset(i, j);

		spriteBatch.Draw(texture, position, source, Lighting.GetColor(i, j), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
		return false;
	}

	/// <returns> Whether the tree was successfully grown. </returns>
	public static bool GrowTree<T>(int i, int j) where T : CustomTree
	{
		while (!WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(i, j + 1)))
			j++; //Find the ground

		var instance = ModContent.GetInstance<T>() as CustomTree;
		int height = instance.TreeHeight;

		if (WorldGen.InWorld(i, j) && WorldMethods.AreaClear(i, j - (height - 1), 1, height))
		{
			WorldGen.KillTile(i, j); //Kill the tile at origin, presumably a sapling
			instance.GenerateTree(i, j, height);

			if (WorldGen.PlayerLOS(i, j))
				instance.GrowEffects(i, j);
		}
		
		return Framing.GetTileSafely(i, j).TileType == instance.Type;
	}

	protected virtual void GenerateTree(int i, int j, int height)
	{
		int variance = WorldGen.genRand.Next(-8, 9) * 2;
		short xOff = 0;

		for (int h = 0; h < height; h++)
		{
			int frameX = WorldGen.genRand.Next(0, 3);

			if (h == 0)
				frameX = 3;
			if (j == height - 1)
				frameX = WorldGen.genRand.Next(4, 7);

			WorldGen.PlaceTile(i, j - h, Type, true);
			var tile = Framing.GetTileSafely(i, j - h);

			if (tile.HasTile && tile.TileType == Type)
			{
				tile.TileFrameX = (short)(frameX * FrameSize);
				tile.TileFrameY = TreeHelper.GetPalmOffset(j, variance, height, ref xOff);
			}
		}

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendTileSquare(-1, i, j + 1 - height, 1, height, TileChangeType.None);
	}
}
