using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clipper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Platform = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    LastSyncAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channel_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    OriginalUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LocalFilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    ProcessingStatus = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LocalThumbnailPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ChannelId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Video_Channel_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    StartTimeSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    EndTimeSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalFilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false, defaultValue: 0L),
                    RelevanceScore = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Type = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    LocalThumbnailPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    VideoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clip_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingJob",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Parameters = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ErrorStackTrace = table.Column<string>(type: "TEXT", nullable: true),
                    ProgressPercentage = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    MaxAttempts = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 3),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 5),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    VideoId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingJob_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessingJob_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ExternalId_Platform",
                table: "Channel",
                columns: new[] { "ExternalId", "Platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_IsActive",
                table: "Channel",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_IsDeleted",
                table: "Channel",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_UserId",
                table: "Channel",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_IsDeleted",
                table: "Clip",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_RelevanceScore",
                table: "Clip",
                column: "RelevanceScore");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_Status",
                table: "Clip",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_Type",
                table: "Clip",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Clips_VideoId",
                table: "Clip",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_IsDeleted",
                table: "ProcessingJob",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_Priority",
                table: "ProcessingJob",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_Status",
                table: "ProcessingJob",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_Status_Priority",
                table: "ProcessingJob",
                columns: new[] { "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_Type",
                table: "ProcessingJob",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_UserId",
                table: "ProcessingJob",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingJobs_VideoId",
                table: "ProcessingJob",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "User",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ChannelId",
                table: "Video",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ExternalId",
                table: "Video",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_IsDeleted",
                table: "Video",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProcessingStatus",
                table: "Video",
                column: "ProcessingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PublishedAt",
                table: "Video",
                column: "PublishedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clip");

            migrationBuilder.DropTable(
                name: "ProcessingJob");

            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.DropTable(
                name: "Channel");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
