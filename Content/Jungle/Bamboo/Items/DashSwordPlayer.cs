using Terraria.DataStructures;

namespace SpiritReforged.Content.Jungle.Bamboo.Items;

public class DashSwordPlayer : ModPlayer
{
	public bool holdingSword;
	public bool dashing;
	public bool hasDashCharge;

	public override void ResetEffects() => holdingSword = false;

	public override void PreUpdate()
	{
		if (dashing)
			Player.maxFallSpeed = 2000f;
	}

	public override void PostUpdateEquips()
	{
		if (Player.velocity.Y == 0 && !Player.ItemAnimationActive)
			hasDashCharge = true;
	}

	public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable) => dashing;
}

public class DashSwordLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		Item item = drawInfo.drawPlayer.HeldItem;
		if (!drawInfo.drawPlayer.GetModPlayer<DashSwordPlayer>().holdingSword || drawInfo.shadow != 0f || drawInfo.drawPlayer.frozen || drawInfo.drawPlayer.dead || drawInfo.drawPlayer.wet && item.noWet)
			return;

		if (item.ModItem is IDashSword dashSword)
			dashSword.DrawHeld(ref drawInfo);
	}
}

public interface IDashSword
{
	public void DrawHeld(ref PlayerDrawSet drawInfo);
}