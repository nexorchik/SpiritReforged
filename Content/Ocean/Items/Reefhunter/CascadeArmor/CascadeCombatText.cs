using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

internal class CascadeCombatText : ILoadable
{
	private int _cIndex = -1;
	private float _damage;

	public float resistanceValue; //Store the Cascade resistance bonus before it disappears

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
		c.Emit(OpCodes.Ldloc_S, (byte)4);
		c.EmitDelegate(SetText);
	}

	private int SetText(int index, float damage)
	{
		if (Main.LocalPlayer.GetModPlayer<CascadeArmorPlayer>().setActive && resistanceValue != 0)
		{
			_cIndex = index;
			_damage = damage;
		}

		return index;
	}

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
		const int timeMax = 10;

		if (index != _cIndex || !TextActive())
			return;

		int resistanceDamage = (int)(_damage * resistanceValue);
		if (resistanceDamage == 0)
			return;

		var shield = TextureAssets.Extra[58].Value;

		var cText = Main.combatText[_cIndex];
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
			Utils.DrawBorderStringFourWay(Main.spriteBatch, font, ((int)(_damage + resistanceDamage)).ToString(), position.X, position.Y, GetColor(.85f), GetColor(.25f), origin, cText.scale);
		}

		Color GetColor(float dark)
		{
			float num3 = cText.scale / CombatText.TargetScale;

			var baseColor = Color.Cyan;
			return new Color((int)(baseColor.R * dark), (int)(baseColor.G * dark), (int)(baseColor.B * dark), baseColor.A) * num3;
		}
	}

	private void CheckActive(On_CombatText.orig_UpdateCombatText orig)
	{
		orig();
		TextActive();
	}

	private bool TextActive()
	{
		if (_cIndex == -1 || !Main.combatText[_cIndex].active || resistanceValue == 0)
		{
			_cIndex = -1;
			resistanceValue = 0;
			return false;
		}

		return true;
	}

	public void Unload() { }
}
