using System;
using System.Collections.Generic;
using System.Linq;

namespace Rythmify.Core.Beatmap;

public partial class BeatmapParser {
	static readonly Dictionary<EventType, Action<BeatmapEvent[], string[]>> eventToParser = new() {
		{EventType.Background, (events, parameters) => { events.Append(ParseBackgroundEvent(parameters)); } },
		// {EventType.Video, (events, parameters) => { events.Append(ParseVideoEvent(parameters)); } },
		// {EventType.Break, (events, parameters) => { events.Append(ParseBreakEvent(parameters)); } },
	};

	private static BeatmapEvent[] ParseEventsSection(string[] lines) {
		BeatmapEvent[] events = Array.Empty<BeatmapEvent>();

		foreach (string line in lines) {
			var parameters = Array.ConvertAll(line.Split(','), (string s) => s.Trim());
			var eventType = BeatmapEvent.GetEventType(parameters[0]);

			if (eventToParser.TryGetValue(eventType, out Action<BeatmapEvent[], string[]> parser))
				parser(events, parameters);
			else
				Logger.LogWarning($"Could not find parser for event type {eventType}");
		}

		return events;
	}
}
