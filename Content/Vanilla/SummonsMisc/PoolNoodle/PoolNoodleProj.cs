using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritReforged.Common.ProjectileCommon;
using Terraria;
using Terraria.GameContent;

namespace SpiritReforged.Content.Vanilla.SummonsMisc.PoolNoodle;

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
}