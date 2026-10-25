using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectShihab.Entities;
using ProjectShihab.Repos;
using ProjectShihab.Shared;

namespace ProjectShihab.Web.Controllers
{
    [Authorize(Roles = $"{Roles.Admin},{Roles.Trainer},{Roles.Tutor}")]
    public class CertificateController(TrainingRepo repo, CurrentUserHelper currentUser) : Controller
    {
        public IActionResult Index()
        {
            var result = currentUser.Role switch
            {
                Roles.Tutor => repo.GetCertificates(tutorId: currentUser.UserId),
                Roles.Trainer => repo.GetCertificates(trainerId: currentUser.UserId),
                _ => repo.GetCertificates()
            };
            if (result.HasError)
                ViewBag.Error = result.Message;
            return View(result.Data ?? []);
        }

        public IActionResult Details(int id)
        {
            var cert = repo.GetCertificate(id).Data;
            var allowed = cert != null && (currentUser.Role == Roles.Admin
                || cert.TutorId == currentUser.UserId || cert.TrainerId == currentUser.UserId);
            if (!allowed)
                return RedirectToAction("Index");
            return View(cert);
        }
    }
}
