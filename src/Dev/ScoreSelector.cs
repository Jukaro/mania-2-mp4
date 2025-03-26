using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core;
using Rythmify.Core.Databases;
using Rythmify.Core.Replay;
using Rythmify.Core.Shared;
using Rythmify.UI;

public class ScoreSelector : Dropdown {
	private GraphicsDevice _graphics;
	private Visuals _scoreVisuals;
	private GradientList _overlay;
	public ReplayData SelectedReplay = null;
	public BeatmapWithScores SelectedBeatmap = null;
	public bool NeedToUpdatePlayers = false;

	public ScoreSelector(GraphicsDevice graphics, Vector2 pos, string name, int margin, Visuals visuals, Visuals scoreVisuals) : base(graphics, pos, margin, name, visuals) {
		_graphics = graphics;
		_scoreVisuals = new Visuals(scoreVisuals);
		_scoreVisuals.Resize(UsableWidth, _scoreVisuals.Height);
		_overlay = new GradientList();
		_overlay.Add(new Gradient(new Microsoft.Xna.Framework.Color(0, 0, 0) * 0.6f, new Microsoft.Xna.Framework.Color(0, 0, 0) * 0.6f));
		_scoreVisuals.InitGradientTexture(_overlay, 255);
	}

	public void UpdateScores(BeatmapWithScores beatmap, ref BeatmapWithScores selectedBeatmap, OsuReplayController osuReplayController) {
		Logger.LogDebug($"Updating scores with beatmap: {beatmap.BeatmapDBInfo.SongTitle}");
		RemoveAll();
		Texture2D texture = Texture2D.FromFile(_graphics, beatmap.TexturePath);
		UpdateScoresDropdown(beatmap.Replays, texture, null, ref selectedBeatmap, osuReplayController);
	}

	public void UpdateScores(List<ReplayData> replays, BeatmapDB beatmapDB, ref BeatmapWithScores selectedBeatmap, OsuReplayController osuReplayController) {
		RemoveAll();
		UpdateScoresDropdown(replays, null, beatmapDB, ref selectedBeatmap, osuReplayController);
	}

	private void UpdateScoresDropdown(List<ReplayData> replays, Texture2D texture, BeatmapDB beatmapDB, ref BeatmapWithScores selectedBeatmap, OsuReplayController osuReplayController) {
		for (int i = 0; i < replays.Count; i++) {
			Add(new Button(_graphics, new Vector2(0, 0), "replay" + i, _scoreVisuals));

			int text_y = 0;
			BeatmapWithScores beatmap = null;

			if (texture != null)
				UIElementsList[i].Visuals.SetTexture(texture);
			else if (beatmapDB != null)
				SetTextureAndBeatmapInfo(UIElementsList[i].Visuals, beatmapDB.Beatmaps[replays[i].BeatmapMD5], ref beatmap, ref text_y);


			UIElementsList[i].Visuals.Texts.Add(new Text(GetPlayerString(replays[i]), new Vector2(0, text_y + 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text(GetScoreString(replays[i]), new Vector2(0, text_y + 20)));
			UIElementsList[i].Visuals.Texts.Add(new Text(GetJudgements1(replays[i]), new Vector2(0, text_y + 40)));
			UIElementsList[i].Visuals.Texts.Add(new Text(GetJudgements2(replays[i]), new Vector2(0, text_y + 60)));

			if (UIElementsList[i] is Button button) {
				int index = i;
				button.SetOnClick(() => LoadBeatmapAndScore(beatmap, replays[index], osuReplayController));
			}
		}
	}

	private void SetTextureAndBeatmapInfo(Visuals visuals, BeatmapDataFromDB beatmapDataFromDB, ref BeatmapWithScores beatmap, ref int text_y) {
		beatmap = new BeatmapWithScores(beatmapDataFromDB);
		beatmap.SetTexturePath();
		visuals.SetTextureFromFileAsync(beatmap.TexturePath);

		string starRating = beatmap.BeatmapDBInfo.ManiaStarRating == null ? "" : beatmap.BeatmapDBInfo.ManiaStarRating[(int)Mods.None].ToString("F2") + "*";
		string title = beatmap.BeatmapDBInfo.SongTitle + " [" + beatmap.BeatmapDBInfo.Difficulty + "]" + " " + starRating;
		visuals.Texts.Add(new Text(title, new Vector2(0, 0)));
		text_y = 20;
	}

	private string GetPlayerString(ReplayData r) {
		return r.PlayerName + " " + r.TimeStamp;
	}

	private string GetScoreString(ReplayData r) {
		string accuracyStr = ScoreMetrics.ComputeV1Accuracy(r).ToString("F2");
		double performancePoints = r.PerformancePoints;

		return r.Score.ToString() + " " + accuracyStr + "%" + " " + performancePoints.ToString("F0") + "pp";
	}

	private string GetJudgements1(ReplayData r) {
		string judgements1 = "Nb 320s: " + r.NbMax300s.ToString() + " | ";
		judgements1 += "Nb 300s: " + r.Nb300s.ToString() + " | ";
		judgements1 += "Nb 200s: " + r.Nb200s.ToString() + " | ";
		judgements1 += "Nb 100s: " + r.Nb100s.ToString();

		return judgements1;
	}

	private string GetJudgements2(ReplayData r) {
		string judgements2 = "Nb 50s: " + r.Nb50s.ToString() + " | ";
		judgements2 += "Nb misses: " + r.NbMiss.ToString();

		return judgements2;
	}

	private void LoadBeatmapAndScore(BeatmapWithScores beatmap, ReplayData score, OsuReplayController osuReplayController) {
		if (score.Inputs == null) {
			try {
				score = ReplayParser.Parse(score.FilePath, 4, false);
			} catch {
				Logger.LogError("Could not parse the replay.");
				return;
			}
		}
		if (beatmap != null) {
			try {
				beatmap.LoadBeatmap();
			} catch {
				Logger.LogError("Could not load the beatmap.");
				return;
			}
		}
		if (beatmap != null)
			osuReplayController.ChangeBeatmap(beatmap);
		osuReplayController.ChangeReplay(score);
	}
}
