using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorldLeaders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Child-friendly event title"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, comment: "Educational event description for 12-year-olds"),
                    Type = table.Column<string>(type: "text", nullable: false, comment: "Event category for educational learning"),
                    IncomeEffect = table.Column<int>(type: "integer", nullable: false, comment: "Income change from event (positive or negative)"),
                    ReputationEffect = table.Column<int>(type: "integer", nullable: false, comment: "Reputation change from event"),
                    HappinessEffect = table.Column<int>(type: "integer", nullable: false, comment: "Population happiness change from event"),
                    IsPositive = table.Column<bool>(type: "boolean", nullable: false, comment: "Whether event is generally positive for learning"),
                    IconEmoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "🎲", comment: "Child-friendly emoji for visual representation"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Event creation timestamp"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Soft delete flag for content management")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Child-friendly username for educational game"),
                    Income = table.Column<int>(type: "integer", nullable: false, defaultValue: 1000, comment: "Monthly income from job and territories"),
                    Reputation = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Reputation percentage (0-100) for territory acquisition"),
                    Happiness = table.Column<int>(type: "integer", nullable: false, defaultValue: 50, comment: "Population happiness meter (0-100) - game over at 0"),
                    CurrentJob = table.Column<string>(type: "text", nullable: false, defaultValue: "Farmer", comment: "Current job level from dice roll progression"),
                    CurrentGameState = table.Column<string>(type: "text", nullable: false, defaultValue: "NotStarted", comment: "Current state of the educational game session"),
                    GameStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "When the child started playing the game"),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Last activity timestamp for session management"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Account creation timestamp"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Last update timestamp - auto-updated on save"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Soft delete flag for child data protection")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Reference to player for personalized learning"),
                    AgentType = table.Column<string>(type: "text", nullable: false, comment: "Type of AI agent for educational personalization"),
                    PlayerInput = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false, comment: "Child's input to AI agent (content moderated)"),
                    AgentResponse = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false, comment: "AI agent response (age-appropriate and educational)"),
                    WasHelpful = table.Column<bool>(type: "boolean", nullable: false, comment: "Whether child found the AI interaction helpful"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Interaction timestamp for session tracking")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIInteractions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false, comment: "Reference to player for personalized learning progress"),
                    LanguageCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, comment: "ISO 639-1 language code for territory languages"),
                    LanguageName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Human-readable language name for child UI"),
                    AccuracyPercentage = table.Column<int>(type: "integer", nullable: false, comment: "Speech recognition accuracy (0-100%) for progress tracking"),
                    ChallengesCompleted = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Number of pronunciation challenges completed"),
                    HasPassed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Whether child achieved 70%+ accuracy requirement"),
                    LastPracticeAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Last practice session timestamp"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Language learning start timestamp"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Last progress update timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageProgress_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Territories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Official country name for geography education"),
                    CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, comment: "ISO 3166-1 alpha-2 country code"),
                    GdpInBillions = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "GDP in billions USD for economics education"),
                    Cost = table.Column<int>(type: "integer", nullable: false, comment: "Purchase cost in game currency"),
                    ReputationRequired = table.Column<int>(type: "integer", nullable: false, comment: "Minimum reputation percentage (0-100) required"),
                    OfficialLanguages = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, comment: "JSON array of official languages for speech challenges"),
                    Tier = table.Column<string>(type: "text", nullable: false, defaultValue: "Small", comment: "Territory difficulty tier based on GDP ranking"),
                    RealGDP = table.Column<long>(type: "bigint", nullable: false, comment: "Real GDP in USD from World Bank data"),
                    GDPRank = table.Column<int>(type: "integer", nullable: false, comment: "World GDP ranking for educational context"),
                    MonthlyIncome = table.Column<int>(type: "integer", nullable: false, comment: "Monthly income generated when owned"),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Whether territory is available for purchase"),
                    OwnedByPlayerId = table.Column<Guid>(type: "uuid", nullable: true, comment: "Player ID who owns this territory (nullable)"),
                    AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "When territory was acquired by current owner"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Territory record creation timestamp"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Last update timestamp"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Soft delete flag for data protection")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Territories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Territories_Players_OwnedByPlayerId",
                        column: x => x.OwnedByPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "GameEvents",
                columns: new[] { "Id", "CreatedAt", "Description", "HappinessEffect", "IconEmoji", "IncomeEffect", "IsPositive", "ReputationEffect", "Title", "Type" },
                values: new object[,]
                {
                    { new Guid("1e6743ae-dc51-43cb-b113-1769e41ad39a"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3935), "You received an award for helping your community. People appreciate your kindness!", 15, "🏆", 200, true, 10, "Community Award", "Social" },
                    { new Guid("1f2e185b-df7d-4376-bf03-627f00402e24"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3917), "Your hard work has been noticed by your supervisor. Great job!", 10, "⭐", 500, true, 5, "Excellent Work Performance!", "Career" },
                    { new Guid("370ef5cb-5c7f-43e6-a764-1dd75389de09"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4072), "Nature provided an excellent harvest this year. Farmers are happy!", 8, "🌾", 400, true, 2, "Beautiful Harvest", "Natural" },
                    { new Guid("4ee362ea-653d-4601-8a7d-1ef1c88cff94"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4045), "You helped someone in need, and it made everyone feel good!", 15, "❤️", 0, true, 5, "Helping Others", "Social" },
                    { new Guid("65b8cd69-9d60-4995-8016-69a9c4bb2790"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4113), "You found a creative solution to a community problem!", 15, "💡", 200, true, 10, "Problem Solving", "Social" },
                    { new Guid("66cbaabd-f15d-4285-848b-9f607e1070d1"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4059), "You learned about a different culture and made new friends!", 10, "🌍", 150, true, 7, "Cultural Exchange", "International" },
                    { new Guid("68113cac-1c4c-4e12-8a56-a46eafdc8553"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4126), "Your efforts to protect the environment are appreciated by everyone!", 18, "🌱", 150, true, 5, "Environmental Care", "Natural" },
                    { new Guid("6e357db6-9fde-4723-8f5f-50f1a0b539e8"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4003), "Unexpected weather affects crops, teaching us about nature's impact on economy.", -3, "🌧️", -200, false, 0, "Natural Weather Event", "Natural" },
                    { new Guid("8401757b-15f4-4ad1-ba7d-7337c10aa683"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4099), "Your kind actions inspired others to be kind too!", 25, "😊", 50, true, 8, "Acts of Kindness", "Social" },
                    { new Guid("94d175a5-a253-486c-9259-3fd36f27e302"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3975), "A new trade agreement brings prosperity to your region!", 12, "🤝", 600, true, 8, "International Trade Deal", "International" },
                    { new Guid("9c643b80-54a1-409a-9d14-b0112f8fe34a"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3948), "You learned a valuable new skill that makes you better at your job!", 8, "📚", 300, true, 3, "New Skill Learned", "Career" },
                    { new Guid("b5c86b88-6b5b-4172-bd4f-63bd2cef1470"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4031), "Your neighborhood organizes a wonderful festival that brings everyone together!", 20, "🎉", 100, true, 3, "Community Festival", "Social" },
                    { new Guid("b9ebc9c7-0b35-46ba-820d-f471ab6d080f"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4086), "Scientists in your region made an amazing discovery that helps everyone!", 12, "🔬", 300, true, 8, "Scientific Discovery", "Career" },
                    { new Guid("ce61ebfd-511c-4e12-a43c-e3ffee4066df"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3962), "The economy is doing well, and your investments are growing!", 5, "📈", 800, true, 5, "Market Success", "Economic" },
                    { new Guid("ceae6747-54b5-48fe-9869-1fdcf9e33ffb"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(4017), "A difficult situation becomes a chance to grow and learn new skills!", 5, "🎓", -100, false, 2, "Learning Opportunity", "Career" },
                    { new Guid("cf869488-7c06-4f29-b987-555a8f114e2e"), new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3990), "Economic conditions are tough, but this is a chance to learn resilience!", -5, "📉", -300, false, -2, "Economic Challenge", "Economic" }
                });

            migrationBuilder.InsertData(
                table: "Territories",
                columns: new[] { "Id", "AcquiredAt", "Cost", "CountryCode", "CountryName", "CreatedAt", "GDPRank", "GdpInBillions", "IsAvailable", "MonthlyIncome", "OfficialLanguages", "OwnedByPlayerId", "RealGDP", "ReputationRequired", "Tier", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("03c974ee-a950-4441-9be2-e9288a4454d0"), null, 12000, "LU", "Luxembourg", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3106), 90, 86.93m, true, 1200, "[\"lb\",\"fr\",\"de\"]", null, 86930000000L, 20, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3106) },
                    { new Guid("0be64112-5ee7-4425-8cc5-6888e0f45eaf"), null, 70000, "CH", "Switzerland", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3465), 22, 807.71m, true, 7000, "[\"de\",\"fr\",\"it\",\"rm\"]", null, 807710000000L, 65, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3466) },
                    { new Guid("0ceea510-85da-4e6f-998d-35087c9cea0e"), null, 150000, "CN", "China", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3764), 2, 17.73m, true, 15000, "[\"zh\"]", null, 17730000000L, 92, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3764) },
                    { new Guid("12737342-fe23-45ed-ab35-0a5fa51cc8d3"), null, 95000, "GB", "United Kingdom", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3701), 6, 3.13m, true, 9500, "[\"en\",\"cy\",\"gd\"]", null, 3130000000L, 82, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3702) },
                    { new Guid("1a4cd16a-47c2-43cd-8ad7-6445d50536fe"), null, 36000, "IL", "Israel", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3376), 46, 481.66m, true, 3600, "[\"he\",\"ar\",\"en\"]", null, 481660000000L, 40, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3376) },
                    { new Guid("1db0c6c4-5218-40a9-ab50-1647c9778ff4"), null, 42000, "BE", "Belgium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3421), 40, 594.41m, true, 4200, "[\"nl\",\"fr\",\"de\"]", null, 594410000000L, 45, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3421) },
                    { new Guid("1eaef1a6-3ef9-46a7-a242-6ab0f24375c6"), null, 5000, "NP", "Nepal", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3026), 100, 36.29m, true, 500, "[\"ne\",\"en\"]", null, 36290000000L, 10, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3027) },
                    { new Guid("2249758a-54de-4bef-b62d-edf9503f9645"), null, 78000, "IT", "Italy", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3658), 16, 2.11m, true, 7800, "[\"it\"]", null, 2110000000L, 70, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3658) },
                    { new Guid("2badf0a2-d3ad-4268-af13-3b1e880950fd"), null, 4500, "MT", "Malta", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3128), 105, 17.32m, true, 450, "[\"mt\",\"en\"]", null, 17320000000L, 8, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3128) },
                    { new Guid("2c77d3b4-3def-43f2-b8dc-50c2d1cd8956"), null, 90000, "FR", "France", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3678), 8, 2.94m, true, 9000, "[\"fr\"]", null, 2940000000L, 80, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3679) },
                    { new Guid("2cc72d7d-7bc6-4feb-9387-8d77be6b117a"), null, 65000, "NL", "Netherlands", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3443), 25, 1.01m, true, 6500, "[\"nl\"]", null, 1010000000L, 60, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3443) },
                    { new Guid("2d2e0de5-ebd3-448f-9a7c-383ddff8876b"), null, 85000, "CA", "Canada", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3529), 12, 2.14m, true, 8500, "[\"en\",\"fr\"]", null, 2140000000L, 75, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3529) },
                    { new Guid("3dcd6c10-28b5-4b9a-b4ad-b97b43bc19f2"), null, 32000, "DK", "Denmark", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3287), 50, 398.30m, true, 3200, "[\"da\"]", null, 398300000000L, 38, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3287) },
                    { new Guid("49e0adb2-6b3d-4880-b2fd-1e05c8924f1e"), null, 35000, "AT", "Austria", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3397), 47, 477.04m, true, 3500, "[\"de\"]", null, 477040000000L, 38, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3398) },
                    { new Guid("4b6c1442-c18e-4ffd-bdc2-1a3e38a6229a"), null, 55000, "SE", "Sweden", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3486), 28, 635.66m, true, 5500, "[\"sv\"]", null, 635660000000L, 55, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3486) },
                    { new Guid("56b32c21-57e3-491d-a578-efca4a5403b8"), null, 35000, "IE", "Ireland", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3265), 45, 498.57m, true, 3500, "[\"en\",\"ga\"]", null, 498570000000L, 40, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3265) },
                    { new Guid("60a23063-a590-404e-a05a-b2ceb2a48d22"), null, 30000, "FI", "Finland", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3308), 52, 297.30m, true, 3000, "[\"fi\",\"sv\"]", null, 297300000000L, 35, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3308) },
                    { new Guid("7508b585-e10f-4d44-bbf9-9b87f1974b94"), null, 72000, "ES", "Spain", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3635), 20, 1.40m, true, 7200, "[\"es\",\"ca\",\"gl\",\"eu\"]", null, 1400000000L, 65, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3635) },
                    { new Guid("7da5085d-ca67-4b3b-8f11-7d076ac1e6c1"), null, 5500, "CY", "Cyprus", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3154), 102, 27.97m, true, 550, "[\"el\",\"tr\",\"en\"]", null, 27970000000L, 10, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3154) },
                    { new Guid("8b3f5295-31cc-4aa5-b423-b7c2e569392b"), null, 7000, "BN", "Brunei", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3176), 110, 16.84m, true, 700, "[\"ms\",\"en\"]", null, 16840000000L, 15, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3176) },
                    { new Guid("a7e90b4a-3845-4773-bf80-38a5cea7c2ff"), null, 6000, "EE", "Estonia", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3081), 98, 38.03m, true, 600, "[\"et\",\"en\"]", null, 38030000000L, 12, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3081) },
                    { new Guid("ad85259f-5857-4ef2-aeb5-057b5c8f5fad"), null, 200000, "US", "United States", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3786), 1, 26.95m, true, 20000, "[\"en\",\"es\"]", null, 26950000000L, 95, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3786) },
                    { new Guid("af864865-0106-4d8d-8809-072439b4fcd5"), null, 40000, "SG", "Singapore", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3353), 42, 397.72m, true, 4000, "[\"en\",\"ms\",\"ta\",\"zh\"]", null, 397720000000L, 45, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3354) },
                    { new Guid("bff7d1e4-b78b-499c-9d38-f2edf20b56e6"), null, 4000, "JM", "Jamaica", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3198), 108, 17.94m, true, 400, "[\"en\"]", null, 17940000000L, 8, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3199) },
                    { new Guid("c15b7344-f64c-49de-aefa-3dd8d1d99043"), null, 110000, "JP", "Japan", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3743), 3, 4.94m, true, 11000, "[\"ja\"]", null, 4940000000L, 88, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3744) },
                    { new Guid("c3e5171a-72c7-4b88-85d1-c4ad909779a4"), null, 75000, "KR", "South Korea", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3549), 18, 1.81m, true, 7500, "[\"ko\"]", null, 1810000000L, 68, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3549) },
                    { new Guid("c6e38104-f119-47ee-9595-e3bb933e5143"), null, 38000, "NO", "Norway", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3329), 43, 482.17m, true, 3800, "[\"no\"]", null, 482170000000L, 42, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3329) },
                    { new Guid("d049560e-d1a7-4757-8fc3-19eb548fd4de"), null, 28000, "NZ", "New Zealand", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3243), 55, 249.89m, true, 2800, "[\"en\",\"mi\"]", null, 249890000000L, 35, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3244) },
                    { new Guid("e59c481d-7120-48c3-ade4-ee452f984e73"), null, 25000, "PT", "Portugal", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3220), 60, 249.89m, true, 2500, "[\"pt\"]", null, 249890000000L, 30, "Medium", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3220) },
                    { new Guid("eabe7dc9-c314-4ca2-a17b-0527f4223a4c"), null, 8000, "IS", "Iceland", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3058), 95, 27.84m, true, 800, "[\"is\",\"en\"]", null, 27840000000L, 15, "Small", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3058) },
                    { new Guid("fce3d21d-72b8-49df-a9de-839ed58a8013"), null, 100000, "DE", "Germany", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3722), 4, 4.26m, true, 10000, "[\"de\"]", null, 4260000000L, 85, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3722) },
                    { new Guid("feec3004-7207-4383-a5f7-392588cad7e3"), null, 80000, "AU", "Australia", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3507), 15, 1.55m, true, 8000, "[\"en\"]", null, 1550000000L, 70, "Major", new DateTime(2025, 8, 3, 8, 46, 29, 827, DateTimeKind.Utc).AddTicks(3507) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIInteractions_AgentType",
                table: "AIInteractions",
                column: "AgentType");

            migrationBuilder.CreateIndex(
                name: "IX_AIInteractions_CreatedAt",
                table: "AIInteractions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AIInteractions_Player_Agent",
                table: "AIInteractions",
                columns: new[] { "PlayerId", "AgentType" });

            migrationBuilder.CreateIndex(
                name: "IX_AIInteractions_PlayerId",
                table: "AIInteractions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_IsDeleted",
                table: "GameEvents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_IsPositive",
                table: "GameEvents",
                column: "IsPositive");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvents_Type",
                table: "GameEvents",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProgress_AccuracyPercentage",
                table: "LanguageProgress",
                column: "AccuracyPercentage");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProgress_HasPassed",
                table: "LanguageProgress",
                column: "HasPassed");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProgress_LanguageCode",
                table: "LanguageProgress",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProgress_LastPracticeAt",
                table: "LanguageProgress",
                column: "LastPracticeAt");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProgress_Player_Language",
                table: "LanguageProgress",
                columns: new[] { "PlayerId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_IsDeleted",
                table: "Players",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Players_LastActiveAt",
                table: "Players",
                column: "LastActiveAt");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Username",
                table: "Players",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Territories_CountryCode",
                table: "Territories",
                column: "CountryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Territories_GDPRank",
                table: "Territories",
                column: "GDPRank");

            migrationBuilder.CreateIndex(
                name: "IX_Territories_IsAvailable",
                table: "Territories",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Territories_IsDeleted",
                table: "Territories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Territories_OwnedByPlayerId",
                table: "Territories",
                column: "OwnedByPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Territories_Tier",
                table: "Territories",
                column: "Tier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIInteractions");

            migrationBuilder.DropTable(
                name: "GameEvents");

            migrationBuilder.DropTable(
                name: "LanguageProgress");

            migrationBuilder.DropTable(
                name: "Territories");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
