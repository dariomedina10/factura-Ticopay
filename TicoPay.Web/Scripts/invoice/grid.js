var generateInvoiceList = "/Invoice/CreateList";
var createUrl = "/Services/Create";
var createResultUrl = "/Services/CreateAjax";
var createNote = "/Invoice/note";

function round(value, decimals) {
    return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
}


function getRequest(url) {
    //  clearErrors();
    abp.ui.setBusy();
    $.ajax({
        url: url,
        context: document.body,
        success: function (data) {
            $("#anyModalBodyService").html(data);
            //$(this).addClass("done");
            $("#anyModalFormService").modal("show");
            abp.ui.clearBusy();
        }, error: function (err) {
            abp.notify.error("¡Error al consultar los datos del Servicio!");
            abp.ui.clearBusy();
        }
    });
}

// formato decimal
function number_format(amount, decimals) {

    amount += ''; // por si pasan un numero en vez de un string
    amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto

    decimals = decimals || 0; // por si la variable no fue fue pasada

    // si no es un numero o es igual a cero retorno el mismo cero
    if (isNaN(amount) || amount === 0)
        return round(0,decimals);

    // si es mayor o menor que cero retorno el valor formateado como numero
    amount = '' + round(amount,decimals);

    ////Formato de serapadores de miles
    //var amount_parts = amount.split('.'),
    //    regexp = /(\d+)(\d{3})/;

    //while (regexp.test(amount_parts[0]))
    //    amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

    //return amount_parts.join('.');
    return amount;
}

function number_format2(amount, decimals) {

    amount += ''; // por si pasan un numero en vez de un string
    amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto

    decimals = decimals || 0; // por si la variable no fue fue pasada

    // si no es un numero o es igual a cero retorno el mismo cero
    if (isNaN(amount) || amount === 0)
        return round(0,decimals);

    // si es mayor o menor que cero retorno el valor formateado como numero
    amount = '' + round(amount,decimals);

    ////Formato de serapadores de miles
    var amount_parts = amount.split('.'),
        regexp = /(\d+)(\d{3})/;

    while (regexp.test(amount_parts[0]))
        amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

    return amount_parts.join('.');
    // return amount;
}
//calula el total de la linea
function calularLinea(row) {

    var cant = parseFloat($(row).find("input.cantidad").val());
    var desc = parseFloat($(row).find("input.descuento").val());
    var precio = parseFloat($(row).find("input.precio").val());
    var subTotal = (precio * cant);
    var impuesto = parseFloat($(row).find("Select.tipoimpuesto").val());



    var montodesc = number_format(subTotal * (desc / 100), 2);
    var subtotalD = subTotal - parseFloat(montodesc);

    $(row).find("input.subtotalneto").val(number_format(subtotalD, 2));

    //if (!$('#Chkgeneral').is(':checked')) {
    if ($('#Chkgeneral').val() == "false") {
        calcularSubTotales();
        $('#porcentajeGrl').val(calcularPorcentaje());
    }


    //var PorcentajeDescGRL = parseFloat($("#porcentajeGrl").val());
    var montodescGRL = 0;//subtotalD * PorcentajeDescGRL / 100;

    var montoImpuesto = number_format((subtotalD - montodescGRL) * (parseFloat(impuesto) / 100), 2);
    var total = (subtotalD - montodescGRL) + parseFloat(montoImpuesto);

    $(row).find("input.netolinea").val(subTotal);
    $(row).find("input.impuesto").val(montoImpuesto);
    $(row).find("input.montodescuento").val(montodesc);
    $(row).find("input.subTotalLinea").val(number_format(subtotalD - montodescGRL, 2));
    $(row).find("input.descuentoGrlLinea").val(number_format(montodescGRL, 2));
    $(row).find("input.totaldescuentolinea").val(number_format(parseFloat(montodesc) + montodescGRL, 2))
    $(row).find("input.total").val(number_format(total, 2));

    calcularSubTotales();


}

//recalcular lineas
function calcularlLineas() {

    $("#gridService tbody").children().each(function () {
        calularLinea($(this));
    });
}
//Caluclar los Sub-totales de impuesto, descuento y linea 
function calcularSubTotales() {

    var subTotalImpuesto = 0;
    var subTotalDescuento = 0;
    var subTotalDescuentoGRL = 0;
    var SubTotalLinea = 0;
    var SubTotalNeto = 0;
    var montoLinea = 0;
    var montoneto = 0;
    var TotalDescuento = 0;



    $("#gridService tbody").children().each(function () {
        //calularLinea($(this));
        subTotalImpuesto += parseFloat($(this).find("input.impuesto").val());
        subTotalDescuento += parseFloat($(this).find("input.montodescuento").val());
        montoneto += parseFloat($(this).find("input.netolinea").val());
        SubTotalNeto += parseFloat($(this).find("input.subtotalneto").val());
        SubTotalLinea += parseFloat($(this).find("input.subTotalLinea").val());
        subTotalDescuentoGRL += parseFloat($(this).find("input.descuentoGrlLinea").val());
        TotalDescuento += parseFloat($(this).find("input.totaldescuentolinea").val());
        montoLinea += parseFloat($(this).find("input.total").val());
    });


    $("#SumNeto").val(number_format(montoneto, 2));
    $("#SumSubtotalneto").val(number_format(SubTotalNeto, 2));

    $("#SumDescuentoLinea").val(number_format(subTotalDescuento, 2));
    $("#SumDescuentoGrlLinea").val(number_format(subTotalDescuentoGRL, 2));
    $("#SumSubTotalDescuento").val(number_format(TotalDescuento, 2));

    $("#SumSubTotalLinea").val(number_format(SubTotalLinea, 2));
    $("#SumImpuestoLinea").val(number_format(subTotalImpuesto, 2));

    $("#SumTotalLinea").val(number_format(montoLinea, 2));
    $("#subtotal_text").val(number_format(montoLinea, 2));

    ////if ($('#Chkgeneral').is(':checked'))
    //if ($('#Chkgeneral').val() == "true") 
    //    $("#DescuentoGrl").val(number_format(subTotalDescuentoGRL, 2));

    //Totales
    $('#subtotalNeto_text').val(number_format2(SubTotalLinea, 2));
    // $('#DescuentoTotal').val(number_format(TotalDescuento, 2));
    $("#TotalImpuesto").val(number_format2(subTotalImpuesto, 2));
    $("#selectTotalInvoice").val(number_format2(montoLinea, 2));
    $('#cashText').val(number_format(montoLinea, 2));

    if ($('#positiveBalanceHidden').val() == 0 || $('#positiveBalanceHidden').val() > (montoLinea))
        $('#positiveBalance').hide();
    else
        $('#positiveBalance').show();

}

// Calular %Descuento
function calcularPorcentaje() {

    var monto = $("#SumNeto").val();
    var montoDescuento = $("#DescuentoGrl").val();
    var porcentaje = (montoDescuento * 100) / monto;
    return number_format(porcentaje, 2);
}

// Calular moonto Descuento
function calcularDescuento() {
    var monto = $("#SumNeto").val();
    var PrcDescuento = $("#porcentajeGrl").val();
    var montoDescuento = (monto * PrcDescuento) / 100;
    return number_format(montoDescuento, 2);
}

//recalcula los impuestos por linea
function calcularDecuentoGrlLinea() {
    $('#porcentajeGrl').val(calcularPorcentaje());
    $("#gridService tbody").children().each(function () {
        calularLinea($(this));
    });
}

//limpia la tabla e inicializa los totales
function limpiarLineas() {
    $("#gridService tbody").children().remove();
    calcularSubTotales();

    $('#subtotal_text').val("0.00");
    $('#DescuentoGrl').val("0.00");
    $('#porcentajeGrl').val("0.00");
    $('#selectTotalInvoice').val("0.00");
}

//obtiene la cantidad de registros en la tabla
function contarLineas() {
    return $("#gridService tbody").children().length;
}

//valida el precio y la descripcion de los servicios
function validarservicios() {
    var valido = true;

    $("#gridService tbody").children().each(function () {
        if (parseFloat($(this).find("input.total").val()) <= 0) {
            abp.message.error("Existen servicios con precio igual o menor a cero.", "Error en el detalle de la factura");
            valido = false;
        }
        if ($(this).find("input.listaServicio").val().trim() == "") {
            abp.message.error("Existen servicios sin descripción.", "Error en el detalle de la factura");
            valido = false;
        }

    });
    return valido
}

//devuelve un arreglo con los registros de la tabla
function obtenerLineas() {
    var lineas = [];
    $("#gridService tbody").children().each(function () {
        if (parseFloat($(this).find("input.total").val()) <= 0) {
            $(this).remove();
        } else {
            var linea = {
                Servicio: $(this).find("input.listaServicio").val(),
                Note: $(this).find("textarea.nota").val(),
                Cantidad: $(this).find("input.cantidad").val(),
                Precio: $(this).find("input.precio").val(),
                Descuento: $(this).find("input.descuento").val(),
                UnidadMedida: $(this).find("input.unidadMedida").val(),
                UnidadMedidaOtra: $(this).find("input.unidadMedidaOtra").val(),
                //IdImpuesto: $(this).find("select.tipoimpuesto").val(),
                IdImpuesto: $(this).find("select.tipoimpuesto option:selected").data("id"),
                Impuesto: $(this).find("input.impuesto").val(),
                TotalImpuesto: $(this).find("input.impuesto").val(),
                TotalDescuento: $(this).find("input.totaldescuentolinea").val(),

            }
            lineas.push(linea);
        }

    });
    return lineas;
}

// inicializa los autocompletar de servcios
function loadAutocomplete() {
    // inicializa autocompletar



    $(".line").unbind();
    $("textarea[maxlength]").unbind();
    $('.listaServicio').typeahead('destroy');

    $('.listaServicio').typeahead({
        source: function (request, response) {
            $.ajax({
                url: "/Invoice/getService",
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                //async: false,
                data: JSON.stringify({ page: 1, limit: 10, search: request }),
                success: function (data) {
                    var values = $.map(data.result, function (item) {
                        return {
                            id: item.id,
                            name: item.name,
                            price: number_format(item.price == 0 ? 0.00 : item.price, 2),
                            tax: number_format(item.tax.rate, 2),
                            idtax: (item.taxId != null ? item.taxId.toString() : null),
                            unitmeasurement: item.unitMeasurement != null ? item.unitMeasurement : null,
                            unitmeasurementothers: item.unitMeasurementOthers
                        };
                    });
                    values.push({ name: request, price: "0.00", tax: "0.00" });
                    response(values);

                }
            });
        },
        displayText: function (item) { return item.name; },
        addItem: { name: "+ Crear nuevo servicio" },
        autoSelect: false,
        delay: 400,
        afterSelect: function (item) {
            if (item.name == "+ Crear nuevo servicio") {
                $(this.$element.parents()[1]).find('.typeahead_3').val("");
                $(this.$element.parents()[1]).find('.newService').prop("checked", true);
                getRequest(createUrl);

            } else {

                $(this.$element.parents()[1]).find("input.cantidad").val("1");
                if (parseFloat(item.price) > 0)
                    $(this.$element.parents()[1]).find("input.precio").val(item.price);
                $(this.$element.parents()[1]).find("input.unidadMedida").val(item.unitmeasurement);
                $(this.$element.parents()[1]).find("input.unidadMedidaOtra").val(item.unitmeasurementothers);
                if (item.idtax != null) {

                    $(this.$element.parents()[1]).find("select.tipoimpuesto").val(item.tax);
                    $(this.$element.parents()[1]).find("select.tipoimpuesto").attr("disabled", "disabled");

                }
                $(this.$element.parents()[1]).find("textarea.nota").removeClass("hidden");
                $(this.$element.parents()[1]).find("textarea.nota").focus();

                var montoImpuesto = (parseFloat(item.price) * parseFloat(item.tax) / 100);
                $(this.$element.parents()[1]).find("input.impuesto").val(number_format(montoImpuesto, 2));
                var total = parseFloat(item.price) + montoImpuesto;

                calularLinea(this.$element.parents()[1]);


            }

        }

    });

    // eventos de cambio de variables
    $(".line").change(function (e) {
        var row = $(this).parents()[1];
        var name = $(row).find(".typeahead_3").val();
        if (name.trim() == "") {
            $(row).find("textarea.nota").addClassClass("hidden");
            $(row).find("textarea.nota").val("");
        }
        var cant = $(row).find("input.cantidad").val();
        var desc = $(row).find("input.descuento").val();
        var precio = $(row).find("input.precio").val();
     
        if (cant <= 0) {

            abp.message.confirm(
                'La cantidad  no puede ser menor o igual a cero. La línea sera eliminada ',
                '¿Seguro desea Eliminar la línea?',
                function (isConfirmed) {
                    if (isConfirmed) {
                        $(row).remove();
                        calcularSubTotales();
                        //calcularDecuentoGrlLinea();
                        return;
                    } else {

                        $(row).find("input.cantidad").val("1");
                        cant = 1;
                        calularLinea(row);

                    }

                }

            );
            $(".confirm").html("Si");
            $(".cancel").html("No");

        }
        if ((desc < 0) || desc >= 100) {
            abp.notify.error("Monto de descuento inválido. El monto debe mayor e igual a 0 y menor a 100%");
            $(row).find("input.descuento").val("0.00");
            $(row).find("input.descuento").focus();
            desc = 0;
        }
        calularLinea(row);
      
    });

    $('.listaServicio').change(function (e) {
        var row = $(this).parents()[1];
        var name = $(row).find(".typeahead_3").val();
        if (name.trim() == "") {
            $(row).find("textarea.nota").addClass("hidden");
            $(row).find("textarea.nota").val("");
        }

    });

    //valida que el precio no sea cero
    //$(".precio").blur(function (e) {

    //    var precio = $(this).val();

    //    if (precio <= 0) {
    //        abp.notify.error("El precio no puede ser menor o igual a cero");
    //        $(this).focus();
    //        return false;
    //    }
    //});

    //evento eliminar linea
    $(".btnDeleteLinea").click(function (e) {

        var row = $(this).parents()[1];
        abp.message.confirm(
            'Eliminar línea',
            '¿Seguro desea Eliminar la línea seleccionada?',
            function (isConfirmed) {
                if (isConfirmed) {
                    $(row).remove();
                    calcularSubTotales();
                    //  calcularDecuentoGrlLinea();

                }

            }

        );
        $(".confirm").html("Si");
        $(".cancel").html("No");
    });

    //validación solo números
    $(".numeric").numeric();

    $("textarea[maxlength]").keyup(function () {

        var limit = $(this).attr("maxlength"); // Límite del textarea
        var value = $(this).val();             // Valor actual del textarea
        var current = value.length;              // Número de caracteres actual
        if (limit < current) {                   // Más del límite de caracteres?
            // Establece el valor del textarea al límite
            $(this).val(value.substring(0, limit));
        }
    });
}

//addlinea
function addlinea() {
    $.ajax({
        url: "/Invoice/addLine",
        success: function (data) {
            $("#gridService tbody").append(data);
            loadAutocomplete();
        }
    });
}

//cargar lineas
function LoadLinea(id) {
    abp.ui.setBusy();
    $.ajax({
        url: "/Invoice/addLine?invoiceID=" + id,
        success: function (data) {
            $("#gridService tbody").append(data);
            loadAutocomplete();
            calcularlLineas();
            abp.ui.clearBusy();
        }
    });


}