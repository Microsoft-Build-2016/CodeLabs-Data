// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.Models
{
    using System.Collections.Generic;

    public class Feedback
    {
        public string Message { get; set; }

        public double Score { get; set; }

        public IList<string> KeyPhrases { get; set; }
    }
}
