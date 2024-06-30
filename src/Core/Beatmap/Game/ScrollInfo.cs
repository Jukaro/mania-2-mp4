using System;

namespace Rythmify.Core.Beatmap;

public class ScrollInfo {
	public int HitPosition;
	public int ScrollSpeed;
	public float SpawnPoint;

	public ScrollInfo(int scrollSpeed, int hitPosition, float spawnPoint) {
		ScrollSpeed = scrollSpeed;
		HitPosition = hitPosition;
		SpawnPoint = spawnPoint;
	}

	public double GetScrollTime(double from, double to, double hitObjectTime, BeatmapTimingPoint[] timingPoints) {
		double timeTook = 0;

		double currentPos = from;
		double currentTime = hitObjectTime;

		int timingPointIndex = timingPoints.Length - 1;
		while (timingPointIndex != -1 && timingPoints[timingPointIndex].Time > currentTime)
			timingPointIndex--;

		while (currentPos > to) {
			double time = timingPointIndex >= 0 ? timingPoints[timingPointIndex].Time : int.MinValue;
			double sliderVelocityMultiplier = timingPointIndex >= 0 ? timingPoints[timingPointIndex].SliderVelocityMultiplier : 1;

			double timeWithThisTimingPoint = currentTime - time;

			double noteTimeWithThisTimingPoint = (6860 + 6860 * (HitPosition / Playfield.PlayfieldHeight)) / (ScrollSpeed * sliderVelocityMultiplier);
			double noteSpeedWithThisTimingPoint = HitPosition / noteTimeWithThisTimingPoint;

			double tmpCurrentPos = currentPos;
			currentPos -= timeWithThisTimingPoint * noteSpeedWithThisTimingPoint;

			if (currentPos <= to) {
				double timeNeededToReachEnd = Math.Abs(to - tmpCurrentPos) / noteSpeedWithThisTimingPoint;
				timeTook += timeNeededToReachEnd;
				break;
			}

			currentTime -= timeWithThisTimingPoint;
			timeTook += timeWithThisTimingPoint;
			timingPointIndex--;
		}

		return timeTook;
	}

	public float GetNoteSpawnTime(double hitObjectTime, BeatmapTimingPoint[] timingPoints) {
		double noteScrollTime = GetScrollTime(HitPosition, SpawnPoint, hitObjectTime, timingPoints);
		Logger.LogDebug($"HitTime: {hitObjectTime}, NoteScrollTime: {noteScrollTime}");
		return (float)(hitObjectTime - noteScrollTime);
	}

	public double GetScrolledDistance(double fromTime, double toTime, BeatmapTimingPoint[] timingPoints) {
		double currentTime = fromTime;
		double scrolledDistance = 0;

		int timingPointIndex = 0;
		while (timingPointIndex != timingPoints.Length && timingPoints[timingPointIndex].Time <= fromTime)
			timingPointIndex++;

		while (currentTime < toTime) {
			double endTime = timingPointIndex != timingPoints.Length ? timingPoints[timingPointIndex].Time : int.MaxValue;
			double sliderVelocityMultiplier = timingPointIndex != timingPoints.Length && timingPointIndex != 0 ? timingPoints[timingPointIndex - 1].SliderVelocityMultiplier : 1;

			double timeWithThisTimingPoint = Math.Min(endTime, toTime) - currentTime;

			double noteTimeWithThisTimingPoint = (6860 + 6860 * (HitPosition / Playfield.PlayfieldHeight)) / (ScrollSpeed * sliderVelocityMultiplier);
			double noteSpeedWithThisTimingPoint = HitPosition / noteTimeWithThisTimingPoint;

			currentTime += timeWithThisTimingPoint;

			scrolledDistance += timeWithThisTimingPoint * noteSpeedWithThisTimingPoint;
			timingPointIndex++;
		}

		return scrolledDistance;
	}

	public float GetNoteY(double spawnTime, double currentTime, BeatmapTimingPoint[] timingPoints) {
		double scrolledDistance = GetScrolledDistance(spawnTime, currentTime, timingPoints);
		return (float)(SpawnPoint + scrolledDistance);
	}
}
