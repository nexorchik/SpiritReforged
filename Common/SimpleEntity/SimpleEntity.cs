namespace SpiritReforged.Common.SimpleEntity;

public class SimpleEntity : Entity
{
	/// <summary> Whether this entity should be saved with the current world data. </summary>
	public bool saveMe;

	public Asset<Texture2D> Texture => SimpleEntitySystem.textures[SimpleEntitySystem.types[GetType()]];

	public virtual string TexturePath => GetType().Namespace.Replace('.', '/') + "/" + GetType().Name;

	public virtual void Load() { }

	public virtual void Update() { }

	public virtual void Draw(SpriteBatch spriteBatch) { }

	public void Kill()
	{
		SimpleEntitySystem.RemoveEntity(whoAmI);
		OnKill();
	}

	public virtual void OnKill() { }

	public virtual SimpleEntity Clone() => MemberwiseClone() as SimpleEntity;
}
