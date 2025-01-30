using System.IO;

namespace Rythmify.Core.Databases;

public static partial class CollectionDBParser {
	public static CollectionDB Parse(string filePath) {
		var bytes = File.ReadAllBytes(filePath);
		int currentByteIndex = 0;

		CollectionDB collectionDB = new();

		collectionDB.GameVersion = Parser.ParseInt(bytes, ref currentByteIndex);
		collectionDB.CollectionCount = Parser.ParseInt(bytes, ref currentByteIndex);

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		for (int i = 0; i < collectionDB.CollectionCount; i++) {
			Collection collection = new();

			collection.Name = Parser.ParseString(bytes, ref currentByteIndex);
			collection.BeatmapCount = Parser.ParseInt(bytes, ref currentByteIndex);

			for (int j = 0; j < collection.BeatmapCount; j++) {
				collection.Beatmaps.Add(Parser.ParseString(bytes, ref currentByteIndex));
			}

			collectionDB.Collections.Add(collection);
		}

		watch.Stop();
		Logger.LogDebug($"CollectionDB: Successfully parsed {collectionDB.CollectionCount} collections in {watch.ElapsedMilliseconds}ms");

		return collectionDB;
	}
}
