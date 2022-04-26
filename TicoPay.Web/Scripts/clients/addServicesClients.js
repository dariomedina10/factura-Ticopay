$(document).ready(function () {
    var createlUrl = "/Clients/CreateAjax";
    var updateUrl = "/Clients/Edit";

    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    jQuery.fn.validateTab = function (toTab, label) {
        var isValid = true;

        $(this).find("input, select,textarea").each(function () {
            if (isValid) {
                isValid = $(this).valid();
               
                    
            } else {
                $(this).valid();
            }
        });
        if (!isValid) {
            $(label).addClass('control-label');

        } else
        {
            $(label).removeClass('control-label');
        }
        return isValid;
        //if (isValid) {
        //    if (!toTab) {
        //        toTab = 0;
        //    }
          
        //}
     
       
    }

    function number_format(amount, decimals) {

        amount += ''; // por si pasan un numero en vez de un string
        amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto

        decimals = decimals || 0; // por si la variable no fue fue pasada

        // si no es un numero o es igual a cero retorno el mismo cero
        if (isNaN(amount) || amount === 0)
            return parseFloat(0).toFixed(decimals);

        // si es mayor o menor que cero retorno el valor formateado como numero
        amount = '' + amount.toFixed(decimals);

        var amount_parts = amount.split('.'),
            regexp = /(\d+)(\d{3})/;

        while (regexp.test(amount_parts[0]))
            amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

        return amount_parts.join('.');
    }
    var data, grid, gridGroup, grid2;


    grid = $("#gridService").grid({
        //dataSource: data,
        dataKey: "Id",
        primaryKey: "Id",
        uiLibrary: "bootstrap",
        notFoundText: "No se han agendados Servicios",
        columns: [
            { field: "Type", title: "Type", hidden: true },
            { field: "Id", title: "IdService", hidden: true },
            { field: "IdDetails", title: "IdService", hidden: true },
            //{ field: "IDGrid", title: "#", width: 50, hidden: true },
            { field: "Name", title: "Servicio", width: "60%", sortable: true },
            { field: "Quantity", title: "Cantidad", sortable: true },
            { field: "DiscountPercentage", title: "% Descuento", sortable: true },
            { field: "Delete", title: "", width: 34, type: "icon", icon: "glyphicon-remove", tooltip: "Delete", events: { "click": Remove } }
        ]
    });


    gridGroup = $("#gridGroup").grid({
        //dataSource: data,
        dataKey: "IDGrid",
        primaryKey: "Id",
        uiLibrary: "bootstrap",
        notFoundText: "No se han agendados Grupos de Servicios",
        columns: [
            { field: "Type", title: "Type", hidden: true },
            { field: "Id", title: "IdGroup", hidden: true },
            { field: "IdDetails", title: "IdService", hidden: true },
            //{ field: "IDGrid", title: "#", width: 50, hidden: true },
            { field: "Name", title: "Grupo de Servicio", width: "60%", sortable: true },
            { field: "Quantity", title: "Cantidad", sortable: true },
            { field: "DiscountPercentage", title: "% Descuento", sortable: true },
            { field: "Delete", title: "", width: 34, type: "icon", icon: "glyphicon-remove", tooltip: "Delete", events: { "click": Remove } }
        ]
    });

    function writeError(control, msg, type) {
        if (type === "success") {
            abp.notify.success(msg);
        } else if (type === "error") {
            abp.notify.error(msg);           
            //var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
            //$("#" + control).html(alert);
        } else { abp.notify.warn(msg); }
    }

    function Remove(e) {
        var id = "#ID" + e.data.record.Type;
        $("#type").val(e.data.record.Type);
        clearErrors();
        $("#Confirmation").modal("show");
        //$(id).val(e.data.id);
        $(id).val(e.data.record["Id"]);
        return false;
    }

    function hideNewService(tipo) {
        var cant = "#Cant" + tipo;
        var discount = "#discountPercentage" + tipo;
        var dd = "#dd" + tipo + "s";
        var newDiv = "#new" + tipo;
        $(cant).val("");
        $(discount).val("");
        $(dd).val("");
        $(newDiv).attr("Hidden", "Hidden");
    }

    function clearErrors() {
        $('#msgErrorAnyModal').html('');
        $('#IndexAlerts').html('');
        $('#alertPaymentInvoice').html('');
        $('#msgNotaError').html('');
    }


    $("#btnOkAddNewService").click(function () {

        //clearErrors();

        if ($('#ddServices').val() == "") {
            writeError('msgNotaError', 'Debe seleccionar un Servicio', 'error');
            abp.ui.clearBusy();
            return false;
        }
        var infoServices = $('#ddServices').val().split("_");

        var data = grid.getById(infoServices[0]);
        if (data != null) {
            writeError('msgNotaError', 'El servicio ya se encuentra seleccionado', 'error');
            abp.ui.clearBusy();
            return false;
        }

        var descuento = $.isNumeric(parseFloat($("#discountPercentageService").val())) ? parseFloat($("#discountPercentageService").val()): 0;
        var cantidad = $.isNumeric(parseFloat($('#CantService').val())) ? parseFloat($('#CantService').val()):0;
        if (cantidad <= 0) {
            writeError('msgNotaError', 'Debe ingresar una cantidad mayor a cero(0)', 'error');
            $('#CantService').focus();
            abp.ui.clearBusy();
            return false;
        }       
        
        if (descuento < 0 || descuento > 100) {
            writeError('msgNotaError', 'Debe ingresar un % descuento mayor o igual a 0  y menor que 100', 'error');
            $('#discountPercentageService').focus();
            abp.ui.clearBusy();
            return false;
        }
        //"IDgrid": grid.count() + 1,

        grid.addRow({ "Type": "Service", "Id": infoServices[0], "IdDetails":null , "Name": $('#ddServices option:selected').text(), "Quantity": number_format(cantidad, 2), "DiscountPercentage": number_format(descuento, 2) });


        hideNewService('Service');
    });

    $(".btnAddService").click(function () {
        $("#newService").removeAttr("Hidden");
    });


    $("#btnCancelAddNewService").click(function () {
        hideNewService('Service');

    });

    $("#btnOkAddNewGroup").click(function (e) {

        //clearErrors();
        e.stopPropagation();
        if ($('#ddGroups').val() == "") {
            writeError('msgNotaError', 'Debe seleccionar un Grupo de Servicios', 'error');
            abp.ui.clearBusy();
            return false;
        }
        var infoGroups = $('#ddGroups').val().split("_");
        var data = gridGroup.getById(infoGroups[0]);

        if (data != null) {
            writeError('msgNotaError', 'El grupo de servicios ya se encuentra seleccionado', 'error');
            abp.ui.clearBusy();
            return false;
        }
       

        var descuento = $.isNumeric(parseFloat($("#discountPercentageGroup").val())) ? parseFloat($("#discountPercentageGroup").val()) :0 ;
        var cantidad = $.isNumeric(parseFloat($('#CantGroup').val())) ? parseFloat($('#CantGroup').val()):0 ;
        if (cantidad <= 0) {
            writeError('msgNotaError', 'Debe ingresar una cantidad mayor a cero(0)', 'error');
            $('#CantGroup').focus();
            abp.ui.clearBusy();
            return false;
        }

        if (descuento < 0 || descuento > 99) {
            writeError('msgNotaError', 'Debe ingresar un % descuento mayor o igual a 0  y menor que 100', 'error');
            $('#discountPercentageGroup').focus();
            abp.ui.clearBusy();
            return false;
        }
      
        //"ID": gridGroup.count() + 1,
        gridGroup.addRow({ "Type": "Group", "Id": infoGroups[0], IdDetails:null, "Name": $('#ddGroups option:selected').text(), "Quantity": number_format(cantidad, 2), "DiscountPercentage": number_format(descuento, 2) });


        hideNewService('Group');
    });

    $(".btnAddGroup").click(function () {
        $("#newGroup").removeAttr("Hidden");
    });


    $("#btnCancelAddNewGroup").click(function () {
        hideNewService('Group');
    });

    $("#btnModalOkDeleteOnDemand").click(function (e) {
        var type = $("#type").val();
        var idName = "#ID" + type;
        $("#Confirmation").modal("hide");
        //var id = parseInt($(idName).val()) - 1;
        var id = $(idName).val();
        if (type == 'Service')
            grid.removeRow(id);
        else
            gridGroup.removeRow(id);
    });

    $('#ddServices').change(function () {

        if ($('#ddServices').val() == "") {
            $("#CantService").val("");
            $("#discountPercentageService").val("");
        } else {
            var infoServices = $('#ddServices').val().split("_");
            $("#CantService").val(number_format(infoServices[1],2));
            $("#discountPercentageService").val(number_format(infoServices[2],2));
        }

    });



    $('#ddGroups').change(function () {

        if ($('#ddGroups').val() == "") {
            $("#CantGroup").val("");
            $("#discountPercentageGroup").val("");
        } else {

            $("#CantGroup").val("1.00");
            $("#discountPercentageGroup").val("0.00");
        }

    });

    //function validarTab()
    //{
    // return ( $("#tab-1").validateTab(0, "#linkDatos")&& 
    //    $("#tab-2").validateTab(1, "#linkLocal"));
    //}
    
    //$('.headertab').click(function (e) {
    //   // debugger;
    //       // e.preventDefault();       
    //    return validarTab();
            
    //});
  
    

        function addClients(grid, gridGroup) {
        //clearErrors();

            if ($("#Name").checkValidity()) {
                var formContainer = $("#formNewClients");
                var d = formContainer.serializeObject();

                var params = JSON.stringify({
                    viewModel: d,
                    listService: grid.getAll(),
                    listGroupService: gridGroup.getAll()
                });


                abp.ui.setBusy();
                $.ajax({
                    url: createlUrl,
                    type: "post",
                    cache: false,
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    //dataType: "json",
                    success: function (data) {
                        //data = data.replace("<!DOCTYPE html>", "").replace("<html>", "").replace("</html>", "");
                        $("#content").html(data);
                        // writeError('IndexAlerts', 'Factura generada correctamente', 'success');
                        abp.ui.clearBusy();
                    }, error: function (xhr, ajaxOptions, thrownError) {
                        writeError("IndexAlerts", "¡Error al crear el cliente!", "error");
                        abp.ui.clearBusy();
                    }
                });
            }
     

       
    }

        function updateClients(grid, gridGroup) {
            //clearErrors();

          
            var formContainer = $("#formEditsClients");
                var d = formContainer.serializeObject();

                var params = JSON.stringify({
                    viewModel: d,
                    listService: grid.getAll(),
                    listGroupService: gridGroup.getAll()
                });


                abp.ui.setBusy();
                $.ajax({
                    url: updateUrl,
                    type: "post",
                    cache: false,
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    //dataType: "json",
                    success: function (data) {                        
                        $("#content").html(data);                        
                        abp.ui.clearBusy();
                    }, error: function (xhr, ajaxOptions, thrownError) {
                        writeError("IndexAlerts", "¡Error al actualizar el cliente!", "error");
                        abp.ui.clearBusy();
                    }
                });
            



        }


    $('#formNewClients').on('submit', function (event) {
        event.preventDefault();

        if (this.checkValidity() == false) {
            //if (!($("#tab-1").validateTab(0, "#linkDatos") && $("#tab-2").validateTab(1, "#linkLocal"))) {
            //    writeError("validation", "Datos invalidos o faltantes. Por favor verifique.", "error");
            event.preventDefault();
            return false;
            //}                
        }
        addClients(grid, gridGroup);
        event.preventDefault();
        // ...
    });

    $('#formEditsClients').on('submit', function (event) {
        event.preventDefault();

        if (this.checkValidity() == false) {
            //$("#tab-1").validateTab(0, "#linkDatos");
            //$("#tab-2").validateTab(1, "#linkLocal");
            //writeError("validation", "Datos invalidos o faltantes. Por favor verifique.", "error");
            event.preventDefault();
            return false;
        } else {
            updateClients(grid, gridGroup);
            event.preventDefault();
            //event.stopPropagation();
        }
    }); 
    

        //$('#btnSaveAjax').click(function () {
        //    //document.querySelectorAll('input[type="text"]').willValidate;
        //    addClients(grid, gridGroup); 

        //});
});