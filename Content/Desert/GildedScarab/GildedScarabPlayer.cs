using SpiritReforged.Common.PlayerCommon;
using Terraria.Audio;

namespace SpiritReforged.Content.Desert.GildedScarab;

internal class GildedScarabPlayer : ModPlayer
{
	public int scarabDefense;
	public int scarabTimer;
	public int scarabFadeTimer;
	private bool canActivate;

	public override void UpdateEquips()
	{
		if (++scarabTimer >= 32)
			scarabTimer = 0;

		if (Player.HasBuff(ModContent.BuffType<GildedScarab_buff>()))
			scarabFadeTimer++;
	}

	public override void ModifyHurt(ref Player.HurtModifiers modifiers)
	{
		if (modifiers.DamageSource.SourceOtherIndex == 3 || modifiers.DamageSource.SourceOtherIndex == 2)
			canActivate = false;
		else
			canActivate = true;
	}

	public override void OnHurt(Player.HurtInfo info)
	{
		if (Player.HasAccessory<GildedScarab>() && canActivate)
		{
			scarabFadeTimer = 0;
			if (Player.HasBuff(ModContent.BuffType<GildedScarab_buff>()))
			{
				Player.ClearBuff(ModContent.BuffType<GildedScarab_buff>());
				scarabDefense = scarabDefense + (int)(info.Damage / 8f) >= 50 || scarabDefense >= 50 ? 50 : scarabDefense + (int)(info.Damage / 8f);
			}
			else
			{
				SoundEngine.PlaySound(SoundID.Item76);
				scarabDefense = info.Damage >= 400 ? 50 : 5 + (int)(info.Damage / 8f);
			}

			Player.AddBuff(ModContent.BuffType<GildedScarab_buff>(), 300);
		}
	}
}