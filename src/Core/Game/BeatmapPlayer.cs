using System;
using System.Collections.Generic;
using Rythmify.Core.Beatmap;

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
	private readonly Skin Skin;

	private bool _isPlaying;
	private double CurrentPlayTime;

	public List<GameNote> RenderedNotes { get; private set; }

	private int _spawnedNotes = 0;

	public BeatmapPlayer(BeatmapData beatmap, Skin skin) {
		_beatmap = beatmap;
		_isPlaying = false;
		CurrentPlayTime = 0;
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
		float noteScrollTime = (6860 + 6860 * (Skin.HitPosition / PlayfieldHeight)) / scrollSpeed;
		float noteScrollSpeed = Skin.HitPosition / noteScrollTime;
		float spawnPoint = -100;
		float timeItTakesToReach0 = Math.Abs(0 - spawnPoint) / noteScrollSpeed;

		bool spawnedNoteThisFrame = false;
		for (int i = _spawnedNotes; i < _beatmap.HitObjects.Length; i++) {
			var hitNote = _beatmap.HitObjects[i];

			var noteSpawnTime = hitNote.Time - noteScrollTime - timeItTakesToReach0;
			var isCrossingNoteTime = previousPlayTime < noteSpawnTime && CurrentPlayTime >= noteSpawnTime;
			var timeSinceSpawnTime = CurrentPlayTime - noteSpawnTime;

			if (spawnedNoteThisFrame && !isCrossingNoteTime)
				break;
			if (!isCrossingNoteTime)
				continue;

			_spawnedNotes++;
			spawnedNoteThisFrame = true;

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
	}

	public const float PlayfieldHeight = 480f;
}
