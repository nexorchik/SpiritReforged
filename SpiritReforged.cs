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
using SpiritReforged.Common.PrimitiveRendering;

namespace SpiritReforged;

public class SpiritReforgedMod : Mod
{
	public static SpiritReforgedMod Instance => ModContent.GetInstance<SpiritReforgedMod>();

	public SpiritReforgedMod() => GoreAutoloadingEnabled = true;

	public override void Load()
	{
		NPCUtils.NPCUtils.AutoloadModBannersAndCritters(this);
		NPCUtils.NPCUtils.TryLoadBestiaryHelper();
		Common.Misc.AutoloadMinionDictionary.AddBuffs(Code);

		ShaderHelpers.GetWorldViewProjection(out Matrix view, out Matrix projection); 
		
		Main.QueueMainThreadAction(() => SpiritReforgedLoadables.BasicShaderEffect = new BasicEffect(Main.graphics.GraphicsDevice)
		{
			VertexColorEnabled = true,
			View = view,
			Projection = projection
		});

		SpiritReforgedLoadables.VertexTrailManager = new TrailManager(this);
		TrailDetours.Initialize();
	}

	public override void Unload()
	{
		NPCUtils.NPCUtils.UnloadMod(this);
		NPCUtils.NPCUtils.UnloadBestiaryHelper();
		Common.Misc.AutoloadMinionDictionary.Unload();
		SpiritReforgedLoadables.BasicShaderEffect = null;
		SpiritReforgedLoadables.VertexTrailManager = null;
		TrailDetours.Unload();
	}

	public ModPacket GetPacket(Common.Misc.ReforgedMultiplayer.MessageType type, int capacity)
	{
		ModPacket packet = GetPacket(capacity + 1);
		packet.Write((byte)type);
		return packet;
	}

	public override void HandlePacket(System.IO.BinaryReader reader, int whoAmI) => Common.Misc.ReforgedMultiplayer.HandlePacket(reader, whoAmI);
}