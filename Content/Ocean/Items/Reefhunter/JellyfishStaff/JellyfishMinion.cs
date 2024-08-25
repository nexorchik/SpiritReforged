using System.Linq;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;
using SpiritReforged.Common.ProjectileCommon;
using Terraria;
using Terraria.Audio;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.JellyfishStaff;

[Common.Misc.AutoloadMinionBuff()]
public class JellyfishMinion : BaseMinion
{
	public JellyfishMinion() : base(800, 1200, new Vector2(28, 28)) { }

	public bool IsPink = false;

	private ref float AiState => ref Projectile.ai[0];
	private ref float AiTimer => ref Projectile.ai[1];

	public override void AbstractSetStaticDefaults()
	{
		Main.projFrames[Type] = 3;
		ProjectileID.Sets.TrailCacheLength[Type] = 10;
		ProjectileID.Sets.TrailingMode[Type] = 2;
	}

	public override void OnSpawn(IEntitySource source) => IsPink = Main.rand.NextBool(2);

	private const int AISTATE_PASSIVEFLOAT = 0; //jellyfish bouncing around player
	private const int AISTATE_FLYTOPLAYER = 1; //ignore tiles and fly to player if too far
	private const int AISTATE_AIMTOTARGET = 2; //slowly aim to target, then charge at them
	private const int AISTATE_DASH = 3; //during the dash
	private const int AISTATE_SHOOT = 4; //when near target, hover in place and shoot lightning

	//Constants used in drawing methods and for the ai pattern
	private const int SHOOTTIME = 40;
	private const int DASHTIME = 30;

	public override void IdleMovement(Player player)
	{
		Projectile.tileCollide = false;
		if(AiState > AISTATE_FLYTOPLAYER)
		{
			AiTimer = 0;
			AiState = AISTATE_PASSIVEFLOAT;
			Projectile.netUpdate = true;
		}

		int flyToPlayerThreshold = 100; //How far the minion needs to be from the player to start flying to them
		int floatThreshold = flyToPlayerThreshold / 2; //How close the minion needs to be to the player to stop flying to them
		switch(AiState)
		{
			case AISTATE_PASSIVEFLOAT:
				Projectile.rotation -= Utils.AngleLerp(Projectile.rotation, 0, 0.05f);

				float speed = Projectile.Distance(player.Center) / 30;
				speed = Math.Min(speed, 5f);

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * speed, 0.05f);

				//bounce in general direction of player but moved upwards, semi randomized timing
				if(AiTimer >= 60 && Main.rand.NextBool(25))
				{
					Projectile.velocity += Projectile.DirectionTo(player.Center * Main.rand.NextFloat(4, 8));
					Projectile.velocity.Y -= Main.rand.NextFloat(3, 4);
					Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
					AiTimer = 0;
					Projectile.netUpdate = true;
				}

				if(Projectile.Distance(player.Center) > flyToPlayerThreshold)
				{
					AiState = AISTATE_FLYTOPLAYER;
					AiTimer = 0;
					Projectile.netUpdate = true;
				}

				break;

			case AISTATE_FLYTOPLAYER:
				float flySpeed = 12f;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * flySpeed, 0.06f);
				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, AdjustedVelocityAngle, 0.2f);

				if(Projectile.Distance(player.Center) <= floatThreshold)
				{
					AiState = AISTATE_PASSIVEFLOAT;
					AiTimer = 0;
					Projectile.netUpdate = true;
				}

				break;
		}

		int maxDistFromPlayer = 1200;
		if(Projectile.Distance(player.Center) > maxDistFromPlayer)
		{
			Projectile.Center = player.Center;
			Projectile.netUpdate = true;
		}

		AiTimer++;
	}

	public override void TargettingBehavior(Player player, NPC target)
	{
		Projectile.tileCollide = true;
		AiTimer++;
		int ShootRange = 200;

		if(AiState < AISTATE_AIMTOTARGET)
		{
			AiState = Projectile.Distance(target.Center) >= ShootRange ? AISTATE_AIMTOTARGET : AISTATE_SHOOT;
			AiTimer = 0;
			Projectile.netUpdate = true;
		}

		switch (AiState)
		{
			case AISTATE_AIMTOTARGET:
				int aimTime = 40;
				float progress = AiTimer / aimTime;
				float aimSpeed = MathHelper.Lerp(8, 0.25f, progress);
				float interpolationSpeed = MathHelper.Lerp(0.05f, 0.2f, progress);

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * aimSpeed, interpolationSpeed);
				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, AdjustedVelocityAngle, 0.1f);

				if(AiTimer >= aimTime)
				{
					AiState = AISTATE_DASH;
					AiTimer = 0;
					float DashSpeed = 30;
					Projectile.velocity = Projectile.DirectionTo(target.Center) * DashSpeed;
					Projectile.rotation = AdjustedVelocityAngle;
					Projectile.netUpdate = true;
				}

				break;

			case AISTATE_DASH:
				//put vfx here maybe

				Projectile.velocity *= 0.9f;
				if(AiTimer >= DASHTIME)
				{
					AiState = Projectile.Distance(target.Center) >= ShootRange ? AISTATE_AIMTOTARGET : AISTATE_SHOOT;
					AiTimer = 0;
					Projectile.netUpdate = true;
				}

				break;

			case AISTATE_SHOOT:

				Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, 0.07f);
				float speed = Math.Max(Projectile.velocity.Length(), 1); //Start with the remaining speed from the dash or other current movement, and prevent it from getting too low from the slow down
				float slowdownTime = SHOOTTIME * 5; //the total length it takes for the minion to slow down fully

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, -Vector2.UnitY.RotatedBy(Projectile.rotation) * speed, 0.25f); //move it in the direction it's facing
				Projectile.velocity *= MathHelper.Lerp(1, 0.66f, Math.Min(AiTimer/ slowdownTime, 1)); //slow it down over time
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * speed, 0.1f); //then, move it towards the target slowly

				if (AiTimer % SHOOTTIME == 0)
				{
					var bolt = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center) * 7, ModContent.ProjectileType<JellyfishBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, IsPink ? 1 : 0);
					bolt.netUpdate = true;
					Projectile.netUpdate = true;
					Projectile.velocity -= Projectile.DirectionTo(target.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3, 5f);

					if (Projectile.Distance(target.Center) >= ShootRange)
					{
						AiState = AISTATE_AIMTOTARGET;
						AiTimer = 0;
					}
				}

				break;
		}
	}

	public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe)
	{
		framespersecond = 8;
		return true;
	}

	private Color GetColor => IsPink ? new Color(248, 148, 255) : new Color(133, 177, 255);
	private float AdjustedVelocityAngle => Projectile.velocity.ToRotation() + MathHelper.PiOver2;
	public override bool PreDraw(ref Color lightColor)
	{
		Color drawColor = GetColor;
		for (int i = 0; i < 2; i++)
		{
			if (i == 1)
				drawColor.A = 0; //Makes the rest of the sprites that use this color outside of this block have 0 alpha unless changed back manually, but we want that here so there's no need

			Projectile.QuickDraw(Main.spriteBatch, Projectile.rotation, null, drawColor);
		}

		void DrawGlowmask(float opacity)
		{
			var glowMask = (Texture2D)Mod.Assets.Request<Texture2D>("Content/Ocean/Items/Reefhunter/JellyfishStaff/JellyfishMinion_glow");
			Vector2 position = Projectile.Center - Main.screenPosition;
			for (int j = 0; j < 4; j++)
			{
				position += Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * j);
				Main.spriteBatch.Draw(glowMask, position, Projectile.DrawFrame(), Projectile.GetAlpha(drawColor * opacity), Projectile.rotation, Projectile.DrawFrame().Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
		}

		if (AiState == AISTATE_SHOOT)
			DrawGlowmask(Math.Min(AiTimer / SHOOTTIME, 1) / 3);

		if(AiState == AISTATE_DASH)
		{
			Color trailColor = GetColor;
			trailColor.A = 0;
			float opacity = (1 - (float)Math.Cos(MathHelper.TwoPi * AiTimer / DASHTIME)) / 4;
			DrawGlowmask(opacity);
			Projectile.QuickDrawTrail(Main.spriteBatch, opacity, Projectile.rotation, null, trailColor);
		}

		return false;
	}

	public override void PostAI()
	{
		Lighting.AddLight(Projectile.Center, GetColor.ToVector3() * .25f); 
		
		//lifted almost directly from old ai
		foreach (Projectile p in Main.projectile.Where(x => x.active && x != null && x.type == Projectile.type && x.owner == Projectile.owner && x != Projectile))
		{
			if (p.Hitbox.Intersects(Projectile.Hitbox) && AiState != AISTATE_DASH)
				Projectile.velocity += Projectile.DirectionFrom(p.Center) / 10;
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (AiState == AISTATE_DASH)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			Collision.HitTiles(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height);
			Projectile.Bounce(oldVelocity, 0.5f);
		}

		return false;
	}
	public override bool MinionContactDamage() => AiState == AISTATE_DASH;

	public override bool? CanCutTiles() => false;
}