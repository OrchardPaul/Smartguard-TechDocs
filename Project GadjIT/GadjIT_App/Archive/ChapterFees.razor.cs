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

namespace Gizmo_V1_02.Archive
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
        public List<VmSmartflowFee> feeItems { get; set; }

        [Parameter]
        public UsrOrDefChapterManagement SelectedChapterObject { get; set; }

        [Parameter]
        public VmSmartflow SelectedChapter { get; set; }



        [Parameter]
        public int SeletedChapterId { get; set; }

        private async void Close()
        {
            await ModalInstance.CloseAsync();
        }

        private void ToggleSelectedFee(VmSmartflowFee selectedFee)
        {
            selectedFee.selected = !selectedFee.selected;
        }


        private async void HandleValidSubmit()
        {
            bool change = false;

            var existingItems = SelectedChapter.Fees ;

            var itemsToAdd = feeItems
                                .Where(C => C.selected)
                                .Where(C => !existingItems
                                                .Select(E => E.FeeName)
                                                .ToList()
                                                .Contains(C.FeeItem.FeeName))
                                .Select(C => C.FeeItem)
                                .ToList();

            var itemsToRemove = existingItems
                                    .Where(C => feeItems
                                                    .Where(V => !V.selected)
                                                    .Select(V => V.FeeItem.FeeName)
                                                    .ToList()
                                                    .Contains(C.FeeName))
                                    .ToList();


            if (itemsToAdd.Count() > 0)
            {
                SelectedChapter.Fees.AddRange(itemsToAdd);
                change = true;
            }

            if (itemsToRemove.Count() > 0)
            {
                foreach(var remove in itemsToRemove)
                {
                    SelectedChapter.Fees.Remove(remove);
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
