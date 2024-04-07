global using Terraria.ModLoader;
global using Terraria;
global using Terraria.ID;
global using Terraria.GameContent;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using ReLogic.Content;
global using System;
global using Terraria.Localization;
global using Terraria.Enums;
global using Terraria.ObjectData;
global using System.Collections.Generic;
global using NPCUtils;

namespace SpiritReforged;

public class SpiritReforgedMod : Mod
{
	public static SpiritReforgedMod Instance => ModContent.GetInstance<SpiritReforgedMod>();

	public SpiritReforgedMod() => GoreAutoloadingEnabled = true;

	public override void Load()
	{
		NPCUtils.NPCUtils.AutoloadModBannersAndCritters(this);
		NPCUtils.NPCUtils.TryLoadBestiaryHelper();
	}

	public override void Unload()
	{
		NPCUtils.NPCUtils.UnloadMod(this);
		NPCUtils.NPCUtils.UnloadBestiaryHelper();
	}
}