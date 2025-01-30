using System.IO;

namespace Rythmify.Core.Databases;

public static partial class CollectionDBWriter {
	public static CollectionDB Write(CollectionDB collectionDB, string filePath) {
		FileStream erase = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
		erase.Close();
		FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);

		Writer.WriteInt(collectionDB.GameVersion, fs);
		Writer.WriteInt(collectionDB.CollectionCount, fs);

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		for (int i = 0; i < collectionDB.CollectionCount; i++) {
			Collection collection = collectionDB.Collections[i];

			Writer.WriteString(collection.Name, fs);
			Writer.WriteInt(collection.BeatmapCount, fs);

			for (int j = 0; j < collection.BeatmapCount; j++) {
				Writer.WriteString(collection.Beatmaps[j], fs);
			}
		}

		watch.Stop();
		Logger.LogDebug($"CollectionDB: Successfully wrote {collectionDB.CollectionCount} collections in {watch.ElapsedMilliseconds}ms");

		return collectionDB;
	}
}
