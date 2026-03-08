using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Rythmify.Core;

public static class ControlRendering {
	public static void Render(Control control, Size size, string filepath) {
		var embeddableControlRoot = new EmbeddableControlRoot { Width = size.Width, Height = size.Height };
		var themedRoot = new ThemeVariantScope { RequestedThemeVariant = ThemeVariant.Dark };

		themedRoot.Child = control;
		embeddableControlRoot.Content = themedRoot;
		embeddableControlRoot.Prepare();

		var rtb = new RenderTargetBitmap(new PixelSize((int)size.Width, (int)size.Height), new Vector(96, 96));
		rtb.Render(embeddableControlRoot);
		rtb.Save(filepath);
		Logger.LogInfo($"[ControlRendering] Finished rendering \"{Path.GetFileName(filepath)}\"");
	}
}
