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
	private Entity _attatchedEntity = null;
	private Vector2 _offset = Vector2.Zero;

	private int _detatchTime;
	private Vector2 _oldPosition;

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
		_opacity = 1;
	}

	public MotionNoiseCone(Vector2 position, Color color, float width, float length, float rotation, int maxTime, float tapering = 1) : this(position, color, color, width, length, rotation, maxTime, tapering)
	{

	}

	public MotionNoiseCone(Entity entity, Vector2 basePosition, Color darkColor, Color lightColor, float width, float length, float rotation, int maxTime, float tapering = 1, int detatchTime = -1) : this(basePosition, darkColor, lightColor, width, length, rotation, maxTime, tapering)
	{
		_attatchedEntity = entity;
		_offset = basePosition - entity.Center;
		_detatchTime = detatchTime;
	}

	public override void Update()
	{
		if (TimeActive == _detatchTime && _attatchedEntity != null)
			_attatchedEntity = null;

		if (_attatchedEntity != null)
		{
			if (!_attatchedEntity.active)
			{
				Kill();
				return;
			}

			Position = _attatchedEntity.Center + _offset;
		}

		else
			Velocity *= 0.97f;

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

		float progress = EaseFunction.EaseCubicOut.Ease(GetProgress()) + Main.GlobalTimeWrappedHourly / 4;
		effect.Parameters["scroll"].SetValue(1 - progress);
		float dissipation = (float)Math.Pow(Math.Max(1.33f * (GetProgress() - 0.25f), 0), 1.25f);

		effect.Parameters["dissipation"].SetValue(dissipation);

		var secondaryColorLerp = Color.Lerp(Color, _lightColor, 1 - dissipation);
		effect.Parameters["uColor"].SetValue(Color.ToVector4());
		effect.Parameters["uColor2"].SetValue(secondaryColorLerp.ToVector4());

		var noiseStretch = new Vector2(1000f, 200f);
		effect.Parameters["xMod"].SetValue(_length / noiseStretch.X);
		effect.Parameters["yMod"].SetValue(_width / noiseStretch.Y);

		effect.Parameters["texExponentLerp"].SetValue(_textureExponent);
		effect.Parameters["numColors"].SetValue(_numColors);
		effect.Parameters["colorLerpExponent"].SetValue(_colorLerpExponent);
		effect.Parameters["finalIntensityMod"].SetValue(_intensityMod);
		effect.Parameters["TaperExponent"].SetValue(_taperingExponent);

		Color lightColor = Color.White;
		if (UseLightColor)
			lightColor = Lighting.GetColor(Position.ToTileCoordinates().X, Position.ToTileCoordinates().Y);

		var square = new SquarePrimitive
		{
			Color = lightColor * _opacity,
			Height = _width * MathHelper.Lerp(1 - dissipation, 1, 0.5f),
			Length = _length,
			Position = Position - Main.screenPosition,
			Rotation = Rotation + MathHelper.Pi,
		};
		PrimitiveRenderer.DrawPrimitiveShape(square, effect);
	}

	private float _textureExponent = 1f;
	private int _numColors = 10;
	private float _colorLerpExponent = 1f;
	private float _intensityMod = 1f;
	private float _taperingExponent = 1f;

	/// <summary>
	/// Sets additional data regarding the shader applied to this particle.
	/// </summary>
	/// <param name="usesLightColor">Whether or not the particle uses the light color at its position.</param>
	/// <param name="textureExponent">Exponentiates the exponent applied to the texture sample as the coordinates go from 0 to 1. A higher exponent here means the exponent on the texture is weaker.</param>
	/// <param name="numColors">The number of colors allowed for posterization</param>
	/// <param name="colorInterpolationExponent">The exponent at which it interpolates from the dark color to the light color.</param>
	/// <param name="finalColorModifier">A modifier applied after all other calculations in the shader</param>
	/// <param name="taperingExponent">The exponent applied to the tapering of the shader. Below 1 makes it curve outwards, above 1 makes it curve inwards.</param>
	/// <returns></returns>
	public MotionNoiseCone SetExtraData(bool usesLightColor = false, float textureExponent = 1, int numColors = 10, float colorInterpolationExponent = 1, float finalColorModifier = 1, float taperingExponent = 1)
	{
		UseLightColor = usesLightColor;
		_textureExponent = textureExponent;
		_numColors = numColors;
		_colorLerpExponent = colorInterpolationExponent;
		_intensityMod = finalColorModifier;
		_taperingExponent = taperingExponent;

		return this;
	}

	public override ParticleLayer DrawLayer => ParticleLayer.AbovePlayer;
}
