using System.IO;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NxPlx.Services.Database;
using Red.Interfaces;

namespace NxPlx.WebApi.Routers
{
    public class ImageRouter
    {
        public static void Register(IRouter router)
        {
            router.Get("/:filename", async (req, res) =>
            {
                var filename = req.Context.ExtractUrlParameter("filename");
                var imagepath = Configuration.ConfigurationService.Current.ImagesFolder;
                var fullpath = Path.Combine(imagepath, filename);

                return await res.SendFile(fullpath, handleRanges: false);
            });
        }
    }
}