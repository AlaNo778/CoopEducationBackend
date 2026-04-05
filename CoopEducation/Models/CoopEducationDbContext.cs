using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CoopEducation.Models;

public partial class CoopEducationDbContext : DbContext
{
    public CoopEducationDbContext()
    {
    }

    public CoopEducationDbContext(DbContextOptions<CoopEducationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advisorship> Advisorships { get; set; }

    public virtual DbSet<ApiLog> ApiLogs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CoopPlacement> CoopPlacements { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentAddress> StudentAddresses { get; set; }

    public virtual DbSet<StudentContact> StudentContacts { get; set; }

    public virtual DbSet<StudentDocument> StudentDocuments { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CoopEducationDB;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advisorship>(entity =>
        {
            entity.HasKey(e => e.AdvisorshipId).HasName("PK__advisors__07973D4CF110669D");

            entity.ToTable("advisorships");

            entity.Property(e => e.AdvisorshipId).HasColumnName("advisorship_id");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("academic_year");
            entity.Property(e => e.AssignedAt)
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Advisorships)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_advisorships_students");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Advisorships)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("fk_advisorships_teachers");
        });

        modelBuilder.Entity<ApiLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__api_logs__9E2397E07ECEE4C4");

            entity.ToTable("api_logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.ApiEndpoint)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("api_endpoint");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("method");
            entity.Property(e => e.Request)
                .IsUnicode(false)
                .HasColumnName("request");
            entity.Property(e => e.Response)
                .IsUnicode(false)
                .HasColumnName("response");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status_code");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__companie__3E267235FD38B9DA");

            entity.ToTable("companies");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .HasColumnName("company_name");
            entity.Property(e => e.CreateAd)
                .HasColumnType("datetime")
                .HasColumnName("create_ad");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("fax");
            entity.Property(e => e.HrName)
                .HasMaxLength(100)
                .HasColumnName("hr_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<CoopPlacement>(entity =>
        {
            entity.HasKey(e => e.PlacementId).HasName("PK__coop_pla__C9DAD93CEFA18AA4");

            entity.ToTable("coop_placements");

            entity.Property(e => e.PlacementId).HasColumnName("placement_id");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("academic_year");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.JobDescription).HasColumnName("job_description");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("job_title");
            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Company).WithMany(p => p.CoopPlacements)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_coop_placements_company");

            entity.HasOne(d => d.Mentor).WithMany(p => p.CoopPlacements)
                .HasForeignKey(d => d.MentorId)
                .HasConstraintName("fk_coop_placements_mentor");

            entity.HasOne(d => d.Student).WithMany(p => p.CoopPlacements)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_coop_placements_student");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.HasKey(e => e.DocTypeId).HasName("PK__document__85153F050157D30F");

            entity.ToTable("document_types");

            entity.Property(e => e.DocTypeId).HasColumnName("doc_type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DocName)
                .HasMaxLength(150)
                .HasColumnName("doc_name");
            entity.Property(e => e.IsRequired)
                .HasDefaultValue(true)
                .HasColumnName("is_required");
            entity.Property(e => e.TypeName)
                .HasMaxLength(20)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.MajorId).HasName("PK__major__DC7AC3C4A74670BC");

            entity.ToTable("majors");

            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.MajorName)
                .HasMaxLength(250)
                .HasColumnName("major_name");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.MentorId).HasName("PK__mentors__E5D27EF3CD741969");

            entity.ToTable("mentors");

            entity.Property(e => e.MentorId).HasColumnName("mentor_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");

            entity.HasOne(d => d.Company).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_mentors_companies");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__refresh___3213E83F31F3C26C");

            entity.ToTable("refresh_tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Expiry)
                .HasColumnType("datetime")
                .HasColumnName("expiry");
            entity.Property(e => e.Revoked)
                .HasDefaultValue(false)
                .HasColumnName("revoked");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_refresh_user");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CCACF7A765");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "UQ__roles__783254B1EACB5AC7").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__staffs__1963DD9CDC1B4432");

            entity.ToTable("staffs");

            entity.HasIndex(e => e.UserId, "UQ__staffs__B9BE370EA8A57693").IsUnique();

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.UserId)
                .HasConstraintName("fk_staffs_users");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__students__2A33069A6285BD86");

            entity.ToTable("students");

            entity.HasIndex(e => e.StudentCode, "UQ__students__6DF33C456EB2F52E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__students__AB6E6164E89A24D0").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__students__B9BE370E9976508B").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Faculty)
                .HasMaxLength(100)
                .HasDefaultValue("คณะวิทยาศาสตร์")
                .HasColumnName("faculty");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gpax)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("gpax");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.MajorId).HasColumnName("major_id");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("student_code");
            entity.Property(e => e.TotalCredits).HasColumnName("total_credits");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Major).WithMany(p => p.Students)
                .HasForeignKey(d => d.MajorId)
                .HasConstraintName("fk_major_student");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .HasConstraintName("fk_students_users");
        });

        modelBuilder.Entity<StudentAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__student___CAA247C87E067A10");

            entity.ToTable("student_addresses");

            entity.HasIndex(e => e.StudentId, "UQ__student___2A33069B8D86CD47").IsUnique();

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Alley)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("alley");
            entity.Property(e => e.District)
                .HasMaxLength(50)
                .HasColumnName("district");
            entity.Property(e => e.HouseNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("house_no");
            entity.Property(e => e.Postcode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("postcode");
            entity.Property(e => e.Province)
                .HasMaxLength(50)
                .HasColumnName("province");
            entity.Property(e => e.Road)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("road");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubDistrict)
                .HasMaxLength(50)
                .HasColumnName("sub_district");
            entity.Property(e => e.VillageNo)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("village_no");

            entity.HasOne(d => d.Student).WithOne(p => p.StudentAddress)
                .HasForeignKey<StudentAddress>(d => d.StudentId)
                .HasConstraintName("fk_student_addresses_students");
        });

        modelBuilder.Entity<StudentContact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__student___024E7A86532B7430");

            entity.ToTable("student_contacts");

            entity.HasIndex(e => e.StudentId, "UQ__student___2A33069BC9949DE2").IsUnique();

            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.Facebook)
                .HasMaxLength(150)
                .HasColumnName("facebook");
            entity.Property(e => e.LineId)
                .HasMaxLength(100)
                .HasColumnName("line_id");
            entity.Property(e => e.PhoneHome)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_home");
            entity.Property(e => e.PhoneMobile)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_mobile");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Student).WithOne(p => p.StudentContact)
                .HasForeignKey<StudentContact>(d => d.StudentId)
                .HasConstraintName("fk_student_contacts_students");
        });

        modelBuilder.Entity<StudentDocument>(entity =>
        {
            entity.HasKey(e => e.DocId).HasName("PK__student___8AD029241BB6EE39");

            entity.ToTable("student_documents");

            entity.Property(e => e.DocId).HasColumnName("doc_id");
            entity.Property(e => e.DocTypeId).HasColumnName("doc_type_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("file_name");
            entity.Property(e => e.FileSize)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("file_size");
            entity.Property(e => e.PlacementId).HasColumnName("placement_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.UploadedAt)
                .HasColumnType("datetime")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.DocType).WithMany(p => p.StudentDocuments)
                .HasForeignKey(d => d.DocTypeId)
                .HasConstraintName("fk_student_document_document_types");

            entity.HasOne(d => d.Placement).WithMany(p => p.StudentDocuments)
                .HasForeignKey(d => d.PlacementId)
                .HasConstraintName("fk_sstudent_document_coop_placments");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentDocuments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_student_document_students");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__teachers__03AE777EDF61ED60");

            entity.ToTable("teachers");

            entity.HasIndex(e => e.UserId, "UQ__teachers__B9BE370E090B3487").IsUnique();

            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .HasConstraintName("fk_teachers_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FDB165FBF");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC5728E666D89").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdateBy).HasColumnName("update_by");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
