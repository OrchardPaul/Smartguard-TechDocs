using System.ComponentModel.DataAnnotations;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppCompanyWorkTypeGroups
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public int WorkTypeGroupId { get; set; }
    }
}
