namespace SpiritReforged.Content.Jungle.Bamboo.Tiles;

public class BambooPikePlayer : ModPlayer
{
	public override void PostUpdate()
	{
		var tile = Framing.GetTileSafely(Player.Bottom - new Vector2(8));

		if (Player.velocity.Y >= 1.25f && tile.HasTile && tile.TileType == ModContent.TileType<BambooPike>() && tile.TileFrameY == 0)
			BambooPike.Strike(Player);
	}
}

public class BambooPikeNPC : GlobalNPC
{
	public override void PostAI(NPC npc)
	{
		var tile = Framing.GetTileSafely(npc.Bottom - new Vector2(8));

		if (npc.velocity.Y >= 1.25f && tile.HasTile && tile.TileType == ModContent.TileType<BambooPike>() && tile.TileFrameY == 0)
			BambooPike.Strike(npc);
	}
}
