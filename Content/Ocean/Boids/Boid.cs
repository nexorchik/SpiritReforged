namespace SpiritReforged.Content.Ocean.Boids;

internal class Boid
{
	private readonly List<BoidObject> Objects = [];

	public int[] TextureLookup { get; }

	public readonly float flockScale;
	public readonly float maxFish;

	private const int SimulationDistance = 2500;

	/// <param name="lookup"> The indexes of <see cref="BoidManager.FishTextures"/> this boid uses. </param>
	public Boid(int[] lookup, float scale = 1, float maxFlockSize = 60)
	{
		TextureLookup = lookup;
		flockScale = scale;
		maxFish = maxFlockSize;
	}

	/// <param name="lookup"> The index of <see cref="BoidManager.FishTextures"/> this boid uses. </param>
	public Boid(int lookup, float scale = 1, float maxFlockSize = 60)
	{
		TextureLookup = [lookup];
		flockScale = scale;
		maxFish = maxFlockSize;
	}

	internal void Populate(Vector2 position, int amount, float spread)
	{
		for (int i = 0; i < amount; i++)
		{
			if (Objects.Count < maxFish)
			{
				var fish = new BoidObject(this)
				{
					position = position + new Vector2(Main.rand.NextFloat(-spread, spread), Main.rand.NextFloat(-spread, spread)),
					velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1))
				};

				Objects.Add(fish);
			}
		}
	}

	public void Update()
	{
		foreach (BoidObject fish in Objects.ToArray())
		{
			if (fish is null)
				continue;

			fish.Update();
			fish.AdjFish.Clear();

			foreach (BoidObject adjfish in Objects)
			{
				if (!fish.Equals(adjfish) && Vector2.DistanceSquared(fish.position, adjfish.position) < BoidObject.Vision * BoidObject.Vision)
					fish.AdjFish.Add(adjfish);
			}

			if (Vector2.DistanceSquared(fish.position, Main.LocalPlayer.Center) > SimulationDistance * SimulationDistance)
				Objects.Remove(fish);
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		foreach (BoidObject fish in Objects.ToArray())
			fish?.Draw(spriteBatch);
	}
}
