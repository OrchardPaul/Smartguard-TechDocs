using System;
using System.Collections.Generic;
using System.Text;
using Gizmo.Context.OR_RESI;

namespace Gizmo.Context.OR_RESI.Custom
{
    public class UsrOrDefChapterManagementComparer : IEqualityComparer<VmUsrOrDefChapterManagement>
    {

            public bool Equals(VmUsrOrDefChapterManagement chapterObject1, VmUsrOrDefChapterManagement chapterObject2)
            {
                if (Object.ReferenceEquals(chapterObject1, null) || Object.ReferenceEquals(chapterObject2, null) ||
                    Object.ReferenceEquals(chapterObject1, chapterObject2)) return false;

                return chapterObject1.ChapterObject.Name == chapterObject2.ChapterObject.Name;
            }

            public int GetHashCode(VmUsrOrDefChapterManagement chapterObject)
            {
                return 0;
            }


    }
}
