namespace KindnessWall.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppVersionChanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppVersionId = c.Int(nullable: false),
                        Description = c.String(),
                        ViewOrder = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppVersions", t => t.AppVersionId, cascadeDelete: true)
                .Index(t => t.AppVersionId);
            
            CreateTable(
                "dbo.AppVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Version = c.String(),
                        ApkUrl = c.String(),
                        Changes = c.String(),
                        LastUpdateVersion = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppVersionChanges", "AppVersionId", "dbo.AppVersions");
            DropIndex("dbo.AppVersionChanges", new[] { "AppVersionId" });
            DropTable("dbo.AppVersions");
            DropTable("dbo.AppVersionChanges");
        }
    }
}
