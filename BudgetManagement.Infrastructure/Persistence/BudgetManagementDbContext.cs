using BudgetManagement.Domain.Entities;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace BudgetManagement.Infrastructure.Persistence;

public class BudgetManagementDbContext : DbContext
{
    public BudgetManagementDbContext(DbContextOptions<BudgetManagementDbContext> options)
        : base(options)
    {
    }

    // DbSet ها — هر موجودیت یک جدول
    public DbSet<BudgetRecord> BudgetRecords { get; set; }
    public DbSet<Nature> Natures { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Fluent API تنظیمات

        modelBuilder.Entity<BudgetRecord>(entity =>
        {
            entity.HasKey(e => e.SubProjectCode);

            entity.Property(e => e.ContractTitle)
                  .IsRequired()
                  .HasMaxLength(300);

            entity.Property(e => e.ContractNumber).HasMaxLength(50);
            entity.Property(e => e.ContractDate).HasMaxLength(50);
            entity.Property(e => e.Contractor).HasMaxLength(250);
            entity.Property(e => e.Agent).HasMaxLength(250);
            //entity.Property(e => e.CompanyType).HasMaxLength(250);
            //entity.Property(e => e.AgentContractNumber).HasMaxLength(250);
            //entity.Property(e => e.AgentContractDate).HasMaxLength(50);
            //entity.Property(e => e.ExecutionType).HasMaxLength(250);
            entity.Property(e => e.ContractStatus).HasMaxLength(250);
            entity.Property(e => e.ExecutiveDept).HasMaxLength(250);
            //entity.Property(e => e.ResponsibleDept).HasMaxLength(250);
            entity.Property(e => e.StartDate).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasMaxLength(50);
            entity.Property(e => e.ExtendedEndDate).HasMaxLength(50);
            //entity.Property(e => e.ActivityType).HasMaxLength(250);
            entity.Property(e => e.WorkReferralMethod).HasMaxLength(250);
            entity.Property(e => e.CreditNumber).HasMaxLength(250);
            entity.Property(e => e.Nature).HasMaxLength(250);

            entity.Property(e => e.TotalContractAmount).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.InitialAmount).HasColumnType("decimal(18,0)").IsRequired(false);
            //entity.Property(e => e.CurrentYearCashCredit).HasColumnType("decimal(18,0)").IsRequired(false);
            //entity.Property(e => e.CurrentYearNonCashCredit).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.CurrentYearTotalCredit).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.TotalCreditFromStart).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.TotalInvoicesAmount).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.TotalWorkProgress).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.CurrentYearInvoicesAmount).HasColumnType("decimal(18,0)").IsRequired(false);
            //entity.Property(e => e.Increase25Percent).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.AdjustmentAmount).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.MaxRequiredCredit).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.CreditDeficitSupply).HasColumnType("decimal(18,0)").IsRequired(false);
            entity.Property(e => e.CreditDeficitCommitment).HasColumnType("decimal(18,0)").IsRequired(false);

        });

        modelBuilder.Entity<Nature>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasData(
                new Nature { Id = 1, Name = "آبرسانی" },
                new Nature { Id = 2, Name = "آبهای سطحی" },
                new Nature { Id = 3, Name = "آسفالت" },
                new Nature { Id = 4, Name = "اصلاح هندسی" },
                new Nature { Id = 5, Name = "ایستگاه اتوبوس" },
                new Nature { Id = 6, Name = "بازسازی ساختمان" },
                new Nature { Id = 7, Name = "بهسازی پیاده رو" },
                new Nature { Id = 8, Name = "بهسازی جداره" },
                new Nature { Id = 9, Name = "تجهیزات ترافیکی" },
                new Nature { Id = 10, Name = "ساماندهی معابر " },
                new Nature { Id = 11, Name = "علائم افقی ترافيکی" },
                new Nature { Id = 12, Name = "فضاهای بی دفاع" },
                new Nature { Id = 13, Name = "مخزن بتنی" },
                new Nature { Id = 14, Name = "نگهداری ساختمان" },
                new Nature { Id = 15, Name = "نگهداری فضای سبز" },
                new Nature { Id = 16, Name = "نگهداری قنوات" },
                new Nature { Id = 17, Name = "نورپردازی" }
                );
        });

    }
}
