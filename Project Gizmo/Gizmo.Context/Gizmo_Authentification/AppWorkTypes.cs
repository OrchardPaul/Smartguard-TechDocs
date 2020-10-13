using System.ComponentModel.DataAnnotations;

namespace Gizmo.Context.Gizmo_Authentification
{
    public partial class AppWorkTypes
    {
        [Key]
        public int Id { get; set; }

        [StringLength(256)]
        public string TypeName { get; set; }
    }
}
