# Building a Smartflow in GadjIT
Smartflows are developed around existing documents within the case management database. You use **GadjIT** to simply select the relevant documents and steps for the **Smartflow** you are creating and then add the details of the actions required once that document is selected by the end user. Required actions can include:

- Creating a `scheduled item` (user task)
- Creating a `history note`
- Automatically `attaching` another document
- Setting or changing the Smartflow `status`
- Adding a `user message`

## Creating your status list
The ``status`` of the **Smartflow** can be changed on the selection of a document or step from within the ``Next Action`` area of the smartflow screen. It is therefore sensible to create your status list before you start to add documents to the flow so they are available to configure in the document properties. A ``status`` can also be configured to *end* a **Smartflow** so when that particular ``status`` is set (by processing the document) the **Smartflow** will not be rescheduled.  
> [!TIP] 
> It is advised that the user should be presented with an alternative course of action should the current **Smartlow** *end* as a consequence of the ``status`` change. 

To add or amend a ``status`` select the status menu item, click the purple **+NEW** button and simply type in your required status description. To amend and existing ``status`` select Edit from the ellipsis next to the ``status`` you wish to amend. 

<a href="/images/gadjIT_StatusCreation.jpg" target="_blank" ><img src="/images/gadjIT_StatusCreation.jpg" alt="Status Creation" title="Status" style="width: 250px;" class="shadow"/></a>         
**Figure 1** - Creating and amending a status.

> [!CAUTION] 
> If you amend or delete a ``status`` that is being used by a document you will need to visit the document properties and reselect the amended or required ``status`` or the **Smartflow** will fail. 

## Adding a new document
From the home page of the Smartflow, click on the documents icon. <img src="/images/gadjIT_DocumentsIcon.jpg" alt="Documents" title="Docs Icon"/>    
Next, click the New button icon <img src="/images/gadjIT_NewIcon.jpg" alt="Add New" title="New Icon"/> to open the document details modal. Here you can click on the drop down arrow in the Select field to view all available documents and steps. 

<a href="/images/gadjIT_SelectDocument.jpg" target="_blank" ><img src="/images/gadjIT_SelectDocument.jpg" alt="Document Selection" title="Select Doc" style="width: 250px;" class="shadow"/></a>         
**Figure 2** - Lists all available documents and workflow steps.

> [!TIP]  
> If you know the name of the document you want to add, start typing its' name into the `Filter` field to reduce the list.

Once you have selected the document you wish to add you can start to configure the actions that you want to take place when the end user processes the document via the **Smartflow** screen.

Key document properties include:

|Field Name               |Action    
|:------------------------|:------------------------
|Display Name             |Allows you to display an alternative document name in the Smartflow          |
|Schedule As Name         |Denotes the name of the scheduled task|
|Reschedule Days          |In how many days the scheduled task is due|
|Complete As Name         |The description of the history item that will be inserted                  |
|Next Status              |Select the Status to be set by the selected item|
|User Message             |An advisory message displayed at the bottom left of the Smartflow screen   |
|Pop up Message (Alert)   |An alert message that the user must click to move on                        |                            

**Figure 3** - Explanation of configurable document properties.

## Adding linked documents/steps
**GadjIT** allows you to *attach* other documents to your main document so the additional document is automatically produced by the **Smartflow** every time the main document is selected and processed. This avoids users forgetting to produce important items, e.g. sending a court application document with the covering letter to the court.

To *attach* a document to your main document hover over the right chevron of the ``Links`` menu item from the ellipsis next to the relevant document and then select **+ Add New**.     
<a href="/images/gadjIT_Attachments.jpg" target="_blank" ><img src="/images/gadjIT_Attachments.jpg" alt="Document Selection" title="Attach Doc" style="width: 250px;" class="shadow"/></a>         
**Figure 4** - Attaching a document to a main document.

Select the required document from the list and click **Save**.  
<a href="/images/gadjIT_NewLinkedItem.jpg" target="_blank" ><img src="/images/gadjIT_NewLinkedItem.jpg" alt="Document Selection" title="Select Doc" style="width: 250px;" class="shadow"/></a>  
**Figure 5** - Selecting the required document to *attach*.
> [!TIP]  
> If you know the name of the document you want to add, start typing its' name into the `Filter` field to reduce the list.

## Re-ordering the list of documents
Once you have created all of your documents within your **Smartflow** you can very easily re-order the list should you wish to do so by simply dragging and dropping the item you want to move to a different location in the list.     
<a href="/images/gadjIT_ReorderingLists.jpg" target="_blank" ><img src="/images/gadjIT_ReorderingLists.jpg" alt="Re-ordering Lists" title="Re-order" style="width: 250px;" class="shadow"/></a>  
**Figure 5** - Drag and drop to re-order a list*.
> [!TIP]  
> The drag and drop feature is available to re-order in all lists in **GadjIT**.