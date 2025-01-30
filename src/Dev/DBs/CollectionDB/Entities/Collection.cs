using System.Collections.Generic;
using System.Linq;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public class Collection {
	public string Name;
	public int BeatmapCount;

	public List<string> Beatmaps = new();

	public Collection() {}

	public Collection(BeatmapFilter filter, BeatmapDB beatmapDB) {

		// TODO : Classe pour filtrer beatmaps et scores
		// List<string> list = beatmapDB.Beatmaps.Values.Where(beatmap =>
		// 	beatmap.Mode == GameMode.Mania
		// 	&& (int)beatmap.CircleSize == 4
		// 	&& beatmap.ManiaStarRating[(int)Mods.None] >= 1
		// 	&& beatmap.ManiaStarRating[(int)Mods.None] < 2
		// 	&& beatmap.RankedStatus == 4
		// 	&& (!_scoresDB.Beatmaps.ContainsKey(beatmap.BeatmapMD5)
		// 		|| _scoresDB.Beatmaps[beatmap.BeatmapMD5].Replays.Where(replay => replay.Score >= 999000).Count() == 0)
		// ).ToList().Select(beatmap => beatmap.BeatmapMD5).ToList();

		// Collection testCollection = new();
		// testCollection.Name = "4*+ 4K 995k";
		// testCollection.BeatmapCount = list.Count;
		// testCollection.Beatmaps = list;

		// collectionDB.Collections.Add(testCollection);
		// collectionDB.CollectionCount++;

		// CollectionDBWriter.Write(collectionDB, "G:/Jeux/osssu/collection.db");
	}
}
