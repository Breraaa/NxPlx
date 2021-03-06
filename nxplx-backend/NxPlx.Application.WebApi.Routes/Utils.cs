﻿using System.IO;
using System.Net;
using System.Threading.Tasks;
using Red;

namespace NxPlx.Infrastructure.WebApi.Routes
{
    public static class Utils
    {
        private const int FileMaxAge = 60 * 60 * 24 * 365;

        public static Task<HandlerType> OldSendSPA(Request req, Response res)
        {
            var file = Path.Combine("public", "index.html");
            
            if (!File.Exists(file))
                return res.SendStatus(HttpStatusCode.NotFound);
            
            return res.SendFile(file);
        }
        public static Task<HandlerType> SendSPA(Request req, Response res)
        {
            var relativePath = req.AspNetRequest.Path.ToString().TrimStart('/');
            if (relativePath.StartsWith("api")) 
                return res.SendStatus(HttpStatusCode.NotFound);
            
            if (relativePath == "") relativePath = "index.html";
            var basePath = Path.GetFullPath("public");
            var fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath));

            if (!fullPath.StartsWith(basePath))
                return res.SendStatus(HttpStatusCode.NotFound);

            if (!File.Exists(fullPath))
                fullPath = Path.Combine(basePath, "index.html");
            
            /*if (relativePath != "index.html")
                res.AddHeader("Cache-Control", $"max-age={FileMaxAge}");*/
            
            return res.SendFile(fullPath);
        }

    }
}