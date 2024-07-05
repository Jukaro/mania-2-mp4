using System.Collections.Generic;

namespace Rythmify.Core;

public class SkinGeneralSection {
	public string Name;
	public string Author;
	public string Version;
	public int AnimationFramerate = -1;
	public bool AllowSliderBallTint = false;
	public bool ComboBurstRandom = false;
	public bool CursorCentre = true;
	public bool CursorExpand = true;
	public bool CursorRotate = true;
	public bool CursorTrailRotate = true;
	public string CustomComboBurstSounds;
	public bool HitCircleOverlayAboveNumber = true;
	public bool LayeredHitSounds = true;
	public bool SliderBallFlip = true;
	public bool SpinnerFadePlayfield = false;
	public bool SpinnerFrequencyModulate = true;
	public bool SpinnerNoBlink = false;

	public override string ToString() => $"Name: {Name}, Author: {Author}, Version: {Version}, AnimationFramerate: {AnimationFramerate}, AllowSliderBallTint: {AllowSliderBallTint}, ComboBurstRandom: {ComboBurstRandom}, CursorCentre: {CursorCentre}, CursorExpand: {CursorExpand}, CursorRotate: {CursorRotate}, CursorTrailRotate: {CursorTrailRotate}, CustomComboBurstSounds: {CustomComboBurstSounds}, HitCircleOverlayAboveNumber: {HitCircleOverlayAboveNumber}, LayeredHitSounds: {LayeredHitSounds}, SliderBallFlip: {SliderBallFlip}, SpinnerFadePlayfield: {SpinnerFadePlayfield}, SpinnerFrequencyModulate: {SpinnerFrequencyModulate}, SpinnerNoBlink: {SpinnerNoBlink}";
}

public class SkinColorsSection {
	public RGB Combo1 = new(255, 192, 0);
	public RGB Combo2 = new(0, 202, 0);
	public RGB Combo3 = new(18, 124, 255);
	public RGB Combo4 = new(242, 24, 57);
	public RGB Combo5;
	public RGB Combo6;
	public RGB Combo7;
	public RGB Combo8;
	public RGB InputOverlayText = new(0, 0, 0);
	public RGB MenuGlow = new(0, 78, 155);
	public RGB SliderBall = new(2, 170, 255);
	public RGB SliderBorder = new(255, 255, 255);
	public RGB SliderTrackOverride;
	public RGB SongSelectActiveText = new(0, 0, 0);
	public RGB SongSelectInactiveText = new(255, 255, 255);
	public RGB SpinnerBackground = new(100, 100, 100);
	public RGB StarBreakAdditive = new(255, 182, 193);

	public override string ToString() => $"Combo1: {Combo1}, Combo2: {Combo2}, Combo3: {Combo3}, Combo4: {Combo4}, Combo5: {Combo5}, Combo6: {Combo6}, Combo7: {Combo7}, Combo8: {Combo8}, InputOverlayText: {InputOverlayText}, MenuGlow: {MenuGlow}, SliderBall: {SliderBall}, SliderBorder: {SliderBorder}, SliderTrackOverride: {SliderTrackOverride}, SongSelectActiveText: {SongSelectActiveText}, SongSelectInactiveText: {SongSelectInactiveText}, SpinnerBackground: {SpinnerBackground}, StarBreakAdditive: {StarBreakAdditive}";
}

public class SkinFontsSection {
}

public class SkinCatchTheBeatSection {
}

public class SkinManiaSection {
	public int Keys;
}


public class SkinData {
	public SkinGeneralSection General = new();
	public SkinColorsSection Colors = new();
	public SkinFontsSection Fonts = new();
	public SkinCatchTheBeatSection CatchTheBeat = new();
	public Dictionary<int, SkinManiaSection> Mania = new();

	public int HitPosition = 0;
}
