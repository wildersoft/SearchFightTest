using System.ComponentModel.DataAnnotations;

namespace SearchFight.Data
{
    public class ProgrammingLanguagesPopularity
    {
        [Key]
        public int ProgrammingLanguagesPopularityID { get; set; }
        public int SearchEngineID { get; set; }
        public int ProgrammingLanguageID { get; set; }
        public int Rating { get; set; }
    }
}