using System.Collections.Generic;
using System.IO;

namespace Rythmify.Core.Databases;

public static partial class ThumbnailsDBWriter {
	public static void Write(ThumbnailsDB thumbnailsDB, string filePath) {
		FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

		Writer.WriteInt(thumbnailsDB.Version, fs);

		Dictionary<string, string> dbFormatThumbnailsDict = new();
		foreach (var (beatmapMD5, thumbnailID) in thumbnailsDB.MD5ToThumbnailID) {
			if (!dbFormatThumbnailsDict.ContainsKey(thumbnailID))
				dbFormatThumbnailsDict.Add(thumbnailID, beatmapMD5);
			else
				dbFormatThumbnailsDict[thumbnailID] += $",{beatmapMD5}";
		}

		Writer.WriteInt(dbFormatThumbnailsDict.Count, fs);

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();

		foreach (var (thumbnailID, beatmapMD5s) in dbFormatThumbnailsDict) {
			Writer.WriteString(thumbnailID, fs);
			Writer.WriteString(beatmapMD5s, fs);
		}

		fs.Close();

		watch.Stop();
		Logger.LogInfo($"[ThumbnailsDBWriter] Successfully wrote {dbFormatThumbnailsDict.Count} thumbnail entries in {watch.ElapsedMilliseconds}ms");
	}
}
