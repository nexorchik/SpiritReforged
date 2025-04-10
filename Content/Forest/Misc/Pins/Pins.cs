using SpiritReforged.Common.ItemCommon.Pins;
using SpiritReforged.Common.ModCompat;

namespace SpiritReforged.Content.Forest.Misc.Pins;

public class PinRed : PinItem { }

public class PinGreen : PinItem { }

public class PinBlue : PinItem { }

public class PinYellow : PinItem { }

public class PinHive : PinItem { }

public class PinButterfly : PinItem { }

public class PinFaeling : PinItem { }

public class PinSavanna : PinItem { }

public class PinSky : PinItem { }

public class PinSword : PinItem { }

public class PinCuriosity : PinItem { }

public class PinBlood : PinItem
{
	public override bool IsLoadingEnabled(Mod mod) => ThoriumCompat.Enabled;
}

public class PinWulfrum : PinItem
{
	public override bool IsLoadingEnabled(Mod mod) => FablesCompat.Enabled;
}