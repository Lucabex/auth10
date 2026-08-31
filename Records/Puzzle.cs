using System.Text.Json.Serialization;

namespace auth10.Records;

public record DailyPuzzle(
    [property:JsonPropertyName("puzzle")] Puzzle puzzle
 );

 public record Puzzle(
    [property:JsonPropertyName("solution")] List<string> Solution,
    [property:JsonPropertyName("fen")] string Fen
 )