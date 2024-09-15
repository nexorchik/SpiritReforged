using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;
using Newtonsoft.Json.Linq;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Particles;

public class DissipatingImage : Particle
{
	private readonly string _texture;
	private readonly float _maxDistortion;
	private float _opacity;
	public bool UseLightColor;
	public DissipatingImage(Vector2 position, Color color, float rotation, float scale, float maxDistortion, string texture, int maxTime)
	{
		Position = position;
		Rotation = rotation;
		Scale = scale;
		_texture = texture;
		_maxDistortion = maxDistortion;
		Color = color;
		MaxTime = maxTime;
	}

	public DissipatingImage UsesLightColor()
	{
		UseLightColor = true;
		return this;
	}

	public override void Update()
	{
		_opacity = EaseFunction.EaseQuadOut.Ease(Progress);
		_opacity = (float)Math.Sin(_opacity * MathHelper.Pi);
	}

	public override ParticleLayer DrawLayer => ParticleLayer.AboveProjectile;

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		if (!AssetLoader.LoadedTextures.TryGetValue(_texture, out Texture2D value))
			throw new ArgumentNullException(_texture, "Given input does not correspond to a loaded asset.");

		else
		{
			Effect effect = AssetLoader.LoadedShaders["DistortDissipateTexture"];
			effect.Parameters["uColor"].SetValue(Color.ToVector4());
			effect.Parameters["uTexture"].SetValue(value);
			effect.Parameters["perlinNoise"].SetValue(AssetLoader.LoadedTextures["noise"]);
			effect.Parameters["Progress"].SetValue(Progress);
			effect.Parameters["xMod"].SetValue(0.5f);
			effect.Parameters["yMod"].SetValue(0.5f);
			effect.Parameters["distortion"].SetValue(_maxDistortion * EaseFunction.EaseQuadIn.Ease(Progress));

			float texExponent = MathHelper.Lerp(4, 0.33f, _opacity);
			effect.Parameters["texExponent"].SetValue(texExponent);

			Color lightColor = Color.White;
			if (UseLightColor)
				lightColor = Lighting.GetColor(Position.ToTileCoordinates().X, Position.ToTileCoordinates().Y);

			float scaleMod = 1 + (Progress / 2);
			var square = new SquarePrimitive
			{
				Color = lightColor * _opacity,
				Height = Scale * value.Height * scaleMod,
				Length = Scale * value.Width * scaleMod,
				Position = Position - Main.screenPosition,
				Rotation = Rotation,
			};
			PrimitiveRenderer.DrawPrimitiveShape(square, effect);
		}
	}
}
