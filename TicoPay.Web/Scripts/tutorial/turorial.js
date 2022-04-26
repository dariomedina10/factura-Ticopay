
const TutorialCia = 1;
const TutorialSer = 2;
const TutorialCli = 3;
const TutorialPro = 4;


function hideAllPopover() {
    $('#nav_Configuration').popover('hide');
    $('#nav_Services').popover('hide');
    $('#nav_Clients').popover('hide');
    $('#nav_Maintenance').popover('hide');
    $('#nav_Productos').popover('hide');

    return false;
}
function popOverConfig() {
    if (sessionStorage.getItem("Tutorial") == TutorialCia) {// Valida si el turorial de compañia esta en curso 
        $("#contentConfiguration").empty();
        $("#contentConfiguration").html('<i class="glyphicon glyphicon-wrench"></i> <span class="nav-label">Configuración</span><span id="nav_Configuration"></span>');
        $('#nav_Compania').popover({ placement: 'bottom', content: 'Haga clic en la opción Compañía', html: true });
        $('#nav_Compania').popover('show');
    }
}
function popOverMant() {
    if (sessionStorage.getItem("Tutorial") == TutorialSer) {// Valida si el turorial de Servicios esta en curso 
        $("#contentMaintenance").empty();
        $("#contentMaintenance").html('<i class="fa fa-edit"></i> <span class="nav-label">Mantenimientos</span><span id="nav_Maintenance" class=""></span>');
        $('#nav_Services').popover({ placement: 'bottom', content: 'Haga clic en la opción Servicios', html: true });
        $('#nav_Services').popover('show');
        $('#nav_Clients').popover('hide');
        $('#nav_Productos').popover('hide');
    }
    if (sessionStorage.getItem("Tutorial") == TutorialPro) {// Valida si el turorial de Productos esta en curso 
        $("#contentMaintenance").empty();
        $("#contentMaintenance").html('<i class="fa fa-edit"></i> <span class="nav-label">Mantenimientos</span><span id="nav_Maintenance" class=""></span>');
        $('#nav_Productos').popover({ placement: 'bottom', content: 'Haga clic en la opción Productos', html: true });
        $('#nav_Productos').popover('show');
        $('#nav_Clients').popover('hide');
        $('#nav_Services').popover('hide');
    }
    if (sessionStorage.getItem("Tutorial") == TutorialCli) {// Valida si el turorial de clientes esta en curso 
        $("#contentMaintenance").empty();
        $("#contentMaintenance").html('<i class="fa fa-edit"></i> <span class="nav-label">Mantenimientos</span><span id="nav_Maintenance" class=""></span>');
        $('#nav_Clients').popover({ placement: 'bottom', content: 'Haga clic en la opción Clientes', html: true });
        $('#nav_Clients').popover('show');
        $('#nav_Services').popover('hide');
        $('#nav_Productos').popover('hide');
    }
}
$('#btnCompanyT').click(function (e) {
    //hideAllPopover();
    $('#nav_Maintenance').popover('hide');
    $('#nav_Productos').popover('hide');
    $('#nav_Services').popover('hide');
    $('#nav_Clients').popover('hide');
    sessionStorage.setItem("Tutorial", TutorialCia);
    moveScroll();
    if ($('#contentConfiguration').parent().hasClass('active')) {
        popOverConfig();
    } else {
        
        $('#nav_Configuration').popover({ content: 'Haga clic en el menú Configuración', html: true });
        $('#nav_Configuration').popover('show');

    }

    return false;
});
$('#btnProductT').click(function (e) {

    $('#nav_Configuration').popover('hide');
    $('#nav_Compania').popover('hide');
    sessionStorage.setItem("Tutorial", TutorialPro);
    moveScrollup();
    if ($('#contentMaintenance').parent().hasClass('active')) {

        popOverMant();
    } else {


        $('#nav_Maintenance').popover({ content: 'Haga clic en el menú Mantenimiento', html: true });
        $('#nav_Maintenance').popover('show');
    }

    return false;
});
$('#btnServicesT').click(function (e) {

    $('#nav_Configuration').popover('hide');
    $('#nav_Compania').popover('hide');
    sessionStorage.setItem("Tutorial", TutorialSer);
    moveScrollup();
    if ($('#contentMaintenance').parent().hasClass('active')) {
        
        popOverMant();
    } else {
        
        
        $('#nav_Maintenance').popover({ content: 'Haga clic en el menú Mantenimiento', html: true });
        $('#nav_Maintenance').popover('show');
    }

    return false;
});
$('#btnClientsT').click(function (e) {

    $('#nav_Configuration').popover('hide');
    $('#nav_Compania').popover('hide');
    sessionStorage.setItem("Tutorial", TutorialCli);
    moveScrollup();
    if ($('#contentMaintenance').parent().hasClass('active')) {
        popOverMant();
    } else {
        
        $('#nav_Maintenance').popover({ content: 'Haga clic en el menú Mantenimiento', html: true });
        $('#nav_Maintenance').popover('show');
    }
    return false;
});

$('#contentConfiguration').click(function (e) {

    popOverConfig();
    return false;
});
$('#contentMaintenance').click(function (e) {

    popOverMant();
    return false;
});


//function popOverCreateSer() {
//    if (sessionStorage.getItem("Tutorial") == TutorialSer) {
//        $('#nav_NewServices').popover({ placement: 'bottom', content: 'Haga clic en la opción de Crear Servicio', html: true });
//        $('#nav_NewServices').popover('show');
//    };
//}
//function popOverSaveSer() {
//    if (sessionStorage.getItem("Tutorial") == TutorialSer) {// Valida si el turorial de compañia esta en curso 
//        $("#contentCreateNewServ").empty();
//        $("#contentCreateNewServ").html('<span class="glyphicon glyphicon-plus" aria-hidden="true"></span>&nbsp;Crear Servicio<span id="nav_NewServices"></span>');
//        $('#popOver_NameServices').popover({ placement: 'top', content: 'Complete todos los datos requeridos del servicio', html: true });
//        $('#popOver_NameServices').popover('show');
//        $('#popOver_SaveServices').popover({ placement: 'left', content: 'Una vez completados los datos, haga clic en Guardar', html: true });
//        $('#popOver_SaveServices').popover('show');
//        sessionStorage.removeItem("Tutorial");
//    }
//}

function popOverCreate(Tutorial, namePop, messg) {
    if (sessionStorage.getItem("Tutorial") == Tutorial) {
        $(namePop).popover({ placement: 'bottom', content: messg, html: true });
        $(namePop).popover('show');
    };
}

function popOverSave(Tutorial, btn, pos2, fintutorial) {
    if ((sessionStorage.getItem("Tutorial") == Tutorial) ) {
        $("#contentCreateNew").empty();
        $("#contentCreateNew").html(btn);
        $('#popOver_Name').popover({ placement: 'top', content: 'Complete todos los datos requeridos', html: true });
        $('#popOver_Name').popover('show');
        $('#popOver_Save').popover({
            placement: pos2 
            , content: 'Una vez completados los datos, haga clic en Guardar', html: true });
        $('#popOver_Save').popover('show');
        if (fintutorial)
            sessionStorage.removeItem("Tutorial");

    }

}
function popOverGuardarCli() {
    $('#popOver_Name').popover('hide');
    $('#popOver_Save').popover('hide');
    sessionStorage.removeItem("Tutorial");
}

function moveScroll()
{
    if ((sessionStorage.getItem("Tutorial") == TutorialCia)) {
        if (window.innerHeight) 
            window.scrollBy(0, window.innerHeight*5 );     
        else 
            window.scrollBy(0, document.body.clientHeight*5 );

       
    }
    
}
function moveScrollup() {
   
    window.scrollTo(0, 0);     

}