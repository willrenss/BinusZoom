namespace BinusZoom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMeetingEndDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meetings", "MeetingEndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meetings", "MeetingEndDate");
        }
    }
}
