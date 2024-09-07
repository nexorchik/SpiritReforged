using SpiritReforged.Common.ProjectileCommon;

namespace SpiritReforged.Content.Ocean.Items.JellyCandle;

public class JellyfishBuff : BasePetBuff<JellyfishPet>
{
	protected override (string, string) BuffInfo => ("Peaceful Jellyfish", "'The Jellyfish is helping you relax'");
}