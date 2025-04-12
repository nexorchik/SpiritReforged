using SpiritReforged.Common.Easing;
using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;
using SpiritReforged.Common.PrimitiveRendering.Trail_Components;
using static SpiritReforged.Common.Easing.EaseFunction;

namespace SpiritReforged.Common.PrimitiveRendering.CustomTrails;

public class SwingTrail(Projectile Projectile, Color Color, float Radians, float Rotation, Vector2 Dist, Vector2 Width, EaseFunction DistanceEasing, Func<Projectile, float> SwingProgress, Action<Effect, SwingTrail> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile) : BaseTrail(Projectile, layer)
{
	public SwingTrail(Projectile Projectile, Color Color, float Radians, float Rotation, float Dist, float Width, Func<Projectile, float> SwingProgress, Action<Effect, SwingTrail> ShaderParams, TrailLayer layer = TrailLayer.UnderProjectile) 
		: this(Projectile, Color, Radians, Rotation, new Vector2(Dist), new Vector2(Width), Linear, SwingProgress, ShaderParams, layer) { }

	public float DissolveSpeed { get; set; }
	public Color Color { get; } = Color;

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

		if (_swingProgress > 0.9f)
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

		ShaderParams(effect, this);

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
}