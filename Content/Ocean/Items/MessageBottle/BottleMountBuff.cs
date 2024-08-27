namespace SpiritReforged.Content.Ocean.Items.MessageBottle;

public class BottleMountBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.mount.SetMount(ModContent.MountType<MessageBottleMount>(), player, false);
		player.buffTime[buffIndex] = 10;
	}
}
