using Terraria.Graphics.Effects;
using static SpiritReforged.Common.Visuals.Skies.AutoloadSkyDict;

namespace SpiritReforged.Common.Visuals.Skies;

public abstract class AutoloadedSky : CustomSky, ILoadable
{
	internal float _fadeOpacity;
	internal bool _isActive;

	public void Load(Mod mod)
	{
		//Load doesn't run on abstract classes, so there's no need to check if this is the abstract class beforehand
		string key = mod.Name + ":" + GetType().Name;
		SkyManager.Instance[mod.Name + ":" + GetType().Name] = (CustomSky)Activator.CreateInstance(GetType());
		LoadedSkies.Add(key, new Func<Player, bool>(ActivationCondition));
	}

	public void Unload()
	{

	}

	/// <summary>
	/// The amount that the opacity of the sky increases or decreases with each tick. Defaults to 0.01f
	/// </summary>
	internal virtual float FadeSpeed { get; set; } = 0.01f;

	/// <summary>
	/// Determines if the sky should be excluded from the normal drawing process, and manually draw it in a detour instead
	/// </summary>
	internal virtual bool DrawUnderSun { get; set; } = false;

	/// <summary>
	/// Runs after the default update hook for skies, which now contains logic for fading in/out.
	/// </summary>
	/// <param name="gameTime"></param>
	internal virtual void OnUpdate(GameTime gameTime) { }

	public override void Update(GameTime gameTime)
	{
		if (_isActive)
			_fadeOpacity = Math.Min(1f, FadeSpeed + _fadeOpacity);
		else
			_fadeOpacity = Math.Max(0f, _fadeOpacity - FadeSpeed);

		OnUpdate(gameTime);
	}

	public override void Activate(Vector2 position, params object[] args)
	{
		_isActive = true;
		OnActivate(args);
	}

	public override void Deactivate(params object[] args)
	{
		_isActive = false;
		OnDeactivate(args);
	}

	public override void Reset()
	{
		_isActive = false;
		OnReset();
	}

	public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
	{
		if (maxDepth < float.MaxValue || minDepth > float.MaxValue || DrawUnderSun)
			return;
	}

	public override bool IsActive() => _isActive || _fadeOpacity > 0;

	/// <summary>
	/// Optional hooks to run when the sky deactivates or resets.
	/// </summary>

	internal virtual void OnActivate(params object[] args) { }
	internal virtual void OnDeactivate(params object[] args) { }
	internal virtual void OnReset() { }

	/// <summary>
	/// To draw the sky. Called either right before the sun/moon are rendered, or through the default skymanager rendering, depending on if DrawUnderSun is true or false.
	/// Do note- the spritebatch params are different depending on when it's drawn- you'd need to account for that and correct the scale
	/// </summary>
	/// <param name="spriteBatch"></param>
	public abstract void DoDraw(SpriteBatch spriteBatch);

	/// <summary>
	/// Determines when the sky should be activated. If this returns true, then the sky will automatically fade in, and it will automatically fade out if it returns false.
	/// </summary>
	/// <param name="p"></param>
	/// <returns></returns>
	internal abstract bool ActivationCondition(Player p);

	public float GetFadeOpacity() => _fadeOpacity;
}