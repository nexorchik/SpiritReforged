using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.WorldGeneration;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Forest.Misc.TornMap;

public class TornMapPiece : ModItem
{
	private static Point16 lastMouseWorld;

	public override void Load()
	{
		On_Player.PlaceThing_Tiles += StopTilePlace;
		On_Player.UpdatePlacementPreview += ChangePlaceType;
	}

	/// <summary> Shuffles the placeable tile type in the preview stage. </summary>
	/// <param name="orig"></param>
	/// <param name="self"> The local player. </param>
	/// <param name="sItem"> The item to be placed. </param>
	private void ChangePlaceType(On_Player.orig_UpdatePlacementPreview orig, Player self, Item sItem)
	{
		if (sItem.type == Type)
		{
			var point = (Main.MouseWorld / 16).ToPoint16();
			if (point != lastMouseWorld) //Shuffle the placeable type when the player hovers over a new tile on the grid
			{
				sItem.createTile = Main.rand.NextBool(3) ? ModContent.TileType<TornMapWallSmall>() : ModContent.TileType<TornMapWall>();
				lastMouseWorld = point;
			}
		}

		orig(self, sItem);
	}

	/// <summary> Prevents tile placement dynamically. </summary>
	/// <param name="orig"></param>
	/// <param name="self"> The local player. </param>
	private void StopTilePlace(On_Player.orig_PlaceThing_Tiles orig, Player self)
	{
		if (self.inventory[self.selectedItem].type != Type || !Main.mouseLeft) //Skips orig
			orig(self);
	}

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 3;

		CrateDatabase.AddCrateRule(ItemID.WoodenCrate, ItemDropRule.Common(Type, 5));
		CrateDatabase.AddCrateRule(ItemID.WoodenCrateHard, ItemDropRule.Common(Type, 5));
	}

	public override void SetDefaults()
	{
		Item.width = Item.height = 28;
		Item.maxStack = Item.CommonMaxStack;
		Item.value = 1000;
		Item.rare = ItemRarityID.White;
		Item.useAnimation = Item.useTime = 30;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<TornMapWall>();
	}

	public override bool AltFunctionUse(Player player) => true;
	public override bool? UseItem(Player player)
	{
		const int Radius = 170;

		if (Main.myPlayer == player.whoAmI && player.altFunctionUse != 2 && !Main.dedServ)
		{
			for (int k = 0; k < 10; k++)
			{
				var dust = Dust.NewDustDirect(player.Center, player.width, player.height, DustID.PortalBolt);
				var vector = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * (Main.rand.Next(50, 100) * 0.04f);

				dust.velocity = vector;
				dust.noGravity = true;
				dust.position = player.Center - Vector2.Normalize(vector) * 34f;
			}

			var point = (player.Center / 16).ToPoint16();
			LightMap(point.X, point.Y, Radius); //Only light up the user's map

			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Item/PageFlip") { Pitch = .5f }, player.Center);
			return true;
		}

		return null;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		foreach (var line in tooltips)
		{
			if (line.Mod == "Terraria" && line.Name == "Placeable") //Read "Can be placed with right click"
				line.Text += $" {Language.GetTextValue("Mods.SpiritReforged.Items.CommonTooltips.AltPlace")}";
		}
	}

	private static void LightMap(int x, int y, int radius)
	{
		int size = radius / 2;
		bool playSound = false;

		for (int i = x - size; i <= x + size; ++i)
		{
			for (int j = y - size; j <= y + size; ++j)
			{
				if (!WorldGen.InWorld(i, j))
					continue;

				float dist = 1 - Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) / (radius * 0.5f);
				float noise = 1f - NoiseSystem.Perlin(i * 2, j * 2) * .75f;

				byte addedLight = (byte)Math.Clamp(255 * dist * 2.5f * noise, 0, 255);

				if (WorldGen.SolidTile(i, j))
					addedLight = (byte)(addedLight * .5f);

				if (addedLight > 0)
				{
					Main.Map.UpdateLighting(i, j, addedLight);

					if (ScanForTreasure(i, j))
						playSound = true;
				}
			}
		}

		Main.refreshMap = true;

		if (playSound)
		{
			SoundEngine.PlaySound(SoundID.CoinPickup with { Pitch = -.5f });
			SoundEngine.PlaySound(SoundID.Coins with { Pitch = 1 });
		}
	}

	/// <returns> Whether a treasure was found at the given coordinates. </returns>
	private static bool ScanForTreasure(int x, int y)
	{
		int type = Main.tile[x, y].TileType;

		if (TileID.Sets.FriendlyFairyCanLureTo[type] && TileObjectData.IsTopLeft(x, y))
		{
			Main.TriggerPing(new Vector2(x, y) + Vector2.One);
			return true;
		}

		return false;
	}
}