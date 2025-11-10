namespace RobinMustache.Benchmarks;

public static class TweetsTemplates
{
    public const string List = @"<ul class=""tweets"">
  {{#.}}
  <li class=""tweet"" id=""tweet-{{{id}}}"">
    <header class=""tweet-header"">
      <strong class=""tweet-user"">@{{{user}}}</strong>
      <span class=""tweet-date"">
        {{created_at}}
        {{created_at}}
      </span>
    </header>

    <div class=""tweet-content"">
      {{content}}
    </div>

    <div class=""tweet-hashtags"">
    {{#hashtags}}
        <a class=""hashtag"" href=""/search?q={{.}}"">{{{.}}}</a>
    {{/hashtags}}
    </div>

    <div class=""tweet-meta"">
      <span class=""note"">Note: {{{note}}}</span>
      <span class=""likes"">Likes {{{likes}}}</span>
      <span class=""retweets"">Retwwets: {{{retweets}}}</span>
      <span class=""bookmarks"">Favorites {{{bookmarks}}}</span>
      <span class=""lang"">{{{language}}}</span>
    </div>

    <div class=""tweet-replies"">
      <ul>
        {{#replies}}
          <li class=""reply-id""><a href=""#tweet-{{{.}}}"">{{{.}}}</a></li>
        {{/replies}}
      </ul>
    </div>
  </li>
  {{/.}}
</ul>";
}