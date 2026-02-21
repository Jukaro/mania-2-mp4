using System;
using System.Collections.Generic;
using System.Linq;
using Rythmify.Core.Replay;

public class Session {
	public DateTime DateTime;
	public List<ReplayData> Replays = new();

	public Session(DateTime dateTime) {
		DateTime = dateTime;
	}

	public override string ToString() {
		string str;

		str = $"ScoreCount: {Replays.Count}\n";
		str += $"FirstScore: {Replays.OrderBy(a => a.ReplayTimeStamp).First().ReplayTimeStamp}\n";
		str += $"LastScore: {Replays.OrderBy(a => a.ReplayTimeStamp).Last().ReplayTimeStamp}\n";

		return str;
	}
}
