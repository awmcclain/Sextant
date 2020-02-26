// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System.Windows.Forms;
using System.Collections.Generic;
using Sextant.Domain.Entities;
using System;
using System.Linq;
using System.Threading;

namespace Sextant.Infrastructure
{
    public class ClipboardDataService : IUserDataService
    {
        private static ILogger _logger;
        private static IExpeditionParser _parser;

        public ClipboardDataService(ILogger logger, IExpeditionParser parser)
        {
            _logger = logger;
            _parser = parser;
        }

        public IEnumerable<StarSystem> GetExpeditionData()
        {
            string clipboardData = GetClipboard();
            return _parser.ParseExpeditionData(clipboardData);
        }

        protected virtual string GetClipboard()
        {
            string clipboardText = string.Empty;

            try
            {
                Thread thread = new Thread(() => clipboardText = Clipboard.GetText(TextDataFormat.Text));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception attempting to get clipboard");
            }

            return clipboardText;
        }
    }
}
