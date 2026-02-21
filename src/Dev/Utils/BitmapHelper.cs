using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

public static class BitmapHelper {
	public static Bitmap CloneBitmap(Bitmap bitmap) {
		var width = bitmap.PixelSize.Width;
		var height = bitmap.PixelSize.Height;
		var dpi = bitmap.Dpi;

		var clone = new WriteableBitmap(new PixelSize(width, height), dpi);

		using (var locked = clone.Lock()) {
			bitmap.CopyPixels(locked, AlphaFormat.Unpremul);
		}

		return clone;
	}
}
