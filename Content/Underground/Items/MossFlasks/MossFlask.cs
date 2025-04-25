using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Common.WorldGeneration;
using Terraria.Audio;
using Terraria.WorldBuilding;

namespace SpiritReforged.Content.Underground.Items.MossFlasks;

[AutoloadGlowmask("255,255,255")]
public abstract class MossFlask : ModItem
{
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.HolyWater);
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(0, 0, 1, 0);

		if (Mod.TryFind(Name + "Projectile", out ModProjectile p))
			Item.shoot = p.Type;
	}
}

public abstract class MossFlaskProjectile : ModProjectile
{
	private static ushort[] StoneTypes;
	private static ushort[] BrickTypes;

	private int _itemType = -1;
	public int ItemType
	{
		get
		{
			if (_itemType != -1)
				return _itemType;

			return _itemType = Mod.Find<ModItem>(Name.Replace("Projectile", string.Empty)).Type;
		}
	}

	/// <summary> The stone and brick types to place, respectively. </summary>
	public abstract (ushort, ushort) Types { get; }

	public override string Texture => base.Texture.Replace("Projectile", string.Empty);
	public override LocalizedText DisplayName => ItemLoader.GetItem(ItemType).DisplayName;

	public override void SetStaticDefaults()
	{
		if (StoneTypes == null && BrickTypes == null)
		{
			List<ushort> stone = [];
			List<ushort> brick = [];

			stone.Add(TileID.Stone);
			brick.Add(TileID.GrayBrick);

			foreach (var flask in Mod.GetContent<MossFlaskProjectile>())
			{
				stone.Add(flask.Types.Item1);
				brick.Add(flask.Types.Item2);
			} //Gather a list based on registered Types

			StoneTypes = [.. stone];
			BrickTypes = [.. brick];
		}
	}

	public override void SetDefaults() => Projectile.CloneDefaults(ProjectileID.HolyWater);

	public override void OnKill(int timeLeft)
	{
		const int area = 5;

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var pt = Projectile.Center.ToTileCoordinates();
			ShapeData data = new();

			WorldUtils.Gen(pt, new Shapes.Circle(area), Actions.Chain(new Modifiers.OnlyTiles(StoneTypes), new SolidIsTouchingAir(true), new ReplaceType(Types.Item1).Output(data)));
			WorldUtils.Gen(pt, new Shapes.Circle(area), Actions.Chain(new Modifiers.OnlyTiles(BrickTypes), new SolidIsTouchingAir(true), new ReplaceType(Types.Item2).Output(data)));

			WorldUtils.Gen(pt, new ModShapes.All(data), Actions.Chain(new Actions.SetFrames(true), new Send()));
		}

		SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

		for (int i = 0; i < 10; i++)
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Projectile.QuickDraw();

		var data = GlowmaskItem.ItemIdToGlowmask[ItemType];
		var glow = data.Glowmask.Value;

		Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(data.GetDrawColor(Projectile)), Projectile.rotation, glow.Size() / 2, Projectile.scale, default);
		return false;
	}
}