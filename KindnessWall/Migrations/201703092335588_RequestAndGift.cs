namespace KindnessWall.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestAndGift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gifts", "CityId", c => c.Int(nullable: false));
            AddColumn("dbo.Gifts", "RegionId", c => c.Int(nullable: false));
            DropColumn("dbo.Gifts", "LocationId");
            DropColumn("dbo.Requests", "FromStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Requests", "FromStatus", c => c.Byte(nullable: false));
            AddColumn("dbo.Gifts", "LocationId", c => c.Int(nullable: false));
            DropColumn("dbo.Gifts", "RegionId");
            DropColumn("dbo.Gifts", "CityId");
        }
    }
}
