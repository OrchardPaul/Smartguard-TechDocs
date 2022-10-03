# Building a Smartflow in GadjIT
Smartflows are developed around exisiting documents within the case management database. You use **GadjIT** to simply select the relevant docuemnts and steps for the **Smartflow** you are creating and then add the details of the required actions once that document is selected by the end user. Required actions can include:

- Creating a `scheduled item` (user task)
- Creating a `history note`
- Automatically `attaching` another document
- Setting or changing the Smartflow `status`
- Adding a `user message`

## Adding a new document
From the home page of the Smartflow click on the documents icon. <img src="/images/gadjIT_Documents_Icon.jpg" alt="Documents" title="Docs Icon"/>    
Click the New button icon <img src="/images/gadjIT_New_Icon.jpg" alt="Add New" title="New Icon"/> to open the document details modal. Here you can click on the drop down arrow in the Select field to view all available documents and steps. 

<a href="/images/gadjIT_Select_Document.jpg" target="_blank" ><img src="/images/gadjIT_Select_Document.jpg" alt="Document Selection" title="Select Doc" style="width: 250px;" class="shadow"/></a>         
**Figure 1** - Lists all available documents and workflow steps.

> [!TIP]  
> If you know the name of the document you want to add, start typing its' name into the `Filter` field to reduce the list.

Once you have selected the document you wish to add you can start to configure the actions that you want to take place once the document has been selected and processed by the end user.

|Field Name               |Action    
|:------------------------|:------------------------
|Display Name             |Allows you to display an alternative document name in the Smartflow          |
|Schedule As Name         |Denotes the name of the scheduled task|
|Reschedule Days          |In how many days the scheduled task is due|
|Complete As Name         |The description of the history item that will be inserted                  |
|Next Status              |Select the Status to be set by the selected item|
|User Message             |An advisory message displayed at the bottom left of the Smartflow screen   |
|Pop up Message (Alert)   |An alert message that the user must click to move on                        |                            

**Figure 2** - Explanation of configurable document properties.

