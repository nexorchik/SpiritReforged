using SpiritReforged.Common.Particle;
using static SpiritReforged.Common.Easing.EaseFunction;
using static Microsoft.Xna.Framework.MathHelper;

namespace SpiritReforged.Content.Particles;

public class Shatter : Particle
{
	Color _baseColor;

	public Shatter(Vector2 position, Color baseColor, int maxTime)
	{
		Position = position;
		Rotation = Main.rand.NextFloat(TwoPi);
		MaxTime = maxTime;
		Scale = 0.3f;
		_baseColor = baseColor;
	}

	public override void Update()
	{
		Color = _baseColor * EaseQuadOut.Ease(1 - Progress);
		Scale = Lerp(0.48f, 0.55f, EaseSine.Ease(EaseCubicOut.Ease(Progress)));
	}

	public override ParticleDrawType DrawType => ParticleDrawType.CustomBatchedAdditiveBlend;

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		Color lightColor = Lighting.GetColor(Position.ToTileCoordinates());
		lightColor = Color.Lerp(Color, Color.MultiplyRGBA(lightColor), 0.5f);
		Texture2D gradient = AssetLoader.LoadedTextures["Bloom"].Value;
		spriteBatch.Draw(gradient, Position - Main.screenPosition, null, _baseColor.MultiplyRGB(lightColor) * EaseCubicIn.Ease(1 - Progress) * .75f, 0, gradient.Size() / 2, Scale, SpriteEffects.None, 0);

		spriteBatch.Draw(Texture, Position - Main.screenPosition - Vector2.UnitX, null, lightColor.MultiplyRGB(new Color(255, 0, 0)), Rotation, Texture.Size() / 2, Scale, SpriteEffects.None, 0);
		spriteBatch.Draw(Texture, Position - Main.screenPosition, null, lightColor.MultiplyRGB(new Color(0, 255, 0)), Rotation, Texture.Size() / 2, Scale, SpriteEffects.None, 0);
		spriteBatch.Draw(Texture, Position - Main.screenPosition + Vector2.UnitX, null, lightColor.MultiplyRGB(new Color(0, 0, 255)), Rotation, Texture.Size() / 2, Scale, SpriteEffects.None, 0);

		float opacity = 0.3f;
		float scale = Lerp(1.25f, 1f, Progress);
		spriteBatch.Draw(Texture, Position - Main.screenPosition - Vector2.UnitX, null, lightColor.MultiplyRGB(new Color(255, 0, 0)) * opacity, Rotation + Pi, Texture.Size() / 2, Scale * scale, SpriteEffects.None, 0);
		spriteBatch.Draw(Texture, Position - Main.screenPosition, null, lightColor.MultiplyRGB(new Color(0, 255, 0)) * opacity, Rotation + Pi, Texture.Size() / 2, Scale * scale, SpriteEffects.None, 0);
		spriteBatch.Draw(Texture, Position - Main.screenPosition + Vector2.UnitX, null, lightColor.MultiplyRGB(new Color(0, 0, 255)) * opacity, Rotation + Pi, Texture.Size() / 2, Scale * scale, SpriteEffects.None, 0);
	}
}
