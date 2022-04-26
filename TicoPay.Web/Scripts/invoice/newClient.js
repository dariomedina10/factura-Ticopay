$(document).ready(function () {
    var positiveBalanceUrl = "/Invoice/SearchPositiveBalance";
    var createClientNew = "/Invoice/CreateClient";

    function writeError(control, msg, type) {
        if (type === "success") {
            abp.notify.success(msg, "");
        } else if (type === "error") {
            abp.notify.error(msg, "");
            var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
            $("#" + control).html(alert);
        } else { abp.notify.warn(msg, ""); }

    }
    function isValidEmailAddress(emailAddress) {
    
		var pattern = new RegExp(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,11})+$/i);
        return pattern.test(emailAddress);
    }

    function saveClientNew(clientId, name, code, typeIdentification, identification, identificacionExtranjero, phoneNumber, mobilNumber, email) {
        $('#ClientName_text').val(name);
        $('#ClientId_hidden').val(clientId);

        $('#IdentificationTypes_DD').val(typeIdentification);
        debugger;
        var ident = typeIdentification == 'NoAsiganda' ? identificacionExtranjero : identification;
        $('#text_identification').val(ident);

        $('#text_ClientPhoneNumber').val(phoneNumber);
        $('#text_ClientMobilNumber').val(mobilNumber);
        $('#text_ClientEmail').val(email);

        var typecoin = $('#CoinTypes_DD').val();
        var saldoFavor = 0;
        abp.ui.setBusy();
        $.ajax({
            url: positiveBalanceUrl,
            data: { clientId: clientId, typeCoin: typecoin  },
            success: function (data) {
                if (data.success == true) {
                    $('#positiveBalanceHidden').val(data.result.code);
                    $('#positiveBalanceLabel').html(data.result.code);
                }
                abp.ui.clearBusy();
            },
            error: function (err) {
                console.log(err);
                writeError('IndexAlerts', 'Error al buscar saldo a favor.', err);
                abp.ui.clearBusy();
            }
        });

        return false;
    }

    $("#btnModalOkAddNewClient").click(function (e) {

        //clearErrors();
        abp.ui.setBusy();

        if ($('#NameClient').val() == "") {
            writeError('msgNotaError', 'Debe ingresar un Nombre', 'error');
            abp.ui.clearBusy();
            return false;
        }

        if ($('#typeidentificacion_DD').val() != "Cedula_Juridica") {
            if ($('#clientLastName').val() == "") {
                writeError('msgNotaError', 'Debe ingresar un Apellido', 'error');
                abp.ui.clearBusy();
                return false;
            }
        }
              

    
        if (($('#EmailClient').val() != "")&& (!isValidEmailAddress($('#EmailClient').val()))) {
            writeError('msgNotaError', 'El Correo Electrónico es invalido, ej. cliente@ticopay.com', 'error');
            abp.ui.clearBusy();
            return false;
        }
        

        $.ajax({
            url: createClientNew,
            type: "POST",
            cache: false,
            data: {
                name: $('#NameClient').val(), lastName: $('#clientLastName').val(), identification: $('#IdIdentificacion').val(),
                email: $('#EmailClient').val(), identificationType: $('#typeidentificacion_DD').val(), identificacionExtranjero: $('#IdExtranjero').val()
            },
            success: function (data) {
                if (data.result.success == true) {
                    saveClientNew(data.result.id, data.result.name, null, data.result.typeIdentification, data.result.identification, data.result.identificacionExtranjero, data.result.phoneNumber, data.result.mobilNumber, data.result.email)
                    abp.ui.clearBusy();
                    $("#addNewClient").modal("hide");
                    $('#NameClient').val("");
                    $('#clientLastName').val("");
                    $('#IdIdentificacion').val("");
                    $('#EmailClient').val("");
                    $('#typeidentificacion_DD').val("");
                    $('#IdExtranjero').val("");
                    $('#labelLastName').attr('class', 'control-label');

                }
                if (data.result.success == false) {
                    writeError("msgNotaError", data.result.name, "error");
                    abp.ui.clearBusy();
                }
            }, error: function (err) {
                writeError("msgNotaError", "¡Error al crear el cliente!", "error");
                abp.ui.clearBusy();
            }
        });
        return false;
    });

    $('#typeidentificacion_DD').on('change', function (e) {
        
        checkValidationIdentificacion();
    });

    function checkValidationIdentificacion() {
        var identificacion = $('#typeidentificacion_DD').val();
       
        if (identificacion == 'Cedula_Juridica') {
            $('#labelLastName').removeClass('control-label');
            $('#clientLastName').attr('required', false);
        } else {
            $('#labelLastName').addClass('control-label');
            $('#clientLastName').attr('required', true);
        }
        if (identificacion == 'NoAsiganda') {
            $('#labelIdentificacion').removeClass('control-label');
            $('#IdIdentificacion').attr('required', false);
            $('#IdIdentificacion').attr('Disabled', true);
            $('#IdExtranjero').removeAttr('Disabled');
            $('#IdIdentificacion').val('');
            $('#labelExtranjero').addClass('control-label');
            $('#IdExtranjero').attr('required', true);
           // $('#IdIdentificacion-error').remove();
            $('#IdExtranjero').focus();

        } else {
            $('#labelIdentificacion').addClass('control-label');
            $('#IdIdentificacion').attr('required', true);
            $('#labelExtranjero').removeClass('control-label');
            $('#IdExtranjero').attr('required', false);
            $('#IdExtranjero').attr('Disabled', true);
            $('#IdIdentificacion').removeAttr('Disabled');
            $('#IdExtranjero').val('');
            //$('#IdExtranjero-error').remove();
            $('#IdIdentificacion').focus();      
        }

    }

    checkValidationIdentificacion();
});