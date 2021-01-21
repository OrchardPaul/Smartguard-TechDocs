using Blazored.Modal;
using GadjIT.ClientContext.OR_RESI;
using GadjIT.ClientContext.OR_RESI.Custom;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.OR_RESI_Chapters
{
    public partial class ChapterFees
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public Action DataChanged { get; set; }

        [Parameter]
        public List<VmChapterFee> feeItems { get; set; }

        [Parameter]
        public int SeletedChapterId { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private void ToggleSelectedFee(VmChapterFee selectedFee)
        {
            selectedFee.selected = !selectedFee.selected;
        }


        private async void HandleValidSubmit()
        {
            await chapterManagementService.UpdateChapterFees(SeletedChapterId, feeItems);
            DataChanged?.Invoke();
            Close();
        }

    }
}
