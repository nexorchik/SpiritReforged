namespace SpiritReforged.Content.Underground.WayfarerSet;

class ExplorerMine : ModBuff
{
	public override void SetStaticDefaults() => Main.buffNoTimeDisplay[Type] = false;

	public override void Update(Player player, ref int buffIndex) => player.pickSpeed -= player.GetModPlayer<WayfarerPlayer>().miningStacks * 0.05f;

	public override bool ReApply(Player player, int time, int buffIndex)
	{
		WayfarerPlayer wayfarerPlayer = player.GetModPlayer<WayfarerPlayer>();
		if (wayfarerPlayer.miningStacks < 4)
			wayfarerPlayer.miningStacks++;

		return false;
	}

	public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
	{
		WayfarerPlayer wayfarerPlayer = Main.LocalPlayer.GetModPlayer<WayfarerPlayer>();
		tip += $"\nMining speed is increased: {wayfarerPlayer.miningStacks} stacks";
		rare = wayfarerPlayer.miningStacks;
	}
}
