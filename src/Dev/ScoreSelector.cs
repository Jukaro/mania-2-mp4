using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Replay;
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

	public void UpdateScoresDropdown(BeatmapWithScores beatmap) {
		Logger.LogDebug($"Updating scores with beatmap: {beatmap.BeatmapDBInfo.SongTitle}");
		RemoveAll();
		for (int i = 0; i < beatmap.Replays.Count; i++) {
			Add(new Button(_graphics, new Vector2(0, 0), "replay" + i, _scoreVisuals));

			string player = beatmap.Replays[i].PlayerName;
			string score = beatmap.Replays[i].Score.ToString();
			string judgements = "Nb maxs: " + beatmap.Replays[i].NbMax300s.ToString() + " | ";
			judgements += "Nb 300s: " + beatmap.Replays[i].Nb300s.ToString() + " | ";
			judgements += "Nb 200s: " + beatmap.Replays[i].Nb200s.ToString() + " | ";
			judgements += "Nb 100s: " + beatmap.Replays[i].Nb100s.ToString() + " | ";
			judgements += "Nb 50s: " + beatmap.Replays[i].Nb50s.ToString() + " | ";
			judgements += "Nb misses: " + beatmap.Replays[i].NbMiss.ToString();

			UIElementsList[i].Visuals.Texts.Add(new Text(player, new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text(score, new Vector2(0, 15)));
			UIElementsList[i].Visuals.Texts.Add(new Text(judgements, new Vector2(0, 30)));
			UIElementsList[i].Visuals.SetTextureFromFile(beatmap.TexturePath);

			if (UIElementsList[i] is Button button) {
				int index = i;
				button.SetOnClick(() => {
					if (beatmap.Replays[index].Inputs == null) {
						beatmap.Replays[index] = ReplayParser.Parse(beatmap.Replays[index].FilePath, 4, false);
					}
					SelectedReplay = beatmap.Replays[index];
					NeedToUpdatePlayers = true;
				});
			}
		}
	}
}
