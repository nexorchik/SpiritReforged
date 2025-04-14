using System.IO;
using Terraria.DataStructures;

namespace SpiritReforged.Content.Underground.NPCs;

public class DunceCrab : ModNPC
{
	private enum State : byte
	{
		Idle,
		Crawl,
		Hide,
		UnHide,
		Fall,
		Flail
	}

	private enum Side : byte
	{
		Up,
		Right,
		Down,
		Left
	}

	private static readonly int[] endFrames = [1, 4, 6, 7, 1, 5];

	public ref float Animation => ref NPC.ai[0];
	public ref float Surface => ref NPC.ai[1];
	private float Angle => Surface / 4f * MathHelper.TwoPi;

	private byte _style;

	public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 7;

	public override void SetDefaults()
	{
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.Size = new Vector2(16);
		NPC.damage = 18;
		NPC.lifeMax = 50;
		NPC.direction = 1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		_style = (byte)Main.rand.Next(3);
		NPC.netUpdate = true;
	}

	public override void AI()
	{
		const int fluff = 2;
		/*if (NPC.wet)
		{
			NPC.aiStyle = NPCAIStyleID.Fighter;
			NPC.noGravity = false;
			return;
		}
		else
		{
			NPC.aiStyle = -1;
		}*/

		if (Colliding())
		{
			NPC.noGravity = true;
			CrawlAlongTiles();
		}
		else
		{
			NPC.noGravity = false;
			Surface = (int)Side.Up;
		}

		bool Colliding()
		{
			return Collision.SolidCollision(NPC.position - new Vector2(fluff), NPC.width + fluff * 2, NPC.height + fluff * 2) || NPC.collideX || NPC.collideY;
		}
	}

	private void CrawlAlongTiles()
	{
		Animation = (int)State.Crawl;

		if (!NPC.collideX && !NPC.collideY && !IntersectsSlope())
			ResolveSide();

		if (Colliding(true) /*|| IntersectsSlope() && (Side)Surface is Side.Right or Side.Left*/)
			ResolveSide(true);

		if (NPC.velocity.Length() < .05f) //Change direction
		{
			if (++NPC.localAI[0] > 10)
			{
				NPC.direction = -NPC.direction;
				NPC.localAI[0] = 0;
			}
		}
		else
		{
			NPC.localAI[0] = 0;
		}

		float gravity = 2;
		NPC.spriteDirection = NPC.direction;
		NPC.rotation = Utils.AngleLerp(NPC.rotation, MathHelper.WrapAngle(Angle), .1f);
		NPC.velocity = new Vector2(NPC.direction, gravity).RotatedBy(Angle);

		void ResolveSide(bool reverse = false)
		{
			int rev = reverse ? -1 : 1;
			Surface = (int)(Side)((Surface + 1 * NPC.direction * rev) % 4);
		}

		bool Colliding(bool x)
		{
			if (x) //The x axis
				return ((Side)Surface is Side.Up or Side.Down) ? NPC.collideX : NPC.collideY;
			else //The y axis
				return ((Side)Surface is Side.Up or Side.Down) ? NPC.collideY : NPC.collideX;
		}
	}

	private bool IntersectsSlope()
	{
		var area = NPC.getRect();

		for (int i = 0; i < 4; i++)
		{
			Vector2 pos = i switch
			{
				1 => area.TopRight(),
				2 => area.BottomRight(),
				3 => area.BottomLeft(),
				_ => area.TopLeft()
			};

			if (Framing.GetTileSafely(pos).Slope != SlopeType.Solid)
				return true;
		}

		return false;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (Main.dedServ)
			return;

		int limit = _style * 3 + 1;

		for (int i = limit; i < limit + 3; i++)
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * .5f, Mod.Find<ModGore>("Dunce" + i).Type);

		for (int i = 0; i < 15; i++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood);
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frame.Width = 38;
		NPC.frame.X = NPC.frame.Width * (int)Animation + NPC.frame.Width * endFrames.Length * _style;

		if (NPC.velocity != Vector2.Zero || NPC.IsABestiaryIconDummy)
		{
			NPC.frameCounter += 0.12f;
			NPC.frameCounter %= endFrames[(int)Animation % endFrames.Length];
			int frame = (int)NPC.frameCounter;

			NPC.frame.Y = frame * frameHeight;
		}
		else
		{

		}
	}

	public override void SendExtraAI(BinaryWriter writer) => writer.Write(_style);
	public override void ReceiveExtraAI(BinaryReader reader) => _style = reader.ReadByte();

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = TextureAssets.Npc[Type].Value;
		var origin = new Vector2(NPC.frame.Width / 2, NPC.frame.Height - NPC.height / 2 - 4);
		var effects = (NPC.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects);

		return false;
	}
}