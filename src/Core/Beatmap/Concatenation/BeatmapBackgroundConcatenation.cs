using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapConcatenation {
	public static Bitmap ConcatenateBeatmapBackground(List<BeatmapWithScores> beatmaps) {
		List<Bitmap> bgs = new();

		foreach (BeatmapWithScores beatmap in beatmaps) {
			beatmap.SetTexturePath();
			Bitmap background = BeatmapDisplayHelper.GetBackground(beatmap) ?? BeatmapDisplayHelper.GetSeasonalBackground();
			bgs.Add(background);
		}

		int width = 1920;
		int height = 1080;

		Bitmap newBg = Dispatcher.UIThread.Invoke(() => {
			var bitmap = new RenderTargetBitmap(new PixelSize(width, height));
			using (var context = bitmap.CreateDrawingContext())
			{
				var bgSliceSize = new Size(width / bgs.Count, height);
				for (int i = 0; i < bgs.Count; i++) {
					var destPos = new Point(i * (width / bgs.Count), 0);
					var sourcePos = new Point(bgs[i].Size.Width / 2 - bgs[i].Size.Width / bgs.Count / 2, 0);
					var destRect = new Rect(destPos, bgSliceSize);
					var sourceRect = new Rect(sourcePos, new Size(bgs[i].Size.Width / bgs.Count, bgs[i].Size.Height));
					context.DrawImage(bgs[i], sourceRect, destRect);
				}
			}
			return bitmap;
		});

		return newBg;

		// RenderTarget2D renderTarget = new RenderTarget2D(graphics, width, height);
		// SpriteBatch spriteBatch = new(graphics);

		// graphics.SetRenderTarget(renderTarget);
		// graphics.Clear(Color.Transparent);


		// spriteBatch.Begin();
		// for (int i = 0; i < beatmaps.Count; i++) {
		// 	Texture2D texture = bgs[i];
		// 	int source_width = texture.Width;
		// 	int source_height = texture.Width * height / width;

		// 	spriteBatch.Draw(
		// 		texture,
		// 		new Rectangle(width / beatmaps.Count * i, 0, width / beatmaps.Count, height),
		// 		new Rectangle(source_width / 2, 0, source_width / beatmaps.Count, source_height),
		// 		Color.White);
		// }
		// spriteBatch.End();

		// graphics.SetRenderTarget(null);

		// Texture2D result = new Texture2D(graphics, width, height);
		// Color[] data = new Color[width * height];
		// renderTarget.GetData(data);
		// result.SetData(data);

		// return result;
	}
}
