namespace KindnessWall.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class VerificationDateTimeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "VerificationDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "VerificationDateTime");
        }
    }
}
