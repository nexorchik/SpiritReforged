namespace SpiritReforged.Common.PrimitiveRendering.Trail_Components;

public abstract class BaseTrail(Projectile projectile, TrailLayer layer)
{
	public bool Dead { get; set; } = false;
	public Projectile MyProjectile { get; set; } = projectile;
	public TrailLayer Layer { get; set; } = layer;

	private readonly int _originalProjectileType = projectile.type;
	private bool _dissolving = false;

	public void BaseUpdate()
	{
		if ((!MyProjectile.active || MyProjectile.type != _originalProjectileType) && !_dissolving)
			StartDissolve();

		if (_dissolving)
			Dissolve();
		else
			Update();
	}

	public void StartDissolve()
	{
		OnStartDissolve();
		_dissolving = true;
	}

	/// <summary>
	/// Behavior for the trail every tick, only called before the trail begins dying
	/// </summary>
	public virtual void Update() { }

	/// <summary>
	/// Behavior for the trail after it starts its death, called every tick after the trail begins dying
	/// </summary>
	public virtual void Dissolve() { }

	/// <summary>
	/// Additional behavior for the trail upon starting its death
	/// </summary>
	public virtual void OnStartDissolve() { }

	/// <summary>
	/// How the trail is drawn to the screen
	/// </summary>
	/// <param name="effect"></param>
	/// <param name="effect2"></param>
	/// <param name="device"></param>
	public virtual void Draw(Effect effect, BasicEffect effect2, GraphicsDevice device)
	{

	}
}
