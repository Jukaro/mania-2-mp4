using System;
using Rythmify.Core.Replay;

namespace Mania2mp4.Displays;

public class Top100ScoreDisplay : ScoreDisplay {
	public double? Weight { get; set; }
	public double? WeightedPP { get; set; }
	public string ElapsedTimeSinceSet { get; set; }

	public Top100ScoreDisplay(ReplayData score) : base(score) {
		ElapsedTimeSinceSet = GetElapsedTimeString(score.TimeStamp);
	}

	private string GetElapsedTimeString(DateTime timestamp) {
		TimeSpan elapsedTime = DateTime.Now - timestamp;
		int twoMonths = 30 * 2;
		int twoYears = 365 * 2;

		if (elapsedTime < TimeSpan.FromMinutes(2))
			return $"{elapsedTime.TotalSeconds:F0} seconds ago";
		else if (elapsedTime < TimeSpan.FromHours(2))
			return $"{elapsedTime.TotalMinutes:F0} minutes ago";
		else if (elapsedTime < TimeSpan.FromDays(2))
			return $"{elapsedTime.TotalHours:F0} hours ago";
		else if (elapsedTime < TimeSpan.FromDays(twoMonths))
			return $"{elapsedTime.TotalDays:F0} days ago";
		else if (elapsedTime < TimeSpan.FromDays(twoYears))
			return $"{elapsedTime.TotalDays / 30:F0} months ago";

		return $"{elapsedTime.TotalDays / 365:F0} years ago";
	}
}
