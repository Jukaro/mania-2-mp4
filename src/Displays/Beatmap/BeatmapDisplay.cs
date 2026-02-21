using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Shared;

namespace Mania2mp4.Displays;

public partial class BeatmapDisplay : ObservableObject
{
	private BeatmapDataFromDB _beatmapDBInfo;
	public string DebugToolTip { get; set; }

	public string StarRating { get; set; }

	[ObservableProperty]
	private string _songTitle;

	[ObservableProperty]
	private string _difficulty;

	[ObservableProperty]
	private string _artistName;

	[ObservableProperty]
	private int _keyCount;

	[ObservableProperty]
	private Bitmap _background = null;

	public string BeatmapMD5 { get; set; }

	public bool IsRanked { get; set; } = false;

	public BeatmapDisplay(BeatmapWithScores beatmap)
	{
		_beatmapDBInfo = beatmap.BeatmapDBInfo;
		DebugToolTip = _beatmapDBInfo.ToString();

		if (_beatmapDBInfo.ManiaStarRating != null && _beatmapDBInfo.ManiaStarRating.ContainsKey((int)Mods.None))
			StarRating = _beatmapDBInfo.ManiaStarRating[0].ToString("F2") + "*";
		else
			StarRating = "";
		SongTitle = _beatmapDBInfo.SongTitle;
		Difficulty = _beatmapDBInfo.Difficulty;
		KeyCount = (int)_beatmapDBInfo.CircleSize;
		ArtistName = _beatmapDBInfo.ArtistName;
		IsRanked = (RankedStatus)_beatmapDBInfo.RankedStatus == RankedStatus.Ranked;

		BeatmapMD5 = _beatmapDBInfo.BeatmapMD5;
	}

	public BeatmapDisplay(BeatmapDataFromDB beatmap)
	{
		_beatmapDBInfo = beatmap;
		DebugToolTip = _beatmapDBInfo.ToString();

		if (_beatmapDBInfo.ManiaStarRating != null && _beatmapDBInfo.ManiaStarRating.ContainsKey((int)Mods.None))
			StarRating = _beatmapDBInfo.ManiaStarRating[0].ToString("F2") + "*";
		else
			StarRating = "";
		SongTitle = _beatmapDBInfo.SongTitle;
		Difficulty = _beatmapDBInfo.Difficulty;
		KeyCount = (int)_beatmapDBInfo.CircleSize;
		ArtistName = _beatmapDBInfo.ArtistName;
		IsRanked = (RankedStatus)_beatmapDBInfo.RankedStatus == RankedStatus.Ranked;

		BeatmapMD5 = _beatmapDBInfo.BeatmapMD5;
	}

	public BeatmapDisplay(BeatmapDisplay beatmapDisplay) {
		_beatmapDBInfo = beatmapDisplay._beatmapDBInfo;
		DebugToolTip = beatmapDisplay.DebugToolTip;
		StarRating = beatmapDisplay.StarRating;
		SongTitle = beatmapDisplay.SongTitle;
		Difficulty = beatmapDisplay.Difficulty;
		KeyCount = beatmapDisplay.KeyCount;
		ArtistName = beatmapDisplay.ArtistName;
		IsRanked = beatmapDisplay.IsRanked;
		BeatmapMD5 = beatmapDisplay.BeatmapMD5;

		Background = beatmapDisplay._background == null ? null : BitmapHelper.CloneBitmap(beatmapDisplay._background);
	}
}
