using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;
using Rythmify.Dev;
using Rythmify.Core.Shared;
using Rythmify.Dev;
using Rythmify.UI;

public class ReplaySelector : Dropdown {
	private GraphicsDevice _graphics;
	private Visuals _scoreVisuals;
	private GradientList _overlay;
	public ReplayData SelectedReplay = null;
	public bool NeedToUpdatePlayers = false;

	public ReplaySelector(GraphicsDevice graphics, Vector2 pos, string name, int margin, Visuals visuals, Visuals scoreVisuals) : base(graphics, pos, margin, name, visuals) {
		_graphics = graphics;
		_scoreVisuals = new Visuals(scoreVisuals);
		_scoreVisuals.Resize(UsableWidth, _scoreVisuals.Height);
		_overlay = new GradientList();
		_overlay.Add(new Gradient(new Microsoft.Xna.Framework.Color(0, 0, 0) * 0.6f, new Microsoft.Xna.Framework.Color(0, 0, 0) * 0.6f));
		_scoreVisuals.InitGradientTexture(_overlay, 255);
	}

	private void UpdateScoresDropdown(List<ReplayData> replays, Texture2D texture, BeatmapDB beatmapDB, BeatmapDataFromDB beatmapDBInfo) {
		for (int i = 0; i < replays.Count; i++) {
			Add(new Button(_graphics, new Vector2(0, 0), "replay" + i, _scoreVisuals));

			int text_y = 0;

			if (texture != null)
				UIElementsList[i].Visuals.SetTexture(texture);
			else if (beatmapDB != null) {
				BeatmapWithScores beatmap = new BeatmapWithScores(beatmapDB.Beatmaps[replays[i].BeatmapMD5]);
				beatmap.SetTexturePath();
				UIElementsList[i].Visuals.SetTextureFromFileAsync(beatmap.TexturePath);

				string starRating = beatmap.BeatmapDBInfo.ManiaStarRating == null ? "" : beatmap.BeatmapDBInfo.ManiaStarRating[(int)Mods.None].ToString("F2") + "*";
				string title = beatmap.BeatmapDBInfo.SongTitle + " [" + beatmap.BeatmapDBInfo.Difficulty + "]" + " " + starRating;
				UIElementsList[i].Visuals.Texts.Add(new Text(title, new Vector2(0, 0)));
				text_y = 20;
			}

			ReplayData r = replays[i];

			string player = r.PlayerName + " " + r.TimeStamp;
			string accuracyStr = ScoreMetrics.ComputeV1Accuracy(replays[i]).ToString("F2");

			double performancePoints = r.PerformancePoints;

			string score = r.Score.ToString() + " " + accuracyStr + "%" + " " + performancePoints.ToString("F0") + "pp";
			string judgements1 = "Nb 320s: " + r.NbMax300s.ToString() + " | ";
			judgements1 += "Nb 300s: " + r.Nb300s.ToString() + " | ";
			judgements1 += "Nb 200s: " + r.Nb200s.ToString() + " | ";
			judgements1 += "Nb 100s: " + r.Nb100s.ToString();
			string judgements2 = "Nb 50s: " + r.Nb50s.ToString() + " | ";
			judgements2 += "Nb misses: " + r.NbMiss.ToString();

			UIElementsList[i].Visuals.Texts.Add(new Text(player, new Vector2(0, text_y + 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text(score, new Vector2(0, text_y + 20)));
			UIElementsList[i].Visuals.Texts.Add(new Text(judgements1, new Vector2(0, text_y + 40)));
			UIElementsList[i].Visuals.Texts.Add(new Text(judgements2, new Vector2(0, text_y + 60)));

			if (UIElementsList[i] is Button button) {
				int index = i;
				button.SetOnClick(() => {
					if (replays[index].Inputs == null) {
						try {
							replays[index] = ReplayParser.Parse(replays[index].FilePath, 4, false);
						} catch {
							Logger.LogError("Could not parse the replay.");
						}
					if (replays[index].Inputs == null) {
						try {
							replays[index] = ReplayParser.Parse(replays[index].FilePath, 4, false);
						} catch {
							Logger.LogError("Could not parse the replay.");
						}
					}
					SelectedReplay = replays[index];
					SelectedReplay = replays[index];
					NeedToUpdatePlayers = true;
				});
			}
		}
	}

	public void UpdateScores(BeatmapWithScores beatmap) {
		Logger.LogDebug($"Updating scores with beatmap: {beatmap.BeatmapDBInfo.SongTitle}");
		RemoveAll();
		Texture2D texture = Texture2D.FromFile(_graphics, beatmap.TexturePath);
		UpdateScoresDropdown(beatmap.Replays, texture, null, beatmap.BeatmapDBInfo);
	}

	public void UpdateScores(List<ReplayData> replays, BeatmapDB beatmapDB) {
		RemoveAll();
		UpdateScoresDropdown(replays, null, beatmapDB, null);
	}
}
