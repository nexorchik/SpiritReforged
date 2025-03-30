namespace SpiritReforged.Content.Underground.WayfarerSet;

class ExplorerPot : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Explorer's Vigor");
		// Description.SetDefault("You're eager for more!");
		Main.buffNoTimeDisplay[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		WayfarerPlayer wayfarerPlayer = player.GetModPlayer<WayfarerPlayer>();
		player.moveSpeed += wayfarerPlayer.movementStacks * 0.05f / 4;
		player.runAcceleration += wayfarerPlayer.movementStacks * 0.015f / 4;
	}

	public override bool ReApply(Player player, int time, int buffIndex)
	{
		WayfarerPlayer wayfarerPlayer = player.GetModPlayer<WayfarerPlayer>();
		if (wayfarerPlayer.movementStacks < 16)
			wayfarerPlayer.movementStacks++;

		return false;
	}

	public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
	{
		WayfarerPlayer wayfarerPlayer = Main.LocalPlayer.GetModPlayer<WayfarerPlayer>();
		tip += $"\nMovement speed is increased by {wayfarerPlayer.movementStacks * 5 / 4}%";
		rare = wayfarerPlayer.movementStacks >> 1;
	}
}
