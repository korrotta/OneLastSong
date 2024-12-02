using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OneLastSong.Converters
{
    public class HtmlToRichTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string text)
            {
                var richTextBlock = new RichTextBlock();
                var paragraph = new Paragraph();
                var parts = Regex.Split(text, @"(\*\*.*?\*\*)");

                foreach (var part in parts)
                {
                    if (part.StartsWith("**") && part.EndsWith("**"))
                    {
                        var bold = new Bold();
                        bold.Inlines.Add(new Run { Text = part.Trim('*') });
                        paragraph.Inlines.Add(bold);
                    }
                    else
                    {
                        paragraph.Inlines.Add(new Run { Text = part });
                    }
                }

                richTextBlock.Blocks.Add(paragraph);
                return richTextBlock;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}