using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Content.Savanna.Tiles;

public class AcaciaTree : ModPalmTree
{
	internal static Asset<Texture2D> Texture, TopsTexture;

	public override TreePaintingSettings TreeShaderSettings => new()
	{
		UseSpecialGroups = true,
		SpecialGroupMinimalHueValue = 11f / 72f,
		SpecialGroupMaximumHueValue = 0.25f,
		SpecialGroupMinimumSaturationValue = 0.88f,
		SpecialGroupMaximumSaturationValue = 1f
	};

	public override void SetStaticDefaults()
	{
		GrowsOnTileId = [ModContent.TileType<SavannaGrass>()];
		
		if (!Main.dedServ)
		{
			var mod = SpiritReforgedMod.Instance;

			Texture = mod.Assets.Request<Texture2D>("Content/Savanna/Tiles/AcaciaTree");
			TopsTexture = mod.Assets.Request<Texture2D>("Content/Savanna/Tiles/AcaciaTree_Tops");
		}
	}

	public override int SaplingGrowthType(ref int style) => ModContent.TileType<AcaciaSapling>();
	public override int DropWood() => ItemID.Wood;
	public override Asset<Texture2D> GetTexture() => Texture;
	public override Asset<Texture2D> GetOasisTopTextures() => TopsTexture; //This is never used
	public override Asset<Texture2D> GetTopTextures() => TopsTexture;
}

//Defines necessary features of our acacia tree because ModPalmTree is inflexible
public class AcaciaTreeSystem : ModSystem
{
	public override void Load()
	{
		On_TileDrawing.GetPalmTreeVariant += GetPalmTreeVariant;
		IL_TileDrawing.DrawTrees += DrawTrees;
	}

	private int GetPalmTreeVariant(On_TileDrawing.orig_GetPalmTreeVariant orig, TileDrawing self, int x, int y)
	{
		int value = orig(self, x, y);

		if (value < 0 && Framing.GetTileSafely(x, y).TileType == ModContent.TileType<SavannaGrass>())
			value *= -1; //Prevent our acacia tree from ever using its non-existant "oasis variant"

		return value;
	}

	private void DrawTrees(ILContext il)
	{
		static ILCursor EmitPalmTreeBiome(ILCursor cursor) => cursor.Emit(OpCodes.Ldloc_S, (byte)75);

		ILCursor c = new(il);

		c.GotoNext(x => x.MatchCall<TileDrawing>("GetPalmTreeBiome"));
		c.GotoNext(MoveType.After, x => x.MatchCallvirt<SpriteBatch>("Draw")); //Go to draw then move backwards

		c.GotoPrev(MoveType.After, x => x.MatchNewobj(typeof(Vector2)));
		EmitPalmTreeBiome(c);
		c.EmitDelegate(ModifyOrigin);

		c.GotoPrev(MoveType.After, x => x.MatchMul());
		EmitPalmTreeBiome(c);
		c.EmitDelegate(ModifyRotation);

		c.GotoPrev(MoveType.After, x => x.MatchNewobj(typeof(Rectangle)));
		EmitPalmTreeBiome(c);
		c.EmitDelegate(ModifyTopFrame);

		c.GotoPrev(MoveType.After, x => x.MatchAdd());
		c.Index -= 4;
		EmitPalmTreeBiome(c);
		c.EmitDelegate(ModifyPosition);
	}

	private static bool IsAcaciaTree(int treeStyle) => treeStyle == 8 + ModContent.TileType<SavannaGrass>(); //Helper

	private Vector2 ModifyPosition(Vector2 position, int treeStyle) => IsAcaciaTree(treeStyle) ? position + new Vector2(8, 0) : position;

	private Vector2 ModifyOrigin(Vector2 origin, int treeStyle) => IsAcaciaTree(treeStyle) ? new Vector2(288 / 2, 96) : origin;

	private float ModifyRotation(float rotation, int treeStyle) => IsAcaciaTree(treeStyle) ? rotation * .5f : rotation;

	private Rectangle ModifyTopFrame(Rectangle frame, int treeStyle)
	{
		if (IsAcaciaTree(treeStyle))
		{
			frame.Width = AcaciaTree.TopsTexture.Width();
			frame.Height = AcaciaTree.TopsTexture.Height();
		}

		return frame;
	}
}
