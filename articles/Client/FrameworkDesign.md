# HTMX + Razor Pages Checklist Application Scaffold

Below is the full scaffold for a .NET Razor Pages application using HTMX for dynamic fragment swapping. This includes all major pages, partials, handlers, view models, and the recommended folder structure.

---

## 📁 Folder Structure
```
/Pages
    /Checklists
        Index.cshtml
        Index.cshtml.cs
        Sections.cshtml
        Sections.cshtml.cs
        SectionDetail.cshtml
        SectionDetail.cshtml.cs
        Questions.cshtml
        Questions.cshtml.cs
        SaveAnswer.cshtml
        SaveAnswer.cshtml.cs
        SignOff.cshtml
        SignOff.cshtml.cs
        /_Fragments
            _SectionList.cshtml
            _SectionDetail.cshtml
            _QuestionBlock.cshtml
            _QuestionMultipleChoice.cshtml
            _QuestionText.cshtml
            _SignOffCard.cshtml
            _Breadcrumb.cshtml
            _ProgressBar.cshtml

/ViewModels
    ChecklistViewModel.cs
    SectionViewModel.cs
    QuestionViewModel.cs
    AnswerViewModel.cs
    SignOffViewModel.cs
    QuestionTypes.cs

/Services
    IChecklistService.cs
    ChecklistService.cs
```

---

# 📄 Pages & Razor Partials

## `/Pages/Checklists/Index.cshtml`
```html
@page
@model IndexModel

<h2>Checklist Dashboard</h2>

<div id="section-list"
     hx-get="/Checklists/Sections"
     hx-trigger="load"
     hx-target="#section-list"
     hx-swap="innerHTML">
</div>

<div id="section-detail"></div>
```

---

## `/Pages/Checklists/Index.cshtml.cs`
```csharp
public class IndexModel : PageModel
{
    public void OnGet() { }
}
```

---

## `/Pages/Checklists/Sections.cshtml`
```html
@page
@model SectionsModel
@await Html.PartialAsync("_Fragments/_SectionList", Model.Sections)
```

---

## `/Pages/Checklists/Sections.cshtml.cs`
```csharp
public class SectionsModel : PageModel
{
    private readonly IChecklistService _service;
    public IEnumerable<SectionViewModel> Sections { get; set; }

    public SectionsModel(IChecklistService service)
    {
        _service = service;
    }

    public async Task OnGet()
    {
        Sections = await _service.GetSectionsAsync();
    }
}
```

---

## `/Pages/Checklists/SectionDetail.cshtml`
```html
@page
@model SectionDetailModel
@await Html.PartialAsync("_Fragments/_SectionDetail", Model.Section)
```

---

## `/Pages/Checklists/SectionDetail.cshtml.cs`
```csharp
public class SectionDetailModel : PageModel
{
    private readonly IChecklistService _service;
    public SectionViewModel Section { get; set; }

    public SectionDetailModel(IChecklistService service)
    {
        _service = service;
    }

    public async Task OnGet(int id)
    {
        Section = await _service.GetSectionAsync(id);
    }
}
```

---

## `/Pages/Checklists/Questions.cshtml`
```html
@page
@model QuestionsModel
@foreach (var q in Model.Questions)
{
    @await Html.PartialAsync("_Fragments/_QuestionBlock", q)
}
```

---

## `/Pages/Checklists/Questions.cshtml.cs`
```csharp
public class QuestionsModel : PageModel
{
    private readonly IChecklistService _service;
    public IEnumerable<QuestionViewModel> Questions { get; set; }

    public QuestionsModel(IChecklistService service)
    {
        _service = service;
    }

    public async Task OnGet(int sectionId)
    {
        Questions = await _service.GetQuestionsAsync(sectionId);
    }
}
```

---

## `/Pages/Checklists/SaveAnswer.cshtml`
```html
@page
@model SaveAnswerModel
@await Html.PartialAsync("_Fragments/_QuestionBlock", Model.Question)
```

---

## `/Pages/Checklists/SaveAnswer.cshtml.cs`
```csharp
public class SaveAnswerModel : PageModel
{
    private readonly IChecklistService _service;
    [BindProperty] public AnswerInputModel Input { get; set; }
    public QuestionViewModel Question { get; set; }

    public SaveAnswerModel(IChecklistService service)
    {
        _service = service;
    }

    public async Task OnPost()
    {
        Question = await _service.SaveAnswerAsync(Input);
    }
}
```

---

## `/Pages/Checklists/SignOff.cshtml`
```html
@page
@model SignOffModel
@await Html.PartialAsync("_Fragments/_SignOffCard", Model.SignOff)
```

---

## `/Pages/Checklists/SignOff.cshtml.cs`
```csharp
public class SignOffModel : PageModel
{
    private readonly IChecklistService _service;
    public SignOffViewModel SignOff { get; set; }

    public SignOffModel(IChecklistService service)
    {
        _service = service;
    }

    public async Task OnPost(int sectionId)
    {
        SignOff = await _service.SignOffSectionAsync(sectionId);
    }
}
```

---

# 🧩 Razor Partials (`/_Fragments`)

## `_SectionList.cshtml`
```html
@model IEnumerable<SectionViewModel>

<ul class="list-group">
@foreach (var section in Model)
{
    <li class="list-group-item">
        <a hx-get="/Checklists/SectionDetail?id=@section.Id"
           hx-target="#section-detail"
           hx-swap="innerHTML transition"
           hx-push-url="true">
            @section.Title
        </a>
    </li>
}
</ul>
```

---

## `_SectionDetail.cshtml`
```html
@model SectionViewModel

@await Html.PartialAsync("_Breadcrumb", Model)

<div id="question-container"
     hx-get="/Checklists/Questions?sectionId=@Model.Id"
     hx-trigger="load"
     hx-swap="innerHTML"
     hx-target="#question-container"></div>

@if (Model.RequiresSignOff)
{
    <div id="signoff-container"
         hx-get="/Checklists/SignOff?sectionId=@Model.Id"
         hx-trigger="load"
         hx-target="#signoff-container"
         hx-swap="innerHTML"></div>
}
```

---

## `_QuestionBlock.cshtml`
```html
@model QuestionViewModel

<div id="question-@Model.Id" class="question-block">
    @switch (Model.Type)
    {
        case QuestionType.MultipleChoice:
            @await Html.PartialAsync("_QuestionMultipleChoice", Model)
            break;
        case QuestionType.Text:
            @await Html.PartialAsync("_QuestionText", Model)
            break;
    }
</div>
```

---

## `_QuestionMultipleChoice.cshtml`
```html
@model QuestionViewModel

<label>@Model.Text</label>
<select name="Answer"
        hx-post="/Checklists/SaveAnswer"
        hx-include="closest .question-block"
        hx-target="#question-@Model.Id"
        hx-swap="outerHTML"
        class="form-select">
    @foreach (var option in Model.Options)
    {
        <option value="@option" selected="@(Model.Answer == option)">@option</option>
    }
</select>
```

---

## `_QuestionText.cshtml`
```html
@model QuestionViewModel

<label>@Model.Text</label>
<textarea name="Answer"
          class="form-control"
          hx-post="/Checklists/SaveAnswer"
          hx-trigger="blur changed delay:500ms"
          hx-include="closest .question-block"
          hx-target="#question-@Model.Id"
          hx-swap="outerHTML">@Model.Answer</textarea>
```

---

## `_SignOffCard.cshtml`
```html
@model SignOffViewModel

<div id="signoff-@Model.SectionId" class="signoff-card">
    @if (!Model.IsSignedOff)
    {
        <button class="btn btn-success"
                hx-post="/Checklists/SignOff"
                hx-include="#signoff-@Model.SectionId"
                hx-target="#signoff-@Model.SectionId"
                hx-swap="outerHTML">
            Sign Off Section
        </button>
    }
    else
    {
        <div class="alert alert-success">
            Signed off by @Model.SignedOffBy on @Model.Date
        </div>
    }
</div>
```

---

# 📦 ViewModels

## `ChecklistViewModel.cs`
```csharp
public class ChecklistViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<SectionViewModel> Sections { get; set; }
}
```

## `SectionViewModel.cs`
```csharp
public class SectionViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool RequiresSignOff { get; set; }
    public IEnumerable<QuestionViewModel> Questions { get; set; }
}
```

## `QuestionViewModel.cs`
```csharp
public class QuestionViewModel
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Answer { get; set; }
    public QuestionType Type { get; set; }
    public IEnumerable<string> Options { get; set; }
}
```

## `AnswerViewModel.cs`
```csharp
public class AnswerInputModel
{
    public int QuestionId { get; set; }
    public string Answer { get; set; }
}
```

## `SignOffViewModel.cs`
```csharp
public class SignOffViewModel
{
    public int SectionId { get; set; }
    public bool IsSignedOff { get; set; }
    public string SignedOffBy { get; set; }
    public DateTime? Date { get; set; }
}
```

## `QuestionTypes.cs`
```csharp
public enum QuestionType
{
    MultipleChoice,
    Text
}
```

---

# 🛠 Services

## `IChecklistService.cs`
```csharp
public interface IChecklistService
{
    Task<IEnumerable<SectionViewModel>> GetSectionsAsync();
    Task<SectionViewModel> GetSectionAsync(int id);
    Task<IEnumerable<QuestionViewModel>> GetQuestionsAsync(int sectionId);
    Task<QuestionViewModel> SaveAnswerAsync(AnswerInputModel input);
    Task<SignOffViewModel> SignOffSectionAsync(int sectionId);
}
```

---

## `ChecklistService.cs`
```csharp
public class ChecklistService : IChecklistService
{
    public Task<IEnumerable<SectionViewModel>> GetSectionsAsync()
    {
        // TODO: connect to your data source
        throw new NotImplementedException();
    }

    public Task<SectionViewModel> GetSectionAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<QuestionViewModel>> GetQuestionsAsync(int sectionId)
    {
        throw new NotImplementedException();
    }

    public Task<QuestionViewModel> SaveAnswerAsync(AnswerInputModel input)
    {
        throw new NotImplementedException();
    }

    public Task<SignOffViewModel> SignOffSectionAsync(int sectionId)
    {
        throw new NotImplementedException();
    }
}
```

---

# ✅ The scaffold is complete
This includes:
- Razor Pages
- HTMX-enabled fragments
- PageModels
- ViewModels
- Service interfaces
- Example implementation structure

If you'd like, I can now:
- Generate SQL schema
- Add authentication/role-based access
- Add client-side validation
- Add progress indicators & breadcrumbs
- Implement the service layer for real data

