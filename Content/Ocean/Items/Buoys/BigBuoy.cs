namespace SpiritReforged.Content.Ocean.Items.Buoys;

public class BigBuoy : Buoy
{
	public override int SpawnNPCType => ModContent.NPCType<BigBuoy_World>();
}

public class BigBuoy_World : Buoy_World
{
	private static Asset<Texture2D> GlowTexture;

	public override Texture2D Glowmask => GlowTexture.Value;

	public override void Load()
	{
		if (!Main.dedServ)
			GlowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
	}

	public override void PostDefaults() => NPC.Size = new Vector2(28, 42);

	public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<BigBuoy>();
}