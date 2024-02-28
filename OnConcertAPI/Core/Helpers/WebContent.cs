namespace OnConcert.Core.Helpers
{
    public static class WebContent
    {
        public static async Task<string> FetchJson(string url)
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }
    }
}