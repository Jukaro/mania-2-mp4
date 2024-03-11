using Rythmify.Core.Shared;

namespace Rythmify.Core.Beatmap;

public class BeatmapGeneralData {
	public string AudioFilename;
	public int AudioLeadIn;
	public int PreviewTime;
	public Countdown Countdown;
	public string SampleSet;
	public double StackLeniency;
	public GameMode GameMode;
	public bool LetterboxInBreaks;
	public bool UseSkinSprites;
	public string OverlayPosition;
	public string SkinPreference;
	public bool EpilepsyWarning;
	public int CountdownOffset;
	public bool SpecialStyle;
	public bool WidescreenStoryboard;
	public bool SamplesMatchPlaybackRate;

	public BeatmapGeneralData() {
		// Assign to default values
		AudioLeadIn = 0;
		PreviewTime = -1;
		Countdown = Countdown.Normal;
		SampleSet = "Normal";
		StackLeniency = 0.7;
		GameMode = GameMode.Standard;
		LetterboxInBreaks = false;
		UseSkinSprites = false;
		OverlayPosition = "NoChange";
		EpilepsyWarning = false;
		CountdownOffset = 0;
		SpecialStyle = false;
		WidescreenStoryboard = false;
		SamplesMatchPlaybackRate = false;
	}

	public override string ToString() => $"AudioFilename: {AudioFilename}\nAudioLeadIn: {AudioLeadIn}\nPreviewTime: {PreviewTime}\nCountdown: {Countdown}\nSampleSet: {SampleSet}\nStackLeniency: {StackLeniency}\nGameMode: {GameMode}\nLetterboxInBreaks: {LetterboxInBreaks}\nUseSkinSprites: {UseSkinSprites}\nOverlayPosition: {OverlayPosition}\nSkinPreference: {SkinPreference}\nEpilepsyWarning: {EpilepsyWarning}\nCountdownOffset: {CountdownOffset}\nSpecialStyle: {SpecialStyle}\nWidescreenStoryboard: {WidescreenStoryboard}\nSamplesMatchPlaybackRate: {SamplesMatchPlaybackRate}";
}
