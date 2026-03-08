using System;
using Microsoft.Win32.SafeHandles;

namespace Rythmify.Core.Beatmap;

public class ScrollInfo {
	public int HitPosition;
	public float SpawnPoint;
	public double DominantBpm;
	public double BeatmapScrollSpeed;
	private int _scrollSpeed;

	public ScrollInfo(int scrollSpeed, int hitPosition, float spawnPoint, double dominantBpm) {
		Logger.LogInfo($"[ScrollInfo] Creating ScrollInfo with ScrollSpeed: {scrollSpeed}, HitPosition: {hitPosition}, SpawnPoint: {spawnPoint}, DominantBpm: {dominantBpm}");
		HitPosition = hitPosition;
		SpawnPoint = spawnPoint;
		DominantBpm = dominantBpm;
		SetScrollSpeed(scrollSpeed);
	}

	public void SetScrollSpeed(int scrollSpeed) {
		_scrollSpeed = Math.Max(scrollSpeed, 1);
		double baseScrollSpeedMultiplier = 1 / GetBPMScrollSpeedMultiplier(DominantBpm);
		BeatmapScrollSpeed = _scrollSpeed * baseScrollSpeedMultiplier;
	}

	public void ScrollSpeedUp() => SetScrollSpeed(_scrollSpeed + 1);
	public void ScrollSpeedDown() => SetScrollSpeed(_scrollSpeed - 1);

	public static double GetBPMScrollSpeedMultiplier(double bpm) => bpm / 100.0;

	public double GetNoteScrollSpeed(double bpm, double sliderVelocityMultiplier) {
		double bpmMultiplier = GetBPMScrollSpeedMultiplier(bpm);

		double scrollSpeed = BeatmapScrollSpeed * bpmMultiplier * sliderVelocityMultiplier;
		double travelTimeToHitPosition = (6860 + 6860 * (HitPosition / Playfield.PlayfieldHeight)) / scrollSpeed;
		double noteSpeed = HitPosition / travelTimeToHitPosition;

		return noteSpeed;
	}

	private static int FindTimingPointAtTime(BeatmapTimingPoint[] timingPoints, double time) {
		int left = 0;
		int right = timingPoints.Length - 1;
		int result = -1;

		while (left <= right) {
			int mid = left + (right - left) / 2;

			if (timingPoints[mid].Time <= time) {
				result = mid;
				left = mid + 1;
			} else {
				right = mid - 1;
			}
		}

		return result;
	}

	public double GetScrollTime(double from, double to, double hitObjectTime, BeatmapTimingPoint[] timingPoints) {
		double timeTook = 0;

		double currentPos = from;
		double currentTime = hitObjectTime;

		int timingPointIndex = FindTimingPointAtTime(timingPoints, hitObjectTime);

		while (currentPos > to) {
			double sliderVelocityMultiplier = timingPointIndex >= 0 ? timingPoints[timingPointIndex].SliderVelocityMultiplier : 1;
			double currentBPM = timingPoints[Math.Clamp(timingPointIndex, 0, timingPoints.Length - 1)].BPM;
			double currentNoteSpeed = GetNoteScrollSpeed(currentBPM, sliderVelocityMultiplier);

			double time = timingPointIndex >= 0 ? timingPoints[timingPointIndex].Time : int.MinValue;
			double timeWithThisTimingPoint = currentTime - time;

			double newPos = currentPos - timeWithThisTimingPoint * currentNoteSpeed;
			if (newPos <= to) {
				double timeNeededToReachEnd = Math.Abs(to - currentPos) / currentNoteSpeed;
				timeTook += timeNeededToReachEnd;
				break;
			}

			currentPos = newPos;
			currentTime -= timeWithThisTimingPoint;
			timeTook += timeWithThisTimingPoint;
			timingPointIndex--;
		}

		return timeTook;
	}

	public float GetNoteSpawnTime(double hitObjectTime, BeatmapTimingPoint[] timingPoints) {
		double noteScrollTime = GetScrollTime(HitPosition, SpawnPoint, hitObjectTime, timingPoints);
		return (float)(hitObjectTime - noteScrollTime);
	}

	public double GetScrolledDistance(double fromTime, double toTime, BeatmapTimingPoint[] timingPoints) {
		double currentTime = fromTime;
		double scrolledDistance = 0;

		int timingPointIndex = FindTimingPointAtTime(timingPoints, fromTime) + 1;

		while (currentTime < toTime) {
			double sliderVelocityMultiplier = timingPointIndex != timingPoints.Length && timingPointIndex != 0 ? timingPoints[timingPointIndex - 1].SliderVelocityMultiplier : 1;
			double currentBPM = timingPoints[Math.Clamp(timingPointIndex - 1, 0, timingPoints.Length - 1)].BPM;
			double currentNoteSpeed = GetNoteScrollSpeed(currentBPM, sliderVelocityMultiplier);

			double endTime = timingPointIndex != timingPoints.Length ? timingPoints[timingPointIndex].Time : int.MaxValue;
			double timeWithThisTimingPoint = Math.Min(endTime, toTime) - currentTime;

			currentTime += timeWithThisTimingPoint;
			scrolledDistance += timeWithThisTimingPoint * currentNoteSpeed;
			timingPointIndex++;
		}

		return scrolledDistance;
	}

	public float GetNoteY(double spawnTime, double currentTime, BeatmapTimingPoint[] timingPoints) {
		double scrolledDistance = GetScrolledDistance(spawnTime, currentTime, timingPoints);
		return (float)(SpawnPoint + scrolledDistance);
	}
}
