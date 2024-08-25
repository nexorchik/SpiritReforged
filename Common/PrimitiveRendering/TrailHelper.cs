using SpiritReforged.Common.PrimitiveRendering.Trail_Components;

namespace SpiritReforged.Common.PrimitiveRendering;

public enum TrailLayer
{
	UnderProjectile,
	UnderCachedProjsBehindNPC,
	AboveProjectile
}

public class TrailManager
{
	private readonly List<BaseTrail> _trails = new List<BaseTrail>();
	private readonly Effect _effect;

	private BasicEffect _basicEffect; //Not readonly due to thread queue

	public TrailManager(Mod mod)
	{
		_trails = [];
		_effect = mod.Assets.Request<Effect>("Assets/Shaders/trailShaders", AssetRequestMode.ImmediateLoad).Value;

		Main.QueueMainThreadAction(() => _basicEffect = new BasicEffect(Main.graphics.GraphicsDevice)
		{
			VertexColorEnabled = true
		});
	}

	public void TryTrailKill(Projectile projectile)
	{
		//Contained data originally for determining faster trail dissipation speeds for certain projectiles
		//But it was all hardcoded for spirit projectiles in this specific file and was a big if else chain and sucked
	}

	public void CreateTrail(Projectile projectile, ITrailColor trailType, ITrailCap trailCap, ITrailPosition trailPosition, float widthAtFront, float maxLength, ITrailShader shader = null, TrailLayer layer = TrailLayer.UnderProjectile, float dissolveSpeed = -1)
	{
		var newTrail = new VertexTrail(projectile, trailType, trailCap, trailPosition, shader ?? new DefaultShader(), layer, widthAtFront, maxLength, dissolveSpeed);
		newTrail.BaseUpdate();
		_trails.Add(newTrail);
	}

	public void CreateCustomTrail(BaseTrail trail)
	{
		trail.BaseUpdate();
		_trails.Add(trail);
	}

	public void UpdateTrails()
	{
		for (int i = 0; i < _trails.Count; i++)
		{
			BaseTrail trail = _trails[i];

			trail.BaseUpdate();
			if (trail.Dead)
			{
				_trails.RemoveAt(i);
				i--;
			}
		}
	}

	public void ClearAllTrails() => _trails.Clear();

	public void DrawTrails(SpriteBatch spriteBatch, TrailLayer layer)
	{
		foreach (BaseTrail trail in _trails)
			if (trail.Layer == layer)
				trail.Draw(_effect, _basicEffect, spriteBatch.GraphicsDevice);
	}

	public void TryEndTrail(Projectile projectile, float dissolveSpeed)
	{
		for (int i = 0; i < _trails.Count; i++)
		{
			BaseTrail trail = _trails[i];

			if (trail.MyProjectile.whoAmI == projectile.whoAmI && trail is VertexTrail t)
			{
				t.DissolveSpeed = dissolveSpeed;
				t.StartDissolve();
				return;
			}
		}
	}

	public static void ManualTrailSpawn(Projectile projectile)
	{
		if (projectile.ModProjectile is IManualTrailProjectile)
			if (Main.netMode == NetmodeID.SinglePlayer)
				(projectile.ModProjectile as IManualTrailProjectile).DoTrailCreation(SpiritReforgedLoadables.VertexTrailManager);

			else
			{// uhh put netcode here !!
			 //SpiritMod.WriteToPacket(SpiritMod.Instance.GetPacket(), (byte)MessageType.SpawnTrail, projectile.whoAmI).Send();
			}
	}
}

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