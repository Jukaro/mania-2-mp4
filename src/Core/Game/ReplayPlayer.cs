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

public class ReplayPlayer {
	private BeatmapData Beatmap;
	private bool IsPlaying;

	private double CurrentPlayTime;

	public List<GameNote> RenderedNotes { get; private set; }
	public Skin Skin { get; private set; }

	public ReplayPlayer(BeatmapData beatmap, Skin skin) {
		Beatmap = beatmap;
		IsPlaying = false;
		CurrentPlayTime = 0;
		RenderedNotes = new List<GameNote>();
		Skin = skin;
	}

	public void Play() {
		IsPlaying = true;
	}

	public void Pause() {
		IsPlaying = false;
	}

	public void Update(double deltaTime) {
		if (!IsPlaying) return;

		var previousPlayTime = CurrentPlayTime;
		CurrentPlayTime += deltaTime;

		int scrollSpeed = 28;
		float noteScrollTime = (6860 + 6860 * (Skin.HitPosition / 480f)) / scrollSpeed;
		float noteScrollSpeed = Skin.HitPosition / noteScrollTime;
		float spawnPoint = -100;
		float timeItTakesToReach0 = Math.Abs(0 - spawnPoint) / noteScrollSpeed;

		foreach (var hitNote in Beatmap.HitObjects) {
			var noteSpawnTime = hitNote.Time - noteScrollTime - timeItTakesToReach0;
			var isCrossingNoteTime = previousPlayTime < noteSpawnTime && CurrentPlayTime >= noteSpawnTime;
			var timeSinceSpawnTime = CurrentPlayTime - noteSpawnTime;
			if (!isCrossingNoteTime)
				continue;

			if (hitNote is HoldHitObject holdHitObject) {
				int holdTime = holdHitObject.EndTime - holdHitObject.Time;
				var holdNoteSize = holdTime * noteScrollSpeed;
				RenderedNotes.Add(new HoldNote {
					Lane = holdHitObject.GetLane(Beatmap.DifficultyData.LaneCount),
					Y = int.MinValue,
					HoldTime = holdTime,
					Height = holdNoteSize,
					DespawnYThreshold = 550 + (int)holdNoteSize,
					SpawnTime = noteSpawnTime
				});
			} else if (hitNote is CircleHitObject circleHitObject) {
				RenderedNotes.Add(new GameNote {
					Lane = circleHitObject.GetLane(Beatmap.DifficultyData.LaneCount),
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
}
