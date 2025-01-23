using System;
using System.IO;
using Rythmify.Core.Shared;

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
