using System;

namespace Rythmify.Core.Beatmap;

public enum EventType {
	Background = 0,
	Video = 1,
	Break = 2,
}

public abstract class BeatmapEvent {
	public int StartTime;
	public EventType Type;

	public static EventType? TryGetEventType(string type) {
		return type switch {
			"0" => EventType.Background,
			"1" => EventType.Video,
			"2" => EventType.Break,
			"Video" => EventType.Video,
			"Break" => EventType.Background,
			_ => null,
		};
	}

	public override abstract string ToString();
}

public class BackgroundEvent : BeatmapEvent {
	public string Filename;
	public int XOffset;
	public int YOffset;

	public BackgroundEvent() {
		Type = EventType.Background;
	}

	public override string ToString() => $"Type: {Type}, StartTime: {StartTime}, Filename: {Filename}, XOffset: {XOffset}, YOffset: {YOffset}";
}

public class VideoEvent : BeatmapEvent {
	public string Filename;
	public int XOffset;
	public int YOffset;

	public VideoEvent() {
		Type = EventType.Video;
	}

	public override string ToString() => $"Type: {Type}, StartTime: {StartTime}, Filename: {Filename}, XOffset: {XOffset}, YOffset: {YOffset}";
}

public class BreakEvent : BeatmapEvent {
	public int EndTime;

	public BreakEvent() {
		Type = EventType.Break;
	}

	public override string ToString() => $"Type: {Type}, StartTime: {StartTime}, EndTime: {EndTime}";
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

public class Effects {
	private readonly int Flags;

	public Effects(int flags) {
		Flags = flags;
	}

	public bool IsKiai => (Flags & (1 << 0)) != 0;
	public bool IsOmitFirstBarLine => (Flags & (1 << 3)) != 0;

	public override string ToString() => $"Kiai: {IsKiai}, OmitFirstBarLine: {IsOmitFirstBarLine}";
}

public class TimingPoint {
	public int Time;
	public double BeatLength;
	public int Meter;
	public int SampleSet;
	public int SampleIndex;
	public int Volume;
	public bool Uninherited;
	public Effects Effects;

	public override string ToString() => $"Time: {Time}, BeatLength: {BeatLength}, Meter: {Meter}, SampleSet: {SampleSet}, SampleIndex: {SampleIndex}, Volume: {Volume}, Uninherited: {Uninherited}, Effects: {{ {Effects} }}";
}

public class BeatmapColor {
	public string Label;
	public byte R;
	public byte G;
	public byte B;

	public override string ToString() => $"Label: {Label} R: {R}, G: {G}, B: {B}";
}

public class HitObjectType {
	private readonly int Flags;

	public HitObjectType(int flags) {
		Flags = flags;
	}

	public bool IsCircle => (Flags & (1 << 0)) != 0;
	public bool IsSlider => (Flags & (1 << 1)) != 0;
	public bool IsNewCombo => (Flags & (1 << 2)) != 0;
	public bool IsSpinner => (Flags & (1 << 3)) != 0;
	public int ComboOffset => (Flags & 0b01110000) >> 4;
	public bool IsHold => (Flags & (1 << 7)) != 0;
}

public enum HitSoundType {
	Normal = 0b1,
	Whistle = 0b10,
	Finish = 0b100,
	Clap = 0b1000,
}

public class HitSound {
	private readonly int Flags;

	public HitSound(int flags) {
		Flags = flags;
	}

	public bool IsNormal => (Flags & (1 << 0)) != 0;
	public bool IsWhistle => (Flags & (1 << 1)) != 0;
	public bool IsFinish => (Flags & (1 << 2)) != 0;
	public bool IsClap => (Flags & (1 << 3)) != 0;

	public override string ToString() => $"Normal: {IsNormal}, Whistle: {IsWhistle}, Finish: {IsFinish}, Clap: {IsClap}";
}

public class HitObject {
	public int X;
	public int Y;
	public int Time;
	public HitObjectType Type;
	public HitSoundType Sound;
	public int[] HitSample;
}

public class Beatmap {
	public BeatmapGeneralData GeneralData;
	public BeatmapEditorData EditorData;
	public BeatmapMetadata Metadata;
	public BeatmapDifficultyData DifficultyData;
	public BeatmapEvent[] Events;
	public TimingPoint[] TimingPoints;
	public BeatmapColor[] Colors;
	public HitObject[] HitObjects;

	public Beatmap() {
		GeneralData = new BeatmapGeneralData();
		EditorData = new BeatmapEditorData();
		Metadata = new BeatmapMetadata();
		DifficultyData = new BeatmapDifficultyData();
		Events = Array.Empty<BeatmapEvent>();
		TimingPoints = Array.Empty<TimingPoint>();
		Colors = Array.Empty<BeatmapColor>();
		HitObjects = Array.Empty<HitObject>();
	}

	public override string ToString() => $"GeneralData:\n{GeneralData}\nEditorData:\n{EditorData}\nMetadata:\n{Metadata}\nDifficultyData:\n{DifficultyData}\nEvents:\n{string.Join("\n", Array.ConvertAll(Events, (e) => e.ToString()))}\nTimingPoints:\n{string.Join("\n", Array.ConvertAll(TimingPoints, (tp) => tp.ToString()))}\nColors:\n{string.Join("\n", Array.ConvertAll(Colors, (c) => c.ToString()))}";
}
