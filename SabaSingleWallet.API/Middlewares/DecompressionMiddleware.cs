using System.IO.Compression;
using System.Text;

namespace SabaSingleWallet.API.Middlewares
{
	public class DecompressionMiddleware
	{
		private readonly RequestDelegate _next;

		public DecompressionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.Request.Method == HttpMethods.Post &&
			  context.Request.Headers["Content-Encoding"].Any(h => h.Contains("gzip")))
			{
				var decompressedStream = new MemoryStream();

				using (var gzipStream = new GZipStream(context.Request.Body, CompressionMode.Decompress))
				{
					await gzipStream.CopyToAsync(decompressedStream);
				}

				decompressedStream.Seek(0, SeekOrigin.Begin);

				var stream = new MemoryStream();

				await decompressedStream.CopyToAsync(stream);

				Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));

				context.Request.Body = decompressedStream;
				context.Request.ContentLength = decompressedStream.Length;
			}

			await _next(context);
		}
	}
}