using SpiritReforged.Common.ItemCommon.Backpacks;
using Terraria.UI;

namespace SpiritReforged.Common.UI.BackpackUI;

[Autoload(Side = ModSide.Client)]
public class BackpackUISystem : ModSystem
{
	private UserInterface backpackUI;

	public static void SetBackpack()
	{
		BackpackUISystem instance = ModContent.GetInstance<BackpackUISystem>();
		var state = instance.backpackUI.CurrentState as BackpackUIState;
		state.SetBackpack(Main.LocalPlayer.GetModPlayer<BackpackPlayer>().Backpack);
	}

	public override void Load()
	{
		backpackUI = new UserInterface();
		backpackUI.SetState(new BackpackUIState());
	}

	public override void UpdateUI(GameTime gameTime)
	{
		if (backpackUI?.CurrentState != null)
			backpackUI?.Update(gameTime);

		//backpackUI.SetState(new BackpackUI());
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

		if (index != -1)
			layers.Insert(index, new LegacyGameInterfaceLayer(
				"SpiritReforged: Backpack UI",
				delegate
				{
					if (Main.playerInventory)
						backpackUI?.Draw(Main.spriteBatch, new GameTime());

					return true;
				},
				InterfaceScaleType.UI)
			);
	}
}