namespace SpiritReforged.Common.PrimitiveRendering.PrimitiveShape;

public interface IPrimitiveShape
{
	/// <summary>
	/// The type of primitive drawing intended for the shape
	/// </summary>
	PrimitiveType GetPrimitiveType { get; }

	/// <summary>
	/// The structure of the primitive shape to draw.<br />
	/// Outputs arrays of VertexPositionColors and shorts to input as buffers for the graphics device
	/// </summary>
	/// <param name="vertices"></param>
	/// <param name="indexes"></param>
	void PrimitiveStructure(out VertexPositionColorTexture[] vertices, out short[] indexes);
}