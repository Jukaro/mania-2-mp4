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
	private readonly Skin _skin;
	private readonly ScrollInfo _scrollInfo;
	private bool _isPlaying;
	private double _currentPlayTime;
	private int _spawnedNotes = 0;

	public List<GameNote> RenderedNotes { get; private set; }

	public BeatmapPlayer(BeatmapData beatmap, Skin skin, ReplayData replayData) {
		_beatmap = beatmap;
		_isPlaying = false;
		_currentPlayTime = replayData.StartDelay == -1 ? 0 : replayData.StartDelay;
		RenderedNotes = new List<GameNote>();
		_skin = skin;
		_spawnedNotes = 0;
		_scrollInfo = new(28, _skin.HitPosition, -100, beatmap.DominantBpm);
	}

	public bool AudioStarted => _currentPlayTime >= _beatmap.GeneralData.AudioLeadIn;

	public void Play() => _isPlaying = true;
	public void Pause() => _isPlaying = false;

	public void Update(double deltaTime) {
		if (!_isPlaying) return;
		_currentPlayTime += deltaTime;

		HandleNoteSpawn(deltaTime);
		HandleNoteScroll();
		HandleNoteDespawn();
	}

	public void HandleNoteDespawn() {
		RenderedNotes.RemoveAll(note => note.Y > note.DespawnYThreshold);
	}

	public void HandleNoteScroll() {
		foreach (var note in RenderedNotes)
			note.Y = _scrollInfo.GetNoteY(note.SpawnTime, _currentPlayTime, _beatmap.TimingPoints);
	}

	public void HandleNoteSpawn(double deltaTime) {
		var previousPlayTime = _currentPlayTime - deltaTime;

		for (int i = _spawnedNotes; i < _beatmap.HitObjects.Length; i++) {
			BeatmapHitObject hitNote = _beatmap.HitObjects[i];

			float noteSpawnTime = _scrollInfo.GetNoteSpawnTime(hitNote.Time, _beatmap.TimingPoints);
			if (_currentPlayTime < noteSpawnTime)
				break;

			bool isCrossingNoteTime = (noteSpawnTime < 0 || previousPlayTime < noteSpawnTime) && _currentPlayTime >= noteSpawnTime;
			if (!isCrossingNoteTime)
				continue;

			if (hitNote is HoldHitObject holdHitObject)
				SpawnHoldNote(holdHitObject);
			else if (hitNote is CircleHitObject circleHitObject)
				SpawnSimpleNote(circleHitObject);
			_spawnedNotes++;
		}
	}

	public void SpawnHoldNote(HoldHitObject holdHitObject) {
		double holdNoteSize = _scrollInfo.GetScrolledDistance(holdHitObject.Time, holdHitObject.EndTime, _beatmap.TimingPoints);

		RenderedNotes.Add(new HoldNote {
			Lane = holdHitObject.GetLane(_beatmap.DifficultyData.LaneCount),
			Y = int.MinValue,
			HoldTime = holdHitObject.EndTime - holdHitObject.Time,
			Height = (float)holdNoteSize,
			DespawnYThreshold = BaseDespawnThreshold + (int)holdNoteSize,
			SpawnTime = _scrollInfo.GetNoteSpawnTime(holdHitObject.Time, _beatmap.TimingPoints)
		});
	}

	public void SpawnSimpleNote(CircleHitObject hitNote) {
		RenderedNotes.Add(new GameNote {
			Lane = hitNote.GetLane(_beatmap.DifficultyData.LaneCount),
			Y = int.MinValue,
			DespawnYThreshold = BaseDespawnThreshold,
			SpawnTime = _scrollInfo.GetNoteSpawnTime(hitNote.Time, _beatmap.TimingPoints)
		});
	}

	public const int BaseDespawnThreshold = 550;
}
