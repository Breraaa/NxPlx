using System.Collections.Generic;

namespace NxPlx.Models.File
{
    public abstract class MediaFileBase : PhysicalFileBase
    {
        public virtual FFMpegProbeDetails MediaDetails { get; set; }
        
        public virtual List<SubtitleFile> Subtitles { get; set; }
    }
}