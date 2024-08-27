namespace SpiritReforged.Common.Particle;

public static class ParticleDetours
{
	public static void Initialize()
	{
		On_Main.DrawProjectiles += On_Main_DrawProjectiles;
		On_Main.DrawNPCs += On_Main_DrawNPCs;
		On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;
		On_Main.DrawDust += On_Main_DrawDust;
	}

	private static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
	{
		orig(self);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AboveDust);
	}

	private static void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self)
	{
		orig(self);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AbovePlayer);
	}

	private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
	{
		orig(self, behindTiles);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AboveNPC);
	}

	private static void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		orig(self);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AboveProjectile);
	}

	public static void Unload()
	{
		On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
		On_Main.DrawNPCs -= On_Main_DrawNPCs;
		On_Main.DrawInfernoRings -= On_Main_DrawInfernoRings;
		On_Main.DrawDust -= On_Main_DrawDust;
	}
}
