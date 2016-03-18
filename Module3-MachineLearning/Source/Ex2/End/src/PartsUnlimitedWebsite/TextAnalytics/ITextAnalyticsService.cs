namespace PartsUnlimited.TextAnalytics
{
    public interface ITextAnalyticsService
    {
        SentimentResult GetSentiment(string inputTextEncoded);

        SentimentResult GetKeyPhrases(string inputTextEncoded);
    }
}
