namespace SpiritReforged.Common.Visuals.Glowmasks;

internal class GlowmaskNPC : GlobalNPC
{
	public static Dictionary<int, GlowmaskInfo> NpcIdToGlowmask = [];

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (NpcIdToGlowmask.TryGetValue(npc.type, out var glow) && glow.DrawAutomatically) 
		{
			Vector2 pos = npc.Center - screenPos + new Vector2(0, npc.gfxOffY);
			SpriteEffects effect = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Main.EntitySpriteDraw(glow.Glowmask.Value, pos, npc.frame, glow.GetDrawColor(npc), npc.rotation, npc.frame.Size() / 2f, npc.scale, effect, 0);
		}
	}
}
