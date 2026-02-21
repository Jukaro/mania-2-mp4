using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Rythmify.Core.Databases;

namespace Rythmify.Core.Beatmap.Concatenation;

public class BeatmapMetadataParameters {
	public string? Title { get; set; }
	public string? TitleUnicode { get; set; }
	public string? Artist { get; set; }
	public string? ArtistUnicode { get; set; }
	public string? Creator { get; set; }
	public string? Version { get; set; }
	public string? Source { get; set; }
	public string? Tags { get; set; }
	public int BeatmapID { get; set; }
	public int BeatmapSetID { get; set; }

	private static Dictionary<string, Func<BeatmapDataFromDB, string>> _metadataParametersToBeatmapDataFromDBFields = new() {
		{ nameof(Title), b => b.SongTitle },
		{ nameof(TitleUnicode), b => b.SongTitleUnicode },
		{ nameof(Artist), b => b.ArtistName },
		{ nameof(ArtistUnicode), b => b.ArtistNameUnicode },
		{ nameof(Creator), b => b.CreatorName },
		{ nameof(Version), b => b.Difficulty },
		{ nameof(Source), b => b.SongSource },
		{ nameof(Tags), b => b.SongTags }
	};

	public void FillMetadataParametersWithBeatmapDataFromDB(BeatmapDataFromDB beatmapDataFromDB) {
		foreach (var propertyInfo in typeof(BeatmapMetadataParameters).GetProperties()) {
			if (propertyInfo.GetValue(this) == null)
				propertyInfo.SetValue(this, _metadataParametersToBeatmapDataFromDBFields[propertyInfo.Name](beatmapDataFromDB));
		}
	}

	public BeatmapMetadataParameters DeepClone() { return (BeatmapMetadataParameters)MemberwiseClone(); }

	public override string ToString() => $"Title: {Title}\nTitleUnicode: {TitleUnicode}\nArtist: {Artist}\nArtistUnicode: {ArtistUnicode}\nCreator: {Creator}\nVersion: {Version}\nSource: {Source}\nTags: [{string.Join(", ", Tags)}]\nBeatmapID: {BeatmapID}\nBeatmapSetID: {BeatmapSetID}";
}

public class BeatmapDifficultyParameters {
	public double? HPDrainRate { get; set; }
	public double? CircleSize { get; set; }
	public double? OverallDifficulty { get; set; }
	public double? ApproachRate { get; set; }
	public double? SliderMultiplier { get; set; }
	public double? SliderTickRate { get; set; } = 1;

	public int? LaneCount => (int?)CircleSize;

	private static Dictionary<string, Func<BeatmapDataFromDB, double>> _difficultyParametersToBeatmapDataFromDBFields = new() {
		{ nameof(HPDrainRate), b => b.HPDrain },
		{ nameof(CircleSize), b => b.CircleSize },
		{ nameof(OverallDifficulty), b => b.OverallDifficulty },
		{ nameof(ApproachRate), b => b.ApproachRate },
		{ nameof(SliderMultiplier), b => b.SliderVelocity }
	};

	public void FillDifficultyParametersWithBeatmapDataFromDB(BeatmapDataFromDB beatmapDataFromDB) {
		foreach (var propertyInfo in typeof(BeatmapDifficultyParameters).GetProperties()) {
			if (propertyInfo.GetValue(this) == null)
				propertyInfo.SetValue(this, _difficultyParametersToBeatmapDataFromDBFields[propertyInfo.Name](beatmapDataFromDB));
		}
	}

	public BeatmapDifficultyParameters DeepClone() { return (BeatmapDifficultyParameters)MemberwiseClone(); }

	public override string ToString() => $"HPDrainRate: {HPDrainRate}\nCircleSize: {CircleSize}\nOverallDifficulty: {OverallDifficulty}\nApproachRate: {ApproachRate}\nSliderMultiplier: {SliderMultiplier}\nSliderTickRate: {SliderTickRate}";
}

public partial class BeatmapConcatenationParameters : ObservableObject {
	public Delay Delay;

	[ObservableProperty]
	private BeatmapMetadataParameters _metadata = new();

	[ObservableProperty]
	private BeatmapDifficultyParameters _difficulty = new();

	public string? FolderName { get; set; }
	public string? BeatmapFilename { get; set; }

	public BeatmapMetadata GetBeatmapMetadata() {
		return new() {
			Title = Metadata.Title ?? "",
			TitleUnicode = Metadata.TitleUnicode ?? "",
			Artist = Metadata.Artist ?? "",
			ArtistUnicode = Metadata.ArtistUnicode ?? "",
			Creator = Metadata.Creator ?? "",
			Version = Metadata.Version ?? "",
			Source = Metadata.Source ?? "",
			Tags = Metadata.Tags != null ? Metadata.Tags.Split(" ") : [],
			BeatmapID = 0,
			BeatmapSetID = -1
		};
	}

	public BeatmapDifficultyData GetBeatmapDifficultyData() {
		return new() {
			HPDrainRate = Difficulty.HPDrainRate ?? 5,
			CircleSize = Difficulty.CircleSize ?? 5,
			OverallDifficulty = Difficulty.OverallDifficulty ?? 5,
			ApproachRate = Difficulty.ApproachRate ?? 5,
			SliderMultiplier = Difficulty.SliderMultiplier ?? 1.4,
			SliderTickRate = Difficulty.SliderTickRate ?? 1
		};
	}

	public BeatmapConcatenationParameters DeepClone() {
		return new() {
			Delay = Delay,
			Metadata = Metadata.DeepClone(),
			Difficulty = Difficulty.DeepClone(),
			FolderName = FolderName,
			BeatmapFilename = BeatmapFilename
		};
	}

	public void ClearMetadata() { Metadata = new(); }
	public void ClearDifficulty() { Difficulty = new(); }
}
