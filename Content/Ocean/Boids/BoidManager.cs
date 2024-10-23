using System.Linq;
using Terraria.Utilities;

namespace SpiritReforged.Content.Ocean.Boids;

public class BoidManager : ILoadable
{
	public static Asset<Texture2D>[] FishTextures { get; private set; }

	internal static List<Boid> Flocks = [];
	private const int SPAWNRATE = 40;

	public void Load(Mod mod)
	{
		if (Main.dedServ) //Don't load on the server
			return;

		const string path = "SpiritReforged/Content/Ocean/Boids/Textures/";
		const int fishTextureCount = 14;
		int flockCount = Main.rand.Next(5) + 8;

		#region load textures
		FishTextures = new Asset<Texture2D>[fishTextureCount];

		for (int i = 0; i < FishTextures.Length; i++)
			FishTextures[i] = ModContent.Request<Texture2D>(path + "fish_" + i);
		#endregion

		for (int i = 0; i < flockCount; i++)
		{
			//Randomly selects indexes of textures the following boid should use
			static int[] GetFishLookup()
			{
				int[] lookups = new int[Main.rand.Next(4) + 1];

				for (int j = 0; j < lookups.Length; ++j)
				{
					int toAdd = Main.rand.Next(fishTextureCount - 2); //Exclude the last 2 fish because they are unique

					if (!lookups.Contains(toAdd))
						lookups[j] = toAdd;
					else
						j--; //If a duplicate exists, try again
				}

				return lookups;
			}

			var boid = new WeightedRandom<Boid>();
			boid.Add(new Boid(GetFishLookup(), 1f, Main.rand.Next(5, 20)), 1);
			boid.Add(new Boid(GetFishLookup(), 1f, Main.rand.Next(30, 60)), .2f);
			boid.Add(new Boid(12, .8f, Main.rand.Next(30, 60)), .14f); //Shrimp
			boid.Add(new Boid(13, .525f, 60), .19f); //Sardine

			Flocks.Add(boid);
		}

		//Add detours
		On_Main.DrawWoF += (On_Main.orig_DrawWoF orig, Main self) =>
		{
			if (!Main.gamePaused)
				Update(); //We can get away with updating here because boids are essentially only visual

			Draw(Main.spriteBatch);
		};
	}

	public void Unload() => Flocks.Clear();

	public static void Draw(SpriteBatch spriteBatch)
	{
		foreach (Boid fishflock in Flocks)
			fishflock.Draw(spriteBatch);
	}

	public static void Update()
	{
		foreach (Boid fishflock in Flocks)
			fishflock.Update();

		//Spawn boid fish
		Player player = Main.LocalPlayer;
		if (player.ZoneBeach && Main.GameUpdateCount % SPAWNRATE == SPAWNRATE - 1 || Main.GameUpdateCount % SPAWNRATE > SPAWNRATE - 3 && player.GetModPlayer<OceanPlayer>().nearLure)
		{
			int flock = Main.rand.Next(0, Flocks.Count);
			const int fluff = 1000;

			var spawnPos = player.Center + new Vector2((Main.screenWidth / 2 + fluff) * Main.rand.NextFloat(-1f, 1f), (Main.screenHeight / 2 + fluff) * Main.rand.NextFloat(-1f, 1f));

			//Don't spawn on-screen
			if (new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Contains(spawnPos.ToPoint()))
				return;

			Point tilePos = spawnPos.ToTileCoordinates();
			if (WorldGen.InWorld(tilePos.X, tilePos.Y, 10))
			{
				Tile tile = Framing.GetTileSafely(tilePos.X, tilePos.Y);

				if (tile.LiquidAmount > 100)
					Flocks[flock].Populate(spawnPos, Main.rand.Next(20, 30), 50f);
			}
		}
	}
}