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
				if (TryGetGlowmask(ModContent.GetModNPC(id).Texture, out var glowMask))
					GlowmaskNPC.NpcIdToGlowmask.Add(id, new(glowMask, color, autoDraw));
			}

			else if (typeof(ModTile).IsAssignableFrom(type))
			{
				int id = (int)tileGetId.MakeGenericMethod(type).Invoke(null, null);
				if (TryGetGlowmask(ModContent.GetModTile(id).Texture, out var glowMask))
					GlowmaskTile.TileIdToGlowmask.Add(id, new(glowMask, color, autoDraw));
			}

			else if (typeof(ModProjectile).IsAssignableFrom(type))
			{
				int id = (int)projGetId.MakeGenericMethod(type).Invoke(null, null);
				if (TryGetGlowmask(ModContent.GetModProjectile(id).Texture, out var glowMask))
					GlowmaskProjectile.ProjIdToGlowmask.Add(id, new(glowMask, color, autoDraw));
			}

			else if (typeof(ModItem).IsAssignableFrom(type))
			{
				int id = (int)itemGetId.MakeGenericMethod(type).Invoke(null, null);
				if (TryGetGlowmask(ModContent.GetModItem(id).Texture, out var glowMask))
					GlowmaskItem.ItemIdToGlowmask.Add(id, new(glowMask, color, autoDraw));
			}
		}
	}

	private static bool TryGetGlowmask(string texture, out Asset<Texture2D> asset)
	{
		if (ModContent.RequestIfExists(texture + "_Glow", out asset))
			return true;

		if (ModContent.RequestIfExists(texture + "_glow", out asset))
			return true;

		return false;
	}
}
