using Microsoft.AspNetCore.Http;


namespace NGO.Model
{
    public class TestFileModel
    {
        //public TestFileModel()
        //{
        //    ChildrensDetails = new List<ChildrensDetailsDataModel1>();
        //}
        //    public IFormFile? Photo { get; set; }
        //    public List<ChildrensDetailsDataModel1> ChildrensDetails { get; set; }
        //    public List<string> Ids { get; set; }
        //}
        //public class ChildrensDetailsDataModel1
        //{
        //    public string ChildCountry { get; set; } = null!;
        //}
        public FileModel? Model { get; set; }
       // [FromForm]
      
        public IFormFile? File { get; set; }
    }
}
