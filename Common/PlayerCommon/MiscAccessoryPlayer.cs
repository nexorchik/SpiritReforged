using System.Linq;
using SpiritReforged.Common.ItemCommon;

namespace SpiritReforged.Common.PlayerCommon;

public class MiscAccessoryPlayer : ModPlayer
{
	public readonly Dictionary<string, bool> accessory = [];
	public readonly Dictionary<string, int> timers = [];

	public override void Initialize()
	{
		accessory.Clear();
		timers.Clear();

		var types = typeof(SpiritReforgedMod).Assembly.GetTypes(); //Add every accessory & timered item to this dict
		foreach (var type in types)
		{
			if (type.IsSubclassOf(typeof(AccessoryItem)) && !type.IsAbstract)
			{
				var item = Activator.CreateInstance(type) as AccessoryItem;
				accessory.Add(item.AccName, false);
			}

			if (typeof(ITimerItem).IsAssignableFrom(type) && !type.IsAbstract)
			{
				var item = Activator.CreateInstance(type) as ITimerItem;

				if (item.TimerCount() == 1)
					timers.Add(type.Name, 0);
				else
					for (int i = 0; i < item.TimerCount(); ++i)
						timers.Add(type.Name + i, 0);
			}
		}
	}

	public override void ResetEffects()
	{
		var accColl = accessory.Keys.ToList(); //Reset every acc
		foreach (string item in accColl)
			accessory[item] = false;

		var timerColl = timers.Keys.ToList(); //Decrement every timer
		foreach (string item in timerColl)
			timers[item]--;
	}

	public override void UpdateDead() => ResetEffects();
}
