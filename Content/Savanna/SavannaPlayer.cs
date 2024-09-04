using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna;

public class SavannaPlayer : ModPlayer
{
	public bool quenchPotion = false;
	public override void ResetEffects()
	{
		quenchPotion = false;
	}
}