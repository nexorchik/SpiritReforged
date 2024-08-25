namespace SpiritReforged.Content.Cloudstalk.Buffs;

public class FlightPotionBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<FlightPlayer>().wingTimeMult += .25f;
}

public class FlightPlayer : ModPlayer
{
	public float wingTimeMult = 1;

	public override void ResetEffects() => wingTimeMult = 1;

	public override void UpdateEquips() => Player.wingTimeMax = (int)(Player.wingTimeMax * wingTimeMult);
}
