using System.Collections;

namespace Zakuska_Scholar.Models
{
    public class searchDto
    {
        public List<articleModel>? articles { get; set; }
        public string search { get; set; }
        public string duzeltilmisSearch { get; set; }
    }
}
