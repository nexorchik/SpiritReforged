using SpiritReforged.Common.PlayerCommon;
using System.Linq;

namespace SpiritReforged.Content.Underground.Zipline;

internal class ZiplineHandler : ILoadable
{
	internal static Asset<Texture2D> ziplineNode;
	internal static Asset<Texture2D> ziplineWire;

	/// <summary> <see cref="Zipline"/>s belonging to all players. </summary>
	public static readonly HashSet<Zipline> ziplines = [];

	public static void Add(Player player, Vector2 position)
	{
		var line = ziplines.Where(x => x.Owner == player).FirstOrDefault();

		if (line == default) //Add a new zipline
		{
			var newLine = new Zipline(player.whoAmI);
			newLine.points.Add(position);

			ziplines.Add(newLine);
		}
		else
		{
			if (line.DrawingLine)
			{
				line.points[0] = line.points[1];
				line.points[1] = position;
			}
			else
				line.points.Add(position);
		}
	}

	public void Load(Mod mod)
	{
		On_Main.DoDraw_Tiles_NonSolid += On_Main_DoDraw_Tiles_NonSolid;

		ziplineNode = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("Content/Underground/Zipline/Zipline");
		ziplineWire = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("Content/Underground/Zipline/Zipline_Chain");
	}

	private static void On_Main_DoDraw_Tiles_NonSolid(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
	{
		orig(self);

		foreach (var zipline in ziplines)
			zipline.Draw(Main.spriteBatch);
	}

	public static bool CheckZipline(Player player)
	{
		foreach (var zipline in ziplines)
		{
			if (zipline.OnZipline(player))
				return true;
		}

		return false;
	}

	public void Unload() { }
}

/// <summary> Only exists to call <see cref="ZiplineHandler.CheckZipline"/>. </summary>
internal class ZiplinePlayer : ModPlayer
{
	private bool wasOnZipline;

	public override void PreUpdateMovement()
	{
		bool onZipline = ZiplineHandler.CheckZipline(Player);

		if (onZipline)
		{
			wasOnZipline = true;

			if (Player.controlDown)
				Player.GetModPlayer<CollisionPlayer>().fallThrough = true;
		}

		if (!onZipline && wasOnZipline) //Reset rotation
			Player.fullRotation = 0;
	}
}
