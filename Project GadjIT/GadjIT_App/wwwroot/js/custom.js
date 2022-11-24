/* 

1. Add your custom JavaScript code below
2. Place the this code in your template:



*/

Backimg = new Image()
Backimg.src = "/css/images/GhostImages/DocDragGhost2.jpg";


function ShowModal(modalId) {
    $('#' + modalId).modal('show');
    $('#' + modalId).appendTo(document.body);
}

function CloseModal(modalId) {
    $('#' + modalId).modal('hide');
}

function HideAll() {
    $('.modal').modal('hide');
}

function DownloadTextFile(filename, fileContent) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + fileContent;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function moveToPosition(position) {
    var el = document.getElementById('mainScroll');
    el.scrollTop = position;
}

function getElementPosition() {
    var el = document.getElementById('mainScroll');
    return el.scrollTop;
}

function addEventListener(elementId) {
    var el = document.getElementById(elementId);
    if (el) {
        el.addEventListener("dragstart", function (e) {
            e.dataTransfer.setDragImage(Backimg, 0, 0);
        }, false);

    }
}

function loadDataTable(tableId) {

    $(tableId).DataTable({

            dom: 'Bflrtip',

            retrieve: true,

            buttons: [

                'copyHtml5',

                'excelHtml5',

                'csvHtml5',

                'pdfHtml5'

            ],

            pagingType: "full_numbers",

            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],

            autoWidth: false,

            scrollX: true,

            order: []

    });

}

function destroyDataTable(tableId) {

    const table = $(tableId).DataTable();

    table.destroy();


}

/* Pop-up Notification / Animation */
function showNotification(NotificationType, strMessage, intDuration) {
  var notificationModal = document.getElementById("notification-modal");
  var InnerBlock = document.getElementById("notification-inner");
  var bgClass = "bg-" + NotificationType.toLowerCase();
  var iconID = "notification-icon-" + NotificationType;


  if(InnerBlock.innerHTML == "") //Do not show notification if previous notification is still visible
  {

    InnerBlock.innerHTML = strMessage;
    
    notificationModal.classList.add(bgClass);

    var items = document.getElementsByClassName("notification-icon");
    for (var i=0; i < items.length; i++) {
      items[i].style.display = "none";
    }
    
    document.getElementById(iconID).style.display = "flex";
    
    notificationModal.classList.add("notification-show");
    notificationModal.classList.remove("notification-hide");


    setTimeout(()=>{
      notificationModal.classList.remove("notification-show");
      notificationModal.classList.add("notification-hide");
    }, intDuration + 10);

    setTimeout(()=>{
        notificationModal.classList.remove("notification-hide");
        notificationModal.classList.remove(bgClass)
        InnerBlock.innerHTML = "";

        for (var i=0; i < items.length; i++) {
          items[i].style.display = "none";
        }

    }, intDuration + 800);
    
    

  }
  
}

function showNotification_test(NotificationType) {
  switch(NotificationType) {
  case "Success":
    showNotification(NotificationType, "Success")
    break;
  case "Warning":
    showNotification(NotificationType, "Warning: this is a medium msg")
    break;
  case "Danger":
    showNotification(NotificationType, "Error: this is a large message to display")
    break;
  default:
    // code block
  }
}

//Prevents the "double render" occuring so the page does
//not flicker or show objects twice after a refresh
//Called by MainLayout.razor>OnAfterRenderAsync
function showPageAfterFirstRender() {
  var items = document.getElementsByClassName("hide-until-full-render");
  while (items.length) {
    items[0].classList.remove("hide-until-full-render");
    
  }

  //Make sure menus are closed especially Responsive menu
  //fncCloseResponsiveMenu();
}

function addClassById(objId, className) {

  var obj = document.getElementById(objId);
  if(obj != null){
    obj.classList.add(className);
  }

}

function addClassByClass(objClass, className) {

  var items = document.getElementsByClassName(objClass);
  var numItems = items.length;
  for(var item = 0; item < numItems; item++) {
    items[item].classList.add(className);  
  }

}

function DownloadDocument(filename, fileContent) {

  var link = document.createElement('a');

  link.download = filename;

  link.href = "data:application/octet-stream;base64," + fileContent;

  document.body.appendChild(link); // Needed for Firefox

  link.click();

  document.body.removeChild(link);

}

DataTable.datetime('D MMM YYYY');

function DataTablesAdd(tableID) {
  
  // Call the below function
  waitForElementToDisplay(tableID,function(){

    ApplyDataTables(tableID);

  },1000,9000);

  
  
  showPageAfterFirstRender();
}



function fncScrollTop(){
  window.scroll({
      top: 0, 
      left: 0, 
      behavior: 'smooth'
    }); 
}




function waitForElementToDisplay(selector, callback, checkFrequencyInMs, timeoutInMs) {
  
  var startTimeInMs = Date.now();
  (function loopSearch() {
    if (document.querySelector(selector) != null) {
      callback();
      return;
    }
    else {
      setTimeout(function () {
        if (timeoutInMs && Date.now() - startTimeInMs > timeoutInMs)
          return;
        loopSearch();
      }, checkFrequencyInMs);
    }
  })();
}
