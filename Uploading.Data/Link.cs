using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uploading.Data
{
    public class Link
    {
        public int Id { get; set; }
        public int? ImageId { get; set; }
        public string ShareLink { get; set; }
        public DateTime Expiration { get; set; }
    }
}
