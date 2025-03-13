using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering;

namespace SpiritReforged.Content.Snow.Frostbite;

public class MistParticle : Particle
{
	private readonly float _maxDistortion;
	private readonly Vector2 _noiseStretch = new(1);

	private float _opacity;
	private float _scaleMod = 1;

	public MistParticle(Vector2 position, Color color, float rotation, float scale, float maxDistortion, Vector2 noiseScale, int maxTime)
	{
		Position = position;
		Color = color;
		Rotation = rotation;
		Scale = scale;
		_maxDistortion = maxDistortion;
		_noiseStretch = noiseScale;
		MaxTime = maxTime;
	}

	public override void Update()
	{
		_opacity = EaseFunction.EaseQuadOut.Ease(Progress);
		_opacity = (float)Math.Sin(_opacity * MathHelper.Pi);
		_scaleMod = 1 + Progress / 2;

		var tile = Position.ToTileCoordinates16();
		Main.instance.TilesRenderer.Wind.GetWindTime(tile.X, tile.Y, 10, out _, out int dirX, out int dirY);
		Velocity += new Vector2(dirX, dirY) * .05f;

		int size = (int)(30 * Scale);
		if (Collision.SolidTiles(Position - new Vector2(size / 2), size, size))
			Velocity *= -.75f;
	}

	public override ParticleLayer DrawLayer => ParticleLayer.BelowSolids;

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		var texture = AssetLoader.LoadedTextures["SmokeSimple"].Value;
		Effect effect = AssetLoader.LoadedShaders["DistortDissipateTexture"];

		effect.Parameters["uColor"].SetValue(Color.ToVector4());
		effect.Parameters["uTexture"].SetValue(texture);
		effect.Parameters["perlinNoise"].SetValue(AssetLoader.LoadedTextures["noise"].Value);
		effect.Parameters["Progress"].SetValue(Progress);
		effect.Parameters["xMod"].SetValue(_noiseStretch.X);
		effect.Parameters["yMod"].SetValue(_noiseStretch.Y);
		effect.Parameters["distortion"].SetValue(_maxDistortion * EaseFunction.EaseQuadIn.Ease(Progress));
		effect.Parameters["texExponent"].SetValue(1);

		var lightColor = Lighting.GetColor(Position.ToTileCoordinates().X, Position.ToTileCoordinates().Y);

		var square = new SquarePrimitive
		{
			Color = lightColor * _opacity,
			Height = Scale * texture.Height * _scaleMod,
			Length = Scale * texture.Width * _scaleMod,
			Position = Position - Main.screenPosition,
			Rotation = Rotation,
		};
		PrimitiveRenderer.DrawPrimitiveShape(square, effect);
	}
}