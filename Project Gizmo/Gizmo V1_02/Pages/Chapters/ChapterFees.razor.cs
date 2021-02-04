using Blazored.Modal;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W.Custom;
using Gizmo_V1_02.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo_V1_02.Pages.Chapters
{
    public partial class ChapterFees
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        [Inject]
        IChapterManagementService chapterManagementService { get; set; }

        [Parameter]
        public Action RefreshFeeOrder { get; set; }


        [Parameter]
        public List<VmChapterFee> feeItems { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement SelectedChapterObject { get; set; }

        [Parameter]
        public VmChapter SelectedChapter { get; set; }



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
            bool change = false;

            var existingItems = SelectedChapter.ChapterItems.Where(C => C.Type == "Fee").ToList(); ;

            var itemsToAdd = feeItems
                                .Where(C => C.selected)
                                .Where(C => !existingItems
                                                .Select(E => E.Name)
                                                .ToList()
                                                .Contains(C.FeeItem.Name))
                                .Select(C => C.FeeItem)
                                .ToList();

            var itemsToRemove = existingItems
                                    .Where(C => feeItems
                                                    .Where(V => !V.selected)
                                                    .Select(V => V.FeeItem.Name)
                                                    .ToList()
                                                    .Contains(C.Name))
                                    .ToList();


            if (itemsToAdd.Count() > 0)
            {
                SelectedChapter.ChapterItems.AddRange(itemsToAdd);
                change = true;
            }

            if (itemsToRemove.Count() > 0)
            {
                foreach(var remove in itemsToRemove)
                {
                    SelectedChapter.ChapterItems.Remove(remove);
                }
                change = true;
            }

            if (change)
            {
                SelectedChapterObject.ChapterData = JsonConvert.SerializeObject(SelectedChapter);
                await chapterManagementService.Update(SelectedChapterObject).ConfigureAwait(false);
            }


            RefreshFeeOrder?.Invoke();
            Close();
        }

    }
}
