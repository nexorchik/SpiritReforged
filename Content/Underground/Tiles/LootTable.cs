using System.Linq;
using Terraria.Utilities;

namespace SpiritReforged.Content.Underground.Tiles;

public struct LootData(int type, int stack)
{
	public int type = type;
	public int stack = stack;
}

/// <summary> Helper for constructing simple loot tables. </summary>
public readonly record struct LootTable
{
	private readonly IList<WeightedRandom<LootData>> branches = [new()];

	public LootTable() { }

	public static LootTable operator +(LootTable a, LootTable b)
	{
		foreach (var branch in b.branches)
			a.branches.Add(branch);

		return a;
	}

	/// <summary> Creates a new loot table and calls <see cref="Add(int, int, int)"/>. </summary>
	public static LootTable Create(int itemType, int itemStack = 1, int weight = 1)
	{
		var t = new LootTable();
		t.branches.Last().Add(new(itemType, itemStack), weight);

		return t;
	}

	/// <summary> Adds to the current branch. </summary>
	public readonly LootTable Add(int itemType, int itemStack = 1, int weight = 1)
	{
		branches.Last().Add(new(itemType, itemStack), weight);
		return this;
	}

	/// <summary> Adds a selection of items to the current branch. Cannot specify weight and stack (each default to 1). </summary>
	public readonly LootTable AddRange(params int[] itemType)
	{
		foreach (int i in itemType)
			branches.Last().Add(new(i, 1));

		return this;
	}

	/// <summary> Releases all rolled item drops into an enumerable. </summary>
	public readonly IEnumerable<Item> Release()
	{
		List<Item> output = [];

		foreach (var item in branches)
		{
			if (item.elements.Count == 0)
				continue;

			var element = (LootData)item;
			output.Add(new Item(element.type, element.stack));
		}

		return output;
	}
}