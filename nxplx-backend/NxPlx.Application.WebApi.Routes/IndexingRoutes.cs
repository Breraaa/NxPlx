using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NxPlx.Abstractions;
using NxPlx.Abstractions.Database;
using NxPlx.Infrastructure.IoC;
using NxPlx.Models;
using Red;
using Red.Extensions;
using Red.Interfaces;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class IndexingRoutes
    {
        public static void BindHandlers(IRouter router)
        {
            router.Post("", Authenticated.Admin, IndexLibraries);
        }

        private static async Task<HandlerType> IndexLibraries(Request req, Response res)
        {
            var container = ResolveContainer.Default;

            var libIds = await req.ParseBodyAsync<JsonValue<int[]>>();
            
            ICollection<Library> libraries;
            await using (var ctx = container.Resolve<IReadNxplxContext>())
            {
                libraries = await ctx.Libraries.Many(l => libIds!.value.Contains(l.Id)).ToListAsync();
            }

            await res.SendStatus(HttpStatusCode.OK);
            await IndexAndBroadcast(container, libIds!.value, libraries);
            return HandlerType.Final;
        }

        private static async Task IndexAndBroadcast(ResolveContainer container, int[] libIds, ICollection<Library> libraries)
        {
            var indexer = container.Resolve<IIndexer>();
            var broadcaster = container.Resolve<IBroadcaster>();
            
            var names = libraries.Select(l => l.Name).ToArray();
            await broadcaster.BroadcastAll(new Broadcast<string[]>("indexing:started", names));
            await indexer.IndexLibraries(libraries);
            await broadcaster.BroadcastAll(new Broadcast<string[]>("indexing:completed", names));
        }
    }
}