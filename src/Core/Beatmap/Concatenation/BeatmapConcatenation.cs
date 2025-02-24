using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
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
		int[] delays = new int[beatmaps.Count];

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
	public static void Concatenate(List<BeatmapWithScores> beatmapList, GraphicsDevice graphics) {
/* --------------------------------- options -------------------------------- */


		BeatmapMetadata metadata = beatmapList[0].Beatmap.Metadata.DeepClone();
		metadata.Title = "defaultTitle";
		metadata.Creator = "defaultCreator";
		metadata.Version = "defaultDifficulty";
		metadata.BeatmapID = 0;
		metadata.BeatmapSetID = -1;

		string beatmapFolder = "defaultFolder";
		string beatmapFilename = "defaultFilename";

		Delay delay = new MeasureDivisionDelay(4);
		// Delay delay = new MillisecondsDelay(100);


/* ------------------------------ concatenation ----------------------------- */


		string folderPath = Path.Combine(Paths.OsuSongsDirectoryPath, beatmapFolder);
		string beatmapPath = Path.Combine(folderPath, beatmapFilename + ".osu");
		string audioPath = Path.Combine(folderPath, "audio.ogg");
		string backgroundPath = Path.Combine(folderPath, "background.jpg");

		Directory.CreateDirectory(folderPath);

		FileStream beatmapFs = new(beatmapPath, FileMode.Create, FileAccess.Write);
		FileStream audioFs = new(audioPath, FileMode.Create, FileAccess.Write);
		FileStream backgroundFs = new(backgroundPath, FileMode.Create, FileAccess.Write);

		List<BeatmapData> beatmaps = beatmapList.Select(b => b.Beatmap).ToList();
		int[] delays = delay.GetDelays(beatmaps);

		BeatmapData beatmapData = ConcatenateBeatmapData(beatmaps, delays);
		Texture2D background = ConcatenateBeatmapBackground(graphics, beatmapList);

		beatmapData.Metadata = metadata;
		beatmapData.GeneralData.AudioFilename = "audio.ogg";
		beatmapData.Events = new BeatmapEvent[] {
			new BackgroundEvent() {
				Filename = "background.jpg",
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
	private static void WriteBackground(Stream backgroundFs, Texture2D background) => background.SaveAsJpeg(backgroundFs, background.Width, background.Height);
}
