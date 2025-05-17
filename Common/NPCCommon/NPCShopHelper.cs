namespace SpiritReforged.Common.NPCCommon;

/// <summary> Stores all non-specific NPCShop modifications for ease of use and organization. </summary>
internal class NPCShopHelper : GlobalNPC
{
	public readonly struct ConditionalEntry(Func<NPCShop, bool> condition, NPCShop.Entry entry)
	{
		/// <summary>
		/// The condition required for the given shop to have this entry. Usually used for checking for the right NPC.
		/// </summary>
		public readonly Func<NPCShop, bool> Condition = condition;

		/// <summary>
		/// The actual shop entry.
		/// </summary>
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
			{
				// Regenerate the item as otherwise the tooltips aren't populated right
				// Since this doesn't Clone, it may remove data - this shouldn't be an issue but should be kept in mind
				// (Cloning doesn't properly regenerate the tooltips)
				Item item = entry.Entry.Item;
				shop.Add(new NPCShop.Entry(new Item(item.type, item.stack)));
			}
		}
	}
}
