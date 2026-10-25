using Microsoft.EntityFrameworkCore;
using ProjectShihab.Data;
using ProjectShihab.Entities;
using ProjectShihab.Shared;

namespace ProjectShihab.Repos
{
    public class MaterialRepo(AsDbContext context, LogRepo logRepo)
    {
        public Result<List<Material>> GetForOfferedCourse(int offeredCourseId)
        {
            try
            {
                return Result<List<Material>>.Ok(context.Materials.Include(e => e.Uploader)
                    .Where(e => e.OfferedCourseId == offeredCourseId).OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Material>>.Fail(e.Message);
            }
        }

        public Result<List<Material>> GetForTraining(int trainingOfferingId)
        {
            try
            {
                return Result<List<Material>>.Ok(context.Materials.Include(e => e.Uploader)
                    .Where(e => e.TrainingOfferingId == trainingOfferingId).OrderByDescending(e => e.Id).ToList());
            }
            catch (Exception e)
            {
                return Result<List<Material>>.Fail(e.Message);
            }
        }

        public Result<Material> GetById(int id)
        {
            try
            {
                return Result<Material>.Ok(context.Materials.Find(id));
            }
            catch (Exception e)
            {
                return Result<Material>.Fail(e.Message);
            }
        }

        // Access rule: admin, the owner of the offering, or an enrolled/paid user.
        public bool CanAccess(Material material, int userId, string role)
        {
            if (role == Roles.Admin || material.UploadedBy == userId)
                return true;

            if (material.OfferedCourseId != null)
            {
                var offered = context.OfferedCourses.Find(material.OfferedCourseId);
                return offered?.TutorId == userId
                    || context.CourseEnrollments.Any(e => e.OfferedCourseId == material.OfferedCourseId && e.StudentId == userId);
            }

            var training = context.TrainingOfferings.Find(material.TrainingOfferingId);
            return training?.TrainerId == userId
                || context.TrainingEnrollments.Any(e => e.TrainingOfferingId == material.TrainingOfferingId && e.TutorId == userId);
        }

        // Verifies the uploader owns the target offering, then stores the row. File handling lives in the web layer.
        public Result<Material> Save(Material model, int userId)
        {
            try
            {
                if (model.OfferedCourseId != null)
                {
                    var offered = context.OfferedCourses.Find(model.OfferedCourseId);
                    if (offered == null || offered.TutorId != userId)
                        return Result<Material>.Fail("You can only add materials to your own course.");
                }
                else if (model.TrainingOfferingId != null)
                {
                    var training = context.TrainingOfferings.Find(model.TrainingOfferingId);
                    if (training == null || training.TrainerId != userId)
                        return Result<Material>.Fail("You can only add materials to your own training.");
                }
                else
                {
                    return Result<Material>.Fail("No target selected.");
                }

                model.UploadedBy = userId;
                model.UploadedAt = DateTime.Now;
                context.Materials.Add(model);
                context.SaveChanges();

                logRepo.Add("Upload Material", "Material", model.Id, $"{model.Title} ({model.FileType})");
                return Result<Material>.Ok(model);
            }
            catch (Exception e)
            {
                return Result<Material>.Fail(e.Message);
            }
        }

        public Result<Material> Delete(int id, int userId)
        {
            try
            {
                var entity = context.Materials.Find(id);
                if (entity == null || entity.UploadedBy != userId)
                    return Result<Material>.Fail("Material not found.");

                context.Materials.Remove(entity);
                context.SaveChanges();
                logRepo.Add("Delete Material", "Material", id, entity.Title);
                return Result<Material>.Ok(entity);
            }
            catch (Exception e)
            {
                return Result<Material>.Fail(e.Message);
            }
        }
    }
}
