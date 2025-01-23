using System;
using System.Collections.Generic;
using Rythmify.Core.Shared;

namespace Rythmify.Core.Databases;

public class CollectionDB {
	public int GameVersion;
	public int CollectionCount;

	public List<Collection> Collections = new();
}
