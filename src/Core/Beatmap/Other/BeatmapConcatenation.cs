using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.Core.Beatmap;

public enum TrimType {
	Start,
	End,
	Full
}

public enum DelayType {
	Milliseconds,
	MeasureDivision
}

public static partial class BeatmapConcatenation {
	public static void Concatenate(List<BeatmapWithScores> beatmapList, GraphicsDevice graphics) {
		var watch = new Stopwatch();
		watch.Start();

		BeatmapMetadata metadata = beatmapList[0].Beatmap.Metadata.DeepClone();
		metadata.Creator = "creator";
		metadata.Version = "version";
		metadata.BeatmapID = 0;
		metadata.BeatmapSetID = -1;

		string beatmapFolder = "testsv1";
		string beatmapFilename = "testv1";

		List<BeatmapData> beatmaps = beatmapList.Select(b => b.Beatmap).ToList();

		Texture2D background = ConcatenateBeatmapBackground(graphics, beatmapList);

		int millisecondsDelay = 100;
		int measureDivision = 4;

		int[] delays = GetDelays(beatmaps, measureDivision, DelayType.MeasureDivision);

		BeatmapData result = ConcatenateBeatmapData(beatmaps, delays);
		result.Metadata = metadata;
		result.GeneralData.AudioFilename = "audio.ogg";
		// ajouter un event au debut et virer les autres ?
		foreach (BeatmapEvent ev in result.Events) {
			if (ev is BackgroundEvent bgEvent)
				bgEvent.Filename = "bg.jpg";
		}

		MemoryStream audioOutput = new();
		ConcatenateBeatmapAudio(beatmapList, audioOutput, delays);

		BeatmapWriter.WriteBeatmap(result, audioOutput, background, beatmapFolder, beatmapFilename);

		watch.Stop();
		Logger.LogDebug($"Concatenated beatmap in {watch.ElapsedMilliseconds}ms");
	}
}
