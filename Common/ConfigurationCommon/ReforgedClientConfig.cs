using SpiritReforged.Content.Ocean;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SpiritReforged.Common.ConfigurationCommon;

class ReforgeClientConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[DefaultValue(OceanGeneration.OceanShape.Piecewise_V)]
	public OceanGeneration.OceanShape OceanShape { get; set; }

	[ReloadRequired]
	[DefaultValue(true)]
	public bool SurfaceWaterTransparency { get; set; }
}