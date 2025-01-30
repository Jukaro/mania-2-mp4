using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Databases;
using Rythmify.UI;
using Rythmify.Core.Shared;

public class BeatmapSelector : Dropdown {
	private GraphicsDevice _graphics;
	private Visuals _beatmapVisuals;
	public BeatmapWithScores SelectedBeatmap = null;
	public int DisplayedBeatmapsCount;

	public BeatmapSelector(GraphicsDevice graphics, Vector2 pos, string name, int margin, Visuals visuals, Visuals beatmapVisuals) : base(graphics, pos, margin, name, visuals) {
		_graphics = graphics;
		_beatmapVisuals = beatmapVisuals;
		DisplayedBeatmapsCount = 20;
	}

	public void Init(List<BeatmapWithScores> beatmaps, int start, ReplaySelector replaySelector) {
		for (int i = 0; i < DisplayedBeatmapsCount; i++) {
			Add(new Button(_graphics, new Vector2(0, 0), "beatmap" + i, _beatmapVisuals));
			UIElementsList[i].Visuals.Resize(UsableWidth, _beatmapVisuals.Height);

			GradientList gdList = new();
			gdList.Add(new Gradient(new Color(0, 0, 0) * 0.5f, Color.Transparent));
			UIElementsList[i].Visuals.InitGradientTexture(gdList, 255);

			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
			UIElementsList[i].Visuals.Texts.Add(new Text("", new Vector2(0, 0)));
		}
		UpdateBeatmapsDropdown(beatmaps, start, replaySelector);
	}

	public void UpdateBeatmapsDropdown(List<BeatmapWithScores> beatmaps, int start, ReplaySelector replaySelector) {
		ResetButtons();

		for (int i = 0; i < DisplayedBeatmapsCount && start + i < beatmaps.Count; i++) {
			BeatmapDataFromDB beatmap = beatmaps[start + i].BeatmapDBInfo;
			string title = beatmap.SongTitle + " [" + beatmap.Difficulty + "]";
			string artist = beatmap.ArtistName;
			string mapper = beatmap.CreatorName;
			string starRating = beatmap.ManiaStarRating == null ? "" : beatmap.ManiaStarRating[(int)Mods.None].ToString("F2") + "*";

			UIElementsList[i].Visuals.Texts[0] = new Text(title, new Vector2(0, 0));
			UIElementsList[i].Visuals.Texts[1] = new Text("by " + artist, new Vector2(0, 20));
			UIElementsList[i].Visuals.Texts[2] = new Text("mapped by " + mapper, new Vector2(0, 40));
			UIElementsList[i].Visuals.Texts[3] = new Text("star rating: " + starRating, new Vector2(0, 60));
			if (beatmaps[start + i].TexturePath == null) {
				beatmaps[start + i].SetTexturePath();
			}
			// Logger.LogDebug($"texturePath ({i}): {beatmaps[start + i].TexturePath}");
			// UIElementsList[i].Visuals.SetTextureFromFile(beatmaps[start + i].TexturePath);
			UIElementsList[i].Visuals.SetTextureFromFileAsync(beatmaps[start + i].TexturePath);
			if (UIElementsList[i] is Button button) {
				int index = i;
				button.SetOnClick(() => {
					SelectedBeatmap = beatmaps[start + index];
					replaySelector.UpdateScores(SelectedBeatmap);
					if (SelectedBeatmap.Beatmap == null) {
						SelectedBeatmap.LoadBeatmap();
					}
				});
			}
		}
	}

	private void ResetButtons() {
		for (int i = 0; i < DisplayedBeatmapsCount; i++) {
			UIElementsList[i].Visuals.Texts[0] = new Text("", new Vector2(0, 0));
			UIElementsList[i].Visuals.Texts[1] = new Text("", new Vector2(0, 0));
			UIElementsList[i].Visuals.Texts[2] = new Text("", new Vector2(0, 0));
			UIElementsList[i].Visuals.Texts[3] = new Text("", new Vector2(0, 0));
			UIElementsList[i].Visuals.SetColor(Color.White);
		}
	}
}
