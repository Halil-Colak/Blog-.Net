using Microsoft.AspNetCore.Http;

namespace Blog.Helper;
public static class ImageSave
{
    public static string SaveImagetoFolder(IFormFile image)
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string fileExtension = Path.GetExtension(image.FileName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(image.FileName);

        string seoFriendlyName = SeoUrl.Url(fileNameWithoutExtension).Replace("-", "");

        string newFileName = $"{seoFriendlyName}_{Guid.NewGuid()}{fileExtension}";

        string filePath = Path.Combine(directoryPath, newFileName);

        using (FileStream stream = new(filePath, FileMode.Create))
        {
            image.CopyTo(stream);
        }

        return $"/upload/{newFileName}";
    }
}

