using SpiritReforged.Common.Easing;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering.Trail_Components;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.PrimitiveRendering.CustomTrails;

public class SwingTrail(Projectile Projectile, Color Color, float Intensity, float Radians, float Rotation, Vector2 Dist, Vector2 Width, EaseFunction DistanceEasing, Func<Projectile, float> SwingProgress, Func<SwingTrail, Effect> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveThreshold = 0.9f) : BaseTrail(Projectile, layer)
{
	public SwingTrail(Projectile Projectile, Color Color, float Intensity, float Radians, float Rotation, float Dist, float Width, Func<Projectile, float> SwingProgress, Func<SwingTrail, Effect> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveThreshold = 0.9f) 
		: this(Projectile, Color, Intensity, Radians, Rotation, new Vector2(Dist), new Vector2(Width), Linear, SwingProgress, ShaderParams, layer, dissolveThreshold) { }

	public float DissolveSpeed { get; set; }
	public Color Color { get; } = Color;
	public float Intensity { get; } = Intensity;

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
			StartDissolve();
			_timeLeft -= 10;
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
		bool useLightColor = true;
		float opacityMod = EaseCubicIn.Ease(_timeLeft / 30f);
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
			RectangleCount = 30
		};
		PrimitiveRenderer.DrawPrimitiveShape(slash, effect);
	}

	public static Effect BasicSwingShaderParams(SwingTrail swingTrail)
	{
		Effect effect;
		effect = AssetLoader.LoadedShaders["SwingTrails"];
		effect.Parameters["baseTexture"].SetValue(AssetLoader.LoadedTextures["supPerlin"].Value);
		effect.Parameters["baseColorLight"].SetValue(swingTrail.Color.ToVector4());
		effect.Parameters["baseColorDark"].SetValue(swingTrail.Color.ToVector4());

		effect.Parameters["coordMods"].SetValue(new Vector2(0.7f, 1f));
		effect.Parameters["textureExponent"].SetValue(new Vector2(0.5f, 1.25f));

		effect.Parameters["timer"].SetValue(Main.GlobalTimeWrappedHourly);
		effect.Parameters["progress"].SetValue(swingTrail.GetSwingProgress());
		effect.Parameters["intensity"].SetValue(swingTrail.Intensity);
		effect.Parameters["opacity"].SetValue(1);
		return effect;
	}
}