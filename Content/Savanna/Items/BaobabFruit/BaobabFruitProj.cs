using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace SpiritReforged.Content.Savanna.Items.BaobabFruit;

public class BaobabFruitProj : ModProjectile
{
	private enum DropType : byte
	{
		Fruit,
		Acorn,
		Worm
	}
	private DropType drop;

	public int Style { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; } //Determines our frame in PreDraw

	public override string Texture => base.Texture.Replace("Proj", "Tile");

	public override void SetDefaults()
	{
		Projectile.Size = new Vector2(10);
		Projectile.friendly = Projectile.hostile = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			var dropType = new WeightedRandom<DropType>();
			dropType.Add(DropType.Fruit, 1f);
			dropType.Add(DropType.Acorn, .9f);
			dropType.Add(DropType.Worm, .001f);

			drop = (DropType)dropType;

			Projectile.netUpdate = true;
		} //Pick a drop when the projectile spawns and sync it
	}

	public override void AI()
	{
		Projectile.rotation += Projectile.velocity.Length() * .08f;
		Projectile.velocity.Y += .5f;
	}

	public override void OnKill(int timeLeft)
	{
		void SpawnItem(int type)
		{
			int id = Item.NewItem(Projectile.GetSource_Death(), Projectile.Center, type);

			if (Main.dedServ)
				NetMessage.SendData(MessageID.SyncItem, number: id);
		}

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			switch (drop)
			{
				case DropType.Fruit:
					SpawnItem(ModContent.ItemType<BaobabFruit>());
					break;
				case DropType.Acorn:
					SpawnItem(ItemID.Acorn);
					break;
				case DropType.Worm:
					NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 16, ModContent.NPCType<DevourerOfSoil>());
					break;
			}
		}

		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.Center);
			if (drop != DropType.Fruit)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = 1f }, Projectile.Center);
				for (int i = 0; i < 20; i++)
				{
					var velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f);
					Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowBlock).velocity = velocity;
				}

				for (int i = 1; i < 3; i++)
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(),
						Mod.Find<ModGore>(nameof(BaobabFruitTile) + i).Type);
			} //Break open

			if (drop == DropType.Worm)
				SoundEngine.PlaySound(SoundID.Roar, Projectile.Center);
		}
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		fallThrough = false;
		return true;
	}

	public override bool? CanCutTiles() => false;

	public override void SendExtraAI(BinaryWriter writer) => writer.Write((byte)drop);

	public override void ReceiveExtraAI(BinaryReader reader) => drop = (DropType)reader.Read();

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = TextureAssets.Projectile[Type].Value;
		var frame = texture.Frame(2, 2, Style, 0, -2, -2);

		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), frame,
			Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None);

		return false;
	}
}