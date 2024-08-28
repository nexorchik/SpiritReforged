using Terraria.DataStructures;

namespace SpiritReforged.Common.PrimitiveRendering;

public static class TrailDetours
{
	public static void Initialize()
	{
		On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += Projectile_NewProjectile;
		On_Main.DrawCachedProjs += Main_DrawCachedProjs;
		On_Main.DrawProjectiles += Main_DrawProjectiles;
	}

	public static void Unload()
	{
		On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -= Projectile_NewProjectile;
		On_Main.DrawCachedProjs -= Main_DrawCachedProjs;
		On_Main.DrawProjectiles -= Main_DrawProjectiles;
	}

	private static void Main_DrawCachedProjs(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
	{
		if (!Main.dedServ && projCache == Main.instance.DrawCacheProjsBehindNPCs)
			SpiritReforgedLoadables.VertexTrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderCachedProjsBehindNPC);

		orig(self, projCache, startSpriteBatch);
	}
	private static void Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		if (!Main.dedServ)
			SpiritReforgedLoadables.VertexTrailManager.DrawTrails(Main.spriteBatch, TrailLayer.UnderProjectile);

		orig(self);

		if (!Main.dedServ)
			SpiritReforgedLoadables.VertexTrailManager.DrawTrails(Main.spriteBatch, TrailLayer.AboveProjectile);
	}

	private static int Projectile_NewProjectile(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, IEntitySource source, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
	{
		int index = orig(source, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
		Projectile projectile = Main.projectile[index];

		if (projectile.ModProjectile is ITrailProjectile)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				(projectile.ModProjectile as ITrailProjectile).DoTrailCreation(SpiritReforgedLoadables.VertexTrailManager);

			else
			{
				//add netcode here again
				//SpiritMod.WriteToPacket(SpiritMod.Instance.GetPacket(), (byte)MessageType.SpawnTrail, index).Send();
			}
		}

		return index;
	}
}