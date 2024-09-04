using SpiritReforged.Common.Misc;
using SpiritReforged.Common.PrimitiveRendering.Trail_Components;
using System;

namespace SpiritReforged.Common.PrimitiveRendering;

public enum TrailLayer
{
	UnderProjectile,
	UnderCachedProjsBehindNPC,
	AboveProjectile
}

public class TrailManager
{
	private readonly List<BaseTrail> _trails = new();
	private readonly Effect _effect;

	private BasicEffect _basicEffect; //Not readonly due to thread queue

	public TrailManager(Mod mod)
	{
		_trails = [];
		_effect = AssetLoader.LoadedShaders["trailShaders"];

		_basicEffect = AssetLoader.BasicShaderEffect;
	}

	public static void TryTrailKill(Projectile projectile)
	{
		//Contained data originally for determining faster trail dissipation speeds for certain projectiles
		//But it was all hardcoded for spirit projectiles in this specific file and was a big if else chain and sucked
		//
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
				(projectile.ModProjectile as IManualTrailProjectile).DoTrailCreation(AssetLoader.VertexTrailManager);

			else
			{
				ModPacket packet = SpiritReforgedMod.Instance.GetPacket(ReforgedMultiplayer.MessageType.SpawnTrail, 1);
				packet.Write(projectile.whoAmI);
				packet.Send();
			}
	}
}