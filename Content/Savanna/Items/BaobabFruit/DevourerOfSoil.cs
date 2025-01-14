using SpiritReforged.Common.PlayerCommon;
using SpiritReforged.Common.SimpleEntity;
using System.Linq;
using Terraria.Audio;

namespace SpiritReforged.Content.Savanna.Items.BaobabFruit;

public class DevourerOfSoil : SimpleEntity
{
	private static readonly Point[] Dimensions = [new Point(30, 38), new Point(22, 18), new Point(14, 22)]; //Excludes 2px(y) padding

	public readonly Vector2[] positions = new Vector2[Length];
	private const int Length = 8;

	private bool playingDeathAnimation;
	private bool justDied = true;
	private bool justSpawned = true;
	private float rotation;
	private int soundDelay;

	public override void Load()
	{
		Size = new Vector2(30);

		On_Main.UpdateAudio_DecideOnNewMusic += PlayBossMusic;
		On_Player.ItemCheck_MeleeHitNPCs += CheckMeleeHit; //Might need synced
	}

	private static void PlayBossMusic(On_Main.orig_UpdateAudio_DecideOnNewMusic orig, Main self)
	{
		orig(self);

		if (!Main.gameMenu && SimpleEntitySystem.entities.Where(x => x is DevourerOfSoil && x.Center.Distance(Main.LocalPlayer.Center) < 1500).Any())
			Main.newMusic = MusicID.Boss1;
	}

	private static void CheckMeleeHit(On_Player.orig_ItemCheck_MeleeHitNPCs orig, Player self, Item sItem, Rectangle itemRectangle, int originalDamage, float knockBack)
	{
		orig(self, sItem, itemRectangle, originalDamage, knockBack);

		foreach (var entity in SimpleEntitySystem.entities)
		{
			if (entity is DevourerOfSoil dos && !dos.playingDeathAnimation)
			{
				foreach (var position in dos.positions)
				{
					if (itemRectangle.Intersects(dos.GetHitbox(position)))
					{
						dos.OnHit();
						return;
					}
				}
			}
		}
	}

	private void CheckProjectileHit()
	{
		foreach (var projectile in Main.ActiveProjectiles)
		{
			if (projectile.friendly)
			{
				foreach (var position in positions)
				{
					var hitbox = GetHitbox(position);

					if (Collision.CheckAABBvLineCollision(position - hitbox.Size() / 2, hitbox.Size(), projectile.Center - projectile.velocity, projectile.Center) 
						|| projectile.ModProjectile is ModProjectile modProj && modProj.Colliding(projectile.getRect(), hitbox) is true)
					{
						OnHit();
						return;
					}
				}
			}
		}
	}

	public override void Update()
	{
		UpdatePositions();

		var target = Main.player.Where(x => x.whoAmI != Main.maxPlayers && x.active && !x.dead).OrderBy(x => x.Distance(Center)).FirstOrDefault();
		if (justSpawned)
		{
			velocity = new Vector2(Math.Sign(target.Center.X - Center.X) * 2f, -4f); //Leap upwards on spawn
			justSpawned = false;
		}

		if (playingDeathAnimation)
			DoDeathAnimation();
		else
		{
			ChaseTarget(target.Center);
			CheckProjectileHit();
		}

		if (InsideTiles())
		{
			if (Main.rand.NextBool(2))
			{
				var tilePos = (Center / 16).ToPoint();
				var dust = Main.dust[WorldGen.KillTile_MakeTileDust(tilePos.X, tilePos.Y, Framing.GetTileSafely(tilePos))];
				dust.fadeIn = Main.rand.NextFloat(1f, 1.25f);
				dust.noGravity = true;
			} //Spawn travel dusts

			if (soundDelay == 0)
			{
				int delay = (int)MathHelper.Clamp(Center.Distance(target.Center) / 16f, 10, 20);
				soundDelay = delay;

				SoundEngine.PlaySound(SoundID.WormDig, Center);
			} //Play digging sounds based on distance
		}

		rotation = velocity.ToRotation();
		position += velocity;
		soundDelay = Math.Max(soundDelay - 1, 0);
	}

	private void ChaseTarget(Vector2 targetPosition)
	{
		const float speed = 5.5f;

		if (InsideTiles())
			velocity = Vector2.Lerp(velocity, Center.DirectionTo(targetPosition) * speed, .015f);
		else
			velocity = new Vector2(velocity.X * .98f, velocity.Y + .1f);
	}

	private void OnHit()
	{
		velocity.Y -= 2f;
		SoundEngine.PlaySound(SoundID.NPCHit1, Center);
		playingDeathAnimation = true; //Instantly die
	}

	private void UpdatePositions()
	{
		static Vector2 GetSegmentDims(int segment)
		{
			var dims = Dimensions[1];

			if (segment == 0)
				dims = Dimensions[0];
			else if (segment == Length - 1)
				dims = Dimensions[2];

			return dims.ToVector2();
		}

		if (justSpawned)
		{
			for (int i = 0; i < positions.Length; i++)
				positions[i] = Center + Vector2.UnitY * i * GetSegmentDims(i).Y;

			return;
		} //Set comfortable segment spawn positions

		positions[0] = Center;
		for (int i = 1; i < positions.Length; i++)
		{
			var ahead = positions[i - 1];
			var angle = positions[i].DirectionTo(ahead);

			positions[i] = ahead - angle * GetSegmentDims(i).Y;
		}
	}

	private void DoDeathAnimation()
	{
		if (InsideTiles())
		{
			if (justDied)
				velocity.Y = -8f; //Shoot out of the ground before dying
			else
			{
				DoDeathEffects();
				Kill();
			}
		}
		else
		{
			justDied = false;
			velocity = new Vector2(velocity.X * .98f, velocity.Y + .2f);
		}

		void DoDeathEffects()
		{
			if (Main.dedServ)
				return;

			for (int i = 0; i < positions.Length; i++)
			{
				int id = (i == positions.Length - 1) ? 3 : (i == 0) ? 1 : 2;
				int goreType = SpiritReforgedMod.Instance.Find<ModGore>(nameof(DevourerOfSoil) + id).Type;

				Gore.NewGore(null, positions[i], Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f), goreType);

				for (int d = 0; d < 10; d++)
					Dust.NewDustPerfect(positions[i] + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f),
						DustID.Blood, Main.rand.NextVector2Unit() * 2f, 0, default, Main.rand.NextFloat(1f, 1.5f));
			}

			SoundEngine.PlaySound(SoundID.NPCDeath1, Center);
			QuickCameraModifiers.SimpleShakeScreen(Main.LocalPlayer, 3f, 3f, 60, 16 * 30);
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (justSpawned)
			return; //Don't bother drawing the first frame alive because rotation and segment positions haven't been initialized

		var texture = Texture.Value;
		int length = positions.Length - 1;

		for (int i = length; i >= 0; i--)
		{
			int frameY = (i == length) ? 2 : (i == 0) ? 0 : 1;
			var frame = texture.Frame(1, Dimensions.Length, 0, frameY) with { Width = Dimensions[frameY].X, Height = Dimensions[frameY].Y };
			
			var position = positions[i];
			float rot = (i == 0) ? rotation : positions[i].AngleTo(positions[i - 1]);

			var lightColor = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16));

			spriteBatch.Draw(texture, position - Main.screenPosition, frame, lightColor, rot + 1.57f, new Vector2(frame.Width / 2, frame.Height), 1, SpriteEffects.None, 0);
		}
	}

	private Rectangle GetHitbox(Vector2 center) => new((int)center.X - width / 2, (int)center.Y - height / 2, width, height);

	private bool InsideTiles()
	{
		var tile = Framing.GetTileSafely(Center);
		return WorldGen.SolidOrSlopedTile(tile);
	}
}
