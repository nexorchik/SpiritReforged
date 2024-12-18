using SpiritReforged.Common.PlayerCommon;
using Terraria.Audio;

namespace SpiritReforged.Content.Forest.Safekeeper;

public class UndeadNPC : GlobalNPC
{
	internal static readonly HashSet<int> undeadTypes = [NPCID.Zombie, NPCID.Drippler, NPCID.Skeleton, NPCID.DemonEye];

	private static readonly HashSet<NPC> toDraw = [];
	private static bool trackingGore;

	private const float decayRate = .025f;
	private float decayTime = 1;

	private static bool ShouldTrackGore(NPC self) => self.life <= 0 && self.TryGetGlobalNPC(out UndeadNPC _) && Main.player[self.lastInteraction].HasAccessory<SafekeeperRing>();

	public override bool InstancePerEntity => true;
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => undeadTypes.Contains(entity.type);

	public override void Load()
	{
		On_NPC.VanillaHitEffect += TrackGore_Vanilla;
		On_NPC.HitEffect_HitInfo += TrackGore_HitInfo;
		On_Gore.NewGore_Vector2_Vector2_int_float += StopGore;
		On_Main.DrawNPCs += DrawNPCsinQueue;
	}

	private static void TrackGore_Vanilla(On_NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg, bool instantKill)
	{
		trackingGore = ShouldTrackGore(self);

		orig(self, hitDirection, dmg, instantKill);

		trackingGore = false;
	}

	private static void TrackGore_HitInfo(On_NPC.orig_HitEffect_HitInfo orig, NPC self, NPC.HitInfo hit)
	{
		trackingGore = ShouldTrackGore(self);

		orig(self, hit);

		trackingGore = false;
	}

	private static int StopGore(On_Gore.orig_NewGore_Vector2_Vector2_int_float orig, Vector2 Position, Vector2 Velocity, int Type, float Scale)
	{
		if (trackingGore)
			return 0; //Skips orig

		return orig(Position, Velocity, Type, Scale);
	}

	private static void DrawNPCsinQueue(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
	{
		const float spotScale = .5f;

		orig(self, behindTiles);

		if (toDraw.Count == 0)
			return; //Nothing to draw; don't restart the spritebatch

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, default, null, null, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

		var effect = AssetLoader.LoadedShaders["NoiseFade"];

		foreach (var npc in toDraw) //Draw all shader-affected NPCs
		{
			var effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var texture = TextureAssets.Npc[npc.type].Value;
			var frame = npc.frame with { Y = 0 };

			float decayTime = 0;
			if (npc.TryGetGlobalNPC(out UndeadNPC gNPC))
				decayTime = gNPC.decayTime;

			effect.Parameters["power"].SetValue(decayTime * 500f);
			effect.Parameters["size"].SetValue(new Vector2(1, Main.npcFrameCount[npc.type]) * spotScale);
			effect.Parameters["noiseTexture"].SetValue(AssetLoader.LoadedTextures["vnoise"]);
			effect.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), frame, Color.Black, npc.rotation, frame.Size() / 2, npc.scale, effects, 0);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin();

		toDraw.Clear();
	}

	public override bool CheckDead(NPC npc)
	{
		if (!Main.player[npc.lastInteraction].HasAccessory<SafekeeperRing>())
			return true;

		decayTime -= decayRate;
		npc.life = 1;
		npc.dontTakeDamage = true;

		npc.NPCLoot();
		SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/NPCDeath/Fire_1") with { Pitch = .25f, PitchVariance = .2f }, npc.Center);

		return false;
	}

	public override void PostAI(NPC npc)
	{
		if (decayTime == 1)
			return;

		if ((decayTime -= decayRate) <= 0)
		{
			SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/NPCDeath/Dust_1") with { Pitch = .5f, PitchVariance = .25f }, npc.Center);
			npc.active = false;
		}

		var dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Asphalt, Scale: Main.rand.NextFloat(.5f, 1.5f));
		dust.velocity = -npc.velocity;
		dust.noGravity = true;
		dust.color = new Color(25, 20, 20);
	}

	public override bool CanHitNPC(NPC npc, NPC target) => decayTime == 1;
	public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => decayTime == 1;

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (decayTime == 1)
			return true;

		toDraw.Add(npc);
		return false;
	}
}
