using SpiritReforged.Content.Desert.GildedScarab;
using SpiritReforged.Content.Underground.Zipline;
using System.Linq;
using Terraria.WorldBuilding;

namespace SpiritReforged.Common.Misc;

/// <summary> Handles cross mod compatibility with Spirit Classic. Does not autoload, and must be added with <see cref="AddSystem"/>. </summary>
[Autoload(false)]
internal class SpiritClassic : ModSystem
{
	private const string CompatName = "SpiritMod";
	public static bool Enabled => CompatMod != null;

	/// <summary> Spirit Classic. </summary>
	private static Mod CompatMod = null;

	/// <summary> The types of Spirit Classic items that have been replaced. </summary>
	private static readonly HashSet<int> ObsoleteItemTypes = [];

	public static bool AddSystem(Mod mod)
	{
		if (!ModLoader.HasMod(CompatName))
			return false;

		CompatMod = ModLoader.GetMod(CompatName);
		mod.AddContent((SpiritClassic)Activator.CreateInstance(typeof(SpiritClassic)));

		return true;
	}

	public override void SetStaticDefaults()
	{
		foreach (var item in Mod.GetContent<ModItem>())
		{
			int id = ClassicItem(item.Name);
			if (id == -1)
				continue;

			ObsoleteItemTypes.Add(id);
		}

		static int ClassicItem(string name)
		{
			var item = CompatMod.GetContent<ModItem>().Where(x => x.Name == name).FirstOrDefault();
			return (item == default) ? -1 : item.Type;
		}
	}

	public override void AddRecipes()
	{
		if (CompatMod.TryFind("Chitin", out ModItem chitin))
			Recipe.Create(ModContent.ItemType<GildedScarab>()).AddRecipeGroup("GoldBars", 5).AddIngredient(chitin.Type, 8).AddTile(TileID.Anvils).Register();
	}

	public override void PostAddRecipes()
	{
		string fullLog = "Disabled recipes from Classic: ";

		foreach (var recipe in Main.recipe)
		{
			if (recipe.Mod == null || recipe.Mod.Name != CompatName)
				continue;

			int cType = recipe.createItem?.type ?? 0;
			if (!ObsoleteItemTypes.Contains(cType))
				continue;

			recipe.DisableRecipe();
			fullLog += $"{ItemLoader.GetItem(cType).Name} ({recipe.RecipeIndex}), ";
		}

		SpiritReforgedMod.Instance.Logger.Info(fullLog.Remove(fullLog.Length - 2, 2));
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		//No mod id for tasks

		int memorial = tasks.FindIndex(x => x.Name == "A Hero's Memorial");
		if (memorial != -1)
			tasks[memorial].Disable();

		int stargrass = tasks.FindIndex(x => x.Name == "Stargrass Micropass");
		if (stargrass != -1)
			tasks[stargrass].Disable();
	}

	public override void PostWorldGen()
	{
		if (CompatMod.TryFind("AsteroidChest", out ModTile asteroid))
		{
			foreach (var c in Main.chest)
			{
				if (c is null)
					continue;

				var tile = Main.tile[c.x, c.y];
				if (tile.TileType == asteroid.Type)
					ReplaceZiplineGun(c.item);
			}
		}

		static void ReplaceZiplineGun(Item[] items)
		{
			foreach (var item in items)
			{
				if (!item.IsAir && ObsoleteItemTypes.Contains(item.type) && item.ModItem.Name == "ZiplineGun")
					item.SetDefaults(ModContent.ItemType<ZiplineGun>());
			}
		}
	}
}
