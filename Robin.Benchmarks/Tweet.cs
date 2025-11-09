using Robin.Generators.Accessor;
using System.Text.Json.Serialization;

namespace Robin.Benchmarks;

[GenerateAccessor]
public record class Tweet(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("user")] string User,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("hashtags")] string[] Hashtags,
    [property: JsonPropertyName("note")] double Note,
    [property: JsonPropertyName("createdat")] DateTime CreatedAt,
    [property: JsonPropertyName("likes")] int Likes,
    [property: JsonPropertyName("retweets")] int Retweets,
    [property: JsonPropertyName("bookmarks")] int Bookmarks,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("replies")] Guid[] Replies
);
