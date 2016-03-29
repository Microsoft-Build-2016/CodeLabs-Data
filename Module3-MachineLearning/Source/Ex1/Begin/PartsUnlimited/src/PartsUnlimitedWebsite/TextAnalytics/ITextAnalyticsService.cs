// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.TextAnalytics
{
    using System.Threading.Tasks;

    public interface ITextAnalyticsService
    {
        Task<SentimentResult> GetSentiment(string text);

        Task<KeyPhraseResult> GetKeyPhrases(string text);
    }
}
