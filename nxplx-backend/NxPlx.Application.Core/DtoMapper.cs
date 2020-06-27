﻿using System.Linq;
using NxPlx.Application.Models;
using NxPlx.Application.Models.Film;
using NxPlx.Application.Models.Series;
using NxPlx.Models;
using NxPlx.Models.Database;
using NxPlx.Models.Details;
using NxPlx.Models.Details.Film;
using NxPlx.Models.Details.Series;
using NxPlx.Models.File;

namespace NxPlx.Application.Core
{
    public class DtoMapper : MapperBase, IDtoMapper
    {
        public DtoMapper()
        {
            SetMapping<DbSeriesDetails, SeriesDto>(seriesDetails => new SeriesDto
            {
                Id = seriesDetails.Id,
                Backdrop = seriesDetails.BackdropPath,
                Poster = seriesDetails.PosterPath,
                VoteAverage = seriesDetails.VoteAverage,
                VoteCount = seriesDetails.VoteCount,
                Name = seriesDetails.Name,
                Networks = Map<Network, NetworkDto>(seriesDetails.Networks.Select(n => n.Entity2)).ToList(),
                Genres = Map<Genre, GenreDto>(seriesDetails.Genres.Select(g => g.Entity2)).ToList(),
                CreatedBy = Map<Creator, CreatorDto>(seriesDetails.CreatedBy.Select(cb => cb.Entity2)).ToList(),
                ProductionCompanies = Map<ProductionCompany, ProductionCompanyDto>(seriesDetails.ProductionCompanies.Select(pc => pc.Entity2)).ToList(),
                Overview = seriesDetails.Overview
            });
            SetMapping<SeasonDetails, SeasonDto>(seasonDetails => new SeasonDto
            {
                Name = seasonDetails.Name,
                Number = seasonDetails.SeasonNumber,
                AirDate = seasonDetails.AirDate,
                Poster = seasonDetails.PosterPath,
                Overview = seasonDetails.Overview,
            });
            SetMapping<DbSeriesDetails, OverviewElementDto>(seriesDetails => new OverviewElementDto
            {
                Id = seriesDetails.Id,
                Kind = "series",
                Poster = seriesDetails.PosterPath,
                Title = seriesDetails.Name,
                Genres = seriesDetails.Genres.Select(g => g.Entity2Id).ToList(),
                Year = seriesDetails.FirstAirDate == null ? 9999 : seriesDetails.FirstAirDate.Value.Year
            });
            SetMapping<DbFilmDetails, OverviewElementDto>(filmDetails => new OverviewElementDto
            {
                Id = filmDetails.Id,
                Kind = "film",
                Poster = filmDetails.PosterPath,
                Title = filmDetails.Title,
                Genres = filmDetails.Genres.Select(g => g.Entity2Id).ToList(),
                Year = filmDetails.ReleaseDate == null ? 9999 : filmDetails.ReleaseDate.Value.Year
            });
            SetMapping<MovieCollection, OverviewElementDto>(movieCollection => new OverviewElementDto
            {
                Id = movieCollection.Id,
                Kind = "collection",
                Poster = movieCollection.PosterPath,
                Title = movieCollection.Name,
                Genres = movieCollection.Movies.SelectMany(f => f.Genres).Select(g => g.Entity2Id).Distinct().ToList(),
                Year = movieCollection.Movies.Min(f => f.ReleaseDate == null ? 9999 : f.ReleaseDate.Value.Year)
            });
            SetMapping<(WatchingProgress wp, EpisodeFile ef), ContinueWatchingDto>(pair => new ContinueWatchingDto
            {
                FileId = pair.ef.Id,
                Kind = "series",
                Poster = pair.ef.SeasonDetails.PosterPath ?? pair.ef.SeriesDetails.PosterPath,
                Title = $"{(pair.ef.SeriesDetails == null ? "" : pair.ef.SeriesDetails.Name)} - {pair.ef.GetNumber()} - {(pair.ef.EpisodeDetails == null ? "" : pair.ef.EpisodeDetails.Name)}",
                Watched = pair.wp.LastWatched,
                Progress = pair.wp.Time / pair.ef.MediaDetails.Duration
            });
            SetMapping<(WatchingProgress wp, FilmFile ff), ContinueWatchingDto>(pair => new ContinueWatchingDto
            {
                FileId = pair.ff.Id,
                Kind = "film",
                Poster = pair.ff.FilmDetails.PosterPath,
                Title = $"{(pair.ff.FilmDetails == null ? "" : pair.ff.FilmDetails.Title)}",
                Watched = pair.wp.LastWatched,
                Progress = pair.wp.Time / pair.ff.MediaDetails.Duration
            });

            SetMapping<FilmFile, FilmDto>(filmFilm => new FilmDto
            {
                Id = filmFilm.FilmDetails.Id,
                Fid = filmFilm.Id,
                Library = filmFilm.PartOfLibraryId,
                Backdrop = filmFilm.FilmDetails.BackdropPath,
                Poster = filmFilm.FilmDetails.PosterPath,
                Title = filmFilm.FilmDetails.Title,
                Budget = filmFilm.FilmDetails.Budget,
                Genres = Map<Genre, GenreDto>(filmFilm.FilmDetails.Genres.Select(e => e.Entity2)).ToList(),
                Overview = filmFilm.FilmDetails.Overview,
                Popularity = filmFilm.FilmDetails.Popularity,
                Revenue = filmFilm.FilmDetails.Revenue,
                Runtime = filmFilm.FilmDetails.Runtime,
                Tagline = filmFilm.FilmDetails.Tagline,
                ImdbId = filmFilm.FilmDetails.ImdbId,
                OriginalLanguage = filmFilm.FilmDetails.OriginalLanguage,
                OriginalTitle = filmFilm.FilmDetails.OriginalTitle,
                PosterPath = filmFilm.FilmDetails.PosterPath,
                ProductionCompanies = Map<ProductionCompany, ProductionCompanyDto>(filmFilm.FilmDetails.ProductionCompanies.Select(e => e.Entity2)).ToList(),
                ProductionCountries = Map<ProductionCountry, ProductionCountryDto>(filmFilm.FilmDetails.ProductionCountries.Select(e => e.Entity2)).ToList(),
                ReleaseDate = filmFilm.FilmDetails.ReleaseDate,
                SpokenLanguages = Map<SpokenLanguage, SpokenLanguageDto>(filmFilm.FilmDetails.SpokenLanguages.Select(e => e.Entity2)).ToList(),
                VoteAverage = filmFilm.FilmDetails.VoteAverage,
                VoteCount = filmFilm.FilmDetails.VoteCount,
                BelongsToCollectionId = filmFilm.FilmDetails.BelongsInCollectionId
            });
            
            SetMapping<FilmFile, InfoDto>(filmFile => new InfoDto
            {
                Id = filmFile.FilmDetails.Id,
                Fid = filmFile.Id,
                Duration = filmFile.MediaDetails.Duration,
                Backdrop = filmFile.FilmDetails.BackdropPath,
                Poster = filmFile.FilmDetails.PosterPath,
                Title = filmFile.FilmDetails.Title,
                Subtitles = filmFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, EpisodeFileDto>(episodeFile => new EpisodeFileDto
            {
                Id = episodeFile.Id,
                EpisodeNumber = episodeFile.EpisodeNumber,
                SeasonNumber = episodeFile.SeasonNumber,
                Subtitles = episodeFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, InfoDto>(episodeFile => new InfoDto
            {
                Id = episodeFile.SeriesDetails.Id,
                Fid = episodeFile.Id,
                Duration = episodeFile.MediaDetails.Duration,
                Backdrop = episodeFile.SeriesDetails.BackdropPath,
                Poster = episodeFile.SeriesDetails.PosterPath,
                Title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2}",
                Subtitles = episodeFile.Subtitles.Select(s => s.Language)
            });
            SetMapping<EpisodeFile, NextEpisodeDto>(episodeFile => new NextEpisodeDto
            {
                Fid = episodeFile.Id,
                Poster = episodeFile.SeriesDetails.PosterPath,
                Title = $"{episodeFile.SeriesDetails.Name} - S{episodeFile.SeasonNumber:D2}E{episodeFile.EpisodeNumber:D2}"
            });
            SetMapping<SubtitleFile, SubtitleFileDto>(subtitleFile => new SubtitleFileDto
            {
                Id = subtitleFile.Id,
                Language = subtitleFile.Language
            });
            
            SetMapping<Genre, GenreDto>(genre => new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name
            });
            SetMapping<ProductionCompany, ProductionCompanyDto>(productionCompany => new ProductionCompanyDto
            {
                Id = productionCompany.Id,
                Name = productionCompany.Name,
                Logo = productionCompany.LogoPath,
                OriginCountry = productionCompany.OriginCountry
            });
            SetMapping<ProductionCountry, ProductionCountryDto>(productionCountry => new ProductionCountryDto
            {
                Iso31661 = productionCountry.Iso3166_1,
                Name = productionCountry.Name
            });
            SetMapping<SpokenLanguage, SpokenLanguageDto>(spokenLanguage => new SpokenLanguageDto
            {
                Iso6391 = spokenLanguage.Iso639_1,
                Name = spokenLanguage.Name
            });
            SetMapping<MovieCollection, MovieCollectionDto>(movieCollection => new MovieCollectionDto
            {
                Id = movieCollection.Id,
                Name = movieCollection.Name,
                Backdrop = movieCollection.BackdropPath,
                Poster = movieCollection.PosterPath,
            });
            SetMapping<Network, NetworkDto>(network => new NetworkDto
            {
                Name = network.Name,
                Logo = network.LogoPath,
                OriginCountry = network.OriginCountry
            });
            SetMapping<Creator, CreatorDto>(creator => new CreatorDto
            {
                Name = creator.Name
            });


            SetMapping<User, UserDto>(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                IsAdmin = user.Admin,
                Email = user.Email,
                PasswordChanged = user.HasChangedPassword,
                Libraries = user.LibraryAccessIds
            });
            SetMapping<Library, LibraryDto>(library => new LibraryDto
            {
                Id = library.Id,
                Name = library.Name,
                Language = library.Language,
                Kind = library.Kind.ToString().ToLowerInvariant()
            });
            SetMapping<Library, AdminLibraryDto>(library => new AdminLibraryDto
            {
                Id = library.Id,
                Name = library.Name,
                Language = library.Language,
                Kind = library.Kind.ToString().ToLowerInvariant(),
                Path = library.Path
            });
            SetMapping<UserSession, UserSessionDto>(userSession => new UserSessionDto
            {
                Id = userSession.Id,
                UserAgent = userSession.UserAgent,
                Expiration = userSession.Expiration
            });
        }
    }
}