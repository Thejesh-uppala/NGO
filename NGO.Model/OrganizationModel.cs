using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGO.Model
{
    public class OrganizationModel
    {
        public int Id { get; set; }
        public string OrgName { get; set; } = null!;
        public string? OrgWelMsg { get; set; }
        public string? PayPalKey { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<OrganizationChapterModel> OrganizationChapters { get; set; }
    }
}
