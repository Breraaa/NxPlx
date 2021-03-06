using System.Linq;
using NxPlx.Models.Database;
using NxPlx.Models.Details.Series;

namespace NxPlx.Models.File
{
    public class EpisodeFile : MediaFileBase
    {
        public string Name { get; set; }

        public int SeasonNumber { get; set; }

        public int EpisodeNumber { get; set; }

        public virtual Library PartOfLibrary { get; set; }
        
        public int PartOfLibraryId { get; set; }
        
        public virtual DbSeriesDetails SeriesDetails { get; set; }
        
        public SeasonDetails SeasonDetails => SeriesDetails?.Seasons.FirstOrDefault(s => s.SeasonNumber == SeasonNumber);
        public EpisodeDetails EpisodeDetails => SeasonDetails?.Episodes.FirstOrDefault(e => e.EpisodeNumber == EpisodeNumber);
        public int? SeriesDetailsId { get; set; }

        public override string ToString()
        {
            return $"{Name} S{SeasonNumber}E{EpisodeNumber}";
        }

        public string GetNumber()
        {
            return $"S{SeasonNumber.ToString().PadLeft(2, '0')}E{EpisodeNumber.ToString().PadLeft(2, '0')}";
        }
    }
}