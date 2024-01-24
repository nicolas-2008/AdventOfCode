namespace AdventOfCode;

public class InputProvider
{
    private const string SessionCookie = "session=53616c7465645f5f16cbfbe4511afcb9787a6393c1b499467b252cde69412676cd833eb8c6c48733457f1a7048e7f33800c6c761b79dbbbf1b7beceaef4ad8ac";

    private readonly HttpClient httpClient;

    public InputProvider()
    {
        this.httpClient = new HttpClient { DefaultRequestHeaders = { { "Cookie", SessionCookie } } };
    }

    public async Task<string> GetStringAsync(int day)
    {
        var response = await httpClient.GetStringAsync($"https://adventofcode.com/2023/day/{day}/input");
        return response;
    }
}