using SpiritReforged.Content.Desert.GildedScarab;
using SpiritReforged.Content.Forest.RoguesCrest;
using SpiritReforged.Content.Ocean.Hydrothermal;
using SpiritReforged.Content.Ocean.Items;

namespace SpiritReforged.Common.ModCompat.Classic;

/// <summary> Handles cross mod compatibility with Spirit Classic. Only loads when Spirit Classic is also loaded. </summary>
internal class SpiritClassic : ModSystem
{
	private const string ClassicName = "SpiritMod";

	public static bool Enabled => ClassicMod != null;

	/// <summary> The loaded Spirit Classic mod instance. Check <see cref="Enabled"/> before using. </summary>
	internal static Mod ClassicMod = null;
	/// <summary> Spirit Classic items and their Reforged replacements. </summary>
	internal static readonly Dictionary<int, int> ClassicToReforged = [];

	public override bool IsLoadingEnabled(Mod mod)
	{
		if (!ModLoader.HasMod(ClassicName))
			return false;

		ClassicMod = ModLoader.GetMod(ClassicName);
		mod.AddContent((ObsoleteItem)Activator.CreateInstance(typeof(ObsoleteItem)));

		return true;
	}

	public override void SetStaticDefaults()
	{
		foreach (var item in Mod.GetContent<ModItem>()) //Populate Classic/Reforged counterpart item array
		{
			int id = ClassicItem(item);
			if (id == -1)
				continue;

			ClassicToReforged.Add(id, item.Type);
			ItemID.Sets.ShimmerTransformToItem[id] = item.Type; //Populate shimmer transformations
		}

		if (ClassicMod.TryFind("SulfurDeposit", out ModItem sulfur)) //Add Sulfur to our Hydrothermal vent pool
			HydrothermalVentPlume.DropPool.Add(sulfur.Type, 3);

		if (ClassicMod.TryFind("Cloudstalk", out ModTile cloudstalk)) //Remove Cloudstalk anchors so it can't grow
			TileObjectData.GetTileData(cloudstalk.Type, 0).AnchorValidTiles = [];

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

		if (ClassicMod.TryFind("SpellswordCrest", out ModItem spellsword))
			Recipe.Create(spellsword.Type).AddIngredient(ModContent.ItemType<RogueCrest>()).AddIngredient(ItemID.CrystalShard, 8)
				.AddIngredient(ItemID.SoulofLight, 15).AddTile(TileID.TinkerersWorkbench).Register();
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
			{
				ModifyRecipe(recipe);
				continue;
			}

			recipe.DisableRecipe();
			fullLog += $"{ItemLoader.GetItem(cType).Name} ({recipe.RecipeIndex}), ";
		}

		SpiritReforgedMod.Instance.Logger.Info(fullLog.Remove(fullLog.Length - 2, 2));
	}

	/// <summary> Modifies recipes added by Classic if the result isn't contained in <see cref="ClassicToReforged"/>. </summary>
	private static void ModifyRecipe(Recipe recipe)
	{
		for (int i = recipe.requiredItem.Count - 1; i >= 0; i--)
		{
			if (recipe.requiredItem[i].ModItem?.Name is "DeepCascadeShard") //Replace Deep Cascade Shards with Mineral Slag
				recipe.requiredItem[i] = new Item(ModContent.ItemType<MineralSlag>(), recipe.requiredItem[i].stack);
		}
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

	/// <summary> Converts all Classic chest loot to their Reforged counterparts. </summary>
	private static void EditContents(Item[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			var item = items[i];

			if (!item.IsAir && ClassicToReforged.TryGetValue(item.type, out int reforgedType))
				item.ChangeItemType(reforgedType);
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