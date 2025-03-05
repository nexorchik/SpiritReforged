using SpiritReforged.Content.Desert.GildedScarab;
using SpiritReforged.Content.Ocean.Hydrothermal;

namespace SpiritReforged.Common.ModCompat.Classic;

/// <summary> Handles cross mod compatibility with Spirit Classic. Does not autoload, and must be added with <see cref="AddSystem"/>. </summary>
[Autoload(false)]
internal class SpiritClassic : ModSystem
{
	private const string ClassicName = "SpiritMod";

	public static bool Enabled => ClassicMod != null;

	/// <summary> The loaded Spirit Classic mod instance. Check <see cref="Enabled"/> before using. </summary>
	internal static Mod ClassicMod = null;
	/// <summary> Spirit Classic items and their Reforged replacements. </summary>
	internal static readonly Dictionary<int, int> ClassicToReforged = [];

	/// <summary> Must be called in a load method. </summary>
	public static bool AddSystem(Mod mod)
	{
		if (!ModLoader.HasMod(ClassicName))
			return false;

		ClassicMod = ModLoader.GetMod(ClassicName);
		mod.AddContent((SpiritClassic)Activator.CreateInstance(typeof(SpiritClassic)));
		mod.AddContent((ObsoleteItem)Activator.CreateInstance(typeof(ObsoleteItem)));

		return true;
	}

	public override void SetStaticDefaults()
	{
		foreach (var item in Mod.GetContent<ModItem>())
		{
			int id = ClassicItem(item);
			if (id == -1)
				continue;

			ClassicToReforged.Add(id, item.Type);

			//Populate shimmer transformations
			ItemID.Sets.ShimmerTransformToItem[id] = item.Type;
		}

		if (ClassicMod.TryFind("SulfurDeposit", out ModItem sulfur))
			HydrothermalVentPlume.DropPool.Add(sulfur.Type, 3);

		static int ClassicItem(ModItem modItem)
		{
			string name;
			var attr = (FromClassicAttribute)Attribute.GetCustomAttribute(modItem.GetType(), typeof(FromClassicAttribute), false);

			if (attr != null)
				name = attr.name;
			else
				name = modItem.Name;

			return ClassicMod.TryFind(name, out ModItem item) ? item.Type : -1;
		}
	}

	public override void AddRecipes()
	{
		if (ClassicMod.TryFind("Chitin", out ModItem chitin))
			Recipe.Create(ModContent.ItemType<GildedScarab>()).AddRecipeGroup("GoldBars", 5).AddIngredient(chitin.Type, 8).AddTile(TileID.Anvils).Register();
	}

	public override void PostAddRecipes()
	{
		string fullLog = "Disabled recipes from Classic: ";

		foreach (var recipe in Main.recipe)
		{
			if (recipe.Mod == null || recipe.Mod.Name != ClassicName)
				continue;

			int cType = recipe.createItem?.type ?? 0;
			if (!ClassicToReforged.ContainsKey(cType))
				continue;

			recipe.DisableRecipe();
			fullLog += $"{ItemLoader.GetItem(cType).Name} ({recipe.RecipeIndex}), ";
		}

		SpiritReforgedMod.Instance.Logger.Info(fullLog.Remove(fullLog.Length - 2, 2));
	}

	/*public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) //PreAddContent handles this more accurately
	{
		int memorial = tasks.FindIndex(x => x.Name == "A Hero's Memorial");
		if (memorial != -1)
			tasks[memorial].Disable();

		int stargrass = tasks.FindIndex(x => x.Name == "Stargrass Micropass");
		if (stargrass != -1)
			tasks[stargrass].Disable();
	}*/

	public override void PostWorldGen()
	{
		foreach (var c in Main.chest)
		{
			if (c is null)
				continue;

			EditContents(c.item);
		}
	}

	private static void EditContents(Item[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			var item = items[i];

			if (!item.IsAir && ClassicToReforged.TryGetValue(item.type, out int reforgedType))
				item.SetDefaults(reforgedType);
		}
	}
}

/// <summary> Denotes an item replacement for Spirit Classic. </summary>
/// <param name="classicName"> The internal name of item to replace. </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class FromClassicAttribute(string classicName) : Attribute
{
	public string name = classicName;
}