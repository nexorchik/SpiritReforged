namespace SpiritReforged.Common.Particle;

public enum ParticleLayer
{
	BelowProjectile,
	AboveProjectile,
	AboveNPC,
	AbovePlayer
}

public enum ParticleDrawType
{
	DefaultAlphaBlend,
	DefaultAdditive,
	Custom
}

public static class ParticleHandler
{
	private static readonly int MaxParticlesAllowed = 500;

	private static Particle[] particles;
	private static int nextVacantIndex;
	private static int activeParticles;
	private static Dictionary<Type, int> particleTypes;
	private static Dictionary<int, Texture2D> particleTextures;

	internal static void RegisterParticles()
	{
		particles = new Particle[MaxParticlesAllowed];
		particleTypes = [];
		particleTextures = [];

		Type baseParticleType = typeof(Particle);
		SpiritReforgedMod spiritMod = ModContent.GetInstance<SpiritReforgedMod>();

		foreach (Type type in spiritMod.Code.GetTypes())
			if (type.IsSubclassOf(baseParticleType) && !type.IsAbstract && type != baseParticleType)
			{
				int assignedType = particleTypes.Count;
				particleTypes[type] = assignedType;

				string texturePath = type.Namespace.Replace('.', '/') + "/" + type.Name;
				var particleTexture = ModContent.RequestIfExists(texturePath, out Asset<Texture2D> tex, AssetRequestMode.ImmediateLoad) ? tex.Value 
					: ModContent.Request<Texture2D>("SpiritReforged/Assets/Textures/ParticleDefault", AssetRequestMode.ImmediateLoad).Value;

				particleTextures[assignedType] = particleTexture;
			}
	}

	internal static void Unload()
	{
		particles = null;
		particleTypes = null;
		particleTextures = null;
	}

	/// <summary>
	/// Spawns the particle instance provided into the world (if the particle limit is not reached).
	/// </summary>
	public static void SpawnParticle(Particle particle)
	{
		if (Main.netMode == NetmodeID.Server || activeParticles == MaxParticlesAllowed)
			return;

		particles[nextVacantIndex] = particle;
		particle.ID = nextVacantIndex;
		particle.Type = particleTypes[particle.GetType()];

		if (nextVacantIndex + 1 < particles.Length && particles[nextVacantIndex + 1] == null)
			nextVacantIndex++;
		else
			for (int i = 0; i < particles.Length; i++)
				if (particles[i] == null)
					nextVacantIndex = i;

		activeParticles++;
	}

	public static void SpawnParticle(int type, Vector2 position, Vector2 velocity, Vector2 origin = default, float rotation = 0f, float scale = 1f)
	{
		var particle = new Particle(); // yes i know constructors exist. yes i'm doing this so you dont have to make constructors over and over.
		particle.Position = position;
		particle.Velocity = velocity;
		particle.Color = Color.White;
		particle.Origin = origin;
		particle.Rotation = rotation;
		particle.Scale = scale;
		particle.Type = type;

		SpawnParticle(particle);
	}

	public static void SpawnParticle(int type, Vector2 position, Vector2 velocity)
	{
		var particle = new Particle();
		particle.Position = position;
		particle.Velocity = velocity;
		particle.Color = Color.White;
		particle.Origin = Vector2.Zero;
		particle.Rotation = 0f;
		particle.Scale = 1f;
		particle.Type = type;

		SpawnParticle(particle);
	}

	/// <summary>
	/// Deletes the particle at the given index. You typically do not have to use this; use Particle.Kill() instead.
	/// </summary>
	public static void DeleteParticleAtIndex(int index)
	{
		particles[index] = null;
		activeParticles--;
		nextVacantIndex = index;
	}

	/// <summary>
	/// Clears all the currently spawned particles.
	/// </summary>
	public static void ClearAllParticles()
	{
		for (int i = 0; i < particles.Length; i++)
			particles[i] = null;

		activeParticles = 0;
		nextVacantIndex = 0;
	}

	internal static void UpdateAllParticles()
	{
		foreach (Particle particle in particles)
		{
			if (particle == null)
				continue;

			particle.TimeActive++;
			particle.Position += particle.Velocity;

			particle.Update();
		}
	}

	internal static void DrawAllParticles(SpriteBatch spriteBatch, ParticleLayer drawLayer)
	{
		foreach (Particle particle in particles)
		{
			if (particle == null)
				continue;

			if(particle.DrawLayer == drawLayer)
			{
				Color additiveColor = particle.Color;
				additiveColor.A = 0;

				switch (particle.DrawType)
				{
					case ParticleDrawType.DefaultAlphaBlend:
						spriteBatch.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);
						break;

					case ParticleDrawType.DefaultAdditive:
						spriteBatch.Draw(particleTextures[particle.Type], particle.Position - Main.screenPosition, null, particle.Color, particle.Rotation, particle.Origin, particle.Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0f);
						break;

					case ParticleDrawType.Custom:
						particle.CustomDraw(spriteBatch);
						break;
				}
			}
		}
	}

	/// <summary>
	/// Gets the texture of the given particle type.
	/// </summary>
	public static Texture2D GetTexture(int type) => particleTextures[type];

	/// <summary>
	/// Returns the numeric type of the given particle.
	/// </summary>
	public static int ParticleType<T>() => particleTypes[typeof(T)];
}
