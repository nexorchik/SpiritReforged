using SpiritReforged.Common.Easing;
using SpiritReforged.Common.Particle;

namespace SpiritReforged.Content.Particles;

public class TexturedPulseCircle : PulseCircle
{
	private readonly string _texture;
	private readonly Vector2 _textureStretch;
	public TexturedPulseCircle(Vector2 position, Color ringColor, Color bloomColor, float ringWidth, float maxRadius, int maxTime, string texture, Vector2 textureStretch, EaseFunction MovementStyle = null, bool inverted = false) : base(position, ringColor, bloomColor, ringWidth, maxRadius, maxTime, MovementStyle, inverted)
	{
		_texture = texture;
		_textureStretch = textureStretch;
	}

	public TexturedPulseCircle(Entity attatchedEntity, Color ringColor, Color bloomColor, float ringWidth, float maxRadius, int maxTime, string texture, Vector2 textureStretch, EaseFunction MovementStyle = null, Vector2? startingPosition = null, bool inverted = false) : base(attatchedEntity, ringColor, bloomColor, ringWidth, maxRadius, maxTime, MovementStyle, startingPosition, inverted)
	{
		_texture = texture;
		_textureStretch = textureStretch;
	}

	public TexturedPulseCircle(Vector2 position, Color color, float ringWidth, float maxRadius, int maxTime, string texture, Vector2 textureStretch, EaseFunction MovementStyle = null, bool inverted = false) : this(position, color, color * 0.25f, ringWidth, maxRadius, maxTime, texture, textureStretch, MovementStyle, inverted) { }

	public TexturedPulseCircle(Entity attatchedEntity, Color color, float ringWidth, float maxRadius, int maxTime, string texture, Vector2 textureStretch, EaseFunction MovementStyle = null, Vector2? startingPosition = null, bool inverted = false) : this(attatchedEntity, color, color * 0.25f, ringWidth, maxRadius, maxTime, texture, textureStretch, MovementStyle, startingPosition, inverted) { }

	public override ParticleLayer DrawLayer => ParticleLayer.AbovePlayer;

	internal override string EffectPassName => "TexturedStyle";

	internal override void EffectExtras(ref Effect curEffect)
	{
		if(!AssetLoader.LoadedTextures.TryGetValue(_texture, out Texture2D value))
		{
			throw new ArgumentNullException(_texture, "Given input does not correspond to a loaded asset.");
		}

		else
		{
			curEffect.Parameters["uTexture"].SetValue(value);
			curEffect.Parameters["textureStretch"].SetValue(_textureStretch);
		}
	}
}
