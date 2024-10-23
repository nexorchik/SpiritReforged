using SpiritReforged.Common.SimpleEntity;
using Terraria.Audio;
using static SpiritReforged.Common.Misc.ReforgedMultiplayer;

namespace SpiritReforged.Content.Ocean.Items.Buoys;

public class BigBuoy : SmallBuoy
{
	public override bool? UseItem(Player player)
	{
		if (player.whoAmI == Main.myPlayer && player.ItemAnimationJustStarted)
		{
			int type = SimpleEntitySystem.types[typeof(BigBuoyEntity)];
			var position = Main.MouseWorld;

			SimpleEntitySystem.NewEntity(type, position);

			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.SpawnSimpleEntity, 2);
				packet.Write(type);
				packet.WriteVector2(position);
				packet.Send();
			}

			return true;
		}

		return null;
	}

	public override void AddRecipes() => CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.IronBar, 8)
			.AddIngredient(ItemID.Wire, 7)
			.AddIngredient(ItemID.Glass, 7)
			.AddTile(TileID.Anvils)
			.Register();
}

public class BigBuoyEntity : SmallBuoyEntity
{
	private static Asset<Texture2D> GlowTexture;

	public override Texture2D Glowmask => GlowTexture.Value;

	public override void Load()
	{
		if (!Main.dedServ)
			GlowTexture = ModContent.Request<Texture2D>(TexturePath + "_Glow");

		saveMe = true;
		width = 46;
		height = 120;
	}

	public override void OnKill()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
			Item.NewItem(GetSource_Death(), Hitbox, ModContent.ItemType<BigBuoy>());

		SoundEngine.PlaySound(SoundID.Dig, Center);
	}
}