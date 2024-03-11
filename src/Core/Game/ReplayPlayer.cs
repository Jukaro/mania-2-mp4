using System;
using System.Collections.Generic;
using Rythmify.Core.Beatmap;

namespace Rythmify.Core.Game;

public class GameNote {
	public int Lane;
	public double Y;
}

public class ReplayPlayer {
	private BeatmapData Beatmap;
	private bool IsPlaying;

	private double CurrentPlayTime;

	public List<GameNote> RenderedNotes { get; private set; }

	public ReplayPlayer(BeatmapData beatmap) {
		Beatmap = beatmap;
		IsPlaying = false;
		CurrentPlayTime = 0;
		RenderedNotes = new List<GameNote>();
	}

	public void Play() {
		IsPlaying = true;
	}

	public void Pause() {
		IsPlaying = false;
	}

	public void Update(double deltaTime) {
		if (!IsPlaying) return;

		var HitPosition = 384;
		var ScrollSpeed = 28;
		var noteScrollTime = (6860 + 6860 * (HitPosition / 480f)) / ScrollSpeed;
		var noteDespawnYThreshold = 550;

		var noteScrollSpeed = Math.Abs(noteDespawnYThreshold) / (double)noteScrollTime;

		var spawnPoint = -100;
		var timeItTakesToReach0 = Math.Abs(0 - spawnPoint) / noteScrollSpeed;

		foreach (var hitNote in Beatmap.HitObjects) {
			var noteSpawnTime = hitNote.Time - noteScrollTime - timeItTakesToReach0;

			var isCrossingNoteTime = CurrentPlayTime < noteSpawnTime && CurrentPlayTime + deltaTime >= noteSpawnTime;

			var timeSinceSpawnTime = CurrentPlayTime - noteSpawnTime;

			if (isCrossingNoteTime) {
				RenderedNotes.Add(new GameNote {
					Lane = hitNote.GetLane(Beatmap.DifficultyData.LaneCount),
					Y = spawnPoint + timeSinceSpawnTime * noteScrollSpeed
				});
			}
		}

		foreach (var note in RenderedNotes) {
			note.Y += (int)(noteScrollSpeed * deltaTime);
		}

		RenderedNotes.RemoveAll(note => note.Y > noteDespawnYThreshold);

		CurrentPlayTime += deltaTime;
	}
}
