using System.IO;
using System.Text;

namespace Rythmify.Core.Beatmap;

public static partial class BeatmapWriter {
	public static void Write(BeatmapData beatmap, Stream outputStream) {
		string str = "osu file format v14\n\n";

		str += GetGeneralSectionString(beatmap.GeneralData) + "\n";
		str += GetEditorSectionString(beatmap.EditorData) + "\n";
		str += GetMetadataSectionString(beatmap.Metadata) + "\n";
		str += GetDifficultySectionString(beatmap.DifficultyData) + "\n";
		str += GetEventsSectionString(beatmap.Events) + "\n";
		str += GetTimingPointsSectionString(beatmap.TimingPoints) + "\n";
		str += GetColoursSectionString(beatmap.Colors) + "\n";
		str += GetHitObjectsSectionString(beatmap.HitObjects) + "\n";

		byte[] beatmapDataAsBytes = new UTF8Encoding(true).GetBytes(str);
		outputStream.Write(beatmapDataAsBytes, 0, beatmapDataAsBytes.Length);
	}

	private static string GetGeneralSectionString(BeatmapGeneralData generalData) {
		string str = "[General]\n";

		str += GetFieldString("AudioFilename", generalData.AudioFilename);
		str += GetFieldString("AudioLeadIn", generalData.AudioLeadIn);
		str += GetFieldString("PreviewTime", generalData.PreviewTime);
		str += GetFieldString("Countdown", generalData.Countdown);
		str += GetFieldString("SampleSet", generalData.SampleSet);
		str += GetFieldString("StackLeniency", generalData.StackLeniency);
		str += GetFieldString("Mode", generalData.GameMode);
		str += GetFieldString("LetterboxInBreaks", generalData.LetterboxInBreaks);
		str += GetFieldString("UseSkinSprites", generalData.UseSkinSprites);
		str += GetFieldString("OverlayPosition", generalData.OverlayPosition);
		str += GetFieldString("SkinPreference", generalData.SkinPreference);
		str += GetFieldString("EpilepsyWarning", generalData.EpilepsyWarning);
		str += GetFieldString("CountdownOffset", generalData.CountdownOffset);
		str += GetFieldString("SpecialStyle", generalData.SpecialStyle);
		str += GetFieldString("WidescreenStoryboard", generalData.WidescreenStoryboard);
		str += GetFieldString("SamplesMatchPlaybackRate", generalData.SamplesMatchPlaybackRate);

		return str;
	}

	private static string GetEditorSectionString(BeatmapEditorData editorData) {
		string str = "[Editor]\n";

		str += GetArrayString("Bookmarks", editorData.Bookmarks, ",");
		str += GetFieldString("DistanceSpacing", editorData.DistanceSpacing);
		str += GetFieldString("BeatDivisor", editorData.BeatDivisor);
		str += GetFieldString("GridSize", editorData.GridSize);
		str += GetFieldString("TimelineZoom", editorData.TimelineZoom);

		return str;
	}

	private static string GetMetadataSectionString(BeatmapMetadata metadata) {
		string str = "[Metadata]\n";

		str += GetFieldString("Title", metadata.Title);
		str += GetFieldString("TitleUnicode", metadata.TitleUnicode);
		str += GetFieldString("Artist", metadata.Artist);
		str += GetFieldString("ArtistUnicode", metadata.ArtistUnicode);
		str += GetFieldString("Creator", metadata.Creator);
		str += GetFieldString("Version", metadata.Version);
		str += GetFieldString("Source", metadata.Source);
		str += GetArrayString("Tags", metadata.Tags, " ");
		str += GetFieldString("BeatmapID", metadata.BeatmapID);
		str += GetFieldString("BeatmapSetID", metadata.BeatmapSetID);

		return str;
	}

	private static string GetDifficultySectionString(BeatmapDifficultyData difficultyData) {
		string str = "[Difficulty]\n";

		str += GetFieldString("HPDrainRate", difficultyData.HPDrainRate);
		str += GetFieldString("CircleSize", difficultyData.CircleSize);
		str += GetFieldString("OverallDifficulty", difficultyData.OverallDifficulty);
		str += GetFieldString("ApproachRate", difficultyData.ApproachRate);
		str += GetFieldString("SliderMultiplier", difficultyData.SliderMultiplier);
		str += GetFieldString("SliderTickRate", difficultyData.SliderTickRate);

		return str;
	}

	private static string GetEventsSectionString(BeatmapEvent[] events) {
		if (events == null || events.Length == 0)
			return "";

		string str = "[Events]\n";

		foreach (BeatmapEvent ev in events) {
			object[] values = null;

			if (ev is BackgroundEvent bgEvent)
				values = new object[] { bgEvent.Type, bgEvent.StartTime, bgEvent.Filename, bgEvent.XOffset, bgEvent.YOffset };
			else if (ev is VideoEvent videoEvent)
				values = new object[] { videoEvent.Type, videoEvent.StartTime, videoEvent.Filename, videoEvent.XOffset, videoEvent.YOffset };
			else if (ev is BreakEvent breakEvent)
				values = new object[] { breakEvent.Type, breakEvent.StartTime, breakEvent.EndTime };
			else
				Logger.LogWarning($"[BeatmapWriter] Unknown event type: {ev.Type}");
			str += GetObjectString(values, ",") + "\n";
		}

		return str;
	}

	private static string GetTimingPointsSectionString(BeatmapTimingPoint[] timingPoints) {
		if (timingPoints == null || timingPoints.Length == 0)
			return "";

		string str = "[TimingPoints]\n";

		foreach (BeatmapTimingPoint tp in timingPoints) {
			object[] values;

			values = new object[] { tp.Time, tp.BeatLength, tp.Meter, tp.SampleSet, tp.SampleIndex, tp.Volume, tp.Uninherited, tp.Effects.Flags };
			str += GetObjectString(values, ",") + "\n";
		}

		return str;
	}

	private static string GetColoursSectionString(BeatmapColor[] colors) {
		if (colors == null || colors.Length == 0)
			return "";

		string str = "[Colours]\n";

		foreach (BeatmapColor color in colors) {
			object[] values;

			values = new object[] { color.R, color.G, color.B };
			str += color.Label + ": " + GetObjectString(values, ",") + "\n";
		}
		return str;
	}

	private static string GetHitObjectsSectionString(BeatmapHitObject[] hitObjects) {
		if (hitObjects == null || hitObjects.Length == 0)
			return "";

		string str = "[HitObjects]\n";

		foreach (BeatmapHitObject ho in hitObjects) {
			object[] values;

			values = new object[] { ho.X, ho.Y, ho.Time, ho.Type.Flags, ho.HitSound.Flags };
			str += GetObjectString(values, ",") + ",";
			if (ho is HoldHitObject holdHitObject)
				values = new object[] { holdHitObject.EndTime, ho.HitSample.NormalSet, ho.HitSample.AdditionSet, ho.HitSample.Index, ho.HitSample.Volume, ho.HitSample.Filename };
			else
				values = new object[] { ho.HitSample.NormalSet, ho.HitSample.AdditionSet, ho.HitSample.Index, ho.HitSample.Volume, ho.HitSample.Filename };
			str += GetObjectString(values, ":") + "\n";
		}

		return str;
	}
}
