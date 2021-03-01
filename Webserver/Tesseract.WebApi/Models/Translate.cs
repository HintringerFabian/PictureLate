using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tesseract.WebApi.Models
{
    public class Translate
    {
        public string SourceLanguage { get; set; }
        public string DestinationLanguage { get; set; }
        public string Text { get; set; }
    }
}