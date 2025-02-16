using SpiritReforged.Common.Misc;

namespace SpiritReforged.Common.Particle;

public static class ParticleDetours
{
	public static void Initialize()
	{
		On_Main.DrawProjectiles += On_Main_DrawProjectiles;
		On_Main.DrawNPCs += On_Main_DrawNPCs;
		On_Main.DrawInfernoRings += On_Main_DrawInfernoRings;
		On_Main.DoDraw_Tiles_NonSolid += On_Main_DoDraw_Tiles_NonSolid;
	}

	private static void On_Main_DrawInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self)
	{
		orig(self);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AbovePlayer);
		Main.spriteBatch.RestartToDefault();
	}

	private static void On_Main_DrawNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
	{
		orig(self, behindTiles);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AboveNPC);
	}

	private static void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
	{
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.BelowProjectile);
		Main.spriteBatch.End();
		orig(self);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.AboveProjectile);
	}

	private static void On_Main_DoDraw_Tiles_NonSolid(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
	{
		orig(self);
		ParticleHandler.DrawAllParticles(Main.spriteBatch, ParticleLayer.BelowSolids);
	}

	public static void Unload()
	{
		On_Main.DrawProjectiles -= On_Main_DrawProjectiles;
		On_Main.DrawNPCs -= On_Main_DrawNPCs;
		On_Main.DrawInfernoRings -= On_Main_DrawInfernoRings;
	}
}
