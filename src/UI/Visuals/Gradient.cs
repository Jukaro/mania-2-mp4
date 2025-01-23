using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Rythmify.UI;

public class Gradient {
	public Color StartColor;
	public Color EndColor;
	public int RedDiff;
	public int GreenDiff;
	public int BlueDiff;
	public int AlphaDiff;

	public Gradient(Color startColor, Color endColor) {
		StartColor = startColor;
		EndColor = endColor;
		RedDiff = EndColor.R - StartColor.R;
		GreenDiff = EndColor.G - StartColor.G;
		BlueDiff = EndColor.B - StartColor.B;
		AlphaDiff = EndColor.A - StartColor.A;
	}

	public Color GetColorFromGradient(float gradientStep, float gradientIndex) {
		float	red;
		float	green;
		float	blue;
		float	alpha;
		float	step;

		float normalizedValue = gradientIndex / gradientStep;
		normalizedValue = normalizedValue * normalizedValue * (3 - 2 * normalizedValue);
		gradientIndex = normalizedValue * gradientStep;

		step = RedDiff / gradientStep;
		red = StartColor.R + (gradientIndex * step);
		step = GreenDiff / gradientStep;
		green = StartColor.G + (gradientIndex * step);
		step = BlueDiff / gradientStep;
		blue = StartColor.B + (gradientIndex * step);
		step = AlphaDiff / gradientStep;
		alpha = StartColor.A + (gradientIndex * step);
		return new ((int)red, (int)green, (int)blue, (int)alpha);
	}
}

public class GradientList {
	public List<Gradient> _gradientList;

	public GradientList() {
		_gradientList = new();
	}

	public int Count => _gradientList.Count;

	public void Add(Gradient gradient) {
		_gradientList.Add(gradient);
	}

	public Gradient ChooseGradient(int gd_index) {
		int gd_nb = _gradientList.Count;
		for (int i = 0; i < gd_nb; i++)
			if (gd_index % gd_nb == i)
				return _gradientList[i];
		return _gradientList[0]; // pareil throw jsp
	}

	public Color GetColor(double i, int step) {
		int	gd_index;
		double	gd_col_index;

		if (_gradientList.Count == 0)
			return new(0, 0, 0, 0); // a changer genre throw jsp

		gd_index = (int)(i / step); // gradient index from 0 to infinity
		gd_col_index = i - gd_index * step;
		Gradient gradient = ChooseGradient(gd_index);
		return gradient.GetColorFromGradient(step, (float)gd_col_index);
	}
}
