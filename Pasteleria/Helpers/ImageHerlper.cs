using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Pasteleria.Helpers
{
    // Helper para procesamiento de imágenes
    public static class ImageHelper
    {
        // Configuración de tamaños
        private const int THUMBNAIL_WIDTH = 300;
        private const int THUMBNAIL_HEIGHT = 300;
        private const int THUMBNAIL_QUALITY = 85;

        private const int IMAGEN_MAX_WIDTH = 1200;
        private const int IMAGEN_MAX_HEIGHT = 1200;
        private const int IMAGEN_QUALITY = 90;

        private const int MIN_WIDTH = 200;
        private const int MIN_HEIGHT = 200;

        public static byte[] GenerarThumbnail(byte[] imagenOriginal)
        {
            if (imagenOriginal == null || imagenOriginal.Length == 0)
                return null;

            try
            {
                using (var inputStream = new MemoryStream(imagenOriginal))
                using (var outputStream = new MemoryStream())
                using (var image = Image.Load(inputStream))
                {
                    // Calcular nuevas dimensiones manteniendo proporción
                    var ratioX = (double)THUMBNAIL_WIDTH / image.Width;
                    var ratioY = (double)THUMBNAIL_HEIGHT / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    // Redimensionar
                    image.Mutate(x => x.Resize(newWidth, newHeight));

                    // Guardar como JPEG
                    var encoder = new JpegEncoder { Quality = THUMBNAIL_QUALITY };
                    image.Save(outputStream, encoder);

                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar thumbnail: {ex.Message}", ex);
            }
        }

        public static byte[] OptimizarImagen(byte[] imagenOriginal)
        {
            if (imagenOriginal == null || imagenOriginal.Length == 0)
                return null;

            try
            {
                using (var inputStream = new MemoryStream(imagenOriginal))
                using (var outputStream = new MemoryStream())
                using (var image = Image.Load(inputStream))
                {
                    // Solo redimensionar si excede el tamaño máximo
                    if (image.Width > IMAGEN_MAX_WIDTH || image.Height > IMAGEN_MAX_HEIGHT)
                    {
                        var ratioX = (double)IMAGEN_MAX_WIDTH / image.Width;
                        var ratioY = (double)IMAGEN_MAX_HEIGHT / image.Height;
                        var ratio = Math.Min(ratioX, ratioY);

                        var newWidth = (int)(image.Width * ratio);
                        var newHeight = (int)(image.Height * ratio);

                        image.Mutate(x => x.Resize(newWidth, newHeight));
                    }

                    // Guardar optimizada
                    var encoder = new JpegEncoder { Quality = IMAGEN_QUALITY };
                    image.Save(outputStream, encoder);

                    return outputStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al optimizar imagen: {ex.Message}", ex);
            }
        }

        public static bool EsImagenValida(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return false;

            try
            {
                using (var stream = new MemoryStream(imageBytes))
                using (var image = Image.Load(stream))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static (int width, int height) ObtenerDimensiones(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return (0, 0);

            try
            {
                using (var stream = new MemoryStream(imageBytes))
                using (var image = Image.Load(stream))
                {
                    return (image.Width, image.Height);
                }
            }
            catch
            {
                return (0, 0);
            }
        }

        public static bool CumpleDimensionesMinimas(byte[] imageBytes, out string mensaje)
        {
            var (width, height) = ObtenerDimensiones(imageBytes);

            if (width < MIN_WIDTH || height < MIN_HEIGHT)
            {
                mensaje = $"La imagen debe tener al menos {MIN_WIDTH}x{MIN_HEIGHT} píxeles. Dimensiones actuales: {width}x{height}";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

        public static double ObtenerTamañoEnKB(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return 0;

            return imageBytes.Length / 1024.0;
        }

        public static double ObtenerTamañoEnMB(byte[] imageBytes)
        {
            return ObtenerTamañoEnKB(imageBytes) / 1024.0;
        }
    }
}