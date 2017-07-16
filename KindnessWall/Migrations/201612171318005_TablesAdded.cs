namespace KindnessWall.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TablesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ImageUrl = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Gifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                        Price = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CategoryId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Locations", t => t.LocationId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CategoryId)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        Title = c.String(),
                        Latitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Longitude = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gifts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Gifts", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Gifts", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Gifts", new[] { "LocationId" });
            DropIndex("dbo.Gifts", new[] { "CategoryId" });
            DropIndex("dbo.Gifts", new[] { "UserId" });
            DropTable("dbo.Locations");
            DropTable("dbo.Gifts");
            DropTable("dbo.Categories");
        }
    }
}
