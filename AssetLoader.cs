using SpiritReforged.Common.PrimitiveRendering;

internal static class AssetLoader
{
	public static TrailManager VertexTrailManager;

	public static BasicEffect BasicShaderEffect;
	public static IDictionary<string, Texture2D> LoadedTextures = new Dictionary<string, Texture2D>();

	public static void Load(Mod mod)
	{
		ShaderHelpers.GetWorldViewProjection(out Matrix view, out Matrix projection);
		Main.QueueMainThreadAction(() => BasicShaderEffect = new BasicEffect(Main.graphics.GraphicsDevice)
		{
			VertexColorEnabled = true,
			View = view,
			Projection = projection
		});

		VertexTrailManager = new TrailManager(mod);

		//Todo: make this automatic for all texture assets through reflection
		LoadedTextures.Add("Bloom", mod.Assets.Request<Texture2D>("Assets/Textures/Bloom", AssetRequestMode.ImmediateLoad).Value);
	}

	public static void Unload()
	{
		VertexTrailManager = null;
		BasicShaderEffect = null;
		LoadedTextures = new Dictionary<string, Texture2D>();
	}
}
