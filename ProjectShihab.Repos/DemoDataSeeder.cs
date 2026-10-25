using System.Text;
using Microsoft.AspNetCore.Identity;
using ProjectShihab.Data;
using ProjectShihab.Entities;

namespace ProjectShihab.Repos
{
    // Fills an empty database with realistic demo data. Every demo account uses the password below.
    public static class DemoDataSeeder
    {
        public const string DemoPassword = "Pass@123";

        public static void Seed(AsDbContext db, string uploadsFolder)
        {
            // Marker account: once the demo trainer exists the demo data has been seeded.
            if (db.Users.Any(u => u.Email == "kamrul.hasan@trainer.com") || db.Groups.Any(g => g.Title == "Class 10"))
                return;

            var rnd = new Random(2026);
            var today = DateTime.Today;
            var hasher = new PasswordHasher<User>();
            var admin = db.Users.First(u => u.Role == Roles.Admin);

            void Log(User? u, string action, string? entity, int? id, string? details, DateTime when) =>
                db.Logs.Add(new Log
                {
                    UserId = u?.Id, UserName = u?.UserName ?? "-", Role = u?.Role ?? "Guest", Action = action,
                    Entity = entity, EntityId = id, Details = details, IpAddress = "127.0.0.1", CreatedAt = when
                });

            DateTime At(DateTime day)
            {
                var t = day.Date.AddHours(rnd.Next(9, 22)).AddMinutes(rnd.Next(60));
                return t > DateTime.Now ? t.AddDays(-1) : t;
            }

            // ---------- extra groups & courses ----------
            var class10 = new Group { Title = "Class 10" };
            db.Groups.Add(class10);
            db.SaveChanges();
            var groupId = db.Groups.ToDictionary(g => g.Title, g => g.Id);
            (string G, string S, decimal P)[] extraCourses =
            [
                ("Class 8", "General Science", 1400), ("Class 9", "Biology", 1800), ("Class 9", "Higher Mathematics", 2000),
                ("Class 10", "Physics", 2000), ("Class 10", "ICT", 1500), ("Class 10", "English Grammar", 1300),
                ("CSE", "Data Structures", 3200), ("CSE", "Object Oriented Programming", 3000), ("CSE", "Web Technologies", 3400),
                ("EEE", "Signals and Systems", 3300), ("EEE", "Power Systems", 3500)
            ];
            foreach (var c in extraCourses)
                db.Courses.Add(new Course { GroupId = groupId[c.G], Subject = c.S, BasePrice = c.P, CreatedBy = admin.Id, CreatedAt = today.AddDays(-120) });
            db.SaveChanges();
            var courses = db.Courses.ToList();

            // ---------- users ----------
            User MakeUser(string name, string role, string gender, int daysAgo, bool active = true)
            {
                var email = name.ToLower().Replace("dr. ", "").Replace(".", "").Replace(" ", ".") + "@" + (role == Roles.Student ? "student" : role.ToLower()) + ".com";
                var u = new User
                {
                    UserName = name, Email = email, Gender = gender, Role = role, Status = active,
                    ContactNumber = "01" + rnd.Next(3, 10) + rnd.Next(10000000, 99999999),
                    Dob = role == Roles.Student ? today.AddYears(-rnd.Next(13, 24)).AddDays(-rnd.Next(365)) : today.AddYears(-rnd.Next(25, 45)).AddDays(-rnd.Next(365)),
                    Address = new[] { "Dhanmondi, Dhaka", "Uttara, Dhaka", "Mirpur, Dhaka", "Banani, Dhaka", "Agrabad, Chattogram", "Zindabazar, Sylhet", "Shaheb Bazar, Rajshahi", "Khulna Sadar, Khulna" }[rnd.Next(8)],
                    CreatedAt = At(today.AddDays(-daysAgo))
                };
                u.PasswordHash = hasher.HashPassword(u, DemoPassword);
                db.Users.Add(u);
                return u;
            }

            var trainers = new List<User>
            {
                MakeUser("Dr. Kamrul Hasan", Roles.Trainer, "Male", 115),
                MakeUser("Nusrat Jahan", Roles.Trainer, "Female", 112),
                MakeUser("Tanvir Ahmed", Roles.Trainer, "Male", 110)
            };
            string[] tutorNames = ["Rafiq Islam", "Sadia Rahman", "Mahmudul Hasan", "Farzana Akter", "Arif Chowdhury", "Tasnim Hossain", "Imran Kabir", "Lamia Sultana"];
            var tutors = tutorNames.Select((n, i) => MakeUser(n, Roles.Tutor, i % 2 == 0 ? "Male" : "Female", 100 - i * 2)).ToList();
            string[] studentNames = ["Ayan Mahmud", "Nabila Haque", "Rakib Hossain", "Mehjabin Chowdhury", "Shakib Ahmed", "Anika Tabassum", "Fahim Rahman", "Sumaiya Islam",
                                     "Tahsin Karim", "Jannatul Ferdous", "Rifat Hasan", "Maliha Noor", "Zarif Ahsan", "Priya Saha", "Nafis Iqbal", "Raisa Anjum"];
            var students = studentNames.Select((n, i) => MakeUser(n, Roles.Student, i % 2 == 0 ? "Male" : "Female", 80 - i * 3, active: i != 15)).ToList();
            db.SaveChanges();

            Log(admin, "Login", "User", admin.Id, null, At(today.AddDays(-116)));
            foreach (var t in trainers)
                Log(admin, "Create Trainer", "User", t.Id, t.Email, t.CreatedAt);
            foreach (var u in tutors.Concat(students))
                Log(u, "Register", "User", u.Id, $"Role: {u.Role}", u.CreatedAt);
            Log(admin, "Deactivate User", "User", students[15].Id, students[15].Email, At(today.AddDays(-5)));

            // ---------- coupons ----------
            var coupons = new List<Coupon>
            {
                new() { Code = "WELCOME10", Type = CouponTypes.Percentage, Value = 10, ExpiryDate = today.AddMonths(6), MaxUses = 100 },
                new() { Code = "EID25", Type = CouponTypes.Percentage, Value = 25, ExpiryDate = today.AddMonths(1), MaxUses = 30 },
                new() { Code = "SAVE500", Type = CouponTypes.Fixed, Value = 500, ExpiryDate = today.AddMonths(3), MaxUses = 50 },
                new() { Code = "FLAT200", Type = CouponTypes.Fixed, Value = 200, ExpiryDate = today.AddMonths(2), MaxUses = 3 },
                new() { Code = "SUMMER24", Type = CouponTypes.Percentage, Value = 30, ExpiryDate = today.AddDays(-20), MaxUses = 40 },
                new() { Code = "TEACHER15", Type = CouponTypes.Percentage, Value = 15, ExpiryDate = today.AddMonths(4), MaxUses = 20 },
                new() { Code = "OLDPROMO", Type = CouponTypes.Fixed, Value = 300, ExpiryDate = today.AddMonths(1), MaxUses = 10, IsActive = false }
            };
            db.Coupons.AddRange(coupons);
            db.SaveChanges();
            foreach (var c in coupons)
                Log(admin, "Create Coupon", "Coupon", c.Id, c.Code, At(today.AddDays(-105)));
            var usableForTutors = coupons.Where(c => c.Code is "TEACHER15" or "WELCOME10" or "SAVE500").ToList();
            var usableForStudents = coupons.Where(c => c.Code is "WELCOME10" or "EID25" or "SAVE500" or "FLAT200").ToList();

            Payment Pay(User u, string kind, decimal price, List<Coupon> pool, DateTime when)
            {
                Coupon? coupon = null;
                if (rnd.NextDouble() < 0.45)
                {
                    var c = pool[rnd.Next(pool.Count)];
                    if (c.UsedCount < c.MaxUses) coupon = c;
                }
                var discount = coupon == null ? 0 : Math.Min(price, coupon.Type == CouponTypes.Percentage ? Math.Round(price * coupon.Value / 100, 2) : coupon.Value);
                if (coupon != null) coupon.UsedCount++;
                var p = new Payment
                {
                    User = u, Kind = kind, OriginalAmount = price, DiscountAmount = discount, FinalAmount = price - discount, Coupon = coupon,
                    TransactionRef = "TXN-" + Guid.NewGuid().ToString("N")[..12].ToUpper(), PaidAt = when
                };
                db.Payments.Add(p);
                return p;
            }

            // ---------- trainings (every course gets one, round-robin over trainers) ----------
            var trainings = courses.Select((c, i) => new TrainingOffering
            {
                Course = c, Trainer = trainers[i % trainers.Count], Price = Math.Round(c.BasePrice * 1.2m / 100) * 100,
                StartDate = today.AddDays(-95), EndDate = today.AddDays(rnd.Next(45, 120)), Status = i % 9 != 8
            }).ToList();
            db.TrainingOfferings.AddRange(trainings);
            db.SaveChanges();
            foreach (var t in trainings)
                Log(t.Trainer, "Create Training", "TrainingOffering", t.Id, $"CourseId: {t.CourseId}, Price: {t.Price}", At(today.AddDays(-95)));

            // ---------- tutors get trained & certified ----------
            var certified = new List<(User Tutor, Course Course)>();
            var trainingEnrollments = new List<(TrainingEnrollment E, Payment P)>();
            foreach (var (tutor, ti) in tutors.Select((t, i) => (t, i)))
            {
                var picks = trainings.Where(t => t.Status).OrderBy(_ => rnd.Next()).Take(2 + ti % 3).ToList();
                foreach (var (training, k) in picks.Select((t, k) => (t, k)))
                {
                    var when = At(today.AddDays(-90 + ti * 3 + k * 4));
                    var payment = Pay(tutor, PaymentKinds.Training, training.Price, usableForTutors, when);
                    var done = k < picks.Count - 1 || ti % 3 != 0; // most finish, a few are still in progress
                    var e = new TrainingEnrollment
                    {
                        Tutor = tutor, TrainingOffering = training, Payment = payment, EnrolledAt = when,
                        Status = done ? EnrollmentStatus.Completed : EnrollmentStatus.Enrolled,
                        CompletedAt = done ? when.AddDays(rnd.Next(14, 25)) : null
                    };
                    db.TrainingEnrollments.Add(e);
                    trainingEnrollments.Add((e, payment));
                }
            }
            db.SaveChanges();

            foreach (var (e, p) in trainingEnrollments)
            {
                p.TargetId = e.TrainingOfferingId;
                Log(e.Tutor, "Enroll Training", "TrainingEnrollment", e.Id, $"Paid {p.FinalAmount:0.00} (txn {p.TransactionRef})", e.EnrolledAt);
                if (e.Status != EnrollmentStatus.Completed) continue;
                var trainer = e.TrainingOffering!.Trainer!;
                Log(trainer, "Complete Training", "TrainingEnrollment", e.Id, $"TutorId: {e.TutorId}", e.CompletedAt!.Value);
                var cert = new Certificate
                {
                    TrainingEnrollment = e, Tutor = e.Tutor, Trainer = trainer, Course = e.TrainingOffering.Course,
                    CertificateNo = "CERT-" + Guid.NewGuid().ToString("N")[..10].ToUpper(), IssuedAt = e.CompletedAt.Value.AddHours(2)
                };
                db.Certificates.Add(cert);
                certified.Add((e.Tutor!, e.TrainingOffering.Course!));
            }
            db.SaveChanges();
            foreach (var cert in db.Certificates.ToList())
                Log(trainers.First(t => t.Id == cert.TrainerId), "Issue Certificate", "Certificate", cert.Id, $"{cert.CertificateNo} to TutorId {cert.TutorId}", cert.IssuedAt);

            // ---------- tutors offer the courses they are certified for ----------
            var offered = certified.Select((x, i) => new OfferedCourse
            {
                Course = x.Course, User = x.Tutor, Price = Math.Round(x.Course.BasePrice * (0.9m + (decimal)rnd.NextDouble() * 0.5m) / 50) * 50,
                StartDate = today.AddDays(-60 + rnd.Next(10)), EndDate = today.AddDays(rnd.Next(30, 150)), Status = i % 11 != 10
            }).ToList();
            db.OfferedCourses.AddRange(offered);
            db.SaveChanges();
            foreach (var o in offered)
                Log(o.User, "Offer Course", "OfferedCourse", o.Id, $"CourseId: {o.CourseId}, Price: {o.Price}", At(today.AddDays(-60)));

            // ---------- materials (small real files) ----------
            Directory.CreateDirectory(uploadsFolder);
            void AddMaterial(User uploader, string title, string ext, string body, OfferedCourse? oc, TrainingOffering? to, DateTime when)
            {
                var stored = Guid.NewGuid().ToString("N") + ext;
                var bytes = ext == ".pdf" ? MinimalPdf(title, body) : Encoding.UTF8.GetBytes(title + Environment.NewLine + new string('=', title.Length) + Environment.NewLine + body);
                File.WriteAllBytes(Path.Combine(uploadsFolder, stored), bytes);
                var m = new Material
                {
                    Title = title, FileType = ext == ".pdf" ? "PDF" : "Document", FilePath = stored,
                    OriginalName = title.Replace(" ", "_").Replace(":", "") + ext, OfferedCourse = oc, TrainingOffering = to, Uploader = uploader, UploadedAt = when
                };
                db.Materials.Add(m);
            }
            foreach (var t in trainings)
            {
                AddMaterial(t.Trainer!, $"{t.Course!.Subject}: Trainer Handbook", ".pdf", $"Teaching guidelines and lesson plan for {t.Course.Subject}.", null, t, At(today.AddDays(-93)));
                AddMaterial(t.Trainer!, $"{t.Course.Subject}: Assessment Rubric", ".txt", "1. Concept clarity\n2. Communication\n3. Problem solving\n4. Class management", null, t, At(today.AddDays(-80)));
            }
            foreach (var o in offered)
            {
                AddMaterial(o.User!, $"{o.Course!.Subject} - Lecture 1 Slides", ".pdf", $"Introduction to {o.Course.Subject}. Topics, goals and reading list.", o, null, At(today.AddDays(-58)));
                AddMaterial(o.User!, $"{o.Course.Subject} - Practice Sheet", ".pdf", "Solve all ten problems before the next class.", o, null, At(today.AddDays(-40)));
                AddMaterial(o.User!, $"{o.Course.Subject} - Class Notes", ".txt", "Key formulas, definitions and worked examples from week 1-3.", o, null, At(today.AddDays(-30)));
            }
            db.SaveChanges();
            foreach (var m in db.Materials.ToList())
                Log(m.Uploader, "Upload Material", "Material", m.Id, $"{m.Title} ({m.FileType})", m.UploadedAt);

            // ---------- students enroll, finish and rate ----------
            string[] goodFeedback = ["Explains every topic very clearly.", "Excellent tutor, my grades improved a lot!", "Very patient and helpful.", "Great materials and practice sheets.", "Best teacher I have had so far."];
            string[] okFeedback = ["Good course, but the pace was a bit fast.", "Helpful overall, wanted more practice problems.", "Decent classes, sometimes started late."];
            var openCourses = offered.Where(o => o.Status).ToList();
            var studentEnrollments = new List<(CourseEnrollment E, Payment P)>();
            foreach (var (student, si) in students.Select((s, i) => (s, i)))
            {
                foreach (var (oc, k) in openCourses.OrderBy(_ => rnd.Next()).Take(2 + si % 3).Select((o, k) => (o, k)))
                {
                    var when = At(today.AddDays(-55 + si * 2 + k * 3));
                    var payment = Pay(student, PaymentKinds.Course, oc.Price, usableForStudents, when);
                    var done = rnd.NextDouble() < 0.55 && when < today.AddDays(-12);
                    var e = new CourseEnrollment
                    {
                        Student = student, OfferedCourse = oc, Payment = payment, EnrolledAt = when,
                        Status = done ? EnrollmentStatus.Completed : EnrollmentStatus.Enrolled,
                        CompletedAt = done ? when.AddDays(rnd.Next(10, 30)) : null
                    };
                    if (e.CompletedAt > today) e.CompletedAt = today.AddDays(-1);
                    db.CourseEnrollments.Add(e);
                    studentEnrollments.Add((e, payment));
                }
            }
            db.SaveChanges();

            foreach (var (e, p) in studentEnrollments)
            {
                p.TargetId = e.OfferedCourseId;
                Log(e.Student, "Enroll Course", "CourseEnrollment", e.Id, $"Paid {p.FinalAmount:0.00} (txn {p.TransactionRef})", e.EnrolledAt);
                if (e.Status != EnrollmentStatus.Completed) continue;
                Log(e.OfferedCourse!.User, "Complete Course", "CourseEnrollment", e.Id, $"StudentId: {e.StudentId}", e.CompletedAt!.Value);
                if (rnd.NextDouble() > 0.8) continue; // some students have not rated yet
                var stars = rnd.NextDouble() < 0.7 ? rnd.Next(4, 6) : rnd.Next(2, 4);
                var rating = new Rating
                {
                    CourseEnrollment = e, Student = e.Student, Tutor = e.OfferedCourse.User, OfferedCourse = e.OfferedCourse, RatingValue = stars,
                    Feedback = stars >= 4 ? goodFeedback[rnd.Next(goodFeedback.Length)] : okFeedback[rnd.Next(okFeedback.Length)],
                    CreatedAt = e.CompletedAt.Value.AddDays(rnd.Next(1, 4)) > today ? today : e.CompletedAt.Value.AddDays(rnd.Next(1, 4))
                };
                db.Ratings.Add(rating);
            }
            db.SaveChanges();
            foreach (var r in db.Ratings.ToList())
                Log(students.First(s => s.Id == r.StudentId), "Rate Course", "Rating", r.Id, $"{r.RatingValue} star(s) for OfferedCourseId {r.OfferedCourseId}", r.CreatedAt);

            // ---------- some login noise ----------
            foreach (var u in trainers.Concat(tutors).Concat(students.Take(15)))
                for (var i = 0; i < 3; i++)
                    Log(u, "Login", "User", u.Id, null, At(today.AddDays(-rnd.Next(0, 40))));
            Log(null, "Login Failed", "User", null, "Email: unknown@mail.com", At(today.AddDays(-3)));
            Log(students[15], "Login Blocked", "User", students[15].Id, "Account is deactivated", At(today.AddDays(-2)));
            db.SaveChanges();
        }

        // Builds a tiny but valid one-page PDF so seeded materials open in any viewer.
        private static byte[] MinimalPdf(string title, string body)
        {
            static string Esc(string s) => s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
            var content = $"BT /F1 20 Tf 60 760 Td ({Esc(title)}) Tj ET\nBT /F1 12 Tf 60 720 Td ({Esc(body)}) Tj ET\nBT /F1 10 Tf 60 60 Td (Amar School - demo material) Tj ET";
            string[] objects =
            [
                "<< /Type /Catalog /Pages 2 0 R >>",
                "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>",
                $"<< /Length {Encoding.ASCII.GetByteCount(content)} >>\nstream\n{content}\nendstream",
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
            ];
            var sb = new StringBuilder("%PDF-1.4\n");
            var offsets = new List<int>();
            for (var i = 0; i < objects.Length; i++)
            {
                offsets.Add(Encoding.ASCII.GetByteCount(sb.ToString()));
                sb.Append($"{i + 1} 0 obj\n{objects[i]}\nendobj\n");
            }
            var xref = Encoding.ASCII.GetByteCount(sb.ToString());
            sb.Append($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
            foreach (var o in offsets)
                sb.Append($"{o:D10} 00000 n \n");
            sb.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}
