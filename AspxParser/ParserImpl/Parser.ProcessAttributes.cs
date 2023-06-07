using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace AspxParser
{
    partial class Parser
    {
        private TagAttributes ProcessAttributes(Match match, bool isDirectiveAttributes, out string directiveName)
        {
            directiveName = string.Empty;

            string id = null;
            bool isRunAtServer = false;
            var attributeNames = match.Groups["attrname"].Captures;
            var attributeValues = match.Groups["attrval"].Captures;
            var attributes = new Dictionary<string, string>(attributeNames.Count, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < attributeNames.Count; ++i)
            {
                var attributeName = attributeNames[i].Value;

                if (isDirectiveAttributes && i == 0 && match.Groups["equal"].Captures[0].Length == 0)
                {
                    directiveName = attributeName;
                    continue;
                }

                var attributeValue = WebUtility.HtmlDecode(attributeValues[i].Value);

                if ("id".EqualsNoCase(attributeName))
                {
                    id = attributeValue;
                }
                else if ("runat".EqualsNoCase(attributeName))
                {
                    isRunAtServer = "server".EqualsNoCase(attributeValue);
                }
                else if (attributes.ContainsKey(attributeName))
                {
                    var location = CreateLocation(match);
                    eventListener.OnError(location, $"Duplicated tag attribute `{attributeName}`.");
                }
                else
                {
                    attributes.Add(string.Intern(attributeName), attributeValue);
                }
            }

            return new TagAttributes(id, isRunAtServer, attributes);
        }
    }
}
