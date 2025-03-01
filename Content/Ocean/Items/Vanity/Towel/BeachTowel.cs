using SpiritReforged.Common.Multiplayer;
using System.IO;

namespace SpiritReforged.Content.Ocean.Items.Vanity.Towel;

[AutoloadEquip(EquipType.HandsOn)]
public class BeachTowel : ModItem
{
	public const string BodyEquip = "BeachTowelBody";

	public override void UpdateVanity(Player player)
	{
		if (player.GetModPlayer<BeachTowelPlayer>().bodyEquip)
			player.GetModPlayer<BeachTowelPlayer>().shirtless = true;
	}

	public override void SetDefaults()
	{
		Item.width = Item.height = 26;
		Item.value = Item.buyPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
		Item.vanity = true;
	}
}

internal class BeachTowelPlayer : ModPlayer
{
	/// <summary> Whether the player has opted to be shirtless. </summary>
	public bool bodyEquip;
	/// <summary> Whether <see cref="bodyEquip"/> is true and <see cref="BeachTowel"/> is equipped in a vanity slot. </summary>
	public bool shirtless;

	public override void ResetEffects() => shirtless = false;
	public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) => new TowelVisibilityData(bodyEquip, (byte)Player.whoAmI).Send();

	public override void FrameEffects()
	{
		if (shirtless)
			Player.body = EquipLoader.GetEquipSlot(Mod, BeachTowel.BodyEquip, EquipType.Body);
	}
}

/// <summary> Syncs <see cref="BeachTowel"/> shirt visibility when updated from the local client. </summary>
internal class TowelVisibilityData : PacketData
{
	private readonly bool _visibility;
	private readonly byte _playerIndex;

	public TowelVisibilityData() { }
	public TowelVisibilityData(bool value, byte playerIndex)
	{
		_visibility = value;
		_playerIndex = playerIndex;
	}

	public override void OnReceive(BinaryReader reader, int whoAmI)
	{
		bool visibility = reader.ReadBoolean();
		byte player = reader.ReadByte();

		if (Main.netMode == NetmodeID.Server)
			new TowelVisibilityData(visibility, player).Send(ignoreClient: whoAmI);

		Main.player[player].GetModPlayer<BeachTowelPlayer>().bodyEquip = visibility;
	}

	public override void OnSend(ModPacket modPacket)
	{
		modPacket.Write(_visibility);
		modPacket.Write(_playerIndex);
	}
}