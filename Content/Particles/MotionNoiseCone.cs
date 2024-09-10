using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;
using SpiritReforged.Common.PrimitiveRendering;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;

namespace SpiritReforged.Content.Particles;

public class MotionNoiseCone : Particle
{
	public bool UseLightColor { get; set; } = false;

	private float _opacity;

	private readonly int _maxTime;
	private readonly float _width;
	private readonly float _length;
	private readonly float _taper;
	private readonly Color _lightColor;

	public MotionNoiseCone(Vector2 position, Color darkColor, Color lightColor, float width, float length, float rotation, int maxTime, float tapering = 1)
	{
		Position = position;
		_maxTime = maxTime;
		Color = darkColor;
		_lightColor = lightColor;
		Rotation = rotation;
		_width = width;
		_length = length;
		_taper = tapering;
	}

	public MotionNoiseCone(Vector2 position, Color color, float width, float length, float rotation, int maxTime, float tapering = 1) : this(position, color, color, width, length, rotation, maxTime, tapering)
	{

	}

	public override void Update()
	{
		float Progress = GetProgress();
		Progress = EaseFunction.EaseQuadOut.Ease(Progress); //make the fade start fast and end slow
		Progress = (float)Math.Sin(Progress * MathHelper.Pi);
		_opacity = EaseFunction.EaseQuadIn.Ease(Progress); //then, bias the opacity to be closer to 0

		Velocity *= 0.9f;

		if (TimeActive > _maxTime)
			Kill();
	}

	private float GetProgress()
	{
		float progress = TimeActive / (float)_maxTime;

		return progress;
	}

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public override void CustomDraw(SpriteBatch spriteBatch)
	{
		Effect effect = AssetLoader.LoadedShaders["MotionNoiseCone"];
		Texture2D texture = AssetLoader.LoadedTextures["vnoise"];
		effect.Parameters["uTexture"].SetValue(texture);
		effect.Parameters["Tapering"].SetValue(_taper);

		var secondaryColorLerp = Color.Lerp(Color, _lightColor, _opacity);
		effect.Parameters["uColor"].SetValue(Color.ToVector4());
		effect.Parameters["uColor2"].SetValue(secondaryColorLerp.ToVector4());

		float progress = EaseFunction.EaseQuadOut.Ease(GetProgress()) / 4;
		effect.Parameters["progress"].SetValue(1 - progress);

		var noiseStretch = new Vector2(900f, 130f);
		effect.Parameters["xMod"].SetValue(_length / noiseStretch.X);
		effect.Parameters["yMod"].SetValue(_width / noiseStretch.Y);

		Color lightColor = Color.White;
		if (UseLightColor)
			lightColor = Lighting.GetColor(Position.ToTileCoordinates().X, Position.ToTileCoordinates().Y);

		var square = new SquarePrimitive
		{
			Color = lightColor * _opacity,
			Height = _width * EaseFunction.EaseCubicOut.Ease(_opacity),
			Length = _length,
			Position = Position - Main.screenPosition,
			Rotation = Rotation + MathHelper.Pi,
		};
		PrimitiveRenderer.DrawPrimitiveShape(square, effect);
	}

	public MotionNoiseCone UsesLightColor()
	{
		UseLightColor = true;
		return this;
	}

	public override ParticleLayer DrawLayer => ParticleLayer.AbovePlayer;
}
