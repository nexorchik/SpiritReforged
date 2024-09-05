namespace SpiritReforged.Content.Ocean.Boids;

internal class BoidObject : Entity
{
	public Vector2 Acceleration { get; set; }

	public const float Vision = 100;
	private const float MaxForce = 0.02f;
	private const float MaxVelocity = 2f;

	private byte frame = 0;
	private readonly int textureID;
	private int spawnTimer = 100;

	private readonly Boid parent;

	public List<BoidObject> AdjFish = [];

	public BoidObject(Boid flock)
	{
		parent = flock;
		textureID = parent.TextureLookup[Main.rand.Next(parent.TextureLookup.Length)];
	}

	private static Vector2 Limit(Vector2 vec, float val)
	{
		if (vec.LengthSquared() > val * val)
			return Vector2.Normalize(vec) * val;

		return vec;
	}

	public Vector2 AvoidTiles(int range) //WIP for Qwerty
	{
		var sum = new Vector2(0, 0);
		Point tilePos = position.ToTileCoordinates();

		const int TileRange = 2;

		for (int i = -TileRange; i < TileRange + 1; i++)
		{
			for (int j = -TileRange; j < TileRange + 1; j++)
			{
				if (WorldGen.InWorld(tilePos.X + i, tilePos.Y + j, 10))
				{
					Tile tile = Framing.GetTileSafely(tilePos.X + i, tilePos.Y + j);
					float pdist = Vector2.DistanceSquared(position, new Vector2(tilePos.X + i, tilePos.Y + j) * 16);
					if (pdist < range * range && pdist > 0 && (tile.HasTile && Main.tileSolid[tile.TileType] || tile.LiquidAmount < 100))
					{
						var d = position - new Vector2(tilePos.X + i, tilePos.Y + j) * 16;
						var norm = Vector2.Normalize(d);
						var weight = norm;
						sum += weight;
					}
				}
			}
		}

		if (sum != Vector2.Zero)
		{
			sum = Vector2.Normalize(sum) * MaxVelocity;
			var acc = sum - velocity;
			return Limit(acc, MaxForce);
		}

		return Vector2.Zero;
	}

	//Avoid you [Client Side]
	//TODO: Entity Pass, not client side maybe?
	public Vector2 AvoidHooman(int range)
	{
		float pdist = Vector2.DistanceSquared(position, Main.LocalPlayer.Center);
		var sum = new Vector2(0, 0);

		if (pdist < range * range && pdist > 0)
		{
			var d = position - Main.LocalPlayer.Center;
			var norm = Vector2.Normalize(d);
			var weight = norm;
			sum += weight;
		}

		if (sum != Vector2.Zero)
		{
			sum = Vector2.Normalize(sum) * MaxVelocity;
			var acc = sum - velocity;
			return Limit(acc, MaxForce);
		}

		return Vector2.Zero;
	}

	//Cant overlap
	public Vector2 Seperation(int range)
	{
		int count = 0;
		var sum = new Vector2(0, 0);
		for (int j = 0; j < AdjFish.Count; j++)
		{
			var OtherFish = AdjFish[j];
			float dist = Vector2.DistanceSquared(position, OtherFish.position);
			if (dist < range * range && dist > 0)
			{
				var d = position - OtherFish.position;
				var norm = Vector2.Normalize(d);
				var weight = norm / dist;
				sum += weight;
				count++;
			}
		}

		if (count > 0)
			sum /= count;

		if (sum != Vector2.Zero)
		{
			sum = Vector2.Normalize(sum) * MaxVelocity;
			var acc = sum - velocity;
			return Limit(acc, MaxForce);
		}

		return Vector2.Zero;
	}

	//Must face the same general direction
	public Vector2 Allignment(int range)
	{
		int count = 0;
		var sum = new Vector2(0, 0);
		for (int j = 0; j < AdjFish.Count; j++)
		{
			var OtherFish = AdjFish[j];
			float dist = Vector2.DistanceSquared(position, OtherFish.position);
			if (dist < range * range && dist > 0)
			{
				sum += OtherFish.velocity;
				count++;
			}
		}

		if (count > 0)
			sum /= count;

		if (sum != Vector2.Zero)
		{
			sum = Vector2.Normalize(sum) * MaxVelocity;
			var acc = sum - velocity;
			return Limit(acc, MaxForce);
		}

		return Vector2.Zero;
	}

	//Must stay close
	public Vector2 Cohesion(int range)
	{
		int count = 0;
		var sum = new Vector2(0, 0);
		for (int j = 0; j < AdjFish.Count; j++)
		{
			var OtherFish = AdjFish[j];
			float dist = Vector2.DistanceSquared(position, OtherFish.position);
			if (dist < range * range && dist > 0)
			{
				sum += OtherFish.position;
				count++;
			}
		}

		if (count > 0)
		{
			sum /= count;
			sum -= position;
			sum = Vector2.Normalize(sum) * MaxVelocity;
			var acc = sum - velocity;
			return Limit(acc, MaxForce);
		}

		return Vector2.Zero;
	}

	public void Draw(SpriteBatch spritebatch)
	{
		Point tilePos = position.ToTileCoordinates();
		Color lightColour = Lighting.GetColor(tilePos.X, tilePos.Y);

		float alpha = MathHelper.Clamp(1 - spawnTimer-- / 100f, 0f, 1f);
		Texture2D texture = BoidManager.FishTextures[textureID].Value;
		var source = texture.Frame(1, 2, 0, frame % 2);

		spritebatch.Draw(texture, position - Main.screenPosition, source, lightColour * alpha, 
			velocity.ToRotation() + (float)Math.PI, source.Size() / 2, parent.flockScale, SpriteEffects.None, 0f);
	}

	public void ApplyForces()
	{
		velocity += Acceleration;
		velocity = Limit(velocity, MaxVelocity);
		position += velocity;
		Acceleration *= 0;
	}

	public void Update()
	{
		//arbitrarily weight
		Acceleration += Seperation(25) * 1.5f;
		Acceleration += Allignment(50) * 1f;
		Acceleration += Cohesion(50) * 1f;
		Acceleration += AvoidHooman(50) * 4f;
		Acceleration += AvoidTiles(100) * 5f;
		ApplyForces();

		if (Main.rand.NextBool(7))
			frame++;
	}
}
