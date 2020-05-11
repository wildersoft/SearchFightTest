using Microsoft.EntityFrameworkCore;

namespace SearchFight.Data
{
    public class SearchFightContext : DbContext
    {
        public DbSet<SearchEngines> SearchEngines { get; set; }
        public DbSet<ProgrammingLanguages> ProgrammingLanguages { get; set; }
        public DbSet<ProgrammingLanguagesPopularity> ProgrammingLanguagesPopularity { get; set; }

        public SearchFightContext(DbContextOptions<SearchFightContext> options) : base(options)
        {
        }
    }
}