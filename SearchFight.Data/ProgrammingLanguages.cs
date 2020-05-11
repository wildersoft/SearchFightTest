using System.ComponentModel.DataAnnotations;

namespace SearchFight.Data
{
    public class ProgrammingLanguages
    {
        [Key]
        public int ProgrammingLanguageID { get; set; }
        public string Name { get; set; }
    }
}