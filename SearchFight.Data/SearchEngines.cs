using System.ComponentModel.DataAnnotations;

namespace SearchFight.Data
{
    public class SearchEngines
    {
        [Key]
        public int SearchEngineID { get; set; }
        public string Name { get; set; }
    }
}