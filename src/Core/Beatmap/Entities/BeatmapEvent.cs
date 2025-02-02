namespace Rythmify.Core.Beatmap;

public enum BeatmapEventType {
	Background = 0,
	Video = 1,
	Break = 2,
}

public abstract class BeatmapEvent {
	public int StartTime;
	public BeatmapEventType Type;

	public static BeatmapEventType? TryGetEventType(string type) {
		return type switch {
			"0" => BeatmapEventType.Background,
			"1" => BeatmapEventType.Video,
			"2" => BeatmapEventType.Break,
			"Video" => BeatmapEventType.Video,
			"Break" => BeatmapEventType.Background,
			_ => null,
		};
	}

	public abstract BeatmapEvent DeepClone();

	public override abstract string ToString();
}

public class BackgroundEvent : BeatmapEvent {
	public string Filename;
	public int XOffset;
	public int YOffset;

	public BackgroundEvent() {
		Type = BeatmapEventType.Background;
	}

	public override BackgroundEvent DeepClone() => (BackgroundEvent)MemberwiseClone();

	public override string ToString() => $"Type: {Type}, StartTime: {StartTime}, Filename: {Filename}, XOffset: {XOffset}, YOffset: {YOffset}";
}

public class VideoEvent : BeatmapEvent {
	public string Filename;
	public int XOffset;
	public int YOffset;

	public VideoEvent() {
		Type = BeatmapEventType.Video;
	}

	public override VideoEvent DeepClone() => (VideoEvent)MemberwiseClone();

	public override string ToString() => $"Type: {Type}, StartTime: {StartTime}, Filename: {Filename}, XOffset: {XOffset}, YOffset: {YOffset}";
}

public class BreakEvent : BeatmapEvent {
	public int EndTime;

	public BreakEvent() {
		Type = BeatmapEventType.Break;
	}

	public override BreakEvent DeepClone() => (BreakEvent)MemberwiseClone();

	public override string ToString() => $"Type: {Type}, StartTime: {StartTime}, EndTime: {EndTime}";
}
