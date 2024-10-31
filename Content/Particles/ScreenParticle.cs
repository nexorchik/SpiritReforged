using SpiritReforged.Common.Particle;

namespace SpiritReforged.Content.Particles;

/// <summary> Represents a particle that can draw with a parallax effect, screen wrapping, adjusting for game-zoom and automatic fade-in and fade-outs. </summary>
public abstract class ScreenParticle : Particle
{
	public Vector2 OriginalScreenPosition;
	public float ParallaxStrength;

	public float ActiveOpacity { get; private set; }

	public virtual bool ActiveCondition => true;

	/// <summary> Used by the default spritebatch drawing for ScreenParticles, use if needed for custom drawing. </summary>
	/// <returns>The position on the screen to draw the particle, with a parallax effect, screen wrapping, and adjusting for game zoom</returns>
	public Vector2 GetDrawPosition()
	{
		//modify the drawing position based on the difference between the current screen position, the original screen position when spawned, and the strength of the parallax effect
		Vector2 drawPosition = Position - Vector2.Lerp(Main.screenPosition, Main.screenPosition - 2 * (OriginalScreenPosition - Main.screenPosition), ParallaxStrength);

		//modify the position to keep particles onscreen at all times, as to not easily all be cleared due to parallax effect
		var ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
		Vector2 UiScreenSize = ScreenSize * Main.UIScale;

		while (drawPosition.X < 0)
			drawPosition.X += UiScreenSize.X;

		while (drawPosition.Y < 0)
			drawPosition.Y += UiScreenSize.Y;

		drawPosition = new Vector2(drawPosition.X % UiScreenSize.X, drawPosition.Y % UiScreenSize.Y) * Main.GameViewMatrix.Zoom;

		return drawPosition - 3 * (ScreenSize * Main.GameViewMatrix.Zoom - ScreenSize) / 4;
	}

	public override ParticleLayer DrawLayer => ParticleLayer.AbovePlayer;

	public override ParticleDrawType DrawType => ParticleDrawType.Custom;

	public sealed override void Update()
	{
		UpdateOnScreen();

		if (ActiveCondition)
			ActiveOpacity = Math.Min(ActiveOpacity + 0.075f, 1);
		else
			ActiveOpacity = Math.Max(ActiveOpacity - 0.075f, 0);
	}

	public virtual void UpdateOnScreen() { }

	public override void CustomDraw(SpriteBatch spriteBatch)
		=> spriteBatch.Draw(Texture, GetDrawPosition(), null, Color * (1f - (float)TimeActive / MaxTime), Rotation, Origin, Scale * Main.GameViewMatrix.Zoom, SpriteEffects.None, 0);
}
