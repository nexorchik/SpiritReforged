using RubbleAutoloader;
using SpiritReforged.Common.ItemCommon;
using SpiritReforged.Common.TileCommon;
using SpiritReforged.Common.TileCommon.PresetTiles;
using SpiritReforged.Common.Visuals.Glowmasks;
using SpiritReforged.Content.Forest.Cloud.Items;
using SpiritReforged.Content.Underground.NPCs;
using SpiritReforged.Content.Underground.Pottery;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;

namespace SpiritReforged.Content.Underground.Tiles;

[AutoloadGlowmask("200,200,200")]
public class BiomePots : PotTile, ILootTile
{
	/// <summary> Mirrors <see cref="Styles"/>. </summary>
	public enum Style : int
	{
		Cavern, Gold, Ice, Desert, Jungle, Dungeon, Corruption, Crimson, Marble, Hell, Mushroom
	}

	/// <summary> Unit for distance-based vfx. </summary>
	private const int DistMod = 200;
	private static readonly HashSet<Point16> GlowPoints = [];

	public override void AddRecord(int type, StyleDatabase.StyleGroup group)
	{
		var record = new TileRecord(group.name, type, group.styles);

		if (group.name == "BiomePotsGold")
			RecordHandler.Records.Add(record.AddRating(5).AddDescription(Language.GetText(TileRecord.DescKey + ".CoinPortal")));
		else
			RecordHandler.Records.Add(record.AddRating(2).AddDescription(Language.GetText(TileRecord.DescKey + ".Biome")));
	}

	public override Dictionary<string, int[]> TileStyles => new()
	{
		{ "Cavern", [0, 1, 2] },
		{ "Gold", [3, 4, 5] },
		{ "Ice", [6, 7, 8] },
		{ "Desert", [9, 10, 11] },
		{ "Jungle", [12, 13, 14] },
		{ "Dungeon", [15, 16, 17] },
		{ "Corruption", [18, 19, 20] },
		{ "Crimson", [21, 22, 23] },
		{ "Marble", [24, 25, 26] },
		{ "Hell", [27, 28, 29] },
		{ "Mushroom", [30, 31, 32] }
	};

	private static Style GetStyle(int frameY) => (Style)(frameY / 36);

	#region drawing detours
	public override void Load(Mod mod)
	{
		DrawOrderSystem.DrawTilesNonSolidEvent += DrawGlow;
		On_TileDrawing.PreDrawTiles += ClearAll;
	}

	private static void DrawGlow()
	{
		foreach (var p in GlowPoints)
		{
			var world = p.ToWorldCoordinates(16, 18);

			float opacity = MathHelper.Clamp(1f - Main.LocalPlayer.DistanceSQ(world) / (DistMod * DistMod), 0, .75f) * Lighting.Brightness(p.X, p.Y);
			DrawGlow(p.ToWorldCoordinates(16, 18) - Main.screenPosition, opacity);
		}
	}

	private static void ClearAll(On_TileDrawing.orig_PreDrawTiles orig, TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
	{
		orig(self, solidLayer, forRenderTargets, intoRenderTargets);

		bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;

		if (!solidLayer && flag)
			GlowPoints.Clear();
	}
	#endregion

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (closer)
		{
			var world = new Vector2(i, j) * 16;
			float strength = Main.LocalPlayer.DistanceSQ(world) / (DistMod * DistMod);

			if (strength < 1 && Main.rand.NextFloat(8f) < 1f - strength)
			{
				var d = Dust.NewDustDirect(world, 16, 16, DustID.TreasureSparkle, 0, 0, Scale: Main.rand.NextFloat());
				d.noGravity = true;
				d.velocity = new Vector2(0, -Main.rand.NextFloat(2f));
			}
		}
	}

	public override bool KillSound(int i, int j, bool fail)
	{
		if (!fail && !Autoloader.IsRubble(Type))
		{
			var pos = new Vector2(i, j).ToWorldCoordinates(16, 16);

			if (GetStyle(Main.tile[i, j].TileFrameY) is Style.Mushroom or Style.Corruption or Style.Crimson)
			{
				SoundEngine.PlaySound(SoundID.NPCHit1 with { Volume = .3f, Pitch = .25f }, pos);
				SoundEngine.PlaySound(SoundID.NPCDeath1, pos);
			}
			else
			{
				SoundEngine.PlaySound(SoundID.Shatter, pos);
				SoundEngine.PlaySound(new SoundStyle("SpiritReforged/Assets/SFX/Tile/PotBreak") with { Volume = .16f, PitchRange = (-.4f, 0), }, pos);
			}

			return false;
		}

		return true;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var style = GetStyle(Main.tile[i, j].TileFrameY);

		if (TileObjectData.IsTopLeft(i, j) && style is Style.Gold)
			GlowPoints.Add(new Point16(i, j));

		if (style is Style.Mushroom)
			Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.Blue.ToVector3());

		return true;
	}

	public static void DrawGlow(Vector2 drawPosition, float opacity)
	{
		const int squareSize = 32;

		var region = new Rectangle((int)drawPosition.X - squareSize / 2, (int)drawPosition.Y - squareSize / 2, squareSize, squareSize);
		Color color = Color.White;

		short[] indices = [0, 1, 2, 1, 3, 2];

		//Note that corner positions are reversed to flip the effect
		VertexPositionColorTexture[] vertices =
		[
			new(new Vector3(region.BottomRight(), 0), color, new Vector2(0, 0)),
			new(new Vector3(region.BottomLeft(), 0), color, new Vector2(1, 0)),
			new(new Vector3(region.TopRight(), 0), color, new Vector2(0, 1)),
			new(new Vector3(region.TopLeft(), 0), color, new Vector2(1, 1)),
		];

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
		Matrix view = Main.GameViewMatrix.TransformationMatrix;
		Effect effect = AssetLoader.LoadedShaders["ShadowFade"];

		foreach (EffectPass pass in effect.CurrentTechnique.Passes)
		{
			effect.Parameters["baseShadowColor"].SetValue(Color.Goldenrod.ToVector4() * opacity);
			effect.Parameters["adjustColor"].SetValue(Color.White.ToVector4() * opacity);
			effect.Parameters["noiseScroll"].SetValue(Main.GameUpdateCount * 0.0015f);
			effect.Parameters["noiseStretch"].SetValue(.5f);
			effect.Parameters["uWorldViewProjection"].SetValue(view * projection);
			effect.Parameters["noiseTexture"].SetValue(AssetLoader.LoadedTextures["vnoise"].Value);
			pass.Apply();

			Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
		}
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (WorldGen.generatingWorld || Autoloader.IsRubble(Type))
			return; //Particularly important for not incrementing Remaining

		var style = GetStyle(frameY);
		int variant = frameX / 36;

		var source = new EntitySource_TileBreak(i, j);
		var position = new Vector2(i, j).ToWorldCoordinates(16, 16);
		int dustType = DustID.Pot;

		bool spawnSlime = PotteryTracker.Remaining == 1;
		PotteryTracker.TrackOne();

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			#region loot
			var p = Main.player[Player.FindClosest(position, 0, 0)];
			var info = LootTableHelper.GetInfo(p);

			AddLoot(TileObjectData.GetTileStyle(Main.tile[i, j])).Resolve(info);

			ItemMethods.SplitCoins(CalculateCoinValue(style), delegate (int type, int stack)
			{
				Item.NewItem(source, position, new Item(type, stack), noGrabDelay: true);
			}); //Always drop coins

			if (p.statLife < p.statLifeMax2)
			{
				int stack = Main.rand.Next(3, 6);

				for (int h = 0; h < stack; h++)
					Item.NewItem(source, position, ItemID.Heart);
			}
			#endregion

			if (spawnSlime)
				NPC.NewNPCDirect(source, position, ModContent.NPCType<PotterySlime>());

			if (style is Style.Mushroom && NPC.CountNPCS(ModContent.NPCType<StompableGnome>()) < 5)
			{
				int count = Main.rand.Next(1, 4);

				for (int c = 0; c < count; c++)
					NPC.NewNPCDirect(source, position + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f), ModContent.NPCType<StompableGnome>());
			}

			if (Main.dedServ)
				return;
		}

		switch (style)
		{
			case Style.Cavern:

				for (int g = 0; g < 3; g++)
				{
					int goreType = Mod.Find<ModGore>("PotCavern" + (g + variant * 3 + 1)).Type;
					Gore.NewGore(source, position, Vector2.Zero, goreType);
				}

				dustType = DustID.Pot;

				break;

			case Style.Gold:

				for (int g = 1; g < 4; g++)
				{
					int goreType = Mod.Find<ModGore>("PotGold" + g).Type;
					Gore.NewGore(source, position, Vector2.Zero, goreType);
				}

				dustType = DustID.Gold;
				break;

			case Style.Ice:

				for (int g = 1; g < 4; g++)
				{
					int goreType = Mod.Find<ModGore>("PotIce" + g).Type;
					Gore.NewGore(source, position, Vector2.Zero, goreType);
				}

				dustType = DustID.Ice;

				break;

			case Style.Desert:

				Gore.NewGore(source, GetRandom(), Vector2.Zero, GoreID.DesertPot1);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, GoreID.DesertPot2);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, GoreID.DesertPot3);
				dustType = DustID.DesertPot;

				break;

			case Style.Jungle:

				Gore.NewGore(source, GetRandom(), Vector2.Zero, 199);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, 200);
				dustType = DustID.WoodFurniture;

				break;

			case Style.Dungeon:

				Gore.NewGore(source, GetRandom(), Vector2.Zero, 201);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, 202);
				dustType = DustID.Bone;

				break;

			case Style.Corruption:

				dustType = DustID.CorruptGibs;

				break;

			case Style.Crimson:

				dustType = DustID.Crimson;

				break;

			case Style.Marble:

				Gore.NewGore(source, GetRandom(), Vector2.Zero, GoreID.GreekPot1);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, GoreID.GreekPot2);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, GoreID.GreekPot3);
				dustType = DustID.MarblePot;

				break;

			case Style.Hell:

				Gore.NewGore(source, GetRandom(), Vector2.Zero, 203);
				Gore.NewGore(source, GetRandom(), Vector2.Zero, 204);
				dustType = DustID.Obsidian;

				break;

			case Style.Mushroom:

				for (int g = 1; g < 4; g++)
				{
					int goreType = Mod.Find<ModGore>("PotMushroom" + g).Type;
					Gore.NewGore(source, position, Vector2.Zero, goreType);
				}

				dustType = DustID.MushroomSpray;

				break;
		}

		for (int d = 0; d < 20; d++)
			Dust.NewDustPerfect(GetRandom(), dustType, Main.rand.NextVector2Unit(), Scale: Main.rand.NextFloat() + .25f);

		Vector2 GetRandom(float distance = 15f) => position + Main.rand.NextVector2Unit() * Main.rand.NextFloat(distance);
	}

	public LootTable AddLoot(int objectStyle)
	{
		var loot = new LootTable();
		var style = GetStyle(objectStyle / 3 * 36);

		if (style is Style.Dungeon)
			loot.AddCommon(ItemID.GoldenKey, 10);

		List<int> potions = [ItemID.SpelunkerPotion, ItemID.HunterPotion,
			ItemID.GravitationPotion, ItemID.LifeforcePotion, ItemID.TitanPotion, ItemID.BattlePotion,
			ItemID.MagicPowerPotion, ItemID.ManaRegenerationPotion, ItemID.BiomeSightPotion, ItemID.HeartreachPotion,
			ModContent.ItemType<DoubleJumpPotion>(), WorldGen.crimson ? ItemID.RagePotion : ItemID.WrathPotion];

		if (style is Style.Cavern)
			potions.AddRange([ItemID.SwiftnessPotion, ItemID.RegenerationPotion, ItemID.SwiftnessPotion]);
		else if (style is Style.Jungle)
			potions.Add(ItemID.SummoningPotion);
		else if (style is Style.Hell)
			potions.AddRange([ItemID.InfernoPotion, ItemID.ObsidianSkinPotion]);

		var pCond0 = ItemDropRule.OneFromOptions(15, [.. potions]);
		var pCond1 = ItemDropRule.OneFromOptions(3, ItemID.PotionOfReturn, ItemID.LuckPotionLesser);

		pCond0.OnSuccess(pCond1);
		pCond1.OnFailedRoll(ItemDropRule.Common(ItemID.LuckPotion, 5));

		loot.Add(pCond0);

		List<int> flasks = [ItemID.FlaskofGold];

		if (style is Style.Jungle)
			flasks.Add(ItemID.FlaskofPoison);
		else if (style is Style.Hell)
			flasks.Add(ItemID.FlaskofFire);

		loot.AddOneFromOptions(32, [.. flasks]);
		loot.Add(ItemDropRule.ByCondition(new DropConditions.Standard(Condition.Multiplayer), ItemID.WormholePotion, 30));

		int type = style switch
		{
			Style.Desert => ItemID.FossilOre,
			Style.Dungeon => ItemID.Bone,
			Style.Marble => ItemID.Javelin,
			Style.Hell => ItemID.LivingFireBlock,
			_ => -1
		};

		if (type != -1)
		{
			if (style is Style.Dungeon)
				loot.Add(ItemDropRule.ByCondition(new DropConditions.Standard(Condition.DownedSkeletron), ItemID.Bone, 2, 10, 15));
			else
				loot.AddCommon(type, 2, 10, 15);
		}

		loot.AddCommon((style is Style.Desert) ? ItemID.ScarabBomb : ItemID.Dynamite, 3, 4, 8);
		loot.AddCommon(TorchType(), 3, 15, 20);
		loot.AddCommon(ArrowType(), 3, 20, 40);
		loot.AddCommon((WorldGen.SavedOreTiers.Silver == TileID.Silver) ? ItemID.SilverBullet : ItemID.TungstenBullet, 3, 20, 40);
		loot.AddCommon(ItemID.HealingPotion, 3, 1, 3);

		return loot;

		int ArrowType()
		{
			int result = style switch
			{
				Style.Ice => ItemID.FrostburnArrow,
				Style.Dungeon => ItemID.BoneArrow,
				Style.Marble => ItemID.JestersArrow,
				Style.Hell => ItemID.HellfireArrow,
				_ => ItemID.WoodenArrow
			};

			if (style is Style.Desert or Style.Jungle)
				result = ItemID.FlamingArrow;
			else if (style is Style.Corruption or Style.Crimson)
				result = ItemID.UnholyArrow;

			return result;
		}

		int TorchType()
		{
			int result = style switch
			{
				Style.Ice => ItemID.IceTorch,
				Style.Desert => ItemID.DesertTorch,
				Style.Jungle => ItemID.JungleTorch,
				Style.Dungeon => ItemID.BoneTorch,
				Style.Marble => ItemID.YellowTorch,
				Style.Hell => ItemID.DemonTorch,
				Style.Mushroom => ItemID.MushroomTorch,
				_ => ItemID.UltrabrightTorch
			};

			return result;
		}
	}

	/// <summary> Calculates coin values similarly to how vanilla pots do. </summary>
	private static int CalculateCoinValue(Style style)
	{
		float value = 200 + WorldGen.genRand.Next(-100, 101);

		float biomeMult = style switch
		{
			Style.Ice => 1.4f,
			Style.Desert => 2.25f,
			Style.Jungle => 2f,
			Style.Dungeon => 2.25f,
			Style.Corruption => 1.9f,
			Style.Crimson => 1.9f,
			Style.Marble => 2.2f,
			Style.Hell => 2.5f,
			_ => 1.25f
		};

		value *= biomeMult;
		value *= 1f + Main.rand.Next(-20, 21) * 0.01f;

		if (Main.hardMode)
			value *= 2;

		if (Main.rand.NextBool(4))
			value *= 1f + Main.rand.Next(5, 11) * 0.01f;

		if (Main.rand.NextBool(8))
			value *= 1f + Main.rand.Next(10, 21) * 0.01f;

		if (Main.rand.NextBool(12))
			value *= 1f + Main.rand.Next(20, 41) * 0.01f;

		if (Main.rand.NextBool(16))
			value *= 1f + Main.rand.Next(40, 81) * 0.01f;

		if (Main.rand.NextBool(20))
			value *= 1f + Main.rand.Next(50, 101) * 0.01f;

		if (Main.expertMode)
			value *= 2.5f;

		if (Main.expertMode && Main.rand.NextBool(2))
			value *= 1.25f;

		if (Main.expertMode && Main.rand.NextBool(3))
			value *= 1.5f;

		if (Main.expertMode && Main.rand.NextBool(4))
			value *= 1.75f;

		if (NPC.downedBoss1)
			value *= 1.1f;

		if (NPC.downedBoss2)
			value *= 1.1f;

		if (NPC.downedBoss3)
			value *= 1.1f;

		if (NPC.downedMechBoss1)
			value *= 1.1f;

		if (NPC.downedMechBoss2)
			value *= 1.1f;

		if (NPC.downedMechBoss3)
			value *= 1.1f;

		if (NPC.downedPlantBoss)
			value *= 1.1f;

		if (NPC.downedQueenBee)
			value *= 1.1f;

		if (NPC.downedGolemBoss)
			value *= 1.1f;

		if (NPC.downedPirates)
			value *= 1.1f;

		if (NPC.downedGoblins)
			value *= 1.1f;

		if (NPC.downedFrost)
			value *= 1.1f;

		return (int)value;
	}
}