﻿using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Series;
using NxPlx.ApplicationHost.Api.Authentication;
using NxPlx.Core.Services;
using NxPlx.Models;

namespace NxPlx.ApplicationHost.Api.Controllers
{
    [Route("api/episode")]
    [ApiController]
    [SessionAuthentication]
    public class EpisodeController : ControllerBase
    {
        private readonly EpisodeService _episodeService;
        private readonly NextEpisodeService _nextEpisodeService;

        public EpisodeController(EpisodeService episodeService, NextEpisodeService nextEpisodeService)
        {
            _episodeService = episodeService;
            _nextEpisodeService = nextEpisodeService;
        }
        
        [HttpGet("{seriesId}/detail")]
        [Send404WhenNull]
        public Task<SeriesDto?> GetSeries([FromRoute, Required]int seriesId) 
            => _episodeService.FindSeriesDetails(seriesId, null);

        [HttpGet("{seriesId}/{seasonNo}/detail")]
        [Send404WhenNull]
        public Task<SeriesDto?> GetSeries([FromRoute, Required]int seriesId, [FromRoute, Required]int seasonNo) 
            => _episodeService.FindSeriesDetails(seriesId, seasonNo);

        [HttpGet("{fileId}/info")]
        [Send404WhenNull]
        public Task<InfoDto?> GetFileInfo([FromRoute, Required] int fileId) 
            => _episodeService.FindEpisodeFileInfo(fileId);

        [HttpGet("{fileId}/watch")]
        public async Task<ActionResult<PhysicalFileResult>> Stream([FromRoute, Required] int fileId)
        {
            var filePath = await _episodeService.FindEpisodeFilePath(fileId);
            if (!System.IO.File.Exists(filePath)) return NotFound();
            return PhysicalFile(filePath, "video/mp4", enableRangeProcessing: true);
        }
        
        [HttpGet("{seriesId}/next")]
        [Send404WhenNull]
        public Task<NextEpisodeDto?> Next([FromRoute, Required] int seriesId, [FromQuery]int? season, [FromQuery]int? episode, [FromQuery]string? mode) 
            => _nextEpisodeService.TryFindNextEpisode(seriesId, season, episode, mode ?? "default");
    }
}