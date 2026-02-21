using System.IO;

namespace Rythmify.Core.Databases;

public static partial class ThumbnailsDBParser {
	public static ThumbnailsDB Parse(string filePath) {
		var bytes = File.ReadAllBytes(filePath);
		int currentByteIndex = 0;

		ThumbnailsDB thumbnailsDB = new();

		thumbnailsDB.Version = Parser.ParseInt(bytes, ref currentByteIndex);
		thumbnailsDB.ThumbnailsCount = Parser.ParseInt(bytes, ref currentByteIndex);

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		for (int i = 0; i < thumbnailsDB.ThumbnailsCount; i++) {
			string thumbnailID = Parser.ParseString(bytes, ref currentByteIndex);
			string beatmapMD5s = Parser.ParseString(bytes, ref currentByteIndex);

			foreach (string beatmapMD5 in beatmapMD5s.Split(','))
				thumbnailsDB.MD5ToThumbnailID[beatmapMD5] = thumbnailID;
		}

		watch.Stop();
		Logger.LogDebug($"ThumbnailsDB: Successfully parsed {thumbnailsDB.ThumbnailsCount} thumbnail entries in {watch.ElapsedMilliseconds}ms");

		return thumbnailsDB;
	}
}
