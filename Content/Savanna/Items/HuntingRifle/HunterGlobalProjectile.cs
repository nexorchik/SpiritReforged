using SpiritReforged.Common.Visuals;
using System.IO;
using Terraria.ModLoader.IO;

namespace SpiritReforged.Content.Savanna.Items.HuntingRifle;

public class HunterGlobalProjectile : GlobalProjectile
{
	private const float damageMultiplier = 1.5f;
	public const float maxRange = 16 * 50; //At this range or greater, our full damage multiplier will be applied

	public bool firedFromHuntingRifle;

	public override bool InstancePerEntity => true;
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation && entity.CountsAsClass(DamageClass.Ranged) && !entity.arrow;

	private float GetMultiplier(Projectile proj)
		=> firedFromHuntingRifle ? MathHelper.Clamp(Main.player[proj.owner].Distance(proj.Center) / maxRange, 0, 1) * (damageMultiplier - 1f) : 0;

	public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
		=> modifiers.SourceDamage *= 1f + GetMultiplier(projectile);
	public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
		=> modifiers.SourceDamage *= 1f + GetMultiplier(projectile);

	public override bool PreDraw(Projectile projectile, ref Color lightColor)
	{
		const int trailLength = 8;

		if (!firedFromHuntingRifle)
			return true;

		Main.instance.LoadProjectile(873); //Ensure these textures are loaded before drawing
		Main.instance.LoadProjectile(686);

		var defaultTexture = TextureAssets.Projectile[projectile.type].Value;

		for (int i = 0; i < trailLength; i++)
		{
			var texture = TextureAssets.Projectile[873].Value;

			float lerp = 1f - i / (float)(trailLength - 1);
			var brightest = TextureColorCache.GetBrightestColor(defaultTexture);
			var color = (Color.Lerp(brightest.MultiplyRGBA(Color.Black * .5f), brightest, lerp) with { A = 0 }) * lerp;
			var position = projectile.Center - Main.screenPosition - projectile.velocity * i;
			var scale = new Vector2(.5f * lerp, 1f) * projectile.scale;

			if (i == 0)
			{
				color = Color.White with { A = 0 };
				texture = TextureAssets.Projectile[686].Value;
				scale = new Vector2(MathHelper.Max(.1f, .25f), 1f) * projectile.scale * .45f;
			}

			Main.EntitySpriteDraw(texture, position, null, color, projectile.rotation, texture.Size() / 2, scale, SpriteEffects.None);
		}

		return false;
	}

	//Sync hasDistanceMultiplier because it will be assigned to in local-client-only locations
	public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) => bitWriter.WriteBit(firedFromHuntingRifle);
	public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) => firedFromHuntingRifle = bitReader.ReadBit();
}
