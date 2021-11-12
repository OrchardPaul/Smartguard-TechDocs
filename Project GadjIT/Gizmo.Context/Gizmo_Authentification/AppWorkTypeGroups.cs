using System.ComponentModel.DataAnnotations;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppWorkTypeGroups
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int parentId { get; set; }

        [StringLength(256)]
        public string GroupName { get; set; }
    }
}
