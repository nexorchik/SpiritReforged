using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpiritReforged.Common.Misc;

namespace SpiritReforged.Content.Ocean.Items.Reefhunter.CascadeArmor;

public class CascadeCombatText : ILoadable
{
	private CombatText _cText;
	private float _damage;

	public float resistanceValue; //Store Cascade resistance bonus before it disappears

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
		c.EmitDelegate(HandleText);
	}

	private int HandleText(int index, float damage)
	{
		if (Main.LocalPlayer.GetModPlayer<CascadeArmorPlayer>().setActive)
		{
			_cText = Main.combatText[index];
			_damage = damage;
		}

		return index;
	}

	private void PostDrawCombatText(ILContext il)
	{
		ILCursor c = new(il);

		c.GotoNext(x => x.MatchLdsfld<Main>("hideUI"));

		c.GotoNext(MoveType.After, x => x.MatchCall("ReLogic.Graphics.DynamicSpriteFontExtensionMethods", "DrawString")); //Reversed gravity
		c.EmitDelegate(DrawResistance);

		c.GotoNext(MoveType.After, x => x.MatchCall("ReLogic.Graphics.DynamicSpriteFontExtensionMethods", "DrawString")); //Normal gravity
		c.EmitDelegate(DrawResistance);
	}

	private void DrawResistance()
	{
		const int timeMax = 10;

		if (!TextActive())
			return;

		int resistanceDamage = (int)(_damage * resistanceValue);
		if (resistanceDamage == 0)
			return;

		int time = Math.Clamp(_cText.lifeTime - 35, 0, timeMax);
		float span = 1f - (float)time / timeMax;

		string text = resistanceDamage.ToString();
		var font = FontAssets.CombatText[0].Value;
		var origin = font.MeasureString(text) / 2;
		var position = _cText.position - Main.screenPosition + origin - new Vector2(span * 18);

		if (time < timeMax)
		{
			Main.spriteBatch.Draw(TextureAssets.Extra[58].Value, position, null, Color.Cyan.Additive() * _cText.alpha, _cText.rotation, TextureAssets.Extra[58].Size() / 2, _cText.scale * .8f * span, default, 0);
			Main.spriteBatch.Draw(TextureAssets.Extra[58].Value, position, null, Color.White * _cText.alpha, _cText.rotation, TextureAssets.Extra[58].Size() / 2, _cText.scale * .75f * span, default, 0);
			
			Utils.DrawBorderStringFourWay(Main.spriteBatch, font, text, position.X, position.Y, GetColor(1f) * span * _cText.alpha, GetColor(.25f) * span * _cText.alpha, origin, _cText.scale);
		}
		else
		{
			_cText.alpha = 0; //Turn the default text invisible
			Utils.DrawBorderStringFourWay(Main.spriteBatch, font, ((int)(_damage + resistanceDamage)).ToString(), position.X, position.Y, GetColor(.85f), GetColor(.25f), origin, _cText.scale);
		}

		Color GetColor(float dark)
		{
			float num3 = _cText.scale / CombatText.TargetScale;

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
		if (_cText is null || !_cText.active || resistanceValue == 0)
		{
			_cText = null;
			resistanceValue = 0;
			return false;
		}

		return true;
	}

	public void Unload() { }
}
