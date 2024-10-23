using SpiritReforged.Common.ItemCommon.FloatingItem;
using SpiritReforged.Common.SimpleEntity;
using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.Audio;
using static SpiritReforged.Common.Misc.ReforgedMultiplayer;

namespace SpiritReforged.Content.Ocean.Items;

public class FishLure : FloatingItem
{
	private static bool WaterBelow()
	{
		for (int i = 0; i < 2; i++)
		{
			var tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY + i);
			if (i == 0 && tile.LiquidAmount >= 255)
				return false; //Already completely submerged

			if (tile.LiquidAmount > 20 && tile.LiquidType == LiquidID.Water)
				return true;
		}

		return false;
	}

	public override float SpawnWeight => .008f;
	public override float Weight => base.Weight * 0.9f;
	public override float Bouyancy => base.Bouyancy * 1.08f;

	public override void SetDefaults()
	{
		Item.width = Item.height = 14;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.value = Item.sellPrice(silver: 10);
	}

	public override void HoldItem(Player player)
	{
		if (CanUseItem(player))
		{
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = Type;
		}
	}

	public override bool CanUseItem(Player player) => WaterBelow() && player.IsTargetTileInItemRange(Item);

	public override bool? UseItem(Player player)
	{
		if (player.whoAmI == Main.myPlayer && player.ItemAnimationJustStarted)
		{
			int type = SimpleEntitySystem.types[typeof(FishLureEntity)];
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
}

public class FishLureEntity : SimpleEntity
{
	public override void Load()
	{
		Size = new Vector2(8);
		saveMe = true;
	}

	public override void Update()
	{
		if (Collision.WetCollision(position, width, height))
			velocity.Y -= .05f;
		else if (!Collision.WetCollision(position, width, height + 2))
			velocity.Y += .1f;
		else
			velocity.Y *= .75f;

		position += velocity;

		//Proximity check
		var player = Main.LocalPlayer;
		var distance = new Vector2(Math.Abs(player.Center.X - Center.X), Math.Abs(player.Center.Y - Center.Y));
		if (distance.X < Main.buffScanAreaWidth / 2 && distance.Y < Main.buffScanAreaHeight / 2)
			player.GetModPlayer<OceanPlayer>().nearLure = true;

		//Pickaxe check
		var heldItem = player.HeldItem;
		if (heldItem != null && Hitbox.Contains(Main.MouseWorld.ToPoint()) && player.IsTargetTileInItemRange(heldItem) && player.HeldItem.pick > 0 && player.ItemAnimationJustStarted)
		{
			Kill();

			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = SpiritReforgedMod.Instance.GetPacket(MessageType.KillSimpleEntity, 1);
				packet.Write(whoAmI);
				packet.Send();
			}
		}
	}

	public override void OnKill()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
			Item.NewItem(GetSource_Death(), Hitbox, ModContent.ItemType<FishLure>());

		SoundEngine.PlaySound(SoundID.Dig, Center);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		float Sin(float numerator) => (float)Math.Sin((Main.timeForVisualEffects + Center.X) / numerator);

		var drawPosition = Center - Main.screenPosition + new Vector2(0, Sin(30f));
		var color = Lighting.GetColor((int)(Center.X / 16), (int)(Center.Y / 16));

		float rotation = Main.instance.TilesRenderer.GetWindCycle((int)(position.X / 16), (int)(position.Y / 16), TileSwaySystem.Instance.SunflowerWindCounter);
		rotation += TileSwayHelper.GetHighestWindGridPushComplex((int)(position.X / 16), (int)(position.Y / 16), 2, 3, 120, 1f, 5, true);

		spriteBatch.Draw(Texture.Value, drawPosition, null, color, rotation * .1f, Texture.Size() / 2, 1, SpriteEffects.None, 0f);
	}
}