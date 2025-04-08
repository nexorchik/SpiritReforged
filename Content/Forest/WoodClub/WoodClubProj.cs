using SpiritReforged.Common.ProjectileCommon.Abstract;

namespace SpiritReforged.Content.Forest.WoodClub;

class WoodClubProj : BaseClubProj
{
	public WoodClubProj() : base(new Vector2(58)) { }

	public override float WindupTimeRatio => 0.6f;

	public override void OnSmash(Vector2 position)
	{
		Collision.HitTiles(Projectile.position, Vector2.UnitY, Projectile.width, Projectile.height);

		DustClouds(8);

		if(Charge == 1)
		{
			float angle = MathHelper.PiOver4 * 1.5f;
			if (Projectile.direction > 0)
				angle = -angle + MathHelper.Pi;

			DoShockwaveCircle(Vector2.Lerp(Projectile.Center, Main.player[Projectile.owner].Center, 0.5f), 280, angle, 0.4f);
		}

		DoShockwaveCircle(Projectile.Bottom - Vector2.UnitY * 8, 180, MathHelper.PiOver2, 0.4f);
	}
}
