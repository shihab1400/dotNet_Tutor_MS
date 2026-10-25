using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectShihab.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxUses = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                    table.CheckConstraint("CK_Coupon_Type", "[Type] in ('Percentage','Fixed')");
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.CheckConstraint("CK_User_Role", "[Role] in ('Admin','Trainer','Tutor','Student')");
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Course_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Course_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CouponId = table.Column<int>(type: "int", nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfferedCourse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    TutorId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferedCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferedCourse_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferedCourse_User_TutorId",
                        column: x => x.TutorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingOffering",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingOffering", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingOffering_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingOffering_User_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    OfferedCourseId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_OfferedCourse_OfferedCourseId",
                        column: x => x.OfferedCourseId,
                        principalTable: "OfferedCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseEnrollment_User_StudentId",
                        column: x => x.StudentId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginalName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OfferedCourseId = table.Column<int>(type: "int", nullable: true),
                    TrainingOfferingId = table.Column<int>(type: "int", nullable: true),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                    table.CheckConstraint("CK_Material_Target", "([OfferedCourseId] is null and [TrainingOfferingId] is not null) or ([OfferedCourseId] is not null and [TrainingOfferingId] is null)");
                    table.ForeignKey(
                        name: "FK_Material_OfferedCourse_OfferedCourseId",
                        column: x => x.OfferedCourseId,
                        principalTable: "OfferedCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_TrainingOffering_TrainingOfferingId",
                        column: x => x.TrainingOfferingId,
                        principalTable: "TrainingOffering",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Material_User_UploadedBy",
                        column: x => x.UploadedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingEnrollment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorId = table.Column<int>(type: "int", nullable: false),
                    TrainingOfferingId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingEnrollment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingEnrollment_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingEnrollment_TrainingOffering_TrainingOfferingId",
                        column: x => x.TrainingOfferingId,
                        principalTable: "TrainingOffering",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingEnrollment_User_TutorId",
                        column: x => x.TutorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseEnrollmentId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TutorId = table.Column<int>(type: "int", nullable: false),
                    OfferedCourseId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.CheckConstraint("CK_Rating_Value", "[Rating] between 1 and 5");
                    table.ForeignKey(
                        name: "FK_Rating_CourseEnrollment_CourseEnrollmentId",
                        column: x => x.CourseEnrollmentId,
                        principalTable: "CourseEnrollment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_OfferedCourse_OfferedCourseId",
                        column: x => x.OfferedCourseId,
                        principalTable: "OfferedCourse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_User_StudentId",
                        column: x => x.StudentId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_User_TutorId",
                        column: x => x.TutorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainingEnrollmentId = table.Column<int>(type: "int", nullable: false),
                    TutorId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CertificateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificate_TrainingEnrollment_TrainingEnrollmentId",
                        column: x => x.TrainingEnrollmentId,
                        principalTable: "TrainingEnrollment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificate_User_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificate_User_TutorId",
                        column: x => x.TutorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_CertificateNo",
                table: "Certificate",
                column: "CertificateNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_CourseId",
                table: "Certificate",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_TrainerId",
                table: "Certificate",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_TrainingEnrollmentId",
                table: "Certificate",
                column: "TrainingEnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_TutorId",
                table: "Certificate",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_Code",
                table: "Coupon",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedBy",
                table: "Course",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Course_GroupId_Subject",
                table: "Course",
                columns: new[] { "GroupId", "Subject" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_OfferedCourseId",
                table: "CourseEnrollment",
                column: "OfferedCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_PaymentId",
                table: "CourseEnrollment",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollment_StudentId_OfferedCourseId",
                table: "CourseEnrollment",
                columns: new[] { "StudentId", "OfferedCourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_Title",
                table: "Group",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Log_CreatedAt",
                table: "Log",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Material_OfferedCourseId",
                table: "Material",
                column: "OfferedCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_TrainingOfferingId",
                table: "Material",
                column: "TrainingOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_Material_UploadedBy",
                table: "Material",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OfferedCourse_CourseId_TutorId",
                table: "OfferedCourse",
                columns: new[] { "CourseId", "TutorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfferedCourse_TutorId",
                table: "OfferedCourse",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CouponId",
                table: "Payment",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UserId",
                table: "Payment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_CourseEnrollmentId",
                table: "Rating",
                column: "CourseEnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rating_OfferedCourseId",
                table: "Rating",
                column: "OfferedCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_StudentId",
                table: "Rating",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_TutorId",
                table: "Rating",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollment_PaymentId",
                table: "TrainingEnrollment",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollment_TrainingOfferingId",
                table: "TrainingEnrollment",
                column: "TrainingOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollment_TutorId_TrainingOfferingId",
                table: "TrainingEnrollment",
                columns: new[] { "TutorId", "TrainingOfferingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOffering_CourseId_TrainerId",
                table: "TrainingOffering",
                columns: new[] { "CourseId", "TrainerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOffering_TrainerId",
                table: "TrainingOffering",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "TrainingEnrollment");

            migrationBuilder.DropTable(
                name: "CourseEnrollment");

            migrationBuilder.DropTable(
                name: "TrainingOffering");

            migrationBuilder.DropTable(
                name: "OfferedCourse");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
