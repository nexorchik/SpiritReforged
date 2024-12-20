using SpiritReforged.Common.Misc;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.ProjectileCommon;
using SpiritReforged.Content.Buffs.SummonTag;
using SpiritReforged.Content.Ocean.Items.Reefhunter.Particles;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.PoolNoodle;

public class PoolNoodleProj : BaseWhipProj
{
	private int Style
	{
		get => (int)Projectile.ai[1];
		set => Projectile.ai[1] = value;
	}

	public override void StaticDefaults() => Main.projFrames[Type] = 7;

	public override void Defaults()
	{
		Projectile.WhipSettings.RangeMultiplier = .8f;
		Projectile.WhipSettings.Segments = 16;
	}

	public override void ModifyDraw(int segment, int numSegments, ref Rectangle frame)
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		frame.Width = texture.Width / 3;
		frame.X = 16 * Style;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int i = 0; i < 3; i++)
			ParticleHandler.SpawnParticle(new BubbleParticle(target.Center, Main.rand.NextVec2CircularEven(2, 2), Main.rand.NextFloat(0.1f, 0.2f), 40));

		base.OnHitNPC(target, hit, damageDone);
		target.AddBuff(ModContent.BuffType<SummonTag3>(), 360);
		target.AddBuff(ModContent.BuffType<PoolNoodleBubbleBuff>(), 600);
	}
}