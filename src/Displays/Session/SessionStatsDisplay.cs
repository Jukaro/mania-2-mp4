using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Mania2mp4.Models;
using Rythmify.Core.Replay;

namespace Mania2mp4.Displays;

public partial class SessionStatsDisplay : ObservableObject {
	[ObservableProperty]
	private string _date;

	[ObservableProperty]
	private int _scoresCount;

	[ObservableProperty]
	private int _pbsCount;

	[ObservableProperty]
	private int _newScoresCount;

	[ObservableProperty]
	private string _inBeatmapPlaytime;

	public SessionStatsDisplay() {}

	public SessionStatsDisplay(DatabasesService databases, Session session) {
		List<ReplayData> scores = session.Replays;

		TimeSpan totalInBeatmapTime = new TimeSpan(0);
		List<SessionScore> sessionScores = new();

		foreach (var score in scores) {
			var beatmap = databases.ScoreDB.Beatmaps[score.BeatmapMD5];
			sessionScores.Add(new SessionScore(score, beatmap));
			totalInBeatmapTime += TimeSpan.FromMilliseconds(beatmap.BeatmapDBInfo.TotalTime);
		}

		Date = session.DateTime.ToShortDateString();
		ScoresCount = scores.Count;
		PbsCount = sessionScores.Count(s => s.LocalRankWhenSet == 1 && s.ScoreDiffVsTopScore != 0);
		NewScoresCount = sessionScores.Count(s => s.ScoreDiffVsTopScore == 0);
		InBeatmapPlaytime = totalInBeatmapTime.ToString(@"%h'h 'mm'm 'ss's'");
	}
}
