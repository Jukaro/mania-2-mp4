using System;

namespace Rythmify.Core.Beatmap;

public class ScrollInfo {
	public float NoteScrollTime; // From 0 to HitPosition
	public float NoteScrollSpeed; // Per ms
	public float TimeItTakesToReach0; // From spawnPoint to 0
	public float SpawnPoint;

	public ScrollInfo(int scrollSpeed, int hitPosition, float spawnPoint) {
		SpawnPoint = spawnPoint;
		NoteScrollTime = (6860 + 6860 * (hitPosition / Playfield.PlayfieldHeight)) / scrollSpeed;
		NoteScrollSpeed = hitPosition / NoteScrollTime;
		TimeItTakesToReach0 = Math.Abs(0 - SpawnPoint) / NoteScrollSpeed;
	}

	public float GetNoteSpawnTime(double hitObjectTime) => (float)(hitObjectTime - NoteScrollTime - TimeItTakesToReach0);
	public float GetNoteY(double spawnTime, double currentTime) => (float)(SpawnPoint + (currentTime - spawnTime) * NoteScrollSpeed);
}
