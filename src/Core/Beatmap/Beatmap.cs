using System;

namespace Rythmify.Core.Beatmap;

public enum EventType {
	Background = 0,
	Video = 1,
	Break = 2,
}

public abstract class BeatmapEvent {
	public EventType Type; // String or number actually
	public int StartTime;

	public static EventType GetEventType(string type) {
		return type switch {
			"0" => EventType.Background,
			"1" => EventType.Video,
			"2" => EventType.Break,
			"Video" => EventType.Video,
			"Break" => EventType.Background,
			_ => throw new ArgumentException($"Unknown event type {type}"),
		};
	}
}

public class BackgroundEvent : BeatmapEvent {
	public string Filename;
	public int XOffset;
	public int YOffset;

	public BackgroundEvent() {
		Type = EventType.Background;
		StartTime = 0;
	}

	public override string ToString() => $"Type: {Type}\nStartTime: {StartTime}\nFilename: {Filename}\nXOffset: {XOffset}\nYOffset: {YOffset}";
}

public class VideoEvent : BeatmapEvent {
	public string Filename;
	public int XOffset;
	public int YOffset;

	public VideoEvent() {
		Type = EventType.Video;
	}

	public override string ToString() => $"Type: {Type}\nStartTime: {StartTime}\nFilename: {Filename}\nXOffset: {XOffset}\nYOffset: {YOffset}";
}

public class BreakEvent : BeatmapEvent {
	public int EndTime;

	public BreakEvent() {
		Type = EventType.Break;
	}

	public override string ToString() => $"Type: {Type}\nStartTime: {StartTime}\nEndTime: {EndTime}";
}

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

	public override string ToString() => $"AudioFilename: {AudioFilename}\nAudioLeadIn: {AudioLeadIn}\nPreviewTime: {PreviewTime}\nCountdown: {Countdown}\nSampleSet: {SampleSet}\nStackLeniency: {StackLeniency}\nGameMode: {GameMode}\nLetterboxInBreaks: {LetterboxInBreaks}\nUseSkinSprites: {UseSkinSprites}\nOverlayPosition: {OverlayPosition}\nSkinPreference: {SkinPreference}\nEpilepsyWarning: {EpilepsyWarning}\nCountdownOffset: {CountdownOffset}\nSpecialStyle: {SpecialStyle}\nWidescreenStoryboard: {WidescreenStoryboard}\nSamplesMatchPlaybackRate: {SamplesMatchPlaybackRate}";
}

public class BeatmapEditorData {
	public int[] Bookmarks;
	public double DistanceSpacing;
	public int BeatDivisor;
	public int GridSize;
	public double TimelineZoom;

	public override string ToString() => $"Bookmarks: [{(Bookmarks == null ? "null" : string.Join(", ", Bookmarks))}]\nDistanceSpacing: {DistanceSpacing}\nBeatDivisor: {BeatDivisor}\nGridSize: {GridSize}\nTimelineZoom: {TimelineZoom}";
}

public class BeatmapMetadata {
	public string Title;
	public string TitleUnicode;
	public string Artist;
	public string ArtistUnicode;
	public string Creator;
	public string Version;
	public string Source;
	public string[] Tags;
	public int BeatmapID;
	public int BeatmapSetID;

	public override string ToString() => $"Title: {Title}\nTitleUnicode: {TitleUnicode}\nArtist: {Artist}\nArtistUnicode: {ArtistUnicode}\nCreator: {Creator}\nVersion: {Version}\nSource: {Source}\nTags: [{string.Join(", ", Tags)}]\nBeatmapID: {BeatmapID}\nBeatmapSetID: {BeatmapSetID}";
}

public class BeatmapDifficultyData {
	public double HPDrainRate;
	public double CircleSize;
	public double OverallDifficulty;
	public double ApproachRate;
	public double SliderMultiplier;
	public double SliderTickRate;

	public override string ToString() => $"HPDrainRate: {HPDrainRate}\nCircleSize: {CircleSize}\nOverallDifficulty: {OverallDifficulty}\nApproachRate: {ApproachRate}\nSliderMultiplier: {SliderMultiplier}\nSliderTickRate: {SliderTickRate}";
}

public class Beatmap {
	public BeatmapGeneralData GeneralData;
	public BeatmapEditorData EditorData;
	public BeatmapMetadata Metadata;
	public BeatmapDifficultyData DifficultyData;
	public BeatmapEvent[] Events;

	public Beatmap() {
		GeneralData = new BeatmapGeneralData();
		EditorData = new BeatmapEditorData();
		Metadata = new BeatmapMetadata();
		DifficultyData = new BeatmapDifficultyData();
		Events = Array.Empty<BeatmapEvent>();
	}

	public override string ToString() => $"GeneralData:\n{GeneralData}\nEditorData:\n{EditorData}\nMetadata:\n{Metadata}\nDifficultyData:\n{DifficultyData}";
}
