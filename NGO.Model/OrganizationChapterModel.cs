using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Model
{
    public class OrganizationChapterModel
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public string ChapterName { get; set; } = null!;
        public string ChapterPresidentName { get; set; } = null!;
        public string ChapterPresidentEmail { get; set; } = null!;
        public int ChapterPresidentPhone { get; set; }
        public string Details { get; set; } = null!;
        public OrganizationModel Organization  { get; set; } = null!;
    }
}
