using SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;

namespace SpiritReforged.Common.PrimitiveRendering;

/// <summary>
/// Static class made to help with manually rendering custom primitive effects
/// For more basic primitive effects, use the vertex trail manager
/// </summary>
public static class PrimitiveRenderer
{
	/// <summary>
	/// Shorthand reference to Main.graphics.GraphicsDevice.
	/// </summary>
	private static GraphicsDevice Graphics => Main.graphics.GraphicsDevice;

	/// <summary>
	/// Renders indexed primitives using the given Vertices, and Indeces to map triangles to given Vertices.<br />
	/// Applies a given shader, or BasicEffect if none is inputted, before rendering, automatically setting WorldViewProjection matrices to properly draw primitives to the screen.<br />
	/// Additional shader parameters must be set before calling this method.
	/// </summary>
	/// <param name="vertices"></param>
	/// <param name="indeces"></param>
	/// <param name="effect"></param>
	public static void RenderPrimitives(VertexPositionColorTexture[] vertices, short[] indeces, PrimitiveType primitiveType = PrimitiveType.TriangleList)
	{
		if (vertices.Length == 0 || indeces.Length == 0)
			return;

		//Graphics.RasterizerState = RasterizerState.CullNone;

		//Determine the number of lines or triangles to draw, given the amount of indeces and type of primitives
		int primitiveCount = 0;
		switch (primitiveType)
		{
			case PrimitiveType.TriangleList:
				primitiveCount = indeces.Length / 3; //Amount of triangles drawn directly equal to 1/3 of inputted indeces
				break;
			case PrimitiveType.TriangleStrip:
				primitiveCount = indeces.Length - 2; //First 3 indeces corresponds to 1 triangle, every index afterwards is another triangle
				break;
			case PrimitiveType.LineList:
				primitiveCount = indeces.Length / 2; //Same as triangle list, but now with lines that take 2 vertices each rather than triangles with 3
				break;
			case PrimitiveType.LineStrip:
				primitiveCount = indeces.Length - 1; //Same as triangle strip, but now with lines that take 2 vertices each rather than triangles with 3
				break;
		}

		//Finally, draw the primitives
		Graphics.DrawUserIndexedPrimitives(primitiveType, vertices, 0, vertices.Length, indeces, 0, primitiveCount);
	}

	/// <summary>
	/// Directly render a given primitive shape, with an optional effect parameter.<br />
	/// Calls the RenderPrimitives method, using the parameters of the given primitive shape to determine vertices, indeces, and type of primitives to draw.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="effect"></param>
	public static void DrawPrimitiveShape(IPrimitiveShape primitiveShape, Effect effect = null)
	{
		ApplyPrimitiveShader(effect);
		primitiveShape.PrimitiveStructure(out VertexPositionColorTexture[] vertices, out short[] indeces);

		RenderPrimitives(vertices, indeces, primitiveShape.GetPrimitiveType);
	}

	public static void DrawPrimitiveShapeBatched(IPrimitiveShape[] primitiveShapes, Effect effect = null)
	{
		ApplyPrimitiveShader(effect);
		foreach (IPrimitiveShape primitiveShape in primitiveShapes)
		{
			primitiveShape.PrimitiveStructure(out VertexPositionColorTexture[] vertices, out short[] indeces);

			RenderPrimitives(vertices, indeces, primitiveShape.GetPrimitiveType);
		}
	}

	private static void ApplyPrimitiveShader(Effect effect = null)
	{
		//If the inputted effect is null, use the static BasicEffect
		if (effect == null)
		{
			BasicEffect basicEffect = SpiritReforgedLoadables.BasicShaderEffect;

			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
				pass.Apply();
		}

		//Otherwise, set WorldViewProjection of the given effect, and apply all passes
		else
		{
			ShaderHelpers.SetEffectMatrices(ref effect);

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				pass.Apply();
		}
	}
}
