using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapConcatenation {
	public static Texture2D ConcatenateBeatmapBackground(GraphicsDevice graphics, List<BeatmapWithScores> beatmaps) {
		List<Texture2D> bgs = new();

		foreach (BeatmapWithScores beatmap in beatmaps) {
			beatmap.SetTexturePath();
			bgs.Add(Texture2D.FromFile(graphics, beatmap.TexturePath));
		}

		int width = 1920;
		int height = 1080;

		RenderTarget2D renderTarget = new RenderTarget2D(graphics, width, height);
		SpriteBatch spriteBatch = new(graphics);

		graphics.SetRenderTarget(renderTarget);
		graphics.Clear(Color.Transparent);


		spriteBatch.Begin();
		for (int i = 0; i < beatmaps.Count; i++) {
			Texture2D texture = bgs[i];
			int source_width = texture.Width;
			int source_height = texture.Width * height / width;

			spriteBatch.Draw(
				texture,
				new Rectangle(width / beatmaps.Count * i, 0, width / beatmaps.Count, height),
				new Rectangle(source_width / 2, 0, source_width / beatmaps.Count, source_height),
				Color.White);
		}
		spriteBatch.End();

		graphics.SetRenderTarget(null);

		Texture2D result = new Texture2D(graphics, width, height);
		Color[] data = new Color[width * height];
		renderTarget.GetData(data);
		result.SetData(data);

		return result;
	}
}
