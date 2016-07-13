using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uploading.Data;

namespace _0607FileUploading.Models
{
    public class ViewModel
    {
        public IEnumerable<Image> Top5Recent { get; set; } 
        public IEnumerable<Image> Top5Viewed { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string User { get; set; }
    }
}