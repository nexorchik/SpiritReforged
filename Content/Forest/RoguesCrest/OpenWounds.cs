using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Multiplayer;
using SpiritReforged.Common.Particle;
using System.IO;
using Terraria.Audio;

namespace SpiritReforged.Content.Forest.RoguesCrest;

public class OpenWounds : ModBuff
{
	public override string Texture => "Terraria/Images/Buff";

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}
}

internal class OpenWoundsNPC : GlobalNPC
{
	public override bool InstancePerEntity => true;

	private short _bleedTime;

	/// <summary> Causes <paramref name="npc"/> to take custom bleed damage for <paramref name="time"/> and consumes <see cref="OpenWounds"/>. </summary>
	public static bool Proc(NPC npc, short time)
	{
		if (npc.TryGetGlobalNPC(out OpenWoundsNPC gNPC))
		{
			gNPC._bleedTime = time;

			int index = npc.FindBuffIndex(ModContent.BuffType<OpenWounds>());
			if (index > -1)
				npc.DelBuff(index);

			if (!Main.dedServ)
				SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = .5f }, npc.Center);

			return true;
		}

		return false;
	}

	public override void UpdateLifeRegen(NPC npc, ref int damage)
	{
		if (_bleedTime > 0)
			npc.lifeRegen -= 20;

		_bleedTime = (short)Math.Max(_bleedTime - 1, 0);

		if (!Main.dedServ && Main.rand.NextBool(10) && _bleedTime > 0)
			ParticleHandler.SpawnParticle(new RedBubble(npc.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f), Color.White, Main.rand.NextFloat(.5f, 1f), 20));
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (npc.HasBuff<OpenWounds>())
		{
			int type = ModContent.ProjectileType<RogueKnifeMinion>();
			var icon = TextureAssets.Projectile[type].Value;
			var source = icon.Frame(1, Main.projFrames[type], 0, (int)(Main.timeForVisualEffects / 4 % Main.projFrames[type]), 0, -2);

			for (int i = 0; i < 2; i++)
				spriteBatch.Draw(icon, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), source, Color.Red.Additive(100), MathHelper.Pi, source.Size() / 2, .75f, default, 0);
		}
	}
}

internal class OpenWoundsPlayer : ModPlayer
{
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (hit.DamageType.CountsAsClass(DamageClass.Melee) && target.HasBuff<OpenWounds>())
		{
			short time = 60 * 5;

			if (OpenWoundsNPC.Proc(target, time) && Main.netMode == NetmodeID.MultiplayerClient)
				new BleedTimeData((short)target.whoAmI, time).Send();
		}
	}
}

/// <summary> Syncs <see cref="OpenWoundsNPC._bleedTime"/>. </summary>
internal class BleedTimeData : PacketData
{
	private readonly short _npc;
	private readonly short _time;

	public BleedTimeData() { }
	public BleedTimeData(short npc, short time)
	{
		_npc = npc;
		_time = time;
	}

	public override void OnReceive(BinaryReader reader, int whoAmI)
	{
		short npcIndex = reader.ReadInt16();
		short time = reader.ReadInt16();

		if (Main.netMode == NetmodeID.Server)
			new BleedTimeData(npcIndex, time).Send(ignoreClient: whoAmI); //Relay to other clients

		if (npcIndex > 0 && npcIndex < Main.maxNPCs)
			OpenWoundsNPC.Proc(Main.npc[npcIndex], time);
	}

	public override void OnSend(ModPacket modPacket)
	{
		modPacket.Write(_npc);
		modPacket.Write(_time);
	}
}