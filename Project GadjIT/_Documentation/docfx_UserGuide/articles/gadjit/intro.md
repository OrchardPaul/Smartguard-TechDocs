<p align="center" width="100%">
    <img src="/images/gadjit-logo-light.jpg" title="GadjIT Logo")
</p>  

# What is GadjIT?
Smartflow is a resulting workflow that can be installed onto compatible Case Management Systems. GadjIT is an online tool developed by [Orchard Rock Development](www.orchardrock.co.uk "Orchard Rock website") for creating and maintaining Smartflows. 

## GadjIT Overview
*(from 2 mins 22 secs onwards)*
> [!Video https://player.vimeo.com/video/600217335?h=ab5097179c]

# Development Options
As you would expect with a modern online facility, GadjIT offers an intuative easy to navigate interface to quickly create and maintain Smartflows. All changes made within GadjIT are instantly available within your Case Management System. GadjIT offers two methods of development, the GadjIT toolkit or Excel.

## GadjIT Toolkit
There is no code required within GadjIT and almost all elements are selected via dropdowns and filtered lists so it is not only an efficient way of working but free from coding errors. The screens are set in an order to assist users with navigation.  

## Develop within Excel
Even though GadjIT offers a rapid development toolkit, it also offers the ability to develop within Excel. This ability is a real strength of GadjIT as Excel is an application most users are already familiar with. 

**Self Documenting**
By exporting a Smartflow to Excel, you provide full documentation in an instance. Several tabs reflect the various screens within GadjIT, each listing all relevant details in an easy to understand format.

**Import Changes**
In addition to exporting a Smartflow to Excel, you can also import any changes back. Simply alter details in the Excel spreadsheet using the lookup tab for access to case precedents before importing back into GadjIT. Once imported, the changes are instantly reflected both in GadJIT as well as the Case Management System.

```mermaid
%%{init: { 'logLevel': 'debug', 'theme': 'base', 'gitGraph': {'showBranches': true, 'showCommitLabel':true,'mainBranchName': 'GadjIT'}} }%%
      gitGraph
        commit id:"Open Smartflow"
        commit id:"Export to Excel"
        branch Excel
        checkout Excel
        commit id:"Add new document"
        checkout GadjIT
        checkout Excel
        checkout GadjIT
        merge Excel
        checkout GadjIT
        commit id:"Smartflow Updated"
        
```
**Chart 1** - Process: Updating a document and fee via Excel

## Process: Updating Smartflows via GadjIT
The following chart details the process for updating a Smartflow via GadjIT. 

> [!NOTE] 
> The CMS is updated automatically as soon as any change within GadjIT is made.

```mermaid
sequenceDiagram
    par Update CMS via GadjIT
        User->>GadjIT: Update Smartflow
    and
        GadjIT-->>CMS: (Updated Smartflow)
    end
```
**Chart 2** - Updating Smartflow in GadjIT

## Process: Updating Smartflows via Excel
The following chart details the process for updating a Smartflow via Excel. 

> [!NOTE] 
> GadjIT and the CMS are both updated automatically as soon as the spreadsheet is imported back into GadjIT

```mermaid
sequenceDiagram
    User->>GadjIT: Export 
    GadjIT-->>Excel: Smartflow
    
    User->>Excel: Update
    User->>Excel: Import
    

    par Update CMS via Excel
        Excel-->>GadjIT: (Updated Spreadsheet)
    and
        GadjIT-->>CMS: (Updated Smartflow)
    end
```
**Chart 3** - Updating Smartflow via Excel

# Advantages of GadjIT
- No Coding required, anyone can develop in GadjIT
- Toolkit offers dropdowns and lookups for speed and accuracy 
- Rich web interface resulting in easy to follow screens
- Rapid development via Excel 
- Self documenting so no need for supporting flowcharts 
- Organised for easy navigation 
- Go live with just a couple of clicks

