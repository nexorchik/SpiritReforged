using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SpiritReforged.Common.ConfigurationCommon;

class ReforgedServerConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ServerSide;

	[DefaultValue(true)]
	public bool VentCritters { get; set; }
}