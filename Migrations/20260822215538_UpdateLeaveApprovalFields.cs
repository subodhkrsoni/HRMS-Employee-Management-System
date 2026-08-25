using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeaveApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovedDate",
                table: "Leaves",
                newName: "ActionDate");

            migrationBuilder.RenameColumn(
                name: "ApprovedBy",
                table: "Leaves",
                newName: "ActionBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActionDate",
                table: "Leaves",
                newName: "ApprovedDate");

            migrationBuilder.RenameColumn(
                name: "ActionBy",
                table: "Leaves",
                newName: "ApprovedBy");
        }
    }
}
