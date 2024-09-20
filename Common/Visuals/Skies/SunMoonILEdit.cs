using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.Graphics.Effects;
using static Terraria.Main;

namespace SpiritReforged.Common.Visuals.Skies;

public class SunMoonDraw : ModSystem
{
	public struct SunMoonData
	{
		public SunMoonData()
		{

		}
		public Vector2 Position;
		public float Scale;
		public Color Color;
		public Color SecondaryColor;
		public static float Rotation //No need to il edit to get this
		{
			get
			{
				float rotation = (float)(time / dayLength) * 2f - 7.3f;
				if (!dayTime)
				{
					rotation = (float)(time / nightLength) * 2f - 7.3f;
					if (WorldGen.drunkWorldGen)
						rotation = rotation / 2 + MathHelper.Pi;
				}

				return rotation;
			}
		}
	}

	private static SunMoonData _sunData;
	private static SunMoonData _moonData;
	public static SunMoonData SunDrawData => _sunData;

	public static SunMoonData MoonDrawData => _moonData;

	public override void Load()
	{
		IL_Main.DrawSunAndMoon += StoreSunMoonData; //Store the position of the sun/moon with an il edit
		On_Main.DrawSunAndMoon += DrawSkyUnderSunMoon;
	}

	private void DrawSkyUnderSunMoon(On_Main.orig_DrawSunAndMoon orig, Main self, SceneArea sceneArea, Color moonColor, Color sunColor, float tempMushroomInfluence)
	{
		foreach (string key in AutoloadSkyDict.LoadedSkies.Keys)
		{
			if (SkyManager.Instance[key].IsActive() && SkyManager.Instance[key] is AutoloadedSky { DrawUnderSun: true } autosky)
				autosky.DoDraw(spriteBatch);
		}

		orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
	}

	/// <summary>
	/// Heavily referencing Dominic Karma's edit here to learn how il even works: https://github.com/DominicKarma/Realistic-Sky/blob/main/Content/Sun/SunPositionSaver.cs
	/// </summary>
	/// <param name="il"></param>
	/// <exception cref="ILPatchFailureException"></exception>
	private void StoreSunMoonData(ILContext il)
	{
		try
		{
			ILCursor cursor = new(il);
			int curIndex = 0;
			int[] storedIndex = [0, 0]; //Seperate, as the sun color is easier to grab before the calculations

			//Some notes on the meaning of the below op codes since it looks like gibberish:
			//Ldsfld refers to a static field being pushed onto the stack
			//Stloc means a value being taken from the stack and stored in a local variable
			//Ldloc means to load a variable at a specific index onto the stack
			//Starg means to store a value to a variable at a specific index

			//Grab the scales
			cursor.GotoNext(i => i.MatchLdsfld<Main>("ForcedMinimumZoom"));
			cursor.GotoNext(MoveType.After, i => i.MatchStloc(out curIndex));
			cursor.Emit(OpCodes.Ldloc, curIndex);
			cursor.EmitDelegate<Action<float>>(scale => _sunData.Scale = scale * 1.1f); //multiplied by 1.1f because the sun's scale is randomly multiplied by 1.1 right before drawing

			cursor.GotoNext(MoveType.After, i => i.MatchStloc(out curIndex));
			cursor.Emit(OpCodes.Ldloc, curIndex);
			cursor.EmitDelegate<Action<float>>(scale => _moonData.Scale = scale);

			//Grab the index of the sun colors
			cursor.GotoNext(i => i.MatchLdsfld<Main>("atmo")); //Jump to the nearest static variable before the colors are defined
			cursor.GotoNext(i => i.MatchLdloca(out storedIndex[0])); //Sun color 1
			cursor.GotoNext(i => i.MatchLdloca(out storedIndex[1])); //Sun color 2

			//Grab sun position
			cursor.GotoNext(i => i.MatchLdsfld<Main>("sunModY")); //Go to when the static field Main.sunModY or Main.moonModY is being pushed onto the stack
			cursor.GotoNext(MoveType.After, i => i.MatchStloc(out curIndex)); //Go one index after the sun's position is defined, and output that position's index
			cursor.Emit(OpCodes.Ldloc, curIndex); //Load the position vector onto the stack with the index we just got
			cursor.EmitDelegate<Action<Vector2>>(position => _sunData.Position = position); //Set the sun position during day, using the position vector on the stack

			//Set sun colors after they've been modified
			cursor.Emit(OpCodes.Ldloc, storedIndex[0]);
			cursor.EmitDelegate<Action<Color>>(color => _sunData.Color = color);
			cursor.Emit(OpCodes.Ldloc, storedIndex[1]);
			cursor.EmitDelegate<Action<Color>>(color2 => _sunData.SecondaryColor = color2);

			//Set the moon color from the parameter, and divide it
			//Ideally I wouldn't need to get the divisor and manually divide it, and could just get the parameter's value after division directly
			//but unfortunately idk how to do that
			cursor.GotoNext(i => i.MatchLdsfld<Main>("atmo"));
			cursor.GotoNext(MoveType.After, i => i.MatchStloc(out curIndex));

			cursor.Emit(OpCodes.Ldarg_1); //Argument corresponding to moon color
			cursor.Emit(OpCodes.Ldloc, curIndex);
			cursor.EmitDelegate<Action<Color, float>>((moonColor, num13) => _moonData.Color = moonColor * num13);

			//Grab moon position
			cursor.GotoNext(i => i.MatchLdsfld<Main>("moonModY")); //Go to when the static field Main.sunModY or Main.moonModY is being pushed onto the stack
			cursor.GotoNext(MoveType.After, i => i.MatchStloc(out curIndex)); //Go one index after the sun's position is defined, and output that position's index
			cursor.Emit(OpCodes.Ldloc, curIndex); //Load the position vector onto the stack with the index we just got
			cursor.EmitDelegate<Action<Vector2>>(position => _moonData.Position = position); //Set the sun position during day, using the position vector on the stack

		}
		catch (Exception e)
		{
			throw new ILPatchFailureException(SpiritReforgedMod.Instance, il, e);
		}
	}

	/// <summary>
	/// Redraws the sun, assuming this is being called from a sky's draw hook
	/// </summary>
	/// <param name="opacity"></param>
	public static void DrawSunFromSky()
	{
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, DefaultSamplerState, DepthStencilState.None, Rasterizer, null, BackgroundViewMatrix.EffectMatrix);

		Texture2D sunTex = TextureAssets.Sun.Value;
		if (eclipse)
			sunTex = TextureAssets.Sun3.Value;
		else if (!gameMenu && player[myPlayer].head == 12)
			sunTex = TextureAssets.Sun2.Value;

		spriteBatch.Draw(sunTex, SunDrawData.Position, null, SunDrawData.Color, SunMoonData.Rotation, sunTex.Size() / 2, SunDrawData.Scale, 0, 0);
		spriteBatch.Draw(sunTex, SunDrawData.Position, null, SunDrawData.SecondaryColor, SunMoonData.Rotation, sunTex.Size() / 2, SunDrawData.Scale, 0, 0);

		//Copy pasted from vanilla's spritebatch params before drawing backgrounds
		spriteBatch.End();
		Matrix transformationMatrix = BackgroundViewMatrix.TransformationMatrix;
		transformationMatrix.Translation -= BackgroundViewMatrix.ZoomMatrix.Translation * new Vector3(1f, BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f, 1f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, transformationMatrix);
	}
}