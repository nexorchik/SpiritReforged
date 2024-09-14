namespace SpiritReforged.Common.Visuals.Glowmasks;

internal class GlowmaskProjectile : GlobalProjectile
{
	public static Dictionary<int, GlowmaskInfo> ProjIdToGlowmask = [];

	public override void PostDraw(Projectile projectile, Color lightColor)
	{
		if (ProjIdToGlowmask.TryGetValue(projectile.type, out var glow) && glow.DrawAutomatically)
		{
			Vector2 pos = projectile.Center - Main.screenPosition + new Vector2(0, projectile.gfxOffY);
			SpriteEffects effect = projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			int frameHeight = glow.Glowmask.Height() / Main.projFrames[projectile.type];
			Rectangle frame = new Rectangle(0, frameHeight * projectile.frame, glow.Glowmask.Width(), frameHeight);
			Main.EntitySpriteDraw(glow.Glowmask.Value, pos, frame, glow.GetDrawColor(projectile), projectile.rotation, frame.Size() / 2f, projectile.scale, effect, 0);
		}
	}
}
