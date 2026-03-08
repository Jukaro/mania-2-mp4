using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using Rythmify.Core.Beatmap.Concatenation;
using Rythmify.UI;

namespace Rythmify.Core.Beatmap;

public enum TrimType {
	Start,
	End,
	Both
}

public abstract class Delay {
	public abstract int[] GetDelays(List<BeatmapData> beatmaps);
}

public class MillisecondsDelay : Delay {
	private int _millisecondsDelay;

	public MillisecondsDelay(int millisecondsDelay) {
		_millisecondsDelay = millisecondsDelay;
	}

	public override int[] GetDelays(List<BeatmapData> beatmaps) {
		return Enumerable.Repeat(_millisecondsDelay, beatmaps.Count - 1).ToArray();
	}
}

public class MeasureDivisionDelay : Delay {
	private int _measureDivision;

	public MeasureDivisionDelay(int measureDivision) {
		_measureDivision = measureDivision;
	}

	public override int[] GetDelays(List<BeatmapData> beatmaps) {
		int[] delays = new int[beatmaps.Count - 1];

		for (int i = 1; i < beatmaps.Count; i++) {
			double lastBeatmapBPM = BeatmapParser.GetDominantBpm(beatmaps[i - 1]);
			double lastBeatmapBeatLength = 60000 / lastBeatmapBPM;
			delays[i - 1] = (int)(lastBeatmapBeatLength / _measureDivision);

			// delays must be divisible by 2 because the maximum precision of beatmap hitobjects timings is 1ms
			if (delays[i - 1] % 2 != 0)
				delays[i - 1] += 1;
		}

		return delays;
	}
}

public static partial class BeatmapConcatenation {
	public static void Concatenate(List<BeatmapWithScores> beatmapList, BeatmapConcatenationParameters parameters) {
/* --------------------------------- options -------------------------------- */


		BeatmapMetadata metadata = parameters.GetBeatmapMetadata();

		BeatmapDifficultyData difficulty = beatmapList[0].Beatmap.DifficultyData.DeepClone();

		string beatmapFolder = parameters.FolderName;
		string beatmapFilename = parameters.BeatmapFilename;

		Delay delay = parameters.Delay;


/* ------------------------------ concatenation ----------------------------- */


		string audioFilename = beatmapFilename + "-" + "audio.ogg";
		string backgroundFilename = beatmapFilename + "-" + "background.jpg";

		string folderPath = Path.Combine(Paths.OsuSongsDirectoryPath, beatmapFolder);
		string beatmapPath = Path.Combine(folderPath, beatmapFilename + ".osu");
		string audioPath = Path.Combine(folderPath, audioFilename);
		string backgroundPath = Path.Combine(folderPath, backgroundFilename);

		Directory.CreateDirectory(folderPath);

		FileStream beatmapFs = new(beatmapPath, FileMode.Create, FileAccess.Write);
		FileStream audioFs = new(audioPath, FileMode.Create, FileAccess.Write);
		FileStream backgroundFs = new(backgroundPath, FileMode.Create, FileAccess.Write);

		List<BeatmapData> beatmaps = beatmapList.Select(b => b.Beatmap).ToList();
		int[] delays = delay.GetDelays(beatmaps);

		Logger.LogInfo("\n[BeatmapConcatenation] Combining timing points and hit objects");
		BeatmapData beatmapData = ConcatenateBeatmapData(beatmaps, delays);
		Bitmap background = ConcatenateBeatmapBackground(beatmapList);

		beatmapData.Metadata = metadata;
		beatmapData.DifficultyData = difficulty;
		beatmapData.GeneralData.AudioFilename = audioFilename;
		beatmapData.Events = new BeatmapEvent[] {
			new BackgroundEvent() {
				Filename = backgroundFilename,
				StartTime = 0,
				XOffset = 0,
				YOffset = 0
			}
		};

		WriteBeatmap(beatmapFs, beatmapData);
		WriteAudio(audioFs, beatmapList, delays);
		WriteBackground(backgroundFs, background);

		beatmapFs.Close();
		audioFs.Close();
		backgroundFs.Close();
	}

	private static void WriteBeatmap(Stream beatmapFs, BeatmapData beatmapData) => BeatmapWriter.Write(beatmapData, beatmapFs);
	private static void WriteAudio(Stream audioFs, List<BeatmapWithScores> beatmaps, int[] delays) => ConcatenateBeatmapAudio(beatmaps, audioFs, delays);
	private static void WriteBackground(Stream backgroundFs, Bitmap background) => background.Save(backgroundFs);
}
