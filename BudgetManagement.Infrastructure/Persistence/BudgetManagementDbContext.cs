using BudgetManagement.Domain.Entities;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace BudgetManagement.Infrastructure.Persistence
{
    public class BudgetManagementDbContext : DbContext
    {
        public BudgetManagementDbContext(DbContextOptions<BudgetManagementDbContext> options)
            : base(options)
        {
        }

        // DbSet ها — هر موجودیت یک جدول
        public DbSet<BudgetRecord> BudgetRecords { get; set; }

    
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

                entity.Property(e=>e.ContractNumber).HasMaxLength(50);
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
        }
    }
}
