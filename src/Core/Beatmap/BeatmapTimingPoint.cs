namespace Rythmify.Core.Beatmap;

public class TimingPointEffects {
	private readonly int Flags;

	public TimingPointEffects(int flags) {
		Flags = flags;
	}

	public bool IsKiai => (Flags & (1 << 0)) != 0;
	public bool IsOmitFirstBarLine => (Flags & (1 << 3)) != 0;

	public override string ToString() => $"Kiai: {IsKiai}, OmitFirstBarLine: {IsOmitFirstBarLine}";
}

public class BeatmapTimingPoint {
	public double Time;
	public double BeatLength;
	public int Meter;
	public int SampleSet;
	public int SampleIndex;
	public int Volume;
	public bool Uninherited;
	public TimingPointEffects Effects;

	public override string ToString() => $"Time: {Time}, BeatLength: {BeatLength}, Meter: {Meter}, SampleSet: {SampleSet}, SampleIndex: {SampleIndex}, Volume: {Volume}, Uninherited: {Uninherited}, Effects: {{ {Effects} }}";
}
