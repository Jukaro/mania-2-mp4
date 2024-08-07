using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Rythmify.Core.Beatmap;
using Rythmify.Core.BeatmapDB;
using Rythmify.Core.Shared;
using Rythmify.UI;

public class BeatmapSelector : Dropdown {
	private GraphicsDevice _graphics;
	private Visuals _beatmapVisuals;
	public BeatmapWithScores SelectedBeatmap = null;

	public BeatmapSelector(GraphicsDevice graphics, Vector2 pos, string name, int margin, Visuals visuals, Visuals beatmapVisuals) : base(graphics, pos, margin, name, visuals) {
		_graphics = graphics;
		_beatmapVisuals = beatmapVisuals;
	}

	public void Init(Dictionary<string, BeatmapWithScores> beatmaps, int start, ReplaySelector replaySelector) {
		for (int i = 0; i < 20; i++) {
			Add(new Button(_graphics, new Vector2(0, 0), "beatmap" + i, _beatmapVisuals));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
		}
		UpdateBeatmapsDropdown(beatmaps, start, replaySelector);
	}

	public void UpdateBeatmapsDropdown(Dictionary<string, BeatmapWithScores> beatmaps, int start, ReplaySelector replaySelector) {
		for (int i = 0; i < 20 && start + i < beatmaps.Count; i++) {
			BeatmapDataFromDatabase beatmap = beatmaps.ElementAt(start + i).Value.BeatmapDBInfo;
			string title = beatmap.SongTitle + " [" + beatmap.Difficulty + "]";
			string artist = beatmap.ArtistName;
			string mapper = beatmap.CreatorName;
			string starRating = beatmap.ManiaStarRating[(int)Mods.None].ToString("F2") + "*";

			UIElementsList[i].Visuals.Texts[0] = new Text(title, new Vector2(0, 0));
			UIElementsList[i].Visuals.Texts[1] = new Text("by " + artist, new Vector2(0, 15));
			UIElementsList[i].Visuals.Texts[2] = new Text("mapped by " + mapper, new Vector2(0, 30));
			UIElementsList[i].Visuals.Texts[3] = new Text("star rating: " + starRating, new Vector2(0, 45));
			Logger.LogDebug($"texturePath ({i}): {beatmaps.ElementAt(start + i).Value.TexturePath}");
			UIElementsList[i].Visuals.SetTextureFromFile(beatmaps.ElementAt(start + i).Value.TexturePath);
			if (UIElementsList[i] is Button button) {
				int index = i;
				button.SetOnClick(() => {
					SelectedBeatmap = beatmaps.ElementAt(start + index).Value;
					replaySelector.UpdateScoresDropdown(SelectedBeatmap);
					if (SelectedBeatmap.Beatmap == null) {
						string folderPath = "E:/osu maps de giga ultra mort/" + SelectedBeatmap.BeatmapDBInfo.FolderName.Trim() + "/";
						string filePath = folderPath + SelectedBeatmap.BeatmapDBInfo.Filename;
						SelectedBeatmap.Beatmap = BeatmapParser.Parse(filePath);
					}
				});
			}
		}
	}
}
