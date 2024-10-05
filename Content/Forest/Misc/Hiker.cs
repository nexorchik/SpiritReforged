using SpiritReforged.Common.ItemCommon.Backpacks;
using SpiritReforged.Common.NPCCommon;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Forest.Misc;

internal class Hiker : PlayerContainerNPC, INPCButtons
{
	public bool Hungry
	{
		get => NPC.ai[3] == 1;
		set => NPC.ai[3] = value ? 1 : 0;
	}

	public override void SetStaticDefaults()
	{
		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;
	}

	public override void Defaults()
	{
		NPC.CloneDefaults(NPCID.SkeletonMerchant);
		NPC.townNPC = true;
	}

	protected override void PreDrawPlayer()
	{
		_drawDummy.head = ArmorIDs.Head.ArchaeologistsHat;
		_drawDummy.body = ArmorIDs.Body.ArchaeologistsJacket;
		_drawDummy.legs = ArmorIDs.Legs.ArchaeologistsPants;
		_drawDummy.back = EquipLoader.GetEquipSlot(Mod, "LeatherBackpack", EquipType.Back);
		_drawDummy.front = EquipLoader.GetEquipSlot(Mod, "LeatherBackpack", EquipType.Front);
		_drawDummy.hair = 2;
		_drawDummy.eyeColor = Color.Blue;
		_drawDummy.hairColor = new Color(63, 42, 33);
		_drawDummy.pantsColor = Color.Brown;
		_drawDummy.skinColor = new Color(206, 143, 76);
	}

	public override List<string> SetNPCNameList()
	{
		List<string> nameList = [];

		for (int i = 0; i < 6; i++)
			nameList.Add(Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Name." + i));

		return nameList;
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		button = "Supplies ([c/AAAAAA:20 silver])";
		button2 = "Map";
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton)
		{
			int cost = Item.buyPrice(0, 0, 20, 0);

			if (Main.LocalPlayer.CanAfford(cost) && Main.LocalPlayer.PayCurrency(cost))
			{
				SpawnBundle();
			}
		}
	}

	private void SpawnBundle()
	{
		var newItem = new Item(ModContent.ItemType<LeatherBackpack>());
		var backpack = newItem.ModItem as BackpackItem;
		
		for (int i = 0; i < backpack.Items.Length; ++i)
		{
			Item item = backpack.Items[i];
			item.SetDefaults(ItemID.Zenith);
		}

		Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(NPC), newItem);
	}

	public override string GetChat()
	{
		if (Hungry && PlayerHasFood(out int type))
			return Language.GetText("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Hungry.Asking." + Main.rand.Next(4)).WithFormatArgs($"[i:{type}]").Value;

		return Language.GetTextValue("Mods.SpiritReforged.NPCs.Hiker.Dialogue.Idle." + Main.rand.Next(4));
	}

	private static bool PlayerHasFood(out int itemId)
	{
		itemId = -1;

		for (int i = 0; i < Main.LocalPlayer.inventory.Length; ++i)
		{
			Item item = Main.LocalPlayer.inventory[i];

			if (item.potion || ItemID.Sets.IsFood[item.type])
			{
				itemId = item.type;
				return true;
			}
		}

		return false;
	}

	public ButtonText[] AddButtons() 
	{
		if (PlayerHasFood(out _))
			return [new ButtonText("Feed", "Feed")];

		return []; 
	}

	public void OnClickButton(ButtonText button)
	{
		if (button.Name == "Feed")
		{

		}
	}
}
