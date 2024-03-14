using System;
using System.Collections.Generic;
using Rythmify.Core.Beatmap;
using Rythmify.Core.Replay;

namespace Rythmify.Core.Game;

public class GameNote {
	public int Lane;
	public float Y;
	public int DespawnYThreshold;
	public float SpawnTime;
}

public class HoldNote : GameNote {
	public int HoldTime;
	public float Height;
}

public class BeatmapPlayer {
	private readonly BeatmapData _beatmap;
	private readonly ReplayData _replay;
	private readonly Skin Skin;

	private bool _isPlaying;
	private double CurrentPlayTime;
	public List<GameNote> RenderedNotes { get; private set; }

	public bool[] RenderedInputs = new bool[4];

	public int CurrentInputIndex = 0;

	private int _spawnedNotes = 0;

	public BeatmapPlayer(BeatmapData beatmap, ReplayData replay, Skin skin) {
		_beatmap = beatmap;
		_replay = replay;
		_isPlaying = false;
		CurrentPlayTime = -1200;
		RenderedNotes = new List<GameNote>();
		Skin = skin;
		_spawnedNotes = 0;
	}

	public void Play() => _isPlaying = true;
	public void Pause() => _isPlaying = false;

	public void Update(double deltaTime) {
		if (!_isPlaying) return;

		var previousPlayTime = CurrentPlayTime;
		CurrentPlayTime += deltaTime;

		int scrollSpeed = 28;
		int jsp = 6860;
		float noteScrollTime = (jsp + jsp * (Skin.HitPosition / PlayfieldHeight)) / scrollSpeed;
		float noteScrollSpeed = Skin.HitPosition / noteScrollTime;
		float spawnPoint = -100;
		float timeItTakesToReach0 = Math.Abs(0 - spawnPoint) / noteScrollSpeed;

		for (int i = _spawnedNotes; i < _beatmap.HitObjects.Length; i++) {
			var hitNote = _beatmap.HitObjects[i];

			var noteSpawnTime = hitNote.Time - noteScrollTime - timeItTakesToReach0;
			if (CurrentPlayTime < noteSpawnTime)
				break;

			var isCrossingNoteTime = previousPlayTime < noteSpawnTime && CurrentPlayTime >= noteSpawnTime;
			var timeSinceSpawnTime = CurrentPlayTime - noteSpawnTime;

			if (!isCrossingNoteTime)
				continue;

			_spawnedNotes++;

			if (hitNote is HoldHitObject holdHitObject) {
				int holdTime = holdHitObject.EndTime - holdHitObject.Time;
				var holdNoteSize = holdTime * noteScrollSpeed;
				RenderedNotes.Add(new HoldNote {
					Lane = holdHitObject.GetLane(_beatmap.DifficultyData.LaneCount),
					Y = int.MinValue,
					HoldTime = holdTime,
					Height = holdNoteSize,
					DespawnYThreshold = 550 + (int)holdNoteSize,
					SpawnTime = noteSpawnTime
				});
			} else if (hitNote is CircleHitObject circleHitObject) {
				RenderedNotes.Add(new GameNote {
					Lane = circleHitObject.GetLane(_beatmap.DifficultyData.LaneCount),
					Y = int.MinValue,
					DespawnYThreshold = 550,
					SpawnTime = noteSpawnTime
				});
			}
		}

		foreach (var note in RenderedNotes)
			note.Y = (float)(spawnPoint + (CurrentPlayTime - note.SpawnTime) * noteScrollSpeed);

		RenderedNotes.RemoveAll(note => note.Y > note.DespawnYThreshold);

		if (CurrentPlayTime + 1750 > _replay.Inputs[CurrentInputIndex].Timestamp)
		{
			Logger.LogDebug($"[{CurrentInputIndex}]: {_replay.Inputs[CurrentInputIndex].HoldTime}, {_replay.Inputs[CurrentInputIndex].Keys}");
			CurrentInputIndex++;
		}

		RenderedInputs[0] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 0)) != 0;
		RenderedInputs[1] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 1)) != 0;
		RenderedInputs[2] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 2)) != 0;
		RenderedInputs[3] = (_replay.Inputs[CurrentInputIndex].Keys & (1 << 3)) != 0;
	}

	public const float PlayfieldHeight = 480f;
}
