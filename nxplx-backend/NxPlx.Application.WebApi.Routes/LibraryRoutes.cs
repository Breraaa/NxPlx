using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NxPlx.Core.Services;
using NxPlx.Infrastructure.Session;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class LibraryRoutes
    {
        public static void BindHandlers(IRouter router)
        {
            router.Get("/list", Authenticated.User, ListLibraries);
            router.Post("", Authenticated.Admin, CreateLibrary);
            router.Delete("", Authenticated.Admin, RemoveLibrary);
            router.Get("/permissions", Validated.RequireUserIdQuery, Authenticated.Admin, GetUserLibraryPermissions);
            router.Put("/permissions", Validated.SetUserPermissionsForm, Authenticated.Admin, SetUserLibraryPermissions);
            router.Get("/browse", Authenticated.Admin, BrowseForDirectory);
        }


        private static Task<HandlerType> BrowseForDirectory(Request req, Response res)
        {
            string cwd = req.Queries["cwd"];

            var entries = LibraryService.GetDirectoryEntries(cwd);

            return res.SendJson(entries);
        }


        private static async Task<HandlerType> CreateLibrary(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();

            var lib = await LibraryService.CreateNewLibrary(form!["name"], form!["path"], form!["language"], form!["kind"]);

            return await res.SendJson(lib);
        }


        private static async Task<HandlerType> RemoveLibrary(Request req, Response res)
        {
            var libraryId = await req.ParseBodyAsync<JsonValue<int>>();

            var ok = await LibraryService.RemoveLibrary(libraryId!.value);

            if (!ok) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendStatus(HttpStatusCode.OK);
        }


        private static async Task<HandlerType> ListLibraries(Request req, Response res)
        {
            var session = req.GetData<UserSession>();

            var libraries = await LibraryService.ListLibraries(session!.User);

            return await res.SendJson(libraries);
        }

        private static async Task<HandlerType> SetUserLibraryPermissions(Request req, Response res)
        {
            var form = await req.GetFormDataAsync();
            var userId = int.Parse(form!["userId"]);
            var libraryIds = form!["libraries"].Select(int.Parse).ToList();

            var ok = await LibraryService.SetUserLibraryPermissions(userId, libraryIds);

            if (!ok) return await res.SendStatus(HttpStatusCode.BadRequest);
            return await res.SendStatus(HttpStatusCode.OK);
        }

        private static async Task<HandlerType> GetUserLibraryPermissions(Request req, Response res)
        {
            var userId = int.Parse(req.Queries["userId"]);
            var libraryAccess = await LibraryService.FindLibraryAccess(userId);
            if (libraryAccess == default) return await res.SendStatus(HttpStatusCode.NotFound);
            return await res.SendJson(libraryAccess);
        }
    }
}