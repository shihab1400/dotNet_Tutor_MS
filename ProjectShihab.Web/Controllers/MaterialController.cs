using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Models;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize]
    public class MaterialController(MaterialRepo repo, OfferedCourseRepo offeredCourseRepo, TrainingRepo trainingRepo,
        CurrentUserHelper currentUser, IWebHostEnvironment env) : Controller
    {
        private const long MaxBytes = 100L * 1024 * 1024;
        private static readonly Dictionary<string, string> Types = new(StringComparer.OrdinalIgnoreCase)
        {
            [".mp4"] = "Video", [".mkv"] = "Video", [".webm"] = "Video", [".mov"] = "Video",
            [".pdf"] = "PDF",
            [".ppt"] = "PPT", [".pptx"] = "PPT",
            [".doc"] = "Document", [".docx"] = "Document", [".txt"] = "Document",
            [".zip"] = "Archive"
        };

        // kind = Course (OfferedCourse id) or Training (TrainingOffering id)
        public IActionResult Index(string kind, int id)
        {
            var isCourse = kind == PaymentKinds.Course;
            if (!isCourse && kind != PaymentKinds.Training)
                return RedirectToAction("Index", "Dashboard");

            var fake = new Material { OfferedCourseId = isCourse ? id : null, TrainingOfferingId = isCourse ? null : id };
            if (!repo.CanAccess(fake, currentUser.UserId, currentUser.Role))
                return RedirectToAction("Denied", "Auth");

            var result = isCourse ? repo.GetForOfferedCourse(id) : repo.GetForTraining(id);
            if (result.HasError)
                ViewBag.Error = result.Message;

            ViewBag.Kind = kind;
            ViewBag.TargetId = id;
            ViewBag.Title = isCourse
                ? offeredCourseRepo.GetById(id).Data?.Course?.Subject
                : trainingRepo.GetOffering(id).Data?.Course?.Subject;
            ViewBag.CanUpload = (isCourse && currentUser.Role == Roles.Tutor && offeredCourseRepo.GetById(id).Data?.TutorId == currentUser.UserId)
                || (!isCourse && currentUser.Role == Roles.Trainer && trainingRepo.GetOffering(id).Data?.TrainerId == currentUser.UserId);
            return View(result.Data ?? []);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Tutor},{Roles.Trainer}")]
        [RequestSizeLimit(MaxBytes)]
        public async Task<IActionResult> Upload(MaterialUploadModel model, IFormFile? file)
        {
            var back = RedirectToAction("Index", new { kind = model.Kind, id = model.TargetId });
            if (!ModelState.IsValid || file == null || file.Length == 0)
            {
                TempData["Error"] = "Title and a file are required.";
                return back;
            }
            if (file.Length > MaxBytes)
            {
                TempData["Error"] = "File is too large (max 100 MB).";
                return back;
            }

            var ext = Path.GetExtension(file.FileName);
            if (!Types.TryGetValue(ext, out var fileType))
            {
                TempData["Error"] = "Unsupported file type.";
                return back;
            }

            var isCourse = model.Kind == PaymentKinds.Course;
            if (isCourse != (currentUser.Role == Roles.Tutor))
                return RedirectToAction("Denied", "Auth");

            var folder = Path.Combine(env.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);
            var storedName = Guid.NewGuid().ToString("N") + ext.ToLower();
            var material = new Material
            {
                Title = model.Title,
                FileType = fileType,
                FilePath = storedName,
                OriginalName = Path.GetFileName(file.FileName),
                OfferedCourseId = isCourse ? model.TargetId : null,
                TrainingOfferingId = isCourse ? null : model.TargetId
            };

            var result = repo.Save(material, currentUser.UserId);
            if (result.HasError)
            {
                TempData["Error"] = result.Message;
                return back;
            }

            await using (var stream = System.IO.File.Create(Path.Combine(folder, storedName)))
                await file.CopyToAsync(stream);

            TempData["Success"] = "Material uploaded.";
            return back;
        }

        // Files are served through the controller so access can be checked (not as public static files).
        public IActionResult Download(int id)
        {
            var material = repo.GetById(id).Data;
            if (material == null || !repo.CanAccess(material, currentUser.UserId, currentUser.Role))
                return RedirectToAction("Denied", "Auth");

            var path = Path.Combine(env.WebRootPath, "uploads", Path.GetFileName(material.FilePath));
            if (!System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(path, "application/octet-stream", material.OriginalName ?? material.FilePath);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Tutor},{Roles.Trainer}")]
        public IActionResult Delete(int id, string kind, int targetId)
        {
            var material = repo.GetById(id).Data;
            var result = repo.Delete(id, currentUser.UserId);
            if (result.HasError)
                TempData["Error"] = result.Message;
            else
            {
                TempData["Success"] = "Material deleted.";
                if (material != null)
                {
                    var path = Path.Combine(env.WebRootPath, "uploads", Path.GetFileName(material.FilePath));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
            }
            return RedirectToAction("Index", new { kind, id = targetId });
        }
    }
}
