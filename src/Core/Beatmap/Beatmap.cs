namespace Rythmify.Core.Beatmap;

public enum GameMode {
	Standard = 0,
	Taiko = 1,
	CatchTheBeat = 2,
	Mania = 3
}

public enum Countdown {
	NoCountdown = 0,
	Normal = 1,
	Half = 2,
	Double = 3
}

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
}

public class BeatmapEditorData {
	public int[] Bookmarks;
	public double DistanceSpacing;
	public int BeatDivisor;
	public int GridSize;
	public double TimelineZoom;
}

public class BeatmapMetadata {
	public string Title;
	public string Artist;
	public string Creator;
	public string Version;
	public string Source;
	public string Tags;
}

public class BeatmapDifficulty {
	public double HPDrainRate;
	public double CircleSize;
	public double OverallDifficulty;
	public double ApproachRate;
	public double SliderMultiplier;
	public double SliderTickRate;
}

public class Beatmap {
	public BeatmapGeneralData GeneralData;
	public BeatmapEditorData EditorData;
	public BeatmapMetadata Metadata;
	public BeatmapDifficulty Difficulty;

	public Beatmap() {
		GeneralData = new BeatmapGeneralData();
		EditorData = new BeatmapEditorData();
		Metadata = new BeatmapMetadata();
		Difficulty = new BeatmapDifficulty();
	}
}
