using System.Collections.Generic;
#if PARSER_HTML_GOOGLE_SHEET
using AngleSharp.Dom;

public interface IExtractTable
{
    public void ExtracTable(IDocument dom, Dictionary<string,List<string>> dicto);
}
#endif
