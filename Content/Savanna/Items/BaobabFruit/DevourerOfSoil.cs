using SpiritReforged.Common.PlayerCommon;
using Terraria.Audio;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Savanna.Items.BaobabFruit;

public class DevourerOfSoil : ModNPC
{
	private static readonly Point[] Dimensions = [new Point(30, 38), new Point(22, 18), new Point(14, 22)]; //Excludes 2px(y) padding

	public readonly Vector2[] positions = new Vector2[Length];
	private const int Length = 8;

	private bool PlayingDeathAnimation => NPC.dontTakeDamage;

	private bool justSpawned = true;

	private bool InsideTiles()
	{
		var tile = Framing.GetTileSafely(NPC.Center);
		return WorldGen.SolidOrSlopedTile(tile);
	}

	public override void SetStaticDefaults()
	{
		var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

		Main.npcFrameCount[Type] = Dimensions.Length;
	}

	public override void SetDefaults()
	{
		NPC.Size = new Vector2(22);
		NPC.friendly = false;
		NPC.behindTiles = true;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 1;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = true;

		NPC.boss = true;
		Music = MusicID.Boss1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < positions.Length; i++)
		{
			int whoAmI = NPC.NewNPC(NPC.GetSource_FromAI(), 0, 0, ModContent.NPCType<DevourerOfSoilBody>(), 0, NPC.whoAmI, i); //Spawn child NPCs
			if (whoAmI == Main.maxNPCs) //We can't spawn all segments because we hit the NPC limit, so kill everyone
			{
				NPC.active = false;
				break;
			}
		}
	}

	public override void AI()
	{
		UpdatePositions();

		if (PlayingDeathAnimation)
		{
			DoDeathAnimation();
			return;
		}

		NPC.TargetClosest();
		var target = Main.player[NPC.target];

		if (justSpawned)
		{
			NPC.velocity = new Vector2(Math.Sign(target.Center.X - NPC.Center.X) * 2f, -4f); //Leap upwards on spawn
			justSpawned = false;
		}

		const float speed = 5.5f;
		if (InsideTiles())
		{
			NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * speed, .01f);

			if (Main.rand.NextBool(2))
			{
				var tilePos = (NPC.Center / 16).ToPoint();
				var dust = Main.dust[WorldGen.KillTile_MakeTileDust(tilePos.X, tilePos.Y, Framing.GetTileSafely(tilePos))];
				dust.fadeIn = Main.rand.NextFloat(1f, 1.25f);
				dust.noGravity = true;
			} //Spawn travel dusts

			if (NPC.soundDelay == 0)
			{
				int delay = (int)MathHelper.Clamp(NPC.Distance(target.Center) / 16f, 10, 20);
				NPC.soundDelay = delay;

				SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
			} //Play digging sounds based on distance
		}
		else
		{
			NPC.velocity = new Vector2(NPC.velocity.X * .98f, NPC.velocity.Y + .08f);
		}

		NPC.rotation = NPC.velocity.ToRotation();
	}

	private void UpdatePositions()
	{
		static Vector2 GetSegmentDims(int segment)
		{
			var dims = Dimensions[1];

			if (segment == 0)
				dims = Dimensions[0];
			else if (segment == Length - 1)
				dims = Dimensions[2];

			return dims.ToVector2();
		}

		if (justSpawned)
		{
			for (int i = 0; i < positions.Length; i++)
				positions[i] = NPC.Center + Vector2.UnitY * i * GetSegmentDims(i).Y;

			return;
		} //Set comfortable segment spawn positions

		positions[0] = NPC.Center;
		for (int i = 1; i < positions.Length; i++)
		{
			var ahead = positions[i - 1];
			var angle = positions[i].DirectionTo(ahead);

			positions[i] = ahead - angle * GetSegmentDims(i).Y;
		}
	}

	private void DoDeathAnimation()
	{
		void DoDeathEffects()
		{
			if (Main.dedServ)
				return;

			for (int i = 0; i < positions.Length; i++)
			{
				int id = (i == positions.Length - 1) ? 3 : (i == 0) ? 1 : 2;
				int goreType = Mod.Find<ModGore>(nameof(DevourerOfSoil) + id).Type;

				Gore.NewGore(NPC.GetSource_Death(), positions[i], Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f), goreType);

				for (int d = 0; d < 10; d++)
					Dust.NewDustPerfect(positions[i] + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f), 
						DustID.Blood, Main.rand.NextVector2Unit() * 2f, 0, default, Main.rand.NextFloat(1f, 1.5f));
			}

			SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
			QuickCameraModifiers.SimpleShakeScreen(Main.LocalPlayer, 3f, 3f, 60, 16 * 30);
		}

		if (InsideTiles())
		{
			if (NPC.localAI[0] == 0)
			{
				NPC.velocity.Y = -8f; //Shoot out of the ground before dying
			}
			else
			{
				DoDeathEffects();
				NPC.active = false;
			}
		}
		else
		{
			if (NPC.localAI[0] == 0)
			{
				var tilePos = (NPC.Center / 16).ToPoint();
				for (int i = 0; i < 10; i++)
				{
					var dust = Main.dust[WorldGen.KillTile_MakeTileDust(tilePos.X, tilePos.Y, Framing.GetTileSafely(tilePos))];
					dust.velocity = (NPC.velocity * Main.rand.NextFloat(.1f, .3f)).RotatedByRandom(1f);
					dust.scale = Main.rand.NextFloat(.9f, 1.5f);
				}

				NPC.localAI[0] = 1;
			} //One-time effects

			NPC.velocity = new Vector2(NPC.velocity.X * .98f, NPC.velocity.Y + .2f);
		}

		NPC.rotation = NPC.velocity.ToRotation();
	}

	public override void HitEffect(NPC.HitInfo hit) => NPC.velocity.Y -= 2f; //Send upwards slightly when hit

	public override bool CheckDead()
	{
		NPC.life = 1;
		NPC.dontTakeDamage = true; //Start our death animation

		return false;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (justSpawned)
			return false; //Don't bother drawing the first frame alive because rotation and segment positions haven't been initialized

		var texture = TextureAssets.Npc[Type].Value;
		int length = positions.Length - 1;

		for (int i = length; i >= 0; i--)
		{
			int frameY = (i == length) ? 2 : (i == 0) ? 0 : 1;
			var frame = texture.Frame(1, Main.npcFrameCount[Type], 0, frameY) with { Width = Dimensions[frameY].X, Height = Dimensions[frameY].Y };
			
			var position = positions[i] + new Vector2(0, NPC.gfxOffY);
			float rotation = (i == 0) ? NPC.rotation : positions[i].AngleTo(positions[i - 1]);

			var lightColor = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16));
			var color = NPC.GetAlpha(NPC.GetNPCColorTintedByBuffs(lightColor));

			Main.EntitySpriteDraw(texture, position - Main.screenPosition, frame, color, rotation + 1.57f, new Vector2(frame.Width / 2, frame.Height), NPC.scale, SpriteEffects.None);
		}

		return false;
	}
}

//Only used for recieving damage on other segments
public class DevourerOfSoilBody : ModNPC
{
	public int ParentWhoAmI { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
	public DevourerOfSoil Parent => (Main.npc[ParentWhoAmI] is NPC npc && npc.active && npc.ModNPC != null && npc.ModNPC is DevourerOfSoil dos) ? dos : null;

	public int Segment { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }

	public override string Texture => base.Texture.Replace("Body", string.Empty);

	public override LocalizedText DisplayName => Language.GetText("Mods.SpiritReforged.NPCs.DevourerOfSoil.DisplayName");

	public override void SetStaticDefaults()
	{
		var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
	}

	public override void SetDefaults()
	{
		NPC.Size = new Vector2(22);
		NPC.friendly = false;
		NPC.behindTiles = true;
		NPC.damage = 0;
		NPC.defense = 0;
		NPC.lifeMax = 1;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.npcSlots = 0;
		NPC.dontCountMe = true;
	}

	public override void AI()
	{
		if (Parent == null)
		{
			NPC.active = false;
			return;
		}

		NPC.Center = Parent.positions[Segment];
		NPC.dontTakeDamage = Parent.NPC.dontTakeDamage;
	}

	public override void HitEffect(NPC.HitInfo hit) => Parent?.NPC.StrikeNPC(hit);

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

	public override bool CheckActive() => false;

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => false;
}
