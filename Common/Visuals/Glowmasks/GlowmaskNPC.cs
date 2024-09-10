using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Core;

namespace SpiritReforged.Common.Visuals.Glowmasks;

internal class GlowmaskNPC : GlobalNPC
{
	public readonly struct GlowmaskInfo(Asset<Texture2D> glowmask, Func<NPC, Color> drawColor)
	{
		public readonly Asset<Texture2D> Glowmask = glowmask;
		public readonly Func<NPC, Color> GetDrawColor = drawColor;
	}

	public static Dictionary<int, GlowmaskInfo> NpcIdToGlowmask = [];

	public override void SetStaticDefaults()
	{
		var types = AssemblyManager.GetLoadableTypes(Mod.Code).Where(x => typeof(ModNPC).IsAssignableFrom(x) && !x.IsAbstract
			&& Attribute.IsDefined(x, typeof(AutoloadGlowmaskAttribute)));
		var getId = typeof(ModContent).GetMethod(nameof(ModContent.NPCType));
		var searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		foreach (var type in types)
		{
			int id = (int)getId.MakeGenericMethod(type).Invoke(null, null);
			string colString = (Attribute.GetCustomAttribute(type, typeof(AutoloadGlowmaskAttribute)) as AutoloadGlowmaskAttribute).StringData;
			Func<NPC, Color> color;
			
			if (!colString.StartsWith("Method:"))
				color = (_) => GetColorFromString(colString);
			else
			{
				string[] split = colString.Split(' ');
				var colorMethod = Mod.Code.GetType("SpiritReforged." + split[0]["Method:".Length..]).GetMethod(split[1], searchFlags);
				color = Delegate.CreateDelegate(typeof(Func<NPC, Color>), null, colorMethod) as Func<NPC, Color>;
			}

			NpcIdToGlowmask.Add(id, new(ModContent.Request<Texture2D>(ModContent.GetModNPC(id).Texture + "_Glow"), color));
		}
	}

	private static Color GetColorFromString(string colorString)
	{
		string[] rgba = colorString.Split(',');

		if (rgba.Length <= 2 || rgba.Length >= 5)
			throw new InvalidCastException("GlowmaskAttribute GlowColorString should be R,G,B or R,G,B,A!");

		byte r = byte.Parse(rgba[0]);
		byte g = byte.Parse(rgba[1]);
		byte b = byte.Parse(rgba[2]);

		if (rgba.Length == 3)
			return new Color(r, g, b);

		return new Color(r, g, b, byte.Parse(rgba[3]));
	}

	public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (NpcIdToGlowmask.TryGetValue(npc.type, out var glow)) 
		{
			Vector2 pos = npc.Center - screenPos + new Vector2(0, npc.gfxOffY);
			SpriteEffects effect = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.Draw(glow.Glowmask.Value, pos, npc.frame, glow.GetDrawColor(npc), npc.rotation, npc.frame.Size() / 2f, npc.scale, effect, 0);
		}
	}
}
