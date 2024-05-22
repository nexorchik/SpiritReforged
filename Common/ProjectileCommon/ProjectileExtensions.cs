namespace SpiritReforged.Common.ProjectileCommon;

internal static class ProjectileExtensions
{
	public static Rectangle DrawFrame(this Projectile projectile)
	{
		Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
		return new Rectangle(0, projectile.frame * texture.Height / Main.projFrames[projectile.type], texture.Width, texture.Height / Main.projFrames[projectile.type]);
	}

	public static void Bounce(this Projectile projectile, Vector2 oldVelocity, float VelocityKeptRatio = 1f) 
		=> projectile.velocity = new Vector2((projectile.velocity.X == oldVelocity.X) 
			? projectile.velocity.X 
			: -oldVelocity.X * VelocityKeptRatio, 
			(projectile.velocity.Y == oldVelocity.Y) 
			? projectile.velocity.Y 
			: -oldVelocity.Y * VelocityKeptRatio);

	/// <summary>
	/// Draws the projectile similar to how vanilla would by default.
	/// </summary>
	/// <param name="proj">The projectile to draw.</param>
	/// <param name="batch">The batch to draw from. If null, this method will use <see cref="Main"/>'s EntitySpriteDraw instead of <see cref="SpriteBatch"/>'s Draw.</param>
	/// <param name="rot">The projectile's rotation. If null, uses the projectile's rotation.</param>
	/// <param name="effect">The sprite effect. If null, will use <see cref="Projectile.spriteDirection"/> to get the appropriate effect.</param>
	/// <param name="color">The draw color. If null, will use <see cref="Lighting.GetColor(int, int)"/> at the projectile's center.</param>
	/// <param name="origin">The draw origin. If null, will use the half-size of the projectile's current frame size.</param>
	public static void QuickDraw(this Projectile proj, SpriteBatch batch = null, float? rot = null, SpriteEffects? effect = null, Color? color = null, Vector2? origin = null)
	{
		Texture2D tex = TextureAssets.Projectile[proj.type].Value;
		Color col = color ?? Lighting.GetColor((int)proj.Center.X / 16, (int)proj.Center.Y / 16);
		effect ??= proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		if (batch == null)
			Main.EntitySpriteDraw(tex, proj.Center - Main.screenPosition, proj.DrawFrame(), proj.GetAlpha(col), rot ?? proj.rotation,
				origin ?? proj.DrawFrame().Size() / 2, proj.scale, effect ?? (proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
		else
			batch.Draw(tex, proj.Center - Main.screenPosition, proj.DrawFrame(), proj.GetAlpha(col), rot ?? proj.rotation,
				origin ?? proj.DrawFrame().Size() / 2, proj.scale, effect ?? (proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
	}

	/// <summary>
	/// Draws the projectile similar to how vanilla would by default.
	/// </summary>
	/// <param name="proj">The projectile to draw.</param>
	/// <param name="batch">The batch to draw from. If null, this method will use <see cref="Main"/>'s EntitySpriteDraw instead of <see cref="SpriteBatch"/>'s Draw.</param>
	/// <param name="baseOpacity">The base opacity of the drawn sprite, which is used to multiply by the fadeout of the trail.</param>
	/// <param name="rotation">The projectile's rotation. If null, uses the projectile's rotation.</param>
	/// <param name="effect">The sprite effect. If null, will use <see cref="Projectile.spriteDirection"/> to get the appropriate effect.</param>
	/// <param name="drawColor">The draw color. If null, will use <see cref="Lighting.GetColor(int, int)"/> at the projectile's center.</param>
	/// <param name="drawOrigin">The draw origin. If null, will use the half-size of the projectile's current frame size.</param>
	public static void QuickDrawTrail(this Projectile proj, SpriteBatch batch = null, float baseOpacity = 0.5f, float? rotation = null, 
		SpriteEffects? effect = null, Color? drawColor = null, Vector2? drawOrigin = null)
	{
		Texture2D tex = TextureAssets.Projectile[proj.type].Value;
		Color color = drawColor ?? Lighting.GetColor((int)proj.Center.X / 16, (int)proj.Center.Y / 16);
		effect ??= proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[proj.type]; i++)
		{
			float opacityMod = (ProjectileID.Sets.TrailCacheLength[proj.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[proj.type];
			opacityMod *= baseOpacity;
			Vector2 drawPosition = proj.oldPos[i] + proj.Size / 2 - Main.screenPosition;

			if (batch == null)
				Main.EntitySpriteDraw(tex, drawPosition, proj.DrawFrame(), proj.GetAlpha(color) * opacityMod,
					rotation ?? proj.oldRot[i], drawOrigin ?? proj.DrawFrame().Size() / 2, proj.scale,
					effect ?? (proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
			else
				batch.Draw(tex, drawPosition, proj.DrawFrame(), proj.GetAlpha(color) * opacityMod,
					rotation ?? proj.oldRot[i], drawOrigin ?? proj.DrawFrame().Size() / 2, proj.scale,
					effect ?? (proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
		}
	}
}
