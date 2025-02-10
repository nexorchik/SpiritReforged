using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Ocean.Items;
using Terraria.DataStructures;
using static SpiritReforged.Common.Misc.ReforgedMultiplayer;

namespace SpiritReforged.Content.Ocean.Hydrothermal.Tiles;

[AutoloadGlowmask("Method:Content.Ocean.Hydrothermal.Tiles.Magmastone Glow")]
public class Magmastone : ModTile, IAutoloadTileItem
{
	/// <summary> The rate in which <see cref="glowPoints"/> value decays. </summary>
	private const float glowDecayRate = .1f;

	/// <summary> Magmastone glow origin points, handled by the local client. </summary>
	private static readonly Dictionary<Point16, float> glowPoints = [];

	private static readonly HashSet<Point16> wireGlowPoints = [];

	/// <summary> Adds a Magmastone glow origin point, handled by the local client. </summary>
	/// <param name="i"> The X tile coordinate. </param>
	/// <param name="j"> The Y tile coordinate. </param>
	/// <returns> Whether the point was successfully added. </returns>
	public static bool AddGlowPoint(int i, int j) => glowPoints.TryAdd(new Point16(i, j), HydrothermalVent.eruptDuration * glowDecayRate + 1);

	/// <summary> Toggles a non-duration based glow point for the local client. Can be synced using <see cref="MessageType.MagmaGlowPoint"/> in multiplayer. </summary>
	/// <param name="i"> The X tile coordinate. </param>
	/// <param name="j"> The Y tile coordinate. </param>
	public static void ToggleWireGlowPoint(int i, int j)
	{
		if (Main.dedServ)
			return;

		var pt = new Point16(i, j);

		if (!wireGlowPoints.Remove(pt))
			wireGlowPoints.Add(pt);
	}

	public static Color Glow(object obj)
	{
		const int range = 10;

		var pos = (Point)obj;
		var defaultColor = Lighting.GetColor(pos) * 2;

		foreach (var pt in wireGlowPoints)
		{
			if (pos.X == pt.X && pos.Y == pt.Y)
				return Color.White;
		}

		foreach (var pt in glowPoints)
		{
			if (pt.Key.ToVector2().Distance(pos.ToVector2()) <= range)
				return Color.Lerp(defaultColor, Color.White, pt.Value);
		}

		return defaultColor;
	}

	public override void Load() => On_Main.UpdateParticleSystems += UpdateGlow;

	/// <summary> Checks whether glow points should be removed and handles duration. </summary>
	private static void UpdateGlow(On_Main.orig_UpdateParticleSystems orig, Main self)
	{
		orig(self);

		if (Main.dedServ)
			return;

		foreach (var pt in wireGlowPoints) //Validity check
		{
			if (Framing.GetTileSafely(pt).TileType != ModContent.TileType<Magmastone>())
			{
				wireGlowPoints.Remove(pt);
				break;
			}
		}

		foreach (var key in glowPoints.Keys) //Update glowPoint durations
		{
			if ((glowPoints[key] -= glowDecayRate) <= 0)
			{
				glowPoints.Remove(key);
				break;
			}
		}
	}

	public void AddItemRecipes(ModItem item) => item.CreateRecipe(25)
			.AddIngredient(ItemID.StoneBlock, 10)
			.AddIngredient(ModContent.ItemType<MineralSlag>(), 1)
			.AddTile(TileID.WorkBenches)
			.Register();

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		TileID.Sets.CanBeDugByShovel[Type] = true;

		AddMapEntry(new Color(200, 160, 80));
		this.Merge(TileID.Sand, TileID.HardenedSand, ModContent.TileType<Gravel>());

		DustType = DustID.Asphalt;
		MineResist = .5f;
		MinPick = 50;
	}

	public override void HitWire(int i, int j)
	{
		if (Main.dedServ)
		{
			var packet = SpiritReforgedMod.Instance.GetPacket(MessageType.MagmaGlowPoint, 2); //Send to multiplayer clients
			packet.Write((short)i);
			packet.Write((short)j);
			packet.Send();
		}
		else
			ToggleWireGlowPoint(i, j);
	}
}