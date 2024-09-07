namespace SpiritReforged.Content.Ocean.Items.KoiTotem;

public class KoiTotemBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.pvpBuff[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex) => player.fishingSkill += 5;
}
