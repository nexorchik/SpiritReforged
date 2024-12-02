
using SpiritReforged.Content.Savanna.Biome;

namespace SpiritReforged.Common.PlayerCommon;

internal class FountainPlayer : ModPlayer
{
	public enum Fountain
	{
		None = 0,
		Savanna
	}

	public Fountain fountain = Fountain.None;

	private void TileReset() => fountain = Fountain.None;

	public override void PreUpdate()
	{
		if (fountain != Fountain.None)
		{
			Main.SceneMetrics.ActiveFountainColor = ModContent.Find<ModWaterStyle>(GetFountainWaterStyle()).Slot;
			//Main.CalculateWaterStyle
		}
	}

	private string GetFountainWaterStyle() => fountain switch
	{
		Fountain.Savanna => "SpiritReforged/" + nameof(SavannaWaterStyle),
		_ => throw new NotImplementedException("Invalid custom water style."),
	};

	public class FountainResetSystem : ModSystem
	{
		public override void ResetNearbyTileEffects() => Main.LocalPlayer.GetModPlayer<FountainPlayer>().TileReset();
	}
}
