using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

internal class ResistanceTextHandler : ILoadable
{
	private struct ResistanceText(int combatTextIndex, float damage, float resistance)
	{
		public int cIndex = combatTextIndex;
		public float damage = damage;
		public float resistanceValue = resistance;

		public readonly void Draw()
		{
			const int timeMax = 10;

			int resistanceDamage = (int)(damage * resistanceValue);
			if (resistanceDamage == 0)
				return;

			var shield = TextureAssets.Extra[58].Value;

			var cText = Main.combatText[cIndex];
			int time = Math.Clamp(cText.lifeTime - 35, 0, timeMax);
			float span = 1f - (float)time / timeMax;

			string text = resistanceDamage.ToString();
			var font = FontAssets.CombatText[0].Value;
			var origin = font.MeasureString(text) / 2;
			var position = cText.position - Main.screenPosition + origin - new Vector2(span * 18);

			if (Main.LocalPlayer.gravDir == -1)
			{
				float posY = Main.screenHeight - (cText.position.Y - Main.screenPosition.Y);
				position = new Vector2(cText.position.X - Main.screenPosition.X, posY) + origin - new Vector2(span * 18);
			}

			if (time < timeMax)
			{
				Main.spriteBatch.Draw(shield, position, null, Color.Cyan.Additive() * cText.alpha, cText.rotation, shield.Size() / 2, cText.scale * .8f * span, default, 0);
				Main.spriteBatch.Draw(shield, position, null, Color.White * cText.alpha, cText.rotation, shield.Size() / 2, cText.scale * .75f * span, default, 0);

				Utils.DrawBorderStringFourWay(Main.spriteBatch, font, text, position.X, position.Y, GetColor(1f) * span * cText.alpha, GetColor(.25f) * span * cText.alpha, origin, cText.scale);
			}
			else
			{
				cText.alpha = 0; //Turn the default text invisible
				Utils.DrawBorderStringFourWay(Main.spriteBatch, font, ((int)(damage + resistanceDamage)).ToString(), position.X, position.Y, GetColor(.85f), GetColor(.25f), origin, cText.scale);
			}

			Color GetColor(float dark)
			{
				float num3 = cText.scale / CombatText.TargetScale;

				var baseColor = Color.Cyan;
				return new Color((int)(baseColor.R * dark), (int)(baseColor.G * dark), (int)(baseColor.B * dark), baseColor.A) * num3;
			}
		}

		public readonly bool Active() => cIndex >= 0 && cIndex < Main.maxCombatText && Main.combatText[cIndex].active;
	}

	private static int lastOpenIndex;
	private readonly HashSet<ResistanceText> texts = [];

	public void ApplyText(float damage, float resistance) => texts.Add(new(lastOpenIndex, damage, resistance));

	public void Load(Mod mod)
	{
		IL_Player.Hurt_HurtInfo_bool += TrackCombatText;
		IL_Main.DoDraw += PostDrawCombatText;
		On_CombatText.UpdateCombatText += CheckActive;
	}

	private void TrackCombatText(ILContext il)
	{
		ILCursor c = new(il);

		c.GotoNext(MoveType.After, x => x.MatchCall<CombatText>("NewText"));
		c.EmitDelegate(SetText); //Only track player damage associated text
	}

	private int SetText(int index) => lastOpenIndex = index;

	private void PostDrawCombatText(ILContext il)
	{
		ILCursor c = new(il);

		c.GotoNext(x => x.MatchLdsfld<Main>("hideUI"));

		c.GotoNext(MoveType.After, x => x.MatchCall("ReLogic.Graphics.DynamicSpriteFontExtensionMethods", "DrawString")); //Reversed gravity
		c.Emit(OpCodes.Ldloc_S, (byte)35);
		c.EmitDelegate(DrawResistance);

		c.GotoNext(MoveType.After, x => x.MatchCall("ReLogic.Graphics.DynamicSpriteFontExtensionMethods", "DrawString")); //Normal gravity
		c.Emit(OpCodes.Ldloc_S, (byte)35);
		c.EmitDelegate(DrawResistance);
	}

	private void DrawResistance(int index)
	{
		foreach (var text in texts)
			if (text.cIndex == index)
				text.Draw();
	}

	private void CheckActive(On_CombatText.orig_UpdateCombatText orig)
	{
		orig();

		HashSet<ResistanceText> removals = [];

		foreach (var text in texts)
			if (!text.Active())
				removals.Add(text);

		foreach (var removal in removals)
			texts.Remove(removal);
	}

	public void Unload() { }
}
