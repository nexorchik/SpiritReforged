using SpiritReforged.Content.Ocean.Boids;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;

internal class CircleBoidObject(Boid flock) : BoidObject(flock)
{
	private float phase;

	public Vector2 Anchor(int range)
	{
		var parent = this.parent as CircleBoid;
		if (parent.anchor is null)
			return Vector2.Zero;

		if (Framing.GetTileSafely(parent.anchor.Value).TileType != ModContent.TileType<OceanPendantTile>())
		{
			parent.anchor = null;
			return Vector2.Zero;
		}

		return position.DirectionTo(parent.anchor.Value) * (position.Distance(parent.anchor.Value) / (16 * range));
	}

	public override void Draw(SpriteBatch spritebatch)
	{
		float alpha = MathHelper.Clamp(1 - spawnTimer-- / 100f, 0f, 1f);
		var lightColour = Lighting.GetColor(position.ToTileCoordinates()) * alpha;

		float scale = parent.flockScale;
		var texture = BoidManager.FishTextures[textureID].Value;
		var source = texture.Frame(1, 2, 0, frame % 2);
		var effects = velocity.X > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

		if (velocity.X > 0)
			phase = MathHelper.Min(phase + .05f, 1);
		else
			phase = MathHelper.Max(phase - .05f, 0);

		lightColour = lightColour.MultiplyRGB(Color.Lerp(Color.White, Color.SlateBlue, phase)); //Darken
		scale *= 1f - phase * .2f;

		spritebatch.Draw(texture, position - Main.screenPosition, source, lightColour ,
			velocity.ToRotation() + (float)Math.PI, source.Size() / 2, scale, effects, 0f);
	}

	public override void Update()
	{
		acceleration += Anchor(50) * .14f;
		acceleration += AvoidHooman(50) * 4f;
		acceleration += AvoidTiles(100) * 5f;
		ApplyForces();

		if (Main.rand.NextBool(7))
			frame++;
	}
}
