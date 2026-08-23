namespace MichiMarket.Services
{
    public class ImagenService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly string[] TiposPermitidos = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxTamanoBytes = 5 * 1024 * 1024;

        public ImagenService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SubirImagen(IFormFile? archivo, string subcarpeta)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!TiposPermitidos.Contains(extension))
                throw new InvalidOperationException("Tipo de archivo no valido. Solo se permiten JPG, PNG y WebP.");

            if (archivo.Length > MaxTamanoBytes)
                throw new InvalidOperationException("La imagen no puede superar 5MB.");

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCarpeta = Path.Combine(_env.WebRootPath, "img", subcarpeta);
            Directory.CreateDirectory(rutaCarpeta);

            var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"img/{subcarpeta}/{nombreArchivo}";
        }

        public void EliminarImagen(string? rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa))
                return;

            var rutaCompleta = Path.Combine(_env.WebRootPath, rutaRelativa.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(rutaCompleta))
                File.Delete(rutaCompleta);
        }
    }
}
