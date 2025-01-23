namespace Rythmify.Core;

public class SkinCatchTheBeatSection {
	public RGB HyperDash = new(255, 0, 0);

	private RGB _hyperDashFruit = null;
	private RGB _hyperDashAfterImage = null;

	public RGB HyperDashFruit {
		get => _hyperDashFruit ?? HyperDash;
		set => _hyperDashFruit = value;
	}

	public RGB HyperDashAfterImage {
		get => _hyperDashAfterImage ?? HyperDash;
		set => _hyperDashAfterImage = value;
	}

	public override string ToString() => $"HyperDash: {HyperDash}, HyperDashFruit: {HyperDashFruit}, HyperDashAfterImage: {HyperDashAfterImage}";
}
