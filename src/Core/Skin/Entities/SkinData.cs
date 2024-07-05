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
}

public class SkinFontsSection {
}

public class SkinManiaSection {
	public int Keys;
}


public class SkinData {
	public SkinGeneralSection General = new();
	public SkinColorsSection Colors = new();
	public SkinFontsSection Fonts = new();
	public Dictionary<int, SkinManiaSection> Mania = new();

	public int HitPosition = 0;
}
