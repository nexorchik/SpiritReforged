using SpiritReforged.Common.PlayerCommon;
using System.Linq;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Underground.Zipline;

internal class ZiplineHandler : ILoadable
{
	internal static Asset<Texture2D> hookTexture;
	internal static Asset<Texture2D> wireTexture;

	/// <summary> <see cref="Zipline"/>s belonging to all players.<para/>
	/// Use <see cref="Add"/> and <see cref="Zipline.RemovePoint"/> instead of directly adding and removing points from this set. </summary>
	public static readonly HashSet<Zipline> ziplines = [];

	/// <summary> Creates a new zipline at <paramref name="position"/> or adds to an existing zipline belonging to <paramref name="player"/>. </summary>
	/// <param name="player"> The zipline owner. </param>
	/// <param name="position"> The position to deploy at. </param>
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
		On_Main.DoDraw_Tiles_NonSolid += DrawAllZiplines;

		hookTexture = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("Content/Underground/Zipline/Zipline_Hook");
		wireTexture = SpiritReforgedMod.Instance.Assets.Request<Texture2D>("Content/Underground/Zipline/Zipline_Wire");
	}

	private static void DrawAllZiplines(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
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

/// <summary> Exists to override <see cref="ModPlayer.PreUpdateMovement"/> and call <see cref="ZiplineHandler.CheckZipline"/>. </summary>
internal class ZiplinePlayer : ModPlayer
{
	public bool assistant = true;
	private bool wasOnZipline;

	public override void PreUpdateMovement()
	{
		bool onZipline = ZiplineHandler.CheckZipline(Player);

		if (onZipline)
		{
			Player.moveSpeed += .2f;
			wasOnZipline = true;

			if (Player.controlDown)
				Player.GetModPlayer<CollisionPlayer>().fallThrough = true;
		}

		if (!onZipline && wasOnZipline) //Reset rotation
			Player.fullRotation = 0;
	}

	public override void SaveData(TagCompound tag) => tag[nameof(assistant)] = assistant;
	public override void LoadData(TagCompound tag) => assistant = tag.GetBool(nameof(assistant));
}
