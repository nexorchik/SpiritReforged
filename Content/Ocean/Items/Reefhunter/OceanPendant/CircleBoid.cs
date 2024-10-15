using SpiritReforged.Content.Ocean.Boids;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.OceanPendant;

internal class CircleBoid(int[] lookup, float scale = 1, float maxFlockSize = 60, float spawnWeight = 1) : Boid(lookup, scale, maxFlockSize, spawnWeight)
{
	public Vector2? anchor;

	internal void Populate(Vector2 position, int amount, float spread, Vector2 anchor)
	{
		for (int i = 0; i < amount; i++)
		{
			if (Objects.Count >= maxFish)
				break;

			var fish = new CircleBoidObject(this)
			{
				position = position + new Vector2(Main.rand.NextFloat(-spread, spread), Main.rand.NextFloat(-spread, spread)),
				velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1))
			};

			this.anchor = anchor;
			Objects.Add(fish);
		}
	}
}
