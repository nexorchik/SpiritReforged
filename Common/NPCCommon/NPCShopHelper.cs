namespace SpiritReforged.Common.NPCCommon;

/// <summary> Stores all non-specific NPCShop modifications for ease of use and organization. </summary>
internal class NPCShopHelper : GlobalNPC
{
	public readonly struct ConditionalEntry(Func<NPCShop, bool> condition, NPCShop.Entry entry)
	{
		public readonly Func<NPCShop, bool> Condition = condition;
		public readonly NPCShop.Entry Entry = entry;
	}

	private static readonly List<ConditionalEntry> EntriesToAdd = [];

	/// <summary> Adds the given entry to the database. This should only be run in <see cref="ModType.SetStaticDefaults"/>. </summary>
	/// <param name="entry">Entry to add.</param>
	public static void AddEntry(ConditionalEntry entry) => EntriesToAdd.Add(entry);

	public override void ModifyShop(NPCShop shop)
	{
		foreach (var entry in EntriesToAdd)
		{
			if (entry.Condition(shop))
				shop.Add(entry.Entry);
		}
	}
}
