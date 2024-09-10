namespace SpiritReforged.Common.Visuals.Glowmasks;

/// <summary>
/// Autoloads the glowmask of the according Entity (currently, only NPCs).<br/>
/// <paramref name="stringData"/> is one of two things. First, the Color of the glowmask, if constant, in one of these formats:<br/>
/// <c>R,G,B,A</c><br/><c>R,G,B</c><br/>All numbers are bytes.<br/><br/>
/// If you want a dynamic color, you can create a static method that takes an <c>NPC</c> and returns <c>Color</c>.<br/>
/// You'll need a fully qualified class name and the name of the method, prepended by "Method:". For example,
/// <code>[AutoloadGlowmask("Method:Content.Savanna.NPCs.Gar.Gar Glow")]</code><br/>
/// This would call <c>Gar.Glow</c>. 
/// </summary>
/// <param name="stringData"></param>
[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadGlowmaskAttribute(string stringData) : Attribute
{
	public string StringData = stringData;
}
