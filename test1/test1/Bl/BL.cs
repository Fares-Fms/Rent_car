namespace test1.Bl
{
    public class BL
    {
        public async Task<string> TryUpload(List<IFormFile>? files, string PathName, string currentValue)
        {
            if (files != null && files.Any(f => f.Length > 0))
            {
                return await UploadImage(files, PathName);
            }
            return currentValue;
        }

        public async Task<string> UploadImage(List<IFormFile> files, string PathName)
        {
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", PathName, imageName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return imageName;
                }
            }
            return "";
        }
    }
}
