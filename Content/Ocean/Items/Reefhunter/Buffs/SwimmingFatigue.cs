namespace SpiritReforged.Content.Ocean.Items.Reefhunter.Buffs;

public class SwimmingFatigue : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Swimming Fatigue");
		// Description.SetDefault("You can't swim very well for a bit");
		Main.debuff[Type] = true;
	}
}
