using SpiritReforged.Content.Forest.Safekeeper;
using SpiritReforged.Content.Savanna.Ecotone;
using SpiritReforged.Content.Underground.Tiles;
using SpiritReforged.Content.Underground.Tiles.Potion;

namespace SpiritReforged;

public partial class SpiritReforgedMod : Mod
{
	public override object Call(params object[] args)
	{
		try
		{
			if (args is null)
				Logger.Error("Call Error: Arguments are null.");

			if (args.Length == 0)
				Logger.Error("Call Error: Arguments are empty.");

			if (args[0] is not string context)
				return null;

			switch (context)
			{
				case "AddUndead":
					{
						return UndeadNPC.AddCustomUndead(args[1..]);
					}
				case "GetSavannaArea":
					{
						return SavannaEcotone.SavannaArea;
					}
				case "AddPotionVat":
					{
						return PotionColorDatabase.ParseNewPotion(args[1..]);
					}
				default:
					{
						Logger.Error($"Call Error: Context '{context}' is invalid.");
						return null;
					}
			}
		}
		catch (Exception e)
		{
			Logger.Error("Call Error: " + e.Message + "\n" + e.StackTrace);
		}

		return null;
	}
}
