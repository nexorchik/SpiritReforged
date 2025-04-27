using SpiritReforged.Common.Easing;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering.Trail_Components;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.PrimitiveRendering.CustomTrails;

public class SwingTrail(Projectile Projectile, Color LightColor, Color DarkColor, float Intensity, float Radians, float TrailLength, float Rotation, Vector2 Dist, Vector2 Width, EaseFunction DistanceEasing, Func<Projectile, float> SwingProgress, Func<SwingTrail, Effect> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveThreshold = 0.9f, bool useLightColor = true) : BaseTrail(Projectile, layer)
{
	public SwingTrail(Projectile Projectile, Color LightColor, Color DarkColor, float Intensity, float Radians, float TrailLength, float Rotation, float Dist, float Width, Func<Projectile, float> SwingProgress, Func<SwingTrail, Effect> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveThreshold = 0.9f, bool useLightColor = true) 
		: this(Projectile, LightColor, DarkColor, Intensity, Radians, TrailLength, Rotation, new Vector2(Dist), new Vector2(Width), Linear, SwingProgress, ShaderParams, layer, dissolveThreshold, useLightColor) { }

	public SwingTrail(Projectile Projectile, Color Color, float Intensity, float Radians, float TrailLength, float Rotation, Vector2 Dist, Vector2 Width, EaseFunction DistanceEasing, Func<Projectile, float> SwingProgress, Func<SwingTrail, Effect> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveThreshold = 0.9f, bool useLightColor = true)
	: this(Projectile, Color, Color, Intensity, Radians, TrailLength, Rotation, Dist, Width, DistanceEasing, SwingProgress, ShaderParams, layer, dissolveThreshold, useLightColor) { }

	public SwingTrail(Projectile Projectile, Color Color, float Intensity, float Radians, float TrailLength, float Rotation, float Dist, float Width, Func<Projectile, float> SwingProgress, Func<SwingTrail, Effect> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveThreshold = 0.9f, bool useLightColor = true)
	: this(Projectile, Color, Color, Intensity, Radians, TrailLength, Rotation, new Vector2(Dist), new Vector2(Width), Linear, SwingProgress, ShaderParams, layer, dissolveThreshold, useLightColor) { }

	public Color LightColor { get; } = LightColor;
	public Color DarkColor { get; } = DarkColor;
	public float Intensity { get; } = Intensity;
	public float TrailLength { get; } = TrailLength;

	public string EffectPass = "CleanStreakPass";
	public float DissolveProgress => _timeLeft / 30f;

	private readonly Player _owner = Main.player[Projectile.owner];

	private int _timeLeft = 30;
	private int _direction = Main.player[Projectile.owner].direction;

	private float _swingProgress;

	private Vector2 _center;

	public override void Dissolve()
	{
		_timeLeft--;

		if (_timeLeft == 0)
			Dead = true;
	}

	public override void OnStartDissolve()
	{

	}

	public override void Update()
	{
		_swingProgress = SwingProgress(Projectile);
		_center = _owner.MountedCenter;
		_direction = _owner.direction;
		MyProjectile = Projectile;

		if (_swingProgress > dissolveThreshold)
		{
			_timeLeft -= 10;
			StartDissolve();
		}
	}

	public float GetSwingProgress() => _swingProgress;

	public override void Draw(Effect effect, BasicEffect _, GraphicsDevice device)
	{
		if (Dead || _timeLeft <= 1) 
			return;

		effect = ShaderParams(this);

		float minDist = Dist.X;
		float maxDist = Dist.Y;
		float opacityMod = EaseCubicIn.Ease(DissolveProgress);
		float minWidth = Width.X;
		float maxWidth = Width.Y;

		Vector2 pos = _center - Main.screenPosition;
		var slash = new PrimitiveSlashArc
		{
			BasePosition = pos,
			MinDistance = minDist,
			MaxDistance = maxDist,
			Width = minWidth,
			MaxWidth = maxWidth,
			AngleRange = new Vector2(Radians / 2 * Projectile.direction, -Radians / 2 * Projectile.direction) * -1,
			DirectionUnit = (_direction * Rotation).ToRotationVector2().RotatedBy(_direction < 0 ? MathHelper.Pi : 0),
			Color = Color.White * opacityMod,
			UseLightColor = useLightColor,
			DistanceEase = DistanceEasing,
			SlashProgress = _swingProgress,
			RectangleCount = 40
		};

		PrimitiveRenderer.DrawPrimitiveShape(slash, effect, EffectPass);
	}

	public static Effect BasicSwingShaderParams(SwingTrail swingTrail)
	{
		Effect effect;
		effect = AssetLoader.LoadedShaders["SwingTrails"];
		effect.Parameters["baseColorLight"].SetValue(swingTrail.LightColor.ToVector4());
		effect.Parameters["baseColorDark"].SetValue(swingTrail.DarkColor.ToVector4());

		effect.Parameters["trailLength"].SetValue(swingTrail.TrailLength * EaseQuadIn.Ease(swingTrail.DissolveProgress));
		effect.Parameters["taperStrength"].SetValue(0.25f);
		effect.Parameters["fadeStrength"].SetValue(0.5f);

		effect.Parameters["progress"].SetValue(swingTrail.GetSwingProgress());
		effect.Parameters["intensity"].SetValue(swingTrail.Intensity);

		return effect;
	}

	public static Effect NoiseSwingShaderParams(SwingTrail swingTrail, string texturePath, Vector2 coordMods)
	{
		Effect effect;
		effect = AssetLoader.LoadedShaders["SwingTrails"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures[texturePath].Value);
		effect.Parameters["baseColorLight"].SetValue(swingTrail.LightColor.ToVector4());
		effect.Parameters["baseColorDark"].SetValue(swingTrail.DarkColor.ToVector4());

		effect.Parameters["coordMods"].SetValue(coordMods);
		effect.Parameters["trailLength"].SetValue(swingTrail.TrailLength * EaseQuadIn.Ease(swingTrail.DissolveProgress));
		effect.Parameters["taperStrength"].SetValue(0.5f);
		effect.Parameters["fadeStrength"].SetValue(3);
		effect.Parameters["textureExponent"].SetValue(new Vector2(0.6f, 3));

		effect.Parameters["timer"].SetValue(0.5f * Main.GlobalTimeWrappedHourly / coordMods.X);
		effect.Parameters["progress"].SetValue(swingTrail.GetSwingProgress());
		effect.Parameters["intensity"].SetValue(swingTrail.Intensity);
		swingTrail.EffectPass = "NoiseStreakPass";

		return effect;
	}
}