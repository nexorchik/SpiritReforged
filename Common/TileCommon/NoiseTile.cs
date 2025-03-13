/*using SpiritReforged.Common.WorldGeneration.Noise;
using Terraria.GameContent.Drawing;

namespace SpiritReforged.Common.TileCommon;

/// <summary> Handler for drawing noise-based overlays on tiles. Call <see cref="AddTile"/> to register a tile for drawing.<para/>
/// The overlay texture must be a horizontal sheet with 2px padding on both axis. Frames can be any size as long as width and height are consistent. </summary>
///// Optionally, use <see cref="INoiseTile"/> for more freedom over drawing.
internal class NoiseTile : GlobalTile
{
	private static readonly Dictionary<int, Asset<Texture2D>> Textures = [];

	/// <summary> Registers <paramref name="modTile"/> for drawing. </summary>
	/// <param name="modTile"> The tile to register. </param>
	/// <param name="texturePath"> The path of the noise texture, including the mod it is from. Defaults to <paramref name="modTile"/>'s texture + "_Noise". </param>
	public static void AddTile(ModTile modTile, string texturePath = null)
	{
		var texture = ModContent.Request<Texture2D>(texturePath ?? (modTile.Texture + "_Noise"));
		Textures.Add(modTile.Type, texture);
	}

	public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
	{
		const float mult = 4.5f;
		const float threshold = .15f;

		if (Textures.TryGetValue(type, out Asset<Texture2D> value))
		{
			if (!TileDrawing.IsVisible(Main.tile[i, j]))
				return;

			float noise = NoiseSystem.PerlinStatic(i * mult, j * mult);
			if (noise < 1f - threshold)
				return;

			var texture = value.Value;

			//if (TileLoader.GetTile(type) is INoiseTile nTile)
			//{
			//	nTile.DrawNoise(i, j, noise, texture, spriteBatch);
			//	return;
			//}

			int height = texture.Height;
			int frames = texture.Width / height;
			int frame = (int)(noise * frames * mult * 10f % frames);
			var source = texture.Frame(frames, 1, frame, 0, -2, -2);
			var offset = Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : new Vector2(Main.offScreenRange);

			spriteBatch.Draw(texture, new Vector2(i, j).ToWorldCoordinates() + offset - Main.screenPosition, source, Lighting.GetColor(i, j), 0, source.Size() / 2, 1, default, 0);
		}
	}
}*/

/*public interface INoiseTile
{
	public void DrawNoise(int i, int j, float noiseValue, Texture2D texture, SpriteBatch spriteBatch);
}*/ //Exclude until needed