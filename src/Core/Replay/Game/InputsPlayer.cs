using System.Numerics;
using Rythmify.Core.Replay;

namespace Rythmify.Core.Game;

public class InputsPlayer {
	public bool[] RenderedInputs;
	public double CurrentPlayTime;
	public int CurrentInputIndex;
	private ReplayData _replay;
	public bool IsPlaying { get; private set; }

	public InputsPlayer(ReplayData replay) {
		_replay = replay;
		RenderedInputs = new bool[7];
		CurrentInputIndex = 0;
		CurrentPlayTime = 0;
		IsPlaying = false;
	}

	public void Play() => IsPlaying = true;
	public void Pause() => IsPlaying = false;

	public void Update(double deltaTime) {
		if (!IsPlaying) return;

		CurrentPlayTime += deltaTime;

		if (CurrentPlayTime > _replay.Inputs[CurrentInputIndex].Timestamp)
			CurrentInputIndex++;

		RenderedInputs[0] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 0)) != 0;
		RenderedInputs[1] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 1)) != 0;
		RenderedInputs[2] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 2)) != 0;
		RenderedInputs[3] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 3)) != 0;
		RenderedInputs[4] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 4)) != 0;
		RenderedInputs[5] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 5)) != 0;
		RenderedInputs[6] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 6)) != 0;
	}
}
