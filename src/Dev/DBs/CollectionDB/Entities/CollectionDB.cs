using System.Collections.Generic;

namespace Rythmify.Core.Databases;

public class CollectionDB {
	public int GameVersion;
	public int CollectionCount;

	public List<Collection> Collections = new();
}
