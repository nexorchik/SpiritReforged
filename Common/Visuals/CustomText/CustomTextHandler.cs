using MonoMod.Cil;
using System.Linq;
using Terraria.GameInput;

namespace SpiritReforged.Common.Visuals.CustomText;

internal class CustomTextHandler : ILoadable
{
	public bool HasTag => _currentTag is not null;

	private static readonly HashSet<CustomText> customText = [];
	private bool _wasSignHover;
	private string _currentTag;

	public void Load(Mod mod)
	{
		foreach (var type in GetType().Assembly.GetTypes())
		{
			if (type.IsSubclassOf(typeof(CustomText)) && !type.IsAbstract)
				customText.Add((CustomText)Activator.CreateInstance(type));
		}

		On_Main.DrawMouseOver += TrackSignText;
		IL_Main.DrawMouseOver += ModifySignHover;
		On_Main.TextDisplayCache.PrepareCache += ModifySignMenu;
		On_Sign.TextSign += VerifyEditTag;
	}

	private void TrackSignText(On_Main.orig_DrawMouseOver orig, Main self)
	{
		if (!Main.mouseText && !Main.LocalPlayer.mouseInterface && Main.signHover != -1)
		{
			var sign = Main.sign[Main.signHover];
			if (sign != null)
			{
				string oldText = sign.text;

				if (!_wasSignHover)
					VerifyTag(oldText);

				_wasSignHover = true;

				if (HasTag)
				{
					sign.text = sign.text.Remove(0, _currentTag.Length); //Remove the special tag before drawing

					orig(self);

					sign.text = oldText; //Revert
					return;
				}
			}
		}

		_wasSignHover = false;
		orig(self);
	}

	/// <summary> Verifies whether the given text contains any <see cref="CustomText.Key"/>s and assigns <see cref="_currentTag"/>. <para/>
	/// Additionally parses parameter data corresponding to the current <see cref="customText"/>. </summary>
	/// <param name="signText"> The sign text. </param>
	private void VerifyTag(string signText)
	{
		const char close = '>';
		const char paramsIndicator = ':';

		foreach (var sig in customText)
		{
			string key = $"<{sig.Key}";
			int length = Math.Min(key.Length, signText.Length);

			if (signText.IndexOf(key, 0, length) == 0 && signText.Contains(close))
			{
				int startIndex = signText.IndexOf(paramsIndicator);

				if (startIndex == -1) //No parameter indicator
				{
					_currentTag = key + close;
					GetText(_currentTag)?.ParseParams(null);
				}
				else //Appears to have parameters; try to parse them
				{
					string paramsText = string.Empty;
					for (int i = startIndex + 1; i < signText.Length; i++)
					{
						if (signText[i] == close)
							break;

						paramsText += signText[i];
					}

					string currentTag = key + paramsIndicator + paramsText + close;
					if (GetText(currentTag)?.ParseParams(paramsText) is true) //Whether parsing was actually successful according to this CustomText
						_currentTag = currentTag;
					else
						_currentTag = null;
				}

				return;
			}
		}

		_currentTag = null;
	}

	private static CustomText GetText(string tag)
	{
		try
		{
			return customText.Where(x => tag.Contains(x.Key)).First();
		}
		catch
		{
			return null;
		}
	}

	private void ModifySignHover(ILContext il)
	{
		ILCursor c = new(il);
		ILLabel label = null;

		c.GotoNext(x => x.MatchCallvirt<SpriteBatch>("End"));
		c.GotoPrev(MoveType.After, x => x.MatchBrtrue(out label));

		if (label is null)
			return;

		c.EmitDelegate(CheckHasTag);
		c.EmitBrfalse(label);
	}

	/// <summary> Calculates custom sign text drawing and passes the data into the <see cref="customText"/> corresponding to <see cref="_currentTag"/>. </summary>
	/// <returns> Whether to draw normal sign text. </returns>
	private bool CheckHasTag()
	{
		if (HasTag)
		{
			string[] array = Utils.WordwrapString(Main.sign[Main.signHover].text, FontAssets.MouseText.Value, 460, 10, out int lineAmount); //Abbreviated vanilla code
			lineAmount++;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, default, SamplerState.PointClamp, null, RasterizerState.CullNone, null, Main.UIScaleMatrix);

			PlayerInput.SetZoom_UI();

			var screenSize = Main.ScreenSize;
			var mouse = Main.MouseScreen;

			PlayerInput.SetZoom_UI();
			PlayerInput.SetZoom_Test();
			float width = 0;

			for (int k = 0; k < lineAmount; k++)
			{
				float x = FontAssets.MouseText.Value.MeasureString(array[k]).X;
				width = Math.Max(width, x);
			}

			width = Math.Min(width, 460);
			var vector = mouse + new Vector2(16);

			if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
				vector += new Vector2(8, 2);

			vector = Vector2.Min(vector, new Vector2(screenSize.X - width, screenSize.Y - 30 * lineAmount));
			var rectangle = new Rectangle((int)vector.X - 10, (int)vector.Y - 5, (int)width + 20, 30 * lineAmount + 7);
			GetText(_currentTag)?.Draw(rectangle, array, lineAmount); //Draw our custom text

			Main.mouseText = true;
			return false;
		}

		return true;
	}

	/// <summary> Removes the current tag text (<see cref="_currentTag"/>) from sign dialogue when interacting with one. </summary>
	/// <param name="orig"></param>
	/// <param name="self"></param>
	/// <param name="text"> The cached <see cref="Main.npcChatText"/> value. </param>
	/// <param name="baseColor"> The color of the text. </param>
	private void ModifySignMenu(On_Main.TextDisplayCache.orig_PrepareCache orig, object self, string text, Color baseColor)
	{
		if (HasTag && !Main.editSign)
		{
			string oldText = text;
			text = oldText.Remove(0, Math.Min(_currentTag.Length, oldText.Length));
		}

		orig(self, text, baseColor);
	}

	/// <summary> Updates <see cref="_currentTag"/> when finished editing a sign. </summary>
	/// <param name="orig"></param>
	/// <param name="i"> The index of the sign in <see cref="Main.sign"/>. </param>
	/// <param name="text"> The sign text. </param>
	private void VerifyEditTag(On_Sign.orig_TextSign orig, int i, string text)
	{
		orig(i, text);
		VerifyTag(text);
	}

	public void Unload() { }
}
