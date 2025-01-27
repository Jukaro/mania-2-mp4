using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;
using Rythmify.Dev;
using Rythmify.UI;

public class ReplaySelector : Dropdown {
	private GraphicsDevice _graphics;
	private Visuals _scoreVisuals;
	public ReplayData SelectedReplay = null;
	public bool NeedToUpdatePlayers = false;

	public ReplaySelector(GraphicsDevice graphics, Vector2 pos, string name, int margin, Visuals visuals, Visuals scoreVisuals) : base(graphics, pos, margin, name, visuals) {
		_graphics = graphics;
		_scoreVisuals = scoreVisuals;
	}

	private void UpdateScoresDropdown(List<ReplayData> replays, Texture2D texture, BeatmapDB beatmapDB) {
		for (int i = 0; i < replays.Count; i++) {
			Add(new Button(_graphics, new Vector2(0, 0), "replay" + i, _scoreVisuals));
			UIElementsList[i].Visuals.Resize(UsableWidth, _scoreVisuals.Height);

			GradientList gdList = new();
			gdList.Add(new Gradient(new Microsoft.Xna.Framework.Color(0, 0, 0) * 0.6f, new Microsoft.Xna.Framework.Color(0, 0, 0) * 0.6f));
			UIElementsList[i].Visuals.InitGradientTexture(gdList, 255);

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

			int totalHits = r.NbMax300s + r.Nb300s + r.Nb200s + r.Nb100s + r.Nb50s + r.NbMiss;
			float accuracyV1 = (r.NbMax300s + r.Nb300s) * 300 + r.Nb200s * 200 + r.Nb100s * 100 + r.Nb50s * 50;
			float accuracyV2 = r.NbMax300s * 320 + r.Nb300s * 300 + r.Nb200s * 200 + r.Nb100s * 100 + r.Nb50s * 50;
			float maxAccuracyV1 = totalHits * 300;
			float maxAccuracyV2 = totalHits * 320;
			float realAccuracyV1 = accuracyV1 / maxAccuracyV1 * 100.0f;
			float realAccuracyV2 = accuracyV2 / maxAccuracyV2 * 100.0f;

			string accuracyStr = realAccuracyV1.ToString("F2");

			double difficultyValue = 0.0f;

			if (beatmapDB != null) {
				BeatmapWithScores beatmap2 = new BeatmapWithScores(beatmapDB.Beatmaps[replays[i].BeatmapMD5]);
				double starRating2 = beatmap2.BeatmapDBInfo.ManiaStarRating == null ? 0.0f : beatmap2.BeatmapDBInfo.ManiaStarRating[(int)Mods.None];

				difficultyValue = 8.0 * Math.Pow(Math.Max(starRating2 - 0.15, 0.05), 2.2) // Star rating to pp curve
									  * Math.Max(0, 5 * (realAccuracyV2 / 100) - 4) // From 80% accuracy, 1/20th of total pp is awarded per additional 1% accuracy
									  * (1 + 0.1 * Math.Min(1, (double)totalHits / 1500)); // Length bonus, capped at 1500 notes
			}

			string score = r.Score.ToString() + " " + accuracyStr + "%" + " " + difficultyValue.ToString("F0") + "pp";
			string judgements1 = "Nb maxs: " + r.NbMax300s.ToString() + " | ";
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
					}
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
		UpdateScoresDropdown(beatmap.Replays, texture, null);
	}

	public void UpdateScores(List<ReplayData> replays, BeatmapDB beatmapDB) {
		RemoveAll();
		UpdateScoresDropdown(replays, null, beatmapDB);
	}
}
