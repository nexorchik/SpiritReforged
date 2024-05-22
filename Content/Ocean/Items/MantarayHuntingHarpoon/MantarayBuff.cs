namespace SpiritReforged.Content.Ocean.Items.MantarayHuntingHarpoon;

public class MantarayBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }
 
    public override void Update(Player player, ref int buffIndex)
    {
        player.mount.SetMount(ModContent.MountType<MantarayMount>(), player);
        player.buffTime[buffIndex] = 10;
    }
}