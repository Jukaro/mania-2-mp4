namespace Rythmify.Core.Shared;

public enum Mods {
	None = 0,
	NoFail = 1,
	Easy = 2,
	TouchDevice = 4,
	Hidden = 8,
	HardRock = 16,
	SuddenDeath = 32,
	DoubleTime = 64,
	Relax = 128,
	HalfTime = 256,
	Nightcore = 512, // Only set along with DoubleTime. i.e: NC only gives 576
	Flashlight = 1024,
	Autoplay = 2048,
	SpunOut = 4096,
	Relax2 = 8192,    // Autopilot
	Perfect = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416
	KeyMod4 = 32768,
	KeyMod5 = 65536,
	KeyMod6 = 131072,
	KeyMod7 = 262144,
	KeyMod8 = 524288,
	FadeIn = 1048576,
	Random = 2097152,
	Cinema = 4194304,
	Target = 8388608,
	KeyMod9 = 16777216,
	KeyModCoop = 33554432,
	KeyMod1 = 67108864,
	KeyMod3 = 134217728,
	KeyMod2 = 268435456,
	ScoreV2 = 536870912,
	Mirror = 1073741824,
	KeyMod = KeyMod1 | KeyMod2 | KeyMod3 | KeyMod4 | KeyMod5 | KeyMod6 | KeyMod7 | KeyMod8 | KeyMod9 | KeyModCoop,
	FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
	ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
}
