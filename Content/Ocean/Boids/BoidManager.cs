using System.Linq;
using Terraria.Utilities;

namespace SpiritReforged.Content.Ocean.Boids;

public class BoidManager : ModSystem
{
	/// <summary> int corresponds to the current world seed. </summary>
	internal static Action<int> OnAddBoids;

	private const int normalFish = 12;
	private const int specialFish = 2;

	public static Asset<Texture2D>[] FishTextures { get; private set; }
	internal static List<Boid> boids = [];

	public override void Load()
	{
		if (Main.dedServ)
			return;

		FishTextures = new Asset<Texture2D>[normalFish + specialFish];
		for (int i = 0; i < FishTextures.Length; i++)
			FishTextures[i] = ModContent.Request<Texture2D>("SpiritReforged/Content/Ocean/Boids/Textures/fish_" + i);

		On_Main.DrawWoF += static (On_Main.orig_DrawWoF orig, Main self) =>
		{
			if (!Main.gamePaused) //Update here because boids are largely visual
				Update();

			Draw(Main.spriteBatch);
			orig(self);
		};
	}

	public override void Unload() => OnAddBoids = null;

	public override void OnWorldLoad()
	{
		int seed = Main.ActiveWorldFileData.Seed;
		int boidCount = 8 + seed % 5; //The number of unique boids in this world

		for (int i = 0; i < boidCount; i++)
		{
			var boid = new WeightedRandom<Boid>();
			boid.Add(new Boid(Lookup(seed + i), 1f, Main.rand.Next(5, 20)), 1);
			boid.Add(new Boid(Lookup(seed + i), 1f, Main.rand.Next(30, 60)), .2f);
			boid.Add(new Boid(12, .8f, Main.rand.Next(30, 60)), .14f); //Shrimp
			boid.Add(new Boid(13, .525f, 60), .19f); //Sardine

			boids.Add((Boid)boid);
		}

		OnAddBoids?.Invoke(seed);
	}

	public static int[] Lookup(int seed)
	{
		int[] lookups = new int[(int)(Math.Abs(Math.Sin(seed)) * 10f) % 4 + 1];
		for (int j = 0; j < lookups.Length; ++j)
		{
			int toAdd = (int)(Math.Abs(Math.Sin(seed + j)) * 10f) % normalFish;

			while (lookups.Contains(toAdd))
				toAdd = ++toAdd % normalFish; //Avoid duplicates

			lookups[j] = toAdd;
		}

		return lookups;
	}

	public override void OnWorldUnload() => boids.Clear();

	public static void Draw(SpriteBatch spriteBatch)
	{
		foreach (Boid fishflock in boids)
			fishflock.Draw(spriteBatch);
	}

	public static void Update()
	{
		foreach (Boid fishflock in boids)
			fishflock.Update();

		var player = Main.LocalPlayer;
		bool nearLure = player.GetModPlayer<OceanPlayer>().nearLure;
		int spawnRate = nearLure ? 28 : 34;

		if ((player.ZoneBeach || nearLure) && Main.GameUpdateCount % spawnRate == 0)
		{
			var weightedBoid = new WeightedRandom<Boid>();
			for (int i = 0; i < boids.Count; i++)
			{
				if (boids[i].spawnWeight > 0)
					weightedBoid.Add(boids[i], boids[i].spawnWeight);
			}

			const int fluff = 1000;
			var spawnPos = player.Center + new Vector2((Main.screenWidth / 2 + fluff) * Main.rand.NextFloat(-1f, 1f), (Main.screenHeight / 2 + fluff) * Main.rand.NextFloat(-1f, 1f));

			//Don't spawn on-screen unless spawned by a lure
			if (!nearLure && new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Contains(spawnPos.ToPoint()))
				return;

			var tilePos = spawnPos.ToTileCoordinates();
			if (WorldGen.InWorld(tilePos.X, tilePos.Y, 10))
			{
				var tile = Framing.GetTileSafely(tilePos);

				if (tile.LiquidAmount > 100)
					((Boid)weightedBoid).Populate(spawnPos, Main.rand.Next(20, 30), 50f);
			}
		}
	}
}