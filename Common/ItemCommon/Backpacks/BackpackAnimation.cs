using Terraria.Audio;

namespace SpiritReforged.Common.ItemCommon.Backpacks;

/// <summary> Controls backpack animations when attempting to move them. </summary>
internal class BackpackAnimation : GlobalItem
{
	private float visualCounter;

	internal void StartAnimation()
	{
		SoundEngine.PlaySound(SoundID.Grab);
		visualCounter = MathHelper.Min(visualCounter + 1, 2.5f);
	}

	public override bool InstancePerEntity => true;
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.ModItem is not null and BackpackItem;

	public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (visualCounter > 0)
		{
			var texture = TextureAssets.Item[item.type].Value;
			float rotation = (float)Math.Sin(visualCounter * 4f) * .3f;

			spriteBatch.Draw(texture, position, frame, Color.Black * .2f, rotation, origin, scale + visualCounter * .15f, SpriteEffects.None, 0);
			spriteBatch.Draw(texture, position - new Vector2(0, visualCounter * 5), frame, drawColor, rotation, origin, scale + visualCounter * .15f, SpriteEffects.None, 0);

			visualCounter = MathHelper.Max(visualCounter - .075f, 0);

			return false;
		}

		return true;
	}

	public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
	{
		if (visualCounter > 0 && line.Mod == "Terraria" && line.Name == "Tooltip1")
		{
			var color = Color.Lerp(Main.MouseTextColorReal, Color.White, visualCounter);
			var position = new Vector2(line.X, line.Y) + (Vector2.UnitX * visualCounter * 3).RotatedBy(Math.Sin(visualCounter * 10f));

			Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(position.X, line.Y), Color.Black * visualCounter * .2f);
			Utils.DrawBorderString(Main.spriteBatch, line.Text, position, color);

			return false;
		}

		return true;
	}
}
