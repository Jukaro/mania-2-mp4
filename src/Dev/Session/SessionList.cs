using System;
using System.Collections.Generic;
using System.Linq;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;

public class SessionList {
	public Dictionary<DateTime, Session> SessionsDict = new();
	public IOrderedEnumerable<KeyValuePair<DateTime, Session>> SessionsOrderedList;

	public SessionList(ScoreDB scoreDB) {
		foreach (BeatmapWithScores beatmap in scoreDB.Beatmaps.Values) {
			foreach (ReplayData replay in beatmap.Replays) {
				if (!SessionsDict.ContainsKey(replay.TimeStamp.Date))
					SessionsDict[replay.TimeStamp.Date] = new();
				SessionsDict[replay.TimeStamp.Date].Replays.Add(replay);
			}
		}

		foreach (Session session in SessionsDict.Values) {
			session.Replays = session.Replays.OrderBy(a => a.TimeStamp).ToList();
		}

		SessionsOrderedList = SessionsDict.OrderBy(a => a.Key);
	}

	public override string ToString() {
		string str;

		str = $"SessionCount: {SessionsDict.Count()}, ";
		str += $"OldestSession: {SessionsOrderedList.First().Key.ToShortDateString()}, ";
		str += $"NewestSession: {SessionsOrderedList.Last().Key.ToShortDateString()}";

		return str;
	}
}
