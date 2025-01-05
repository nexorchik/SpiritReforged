using SpiritReforged.Common.ItemCommon.Pins;
using SpiritReforged.Common.UI.System;
using Terraria.GameInput;
using Terraria.UI;

namespace SpiritReforged.Common.UI.PinInterface;

internal class PinUIState : AutoUIState
{
	private static Asset<Texture2D> buttonTexture;

	private UIElement openButton;
	private bool _listActive = false;

	public override void OnInitialize()
	{
		Width = Height = StyleDimension.Fill;

		buttonTexture = ModContent.Request<Texture2D>(GetType().Namespace.Replace(".", "/") + "/OpenButton");

		openButton = new();
		openButton.Width.Set(30, 0);
		openButton.Height.Set(30, 0);
		openButton.Top.Set(-openButton.Height.Pixels - 14, 1); //Position next to the 'close map' button
		openButton.Left.Set(openButton.Width.Pixels + 14, 0);
		openButton.OnLeftClick += ToggleList;
		Append(openButton);

		Main.OnPostFullscreenMapDraw += ForceDraw;
	}

	private void ToggleList(UIMouseEvent evt, UIElement listeningElement) => SetStorageSlots(!(_listActive = !_listActive));

	private void ForceDraw(Vector2 arg1, float arg2) //By default, UI doesn't draw when the fullscreen map is open
	{
		if (UISystem.GetState<PinUIState>().UserInterface.CurrentState is null)
			UISystem.SetActive<PinUIState>();

		int oldMouseX = Main.mouseX;
		int oldMouseY = Main.mouseY;

		PlayerInput.SetZoom_UI();

		Main.spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

		if (UISystem.GetState<PinUIState>().UserInterface.CurrentState is not null)
		{
			UserInterface.CurrentState.Update(new GameTime());
			UserInterface.CurrentState.Draw(Main.spriteBatch);
		}

		Main.spriteBatch.End();

		Main.mouseX = oldMouseX;
		Main.mouseY = oldMouseY;
	}

	public override void Update(GameTime gameTime)
	{
		if (!Main.mapFullscreen)
		{
			SetStorageSlots(true);
			_listActive = false;

			UISystem.SetInactive<PinUIState>(); //Close the UI
			return;
		}

		base.Update(gameTime);
	}

	private void SetStorageSlots(bool clear)
	{
		if (clear)
		{
			List<UIElement> removals = [];

			foreach (var item in Children)
				if (item is PinUISlot)
					removals.Add(item);

			foreach (var item in removals)
				RemoveChild(item);
		}
		else
		{
			int count = 0;
			foreach (string name in PinSystem.ItemByName.Keys)
			{
				Append(new PinUISlot(name, PinPlayer.Obtained(Main.LocalPlayer, name))
				{
					Left = new StyleDimension(openButton.Left.Pixels + 32 + count * 32, 0),
					Top = new StyleDimension(openButton.Top.Pixels + 2, openButton.Top.Percent)
				});

				count++;
			}
		}
	}

	public override void Draw(SpriteBatch spriteBatch) => base.Draw(spriteBatch);

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);

		if (openButton.IsMouseHovering)
		{
			Main.LocalPlayer.mouseInterface = true;

			DrawButton(Color.White, 1);
		}
		else
		{
			DrawButton(Color.White * .5f, 0);
		}

		void DrawButton(Color color, int frame)
		{
			var position = openButton.GetDimensions().Center() - new Vector2(buttonTexture.Width() / 2, buttonTexture.Height() / 4);
			var source = buttonTexture.Frame(1, 2, 0, frame, sizeOffsetY: -2);

			spriteBatch.Draw(buttonTexture.Value, position, source, color);
		}
	}
}
