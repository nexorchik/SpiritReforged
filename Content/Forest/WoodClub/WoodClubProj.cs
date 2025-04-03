using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using SpiritReforged.Common.ProjectileCommon;

namespace SpiritReforged.Content.Forest.WoodClub;

class WoodClubProj : BaseClubProj
{
	public WoodClubProj() : base(new Vector2(58)) { }

	public override void SafeSetStaticDefaults() => Main.projFrames[Projectile.type] = 2;

	public override void Smash(Vector2 position)
	{
		//chungus dust temporary
		for (int k = 0; k <= 100 * Charge; k++)
			Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0, -3);
	}
}
