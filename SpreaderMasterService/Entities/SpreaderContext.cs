using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace SpreaderMasterService.Entities
{
    public partial class SpreaderContext : DbContext
    {
        public SpreaderContext()
        {
        }

        public SpreaderContext(DbContextOptions<SpreaderContext> options)
            : base(options)
        {
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpreaderContext>
        {
            public SpreaderContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                var builder = new DbContextOptionsBuilder<SpreaderContext>();
                var connectionString = configuration.GetConnectionString("SpreaderDatabase");
                builder.UseNpgsql(connectionString);
                return new SpreaderContext(builder.Options);
            }
        }

        public virtual DbSet<Agents> Agents { get; set; }
        public virtual DbSet<AgentsLog> AgentsLog { get; set; }
        public virtual DbSet<AgentsLogTypes> AgentsLogTypes { get; set; }
        public virtual DbSet<AgentsWorkers> AgentsWorkers { get; set; }
        public virtual DbSet<AgentsWorkersAccess> AgentsWorkersAccess { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<JobsAccess> JobsAccess { get; set; }
        public virtual DbSet<JobsCron> JobsCron { get; set; }
        public virtual DbSet<JobsLog> JobsLog { get; set; }
        public virtual DbSet<JobsLogTypes> JobsLogTypes { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agents>(entity =>
            {
                entity.ToTable("agents");

                entity.HasIndex(e => e.Netname)
                    .HasName("agents_netname");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Cpucount).HasColumnName("cpucount");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Lastping).HasColumnName("lastping");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Netname)
                    .IsRequired()
                    .HasColumnName("netname")
                    .HasColumnType("character varying");

                entity.Property(e => e.Totalmemory).HasColumnName("totalmemory");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<AgentsLog>(entity =>
            {
                entity.ToTable("agents_log");

                entity.HasIndex(e => e.Created)
                    .HasName("agents_log_created_log_type")
                    .HasFilter("(log_type >= 2)");

                entity.HasIndex(e => e.Jobid)
                    .HasName("agents_log_jobid");

                entity.HasIndex(e => e.LogType)
                    .HasName("agents_log_log_type");

                entity.HasIndex(e => e.Taskid)
                    .HasName("agents_log_taskid");

                entity.HasIndex(e => e.Workerid)
                    .HasName("agents_log_workerid");

                entity.HasIndex(e => new { e.Agentid, e.Created })
                    .HasName("agents_log_agentid_created");

                entity.HasIndex(e => new { e.Jobid, e.Created })
                    .HasName("agents_log_jobid_created_log_type")
                    .HasFilter("(log_type >= 2)");

                entity.HasIndex(e => new { e.Workerid, e.Created })
                    .HasName("agents_log_workerid_created");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.LogType).HasColumnName("log_type");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.Taskid).HasColumnName("taskid");

                entity.Property(e => e.Workerid).HasColumnName("workerid");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.AgentsLog)
                    .HasForeignKey(d => d.Agentid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("agents_log_agentid_fkey");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.AgentsLog)
                    .HasForeignKey(d => d.Jobid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("agents_log_jobid_fkey");

                entity.HasOne(d => d.LogTypeNavigation)
                    .WithMany(p => p.AgentsLog)
                    .HasForeignKey(d => d.LogType)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("agents_log_log_type_fkey");

                entity.HasOne(d => d.Worker)
                    .WithMany(p => p.AgentsLog)
                    .HasForeignKey(d => d.Workerid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("agents_log_workerid_fkey");
            });

            modelBuilder.Entity<AgentsLogTypes>(entity =>
            {
                entity.ToTable("agents_log_types");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<AgentsWorkers>(entity =>
            {
                entity.ToTable("agents_workers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Lastping).HasColumnName("lastping");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.AgentsWorkers)
                    .HasForeignKey(d => d.Agentid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("agents_workers_agentid_fkey");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.AgentsWorkers)
                    .HasForeignKey(d => d.Jobid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("agents_workers_jobid_fkey");
            });

            modelBuilder.Entity<AgentsWorkersAccess>(entity =>
            {
                entity.HasKey(e => new { e.Workerid, e.Accessid })
                    .HasName("agents_workers_access_pkey");

                entity.ToTable("agents_workers_access");

                entity.Property(e => e.Workerid).HasColumnName("workerid");

                entity.Property(e => e.Accessid).HasColumnName("accessid");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.Access)
                    .WithMany(p => p.AgentsWorkersAccess)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Accessid)
                    .HasConstraintName("agents_workers_access_accessid_fkey");

                entity.HasOne(d => d.Worker)
                    .WithMany(p => p.AgentsWorkersAccess)
                    .HasForeignKey(d => d.Workerid)
                    .HasConstraintName("agents_workers_access_workerid_fkey");
            });

            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.ToTable("jobs");

                entity.HasIndex(e => e.Code)
                    .HasName("jobs_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("character varying");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Params).HasColumnName("params");

                entity.Property(e => e.Uri)
                    .HasColumnName("uri")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<JobsAccess>(entity =>
            {
                entity.HasKey(e => new { e.Jobid, e.Code })
                    .HasName("jobs_access_pkey");

                entity.ToTable("jobs_access");

                entity.HasIndex(e => e.Id)
                    .HasName("jobs_access_id_key")
                    .IsUnique();

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("character varying");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobsAccess)
                    .HasForeignKey(d => d.Jobid)
                    .HasConstraintName("jobs_access_jobid_fkey");
            });

            modelBuilder.Entity<JobsCron>(entity =>
            {
                entity.ToTable("jobs_cron");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Accessid).HasColumnName("accessid");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Dayofmonth).HasColumnName("dayofmonth");

                entity.Property(e => e.Daysofweek)
                    .IsRequired()
                    .HasColumnName("daysofweek")
                    .HasMaxLength(7)
                    .IsFixedLength()
                    .HasDefaultValueSql("'       '::bpchar");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("character varying")
                    .HasDefaultValueSql("''::character varying");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.LastRun).HasColumnName("last_run");

                entity.Property(e => e.LastTaskid).HasColumnName("last_taskid");

                entity.Property(e => e.Params)
                    .IsRequired()
                    .HasColumnName("params")
                    .HasColumnType("character varying");

                entity.Property(e => e.Starttime)
                    .HasColumnName("starttime")
                    .HasColumnType("time without time zone");

                entity.Property(e => e.Taskkey)
                    .HasColumnName("taskkey")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.Access)
                    .WithMany(p => p.JobsCron)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Accessid)
                    .HasConstraintName("jobs_cron_accessid_fkey");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobsCron)
                    .HasForeignKey(d => d.Jobid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("jobs_cron_jobid_fkey");

                entity.HasOne(d => d.LastTask)
                    .WithMany(p => p.JobsCron)
                    .HasForeignKey(d => d.LastTaskid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("jobs_cron_last_taskid_fkey");
            });

            modelBuilder.Entity<JobsLog>(entity =>
            {
                entity.ToTable("jobs_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.LogType).HasColumnName("log_type");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobsLog)
                    .HasForeignKey(d => d.Jobid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("jobs_log_jobid_fkey");

                entity.HasOne(d => d.LogTypeNavigation)
                    .WithMany(p => p.JobsLog)
                    .HasForeignKey(d => d.LogType)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("jobs_log_log_type_fkey");
            });

            modelBuilder.Entity<JobsLogTypes>(entity =>
            {
                entity.ToTable("jobs_log_types");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.ToTable("tasks");

                entity.HasIndex(e => e.Processed)
                    .HasName("tasks_processed");

                entity.HasIndex(e => e.ProcessedWithErrors)
                    .HasName("tasks_process_with_errors")
                    .HasFilter("processed");

                entity.HasIndex(e => e.Processing)
                    .HasName("tasks_processing");

                entity.HasIndex(e => e.Stoptime)
                    .HasName("tasks_stoptime_processed_without_errors")
                    .HasFilter("(processed AND (NOT processed_with_errors))");

                entity.HasIndex(e => e.Taskkey)
                    .HasName("tasks_taskkey")
                    .HasFilter("(NOT processed)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Accessid).HasColumnName("accessid");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Params).HasColumnName("params");

                entity.Property(e => e.Processed)
                    .HasColumnName("processed")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.ProcessedWithErrors)
                    .HasColumnName("processed_with_errors")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Processing)
                    .HasColumnName("processing")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Starttime).HasColumnName("starttime");

                entity.Property(e => e.Stoptime).HasColumnName("stoptime");

                entity.Property(e => e.Taskkey)
                    .HasColumnName("taskkey")
                    .HasColumnType("character varying");

                entity.Property(e => e.Workerid).HasColumnName("workerid");

                entity.HasOne(d => d.Access)
                    .WithMany(p => p.Tasks)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Accessid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("tasks_accessid_fkey");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.Agentid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("tasks_agentid_fkey");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.Jobid)
                    .HasConstraintName("tasks_jobid_fkey");

                entity.HasOne(d => d.Worker)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.Workerid)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("tasks_workerid_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
