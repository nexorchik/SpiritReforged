using System.Linq;
using Terraria.ModLoader.Core;

namespace SpiritReforged.Common.Visuals.Glowmasks;

internal class GlowmaskAutoloader : ModSystem
{
	public override void PostSetupContent()
	{
		var types = AssemblyManager.GetLoadableTypes(Mod.Code).Where(x => !x.IsAbstract && Attribute.IsDefined(x, typeof(AutoloadGlowmaskAttribute)));
		var npcGetId = typeof(ModContent).GetMethod(nameof(ModContent.NPCType));
		var tileGetId = typeof(ModContent).GetMethod(nameof(ModContent.TileType));
		var projGetId = typeof(ModContent).GetMethod(nameof(ModContent.ProjectileType));
		var itemGetId = typeof(ModContent).GetMethod(nameof(ModContent.ItemType));

		foreach (var type in types)
		{
			Func<object, Color> color = AutoloadGlowmaskAttribute.GetAttributeInfo(Mod, type, out bool autoDraw);

			if (typeof(ModNPC).IsAssignableFrom(type))
			{
				int id = (int)npcGetId.MakeGenericMethod(type).Invoke(null, null);
				GlowmaskNPC.NpcIdToGlowmask.Add(id, new(ModContent.Request<Texture2D>(ModContent.GetModNPC(id).Texture + "_Glow"), color, autoDraw));
			}
			else if (typeof(ModTile).IsAssignableFrom(type))
			{
				int id = (int)tileGetId.MakeGenericMethod(type).Invoke(null, null);
				GlowmaskTile.TileIdToGlowmask.Add(id, new(ModContent.Request<Texture2D>(ModContent.GetModTile(id).Texture + "_Glow"), color, autoDraw));
			}
			else if (typeof(ModProjectile).IsAssignableFrom(type))
			{
				int id = (int)projGetId.MakeGenericMethod(type).Invoke(null, null);
				GlowmaskProjectile.ProjIdToGlowmask.Add(id, new(ModContent.Request<Texture2D>(ModContent.GetModProjectile(id).Texture + "_Glow"), color, autoDraw));
			}
			else if (typeof(ModItem).IsAssignableFrom(type))
			{
				int id = (int)itemGetId.MakeGenericMethod(type).Invoke(null, null);
				GlowmaskItem.ItemIdToGlowmask.Add(id, new(ModContent.Request<Texture2D>(ModContent.GetModItem(id).Texture + "_Glow"), color, autoDraw));
			}
		}
	}
}
