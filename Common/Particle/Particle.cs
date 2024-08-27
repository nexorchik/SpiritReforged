namespace SpiritReforged.Common.Particle;

/// <summary>
/// Represents a particle with a position, velocity, rotation, scale and transparency.
/// TL;DR: Better dust.
/// </summary>
public class Particle
{
	public int ID; // you don't have to use this
	public int Type; // you don't have to use this
	public Vector2 Position;
	public Vector2 Velocity;
	public Vector2 Origin;
	public Color Color;
	public float Rotation;
	public float Scale;
	public uint TimeActive;
	public ParticleLayer DrawLayer;

	public Texture2D Texture => ParticleHandler.GetTexture(Type);

	/// <summary>
	/// Determines whether the particle is drawn using its default drawing method, the default drawing method with additive color blending, or a custom drawing type.
	/// If set to Custom, the CustomDraw method will be called.
	/// </summary>
	public virtual ParticleDrawType DrawType => ParticleDrawType.DefaultAlphaBlend;

	/// <summary>
	/// The chance at any given tick that this particle will spawn.
	/// Return 0f if you want the particle to not naturally spawn (if you want to spawn it yourself).
	/// This hook will not run if Particle.ActiveCondition returns false.
	/// </summary>
	public virtual float SpawnChance => 0f;

	/// <summary>
	/// Call this when you want to clear your particle and remove it from the world.
	/// </summary>
	public void Kill() => ParticleHandler.DeleteParticleAtIndex(ID);

	/// <summary>
	/// Called every tick. Update your particle in this method.
	/// Particle velocity is automatically added to the particle position for you, and TimeAlive is incremented.
	/// </summary>
	public virtual void Update() { }

	/// <summary>
	/// Allows you to do custom drawing for your particle. Only called if Particle.UseCustomDrawing is true.
	/// </summary>
	public virtual void CustomDraw(SpriteBatch spriteBatch) { }

	/// <summary>
	/// Called if the particle tries to naturally spawn. This can only be called if Particle.SpawnChance returns a value greater than 0f and Particle.ActiveCondition is true.
	/// Use this to spawn your particle randomly.
	/// </summary>
	public virtual void OnSpawnAttempt() { }
}
