using System;

namespace Rythmify.Core.Beatmap;

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

	public override string ToString(){
		var values = new string[]{
			IsNormal ? "Normal" : null,
			IsWhistle ? "Whistle" : null,
			IsFinish ? "Finish" : null,
			IsClap ? "Clap" : null
		};
		var filteredValues = Array.FindAll(values, (string s) => !string.IsNullOrEmpty(s));
		return filteredValues.Length > 0 ? string.Join(", ", filteredValues) : "None";
	}
}

public enum HitObjectType {
	Circle,
	Hold
}

public class HitObjectTypeFlag {
	private readonly int Flags;

	public HitObjectTypeFlag(int flags) {
		Flags = flags;
	}

	public bool IsCircle => (Flags & (1 << 0)) != 0;
	public bool IsSlider => (Flags & (1 << 1)) != 0;
	public bool IsNewCombo => (Flags & (1 << 2)) != 0;
	public bool IsSpinner => (Flags & (1 << 3)) != 0;
	public int ComboOffset => (Flags & 0b01110000) >> 4;
	public bool IsHold => (Flags & (1 << 7)) != 0;

	public HitObjectType Type {
		get {
			if (IsCircle) return HitObjectType.Circle;
			if (IsHold) return HitObjectType.Hold;
			throw new ArgumentException($"HitObjectType {Flags} has an unsupported type");
		}
	}
}

public class HitSample {
	public int NormalSet;
	public int AdditionSet;
	public int Index;
	public int Volume;
	public string Filename;

	public override string ToString() => $"NormalSet: {NormalSet}, AdditionSet: {AdditionSet}, Index: {Index}, Volume: {Volume}, Filename: {Filename ?? "None"}";
}

public class BeatmapHitObject {

	public int X;
	public int Y;
	public int Time;
	public HitObjectTypeFlag Type;
	public HitSound HitSound;
	public HitSample HitSample;

	public int GetLane(int laneCount) => (int)Math.Floor(X * laneCount / 512f);
}

public class CircleHitObject : BeatmapHitObject {
	public override string ToString() => $"Type: {Type.Type}, X: {X}, Y: {Y}, Time: {Time}, HitSound: [{HitSound}], HitSample: {{ {HitSample} }}";
}

public class HoldHitObject : BeatmapHitObject {
	public int EndTime;

	public override string ToString() => $"Type: {Type.Type}, X: {X}, Y: {Y}, Time: {Time}, EndTime: {EndTime}, HitSound: [{HitSound}], HitSample: {{ {HitSample} }}";
}
