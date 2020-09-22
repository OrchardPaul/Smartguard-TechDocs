using System;
using System.Collections.Generic;

namespace Gizmo.Context.OR_RESI
{
    public partial class UsrOrDefChapterManagement
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string CaseTypeGroup { get; set; }
        public string CaseType { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int? SeqNo { get; set; }
        public string SuppressStep { get; set; }
        public string EntityType { get; set; }
        public string AsName { get; set; }
        public string CompleteName { get; set; }
        public int? RescheduleDays { get; set; }
    }
}
