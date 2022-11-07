using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    // Через данный механизм будем загружать файлы в систему
    public class AttachController : ControllerBase
    {

        // Пост запрос Загрузки n-го количества файлов
        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var res = new List<MetadataModel>();
            foreach (var file in files)
            {
                res.Add(await UploadFile(file));
            }
            return res;
        }

        // Функция загрузки одного файла 
        private async Task<MetadataModel> UploadFile(IFormFile file)
        {
            var tempPath = Path.GetTempPath();// папка временных файлов
            
            var meta = new MetadataModel // Мета информация о файле
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length,
            };
            
            var newPath = Path.Combine(tempPath, meta.TempId.ToString()); 
            // создаем файлИНФ c информацией о загруженном файле
            var fileinfo = new FileInfo(newPath);
            // проверяем есть ли такой файлИНФ 
            if (fileinfo.Exists)
            {
                throw new Exception("file exist");
            }
            else
            {
                if (fileinfo.Directory == null)
                {
                    throw new Exception("temp is null");
                }
                else
                if (!fileinfo.Directory.Exists)
                {
                    fileinfo.Directory?.Create();// создаем директорию если ее нет
                }

                using (var stream = System.IO.File.Create(newPath)) // используем поток для записи файла
                {
                    await file.CopyToAsync(stream);
                }
                return meta;
            }
        }
    }
}
