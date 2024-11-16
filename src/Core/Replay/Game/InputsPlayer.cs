using Rythmify.Core.Replay;

namespace Rythmify.Core.Game;

public class InputsPlayer {
	public bool[] RenderedInputs;
	public double CurrentPlayTime;
	public int CurrentInputIndex;
	private ReplayData _replay;
	public bool IsPlaying { get; private set; }

	public InputsPlayer(ReplayData replay) {
		Init(replay);
	}

	public void Init(ReplayData replay) {
		_replay = replay;
		RenderedInputs = new bool[replay.LaneCount];
		CurrentInputIndex = 0;
		CurrentPlayTime = replay.StartDelay;
		IsPlaying = false;
	}

	public void Play() => IsPlaying = true;
	public void Pause() => IsPlaying = false;

	public void Update(double deltaTime) {
		if (!IsPlaying) return;

		CurrentPlayTime += deltaTime;

		if (CurrentPlayTime > _replay.Inputs[CurrentInputIndex].Timestamp)
			CurrentInputIndex++;

		for (int i = 0; i < RenderedInputs.Length; i++)
			RenderedInputs[i] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << i)) != 0;;
	}
}
