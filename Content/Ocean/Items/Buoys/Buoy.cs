using SpiritReforged.Common.TileCommon.TileSway;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.Buoys;

public class Buoy : ModItem
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

	public virtual int SpawnNPCType => ModContent.NPCType<Buoy_World>();

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
		Item.value = Item.sellPrice(silver: 2);
		Item.makeNPC = SpawnNPCType;
	}

	public override bool CanUseItem(Player player) => WaterBelow() && player.IsTargetTileInItemRange(Item);
}

public class Buoy_World : ModNPC
{
	private static Asset<Texture2D> GlowTexture;

	public virtual Texture2D Glowmask => GlowTexture.Value;

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.Items." + Name.Replace("_World", string.Empty) + ".DisplayName");

	public override void Load()
	{
		if (!Main.dedServ)
			GlowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
	}

	public override void SetStaticDefaults()
	{
		var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
	}

	public sealed override void SetDefaults()
	{
		NPC.dontTakeDamage = true;
		NPC.lifeMax = 1;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.dontCountMe = true;
		NPC.npcSlots = 0;
		NPC.ShowNameOnHover = false;

		PostDefaults();
	}

	public virtual void PostDefaults() => NPC.Size = new Vector2(16, 18);

	public override void AI()
	{
		if (Collision.WetCollision(NPC.position, NPC.width, NPC.height + 8))
			NPC.velocity.Y -= .05f;
		else if (!Collision.WetCollision(NPC.position, NPC.width, NPC.height + 12))
			NPC.velocity.Y += .1f;
		else
			NPC.velocity.Y *= .75f;

		Lighting.AddLight(NPC.Top, .3f, .1f, .1f);

		#region pickaxe check
		var player = Main.LocalPlayer;
		var heldItem = player.HeldItem;

		if (heldItem != null && NPC.getRect().Contains(Main.MouseWorld.ToPoint()) && player.IsTargetTileInItemRange(heldItem) && player.HeldItem.pick > 0 && player.ItemAnimationJustStarted)
			NPC.SimpleStrikeNPC(1, 1);
		#endregion
	}

	public override void OnKill() => SoundEngine.PlaySound(SoundID.Dig, NPC.Center);

	public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.AddCommon<Buoy>();

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

	public override bool NeedSaving() => true;

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		float Sin(float numerator) => (float)Math.Sin((Main.timeForVisualEffects + NPC.Center.X) / numerator);

		var texture = TextureAssets.Npc[Type].Value;
		var origin = new Vector2(texture.Width / 2, texture.Height);
		var drawPosition = NPC.position - screenPos + new Vector2(0, Sin(30f) + NPC.gfxOffY) + origin;
		var color = Lighting.GetColor((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16));

		float rotation = Main.instance.TilesRenderer.GetWindCycle((int)(NPC.position.X / 16), (int)(NPC.position.Y / 16), TileSwaySystem.Instance.SunflowerWindCounter);
		rotation += TileSwayHelper.GetHighestWindGridPushComplex((int)(NPC.position.X / 16), (int)(NPC.position.Y / 16), 2, 3, 120, 1f, 5, true);

		spriteBatch.Draw(texture, drawPosition, null, color, rotation * .1f, origin, 1, SpriteEffects.None, 0f);

		var glowColor = Color.White * (1f - Sin(10f) * .3f);
		spriteBatch.Draw(Glowmask, drawPosition, null, glowColor, rotation * .1f, origin, 1, SpriteEffects.None, 0f);

		return false;
	}
}