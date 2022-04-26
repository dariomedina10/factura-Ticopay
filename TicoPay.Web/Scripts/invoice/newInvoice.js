var generateInvoiceList = "/Invoice/CreateList";
var createUrl = "/Services/Create";
var createResultUrl = "/Services/CreateAjax";
var createNote = "/Invoice/note";
var generateNote = "/Invoice/ApplyNote";
var createProduct = "/Products/Create";
var createProductUrl = "/Products/CreateAjax";
//var PrintNote = "/PdfGenerator/printPos";
var cantidadRedondeo = 2;



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
            $("#anyModalFormServiceProduct").modal("show");
            $(".radio").show();
            abp.ui.clearBusy();
        }, error: function (err) {
            abp.notify.error("¡Error al consultar los datos del Servicio!");
            abp.ui.clearBusy();
        }
    });
}

function commaSeparateNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }
    return val;
}

function getRound(num, precision = cantidadRedondeo) {
    // half epsilon to correct edge cases.
    var c = 0.5 * Number.EPSILON * num;
    var p = 1; while (precision-- > 0) p *= 10;
    if (num < 0)
        p *= -1;
    return Math.round((num + c) * p) / p;
}

var quitarFormato = function (valor) {
    if (valor == undefined || valor == null)
        return 0;

    var valorstr = valor.toString();
    var seguir = true;
    while (seguir) {
        var pos = valorstr.indexOf(",");
        if (pos > 0)
            valorstr = valorstr.replace(',', "");
        else
            seguir = false;
    }
    return parseFloat(valorstr);
}

function redondearVista(x) {
    var redondeo = parseFloat(Math.round(x * Math.pow(10, cantidadRedondeo -1)) / Math.pow(10, cantidadRedondeo-1)).toFixed(cantidadRedondeo-1);
    return redondeo;
}

// formato decimal
function number_format(amount, decimals = cantidadRedondeo) {

    amount += ''; // por si pasan un numero en vez de un string
    amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto

    decimals = decimals || 0; // por si la variable no fue fue pasada

    // si no es un numero o es igual a cero retorno el mismo cero
    if (isNaN(amount) || amount === 0)
        return parseFloat(0).toFixed(decimals);

    // si es mayor o menor que cero retorno el valor formateado como numero
    amount = '' + getRound(amount, decimals);

    if (isNaN(amount) || amount === 0)
        return parseFloat(0).toFixed(decimals);

    if (amount.indexOf(".") == -1)
        amount += '.00';
    else {
        var amount_parts = amount.split('.');
        if (amount_parts[1].length < decimals)
            amount_parts[1] = amount_parts[1] + '0';
        amount = amount_parts.join('.');
    }
    ////Formato de serapadores de miles
    //var amount_parts = amount.split('.'),
    //    regexp = /(\d+)(\d{3})/;

    //while (regexp.test(amount_parts[0]))
    //    amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

    //return amount_parts.join('.');
    return amount;
}

function number_format2(amount, decimals) {

    var amountstring = amount + "";
    if (amountstring.indexOf(".") == -1)
        amountstring += '.00';
    else {
        var amount_parts = amountstring.split('.');
        if (amount_parts[1].length < 2)
            amount_parts[1] = amount_parts[1] + '0';
        amountstring = amount_parts.join('.');
    }
    return amountstring.replace(/\D/g, "")
        .replace(/([0-9])([0-9]{2})$/, '$1.$2')
        .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ",");
}

var numberDecimal = function (numero) {
    numero = number_format(numero);
    return commaSeparateNumber(numero);
}


function number_format_NoTrunc(num) {
    var with2Decimals = num.toString().match(/^-?\d+(?:\.\d{0,2})?/)[0]
    return with2Decimals
}

//calula el total de la linea
function calularLinea(row) {

    var cant = quitarFormato($(row).find("input.cantidad").val());
    var desc = quitarFormato($(row).find("input.descuento").val());
    if (isNaN(desc)) { desc = 0; }
    var taxdesc = desc * 0.01;
    var precio = quitarFormato($(row).find("input.precio").val());
    var subTotal = getRound(precio * cant);
    var impuesto = $(row).find("Select.tipoimpuesto").val();

    $(row).find("input.precio").val(number_format2(precio));
    $(row).find("input.descuento").val(number_format2(desc));

    
    var montodesc = getRound(subTotal * taxdesc);
    var subtotalD = getRound(subTotal - montodesc);
    var dat = number_format(subtotalD);
    $(row).find("input.subtotalneto").val(dat);
    
    if (desc > 0) {
        $(".desc").removeClass("hidden");
        if ($(".desc").val() == "")
            $(".desc").focus();
    } else {
        $(".desc").addClass("hidden");
    }
    
    //if (!$('#Chkgeneral').is(':checked')) {
    if ($('#Chkgeneral').val() == "false") {
        calcularSubTotales();
        $('#porcentajeGrl').val(calcularPorcentaje());
    }


    //var PorcentajeDescGRL = parseFloat($("#porcentajeGrl").val());
    var montodescGRL = 0;//subtotalD * PorcentajeDescGRL / 100;
    //var montoImpuesto = parseFloat(redondear((subtotalD - montodescGRL) * (parseFloat(impuesto) / 100)));
    var taxImpuesto = impuesto / 100;
    var importeTotal = getRound(subtotalD - montodescGRL);
    var totaldescuentolinea = getRound(montodesc + montodescGRL);
    var montoImpuesto = getRound(importeTotal * taxImpuesto);
    var total = getRound(subtotalD + montoImpuesto);

    var subTotalLinea = getRound(subtotalD - montodescGRL);

    $(row).find("input.netolinea").val(number_format2(subTotal));
    $(row).find("input.impuesto").val(number_format2(montoImpuesto));
    $(row).find("input.montodescuento").val(number_format2(montodesc));
    $(row).find("input.subTotalLinea").val(number_format2(subTotalLinea));
    $(row).find("input.descuentoGrlLinea").val(number_format2(montodescGRL));
    $(row).find("input.totaldescuentolinea").val(number_format2(totaldescuentolinea));
    $(row).find("input.total").val(number_format2(total));

    calcularSubTotales();


}

//recalcular lineas
function calcularlLineas() {

    $("#gridService tbody").children().each(function () {
        calularLinea($(this));
    });
}
function desformatearNumero(valor) {
    valor = valor.replace(/,(?=\d*[\.,])/g, "");
    return valor;
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
        subTotalImpuesto += quitarFormato($(this).find("input.impuesto").val());
        subTotalDescuento += quitarFormato($(this).find("input.montodescuento").val());
        montoneto += quitarFormato($(this).find("input.netolinea").val());
        SubTotalNeto += quitarFormato($(this).find("input.subtotalneto").val());
        //SubTotalLinea += parseFloat($(this).find("input.subTotalLinea").val());
        subTotal = quitarFormato($(this).find("input.subTotalLinea").val());
        SubTotalLinea += quitarFormato(subTotal);
        subTotalDescuentoGRL += quitarFormato($(this).find("input.descuentoGrlLinea").val());
        TotalDescuento += quitarFormato($(this).find("input.totaldescuentolinea").val());
        montoLinea += quitarFormato($(this).find("input.total").val());
    });
    //montoLinea = getRound(SubTotalNeto + subTotalImpuesto);

    $("#SumNeto").val(number_format2(getRound(montoneto)));
    $("#SumSubtotalneto").val(number_format2(getRound(SubTotalNeto)));

    $("#SumDescuentoLinea").val(number_format2(getRound(subTotalDescuento)));
    $("#SumDescuentoGrlLinea").val(number_format2(getRound(subTotalDescuentoGRL)));
    $("#SumSubTotalDescuento").val(number_format2(getRound(TotalDescuento)));

    $("#SumSubTotalLinea").val(number_format2(getRound(SubTotalLinea)));
    $("#SumImpuestoLinea").val(number_format2(getRound(subTotalImpuesto)));

    $("#SumTotalLinea").val(number_format2(montoLinea));
    $("#subtotal_text").val(number_format2(montoLinea));

    ////if ($('#Chkgeneral').is(':checked'))
    //if ($('#Chkgeneral').val() == "true") 
    //    $("#DescuentoGrl").val(number_format(subTotalDescuentoGRL, 2));

    //Totales
    $('#subtotalNeto_text').val(number_format2(getRound(SubTotalLinea)));
    // $('#DescuentoTotal').val(number_format(TotalDescuento, 2));
    $("#TotalImpuesto").val(number_format2(getRound(subTotalImpuesto)));

    $("#selectTotalInvoice").val(number_format2(getRound(montoLinea)));
    $('#cashText').val(number_format2(getRound(montoLinea)));

    if ($('#positiveBalanceHidden').val() == 0 || $('#positiveBalanceHidden').val() > (montoLinea))
        $('#positiveBalance').hide();
    else
        $('#positiveBalance').show();

}

// Calular %Descuento
function calcularPorcentaje() {
    var monto = $("#SumNeto").val();
    var montoDescuento = quitarFormato($("#DescuentoGrl").val());
	var porcentaje = (montoDescuento * 100) / parseFloat(monto);
    return number_format(porcentaje);
}

// Calular moonto Descuento
function calcularDescuento() {
    var monto = $("#SumNeto").val();
    var PrcDescuento = $("#porcentajeGrl").val();
    var montoDescuento = (monto * PrcDescuento) / 100;
    return number_format(montoDescuento);
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
        if (quitarFormato($(this).find("input.total").val()) <= 0) {
            abp.message.error("Existen producto o servicio con precio igual o menor a cero.", "Error en el detalle de la factura");
            valido = false;
        }
        if ($(this).find("input.listaServicio").val().trim() == "") {
            abp.message.error("Existen producto o servicio sin descripción.", "Error en el detalle de la factura");
            valido = false;
        }
        //if ($(this).find("textarea.desc").val().trim() == "" && $(this).find("input.descuento").val() > 0) {
        //    abp.message.error("Existen producto o servicio sin descripción en el descuento.", "Error en el detalle de la factura");
        //    valido = false;
        //}
        var descriptionDiscount = $(this).find("textarea.desc").val().trim();
        if (descriptionDiscount.length > 80) {
            abp.notify.error("La descripción del descuento no puede tener más de 80 caracteres.");
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
                DescriptionDiscount: $(this).find("textarea.desc").val(),
                Cantidad: quitarFormato($(this).find("input.cantidad").val()),
                Precio: quitarFormato($(this).find("input.precio").val()),
                Descuento: quitarFormato($(this).find("input.descuento").val()),
                UnidadMedida: $(this).find("select.unidadMedida option:selected").data("id"),
                UnidadMedidaOtra: $(this).find("input.unidadMedidaOtra").val(),
                //IdImpuesto: $(this).find("select.tipoimpuesto").val(),
                IdImpuesto: $(this).find("select.tipoimpuesto option:selected").data("id"),
                Impuesto: quitarFormato($(this).find("input.impuesto").val()),
                Tipo: $(this).find("input.tipo").val(),
                IdProduct: $(this).find("input.idProduct").val(),
                TotalImpuesto: quitarFormato($(this).find("input.impuesto").val()),
                TotalDescuento: quitarFormato($(this).find("input.totaldescuentolinea").val()),

            }
            lineas.push(linea);
        }
    });
    return lineas;
}

function obtenerLineasNote() {
    var lineas = [];
    $("#gridService tbody").children().each(function () {
        if (parseFloat($(this).find("input.total").val()) <= 0) {
            $(this).remove();
        } else {
            var linea = {
                Title: $(this).find("input.listaServicio").val(),
                Note: $(this).find("textarea.nota").val(),
                DescriptionDiscount: $(this).find("textarea.desc").val(),
                Quantity: number_format($(this).find("input.cantidad").val(),2),
                PricePerUnit: number_format($(this).find("input.precio").val(),2),
                DiscountPercentage: number_format($(this).find("input.descuento").val(),2),
                UnitMeasurement: $(this).find("input.unidadMedida").val(),
                UnitMeasurementOthers: $(this).find("input.unidadMedidaOtra").val(),
                TaxId: $(this).find("select.tipoimpuesto option:selected").data("id"),
                TaxAmount: number_format($(this).find("input.impuesto").val(),2),            
                Total: number_format($(this).find("input.total").val(),2),

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
                    var values = $.map(data.result.servicio, function (item) {
                        return {
                            id: item.id,
                            name: item.name,
                            price: number_format2(item.price == 0 ? 0.00 : item.price),
                            tax:  number_format2(item.tax.rate),
                            idtax: (item.taxId != null ? item.taxId.toString() : null),
                            unidadMedida: item.unitMeasurement != null ? item.unitMeasurement : null,
                            unitmeasurement: item.unitMeasurement != null ? item.unitMeasurement : null,
                            unitmeasurementothers: item.unitMeasurementOthers,
                            tipo: item.tipo,
                            idService: item.idService != null ? item.idService : null
                        };
                    });
                    var products = $.map(data.result.products, function (item) {
                        return {
                            id: item.id,
                            name: item.name,
                            price: number_format2(item.retailPrice == 0 ? 0.00 : item.retailPrice),
                            tax: number_format2(item.tax.rate),
                            idtax: number_format2(item.taxId != null ? item.taxId.toString() : null),
                            unidadMedida: item.unitMeasurement != null ? item.unitMeasurement : null,
                            unitmeasurement: item.unitMeasurement != null ? item.unitMeasurement : null,
                            unitmeasurementothers: null,
                            tipo: item.tipo,
                            //idProduct: item.idProduct != null ? item.idProduct : null
                        };
                    });
                    values = values.concat(products);
                    values.push({ name: request, price: "0.00", tax: "0.00" });
                    response(values);
                }
            });
        },
        displayText: function (item) { return item.name; },
        addItem: { name: "+ Crear nuevo producto o servicio" },
        autoSelect: false,
        delay: 400,
        afterSelect: function (item) {
            if (item.name == "+ Crear nuevo producto o servicio") {
                $(this.$element.parents()[1]).find('.typeahead_3').val("");
                $(this.$element.parents()[1]).find('.newService').prop("checked", true);
                getRequest(createProduct);

            } else {

                $(this.$element.parents()[1]).find("input.cantidad").val("1");
                if (parseFloat(item.price) > 0)

                    $(this.$element.parents()[1]).find("input.precio").val(commaSeparateNumber(item.price));
                if (item.unidadMedida != null) {
                $(this.$element.parents()[1]).find("select.unidadMedida").val(item.unidadMedida);
                $(this.$element.parents()[1]).find("select.unidadMedida").attr("disabled", "disabled");
                }
                //$(this.$element.parents()[1]).find("input.unidadMedidaOtra").val(item.unitmeasurementothers);
                if (item.idtax != null) {

                    $(this.$element.parents()[1]).find("select.tipoimpuesto").val(item.tax);
                    $(this.$element.parents()[1]).find("Select.tipoimpuesto").attr("disabled", "disabled");

                }
                $(this.$element.parents()[1]).find("input.tipo").val(item.tipo);

                //if (item.id != null) {
                //    $(this.$element.parents()[1]).find("input.idService").val(item.id);
                //}
                if (item.id != null) {
                    $(this.$element.parents()[1]).find("input.idProduct").val(item.id);
                }

                $(this.$element.parents()[1]).find("textarea.nota").removeClass("hidden");
                $(this.$element.parents()[1]).find("textarea.nota").focus();

                var montoImpuesto = (parseFloat(item.price) * parseFloat(item.tax) / 100);
                $(this.$element.parents()[1]).find("input.impuesto").val(number_format2(montoImpuesto));
                var total = parseFloat(item.price) + montoImpuesto;

                calularLinea(this.$element.parents()[1]);


            }

        }


    });

    // eventos de cambio de variables
    $(".line").change(function (e) {
        var row = $(this).parents()[1];
        if ($(row).find("input.precio").val().length > 20){
            abp.notify.error("El precio no puede ser mayor a 9,999,999,999,999.99");
            $(row).find("input.precio").val('0.00');
            $(row).find("input.precio").focus();
            desc = 0;
        }
        if ($(row).find("input.descuento").val().length > 5) {
            abp.notify.error("Monto de descuento inválido. El monto debe ser mayor e igual a 0.00 y menor a 100%");
            $(row).find("input.descuento").val("0.00");
            $(row).find("input.descuento").focus();
            desc = 0;
        }

      
        var name = $(row).find(".typeahead_3").val();
        if (name.trim() == "") {
            $(row).find("textarea.nota").addClass("hidden");
            $(row).find("textarea.nota").val("");
        }
        var cant = number_format($(row).find("input.cantidad").val(),2);
        var desc = number_format($(row).find("input.descuento").val(),2);
        var precio = number_format($(row).find("input.precio").val(),2);
      //  $(row).find("input.precio").val(number_format(precio, 2));
       // $('#subtotalNeto_text').val(number_format2(SubTotalLinea, 2));
     
        //var subTotal = (precio * cant);
        ////var impuesto = $(row).find("Select.tipoimpuesto option:selected").text().split(" % ");
        //var impuesto = $(row).find("Select.tipoimpuesto").val();

        if (precio > 9999999999999.99) {
            abp.notify.error("El precio no puede ser mayor a 9,999,999,999,999.99");
            $(row).find("input.precio").val('0');
            $(row).find("input.precio").focus();
        }
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
            abp.notify.error("Monto de descuento inválido. El monto debe ser mayor e igual a 0.00 y menor a 100%");
            $(row).find("input.descuento").val("0.00");
            $(row).find("input.descuento").focus();
            desc = 0;
        }
        calularLinea(row);
        return false;
        ////if (!$('#Chkgeneral').is(':checked')) {
        //if ($('#Chkgeneral').val() == "false") {

        //    calcularDecuentoGrlLinea();
        //}   
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

    $(".numeric").on({
        "focus": function (event) {
            $(event.target).select();
        },
        "keyup": function (event) {
            $(event.target).val(function (index, value) {
                return value.replace(/\D/g, "")
                    .replace(/([0-9])([0-9]{2})$/, '$1.$2')
                    .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ",");
            });
        }
    });
}  

//addlinea
function addlinea() {
    var max = $("#MaxLines").val();
    var countlines = contarLineas();

    if (max < 0 || (max > 0 && countlines < max)) {
        $.ajax({
            url: "/Invoice/addLine",
            success: function (data) {
                $("#gridService tbody").append(data);
                loadAutocomplete();
            }
        });
    } else {
        abp.notify.warn("No se puede agregar más lineas al detalle. Máximo de lineas según formato de impresión: " + max);
    }
   
}

//cargar lineas
function LoadLinea(/*id*/list, isreverse) {
    abp.ui.setBusy();
    $.ajax({
        url: "/Invoice/addLine", //"/Invoice/addLine?invoiceID=" + id,
        type: "POST",
        cache: false,
        data: JSON.stringify(list),

        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            $("#gridService tbody").append(data);
            loadAutocomplete();
            calcularlLineas();
            abp.ui.clearBusy();
            $(".descuento").attr("disabled", "disabled");
            $(".tipoimpuesto").attr("disabled", "disabled");
            //$(".unidadMedida").attr("disabled", "disabled");
            $(".listaServicio").attr("disabled", "disabled");
            if (isreverse == "True") {
                $(".cantidad").attr("disabled", "disabled");
                $(".precio").attr("disabled", "disabled");
                $(".btnDeleteLinea").remove();
            }
        }

    });

   

   
}

function printicket(url) {

    var n = window.open(url);
   //debugger;
   // abp.ui.setBusy();
   // $.ajax({
   //     url: "/PdfGenerator/Invoiceprint?id="+id, //"/Invoice/addLine?invoiceID=" + id,
   //    cache: false,      
   //     success: function (data) {

   //         window.open(data);
   //         window.print();


   //     }

   // });
}

//function printnote(id, url, tenantId, typeDocument) {


//        var sessionId = $("#sid").val();
            
//    var input = {
//        Id: id,
//        Sid: sessionId           
//        };

//        abp.ui.setBusy();
//        $.ajax({
//            url: PrintNote,
//            type: "POST",
//            cache: false,
//            data: JSON.stringify(input),
//            dataType: "json",
//            contentType: 'application/json; charset=utf-8',          
//            success: function (data) {

//                if (data.codeError == 1) {
                    
//                    //jsWebClientPrint.print('sid=' + sessionId);
//                    jsWebClientPrint.print('id=' + id + '&tenantId=' + tenantId + '&tipoDocumento=' + typeDocument);
//                    window.location.href = url;

//                } else {
//                    abp.notify.error(data.error);                  
//                }
//            },
//            error: function (err) {
//              abp.notify.error(err);
              

//            }
//        });
    
//}




$(document).ready(function () {


    // Añade una nueva linea a la tabla
    $("#newLine").click(function (e) {
        addlinea();
    });

    //Se verifica si el descuento general es por monto o porcentaje
    $('#Chkgeneral').change(function () {

        //var isChecked = $(this).is(':checked');
        var isChecked = ($('#Chkgeneral').val() == "true");
        if (isChecked) {
            $('#porcentajeGrl').val(calcularPorcentaje());
            $('#DescuentoGrl').attr('disabled', 'disabled');
            $('#porcentajeGrl').removeAttr('disabled');
            $('#porcentajeGrl').removeClass("hidden");
            $('#DescuentoGrl').addClass("hidden");
            $('#lbPrc').addClass("hidden");


        } else {
            $('#porcentajeGrl').attr('disabled', 'disabled');
            $('#DescuentoGrl').removeAttr('disabled');
            $('#DescuentoGrl').val(calcularDescuento());
            $('#porcentajeGrl').addClass("hidden");
            $('#DescuentoGrl').removeClass("hidden");
            $('#lbPrc').removeClass("hidden");
            $('#Prc').html($('#porcentajeGrl').val());
            //resultFormatNumber("" + "0.00").then((val) => { $('#discountAmount').val(val) });
        }

    });

    //calula descuento segun cambio en porcentaje
	$('#porcentajeGrl').change(function () {
        var subtotal = parseFloat($("#SumSubtotalneto").val());
        var descuento = parseFloat($("#porcentajeGrl").val());
        if ((descuento < 0) || descuento >= 100) {

            abp.notify.error("Monto de descuento inválido. El monto debe ser mayor e igual a 0 y menor a 100%");
            $("#porcentajeGrl").val("0.00");
            descuento = 0;
        }
        //$('#DescuentoGrl').val(calcularDescuento());
        //calcularDecuentoGrlLinea();

    });

    //cambio en monto descuento
    $('#DescuentoGrl').change(function () { 
        var descuento = parseFloat(number_format($("#DescuentoGrl").val(),2));
        var subtotal = parseFloat(number_format($("#SumSubtotalneto").val(),2)); 
        if ((descuento < 0) || descuento > subtotal) {

            abp.notify.error("Monto de descuento inválido. El monto debe ser mayor e igual a 0 y menor que el subtotal de la factura: " + subtotal.toString());
            $("#DescuentoGrl").val("0.00");
            descuento = 0;
        }    
        $('#porcentajeGrl').val(number_format2(calcularPorcentaje(),2));
        $('#Prc').html($('#porcentajeGrl').val());
        //calcularDecuentoGrlLinea();

    });



    //Generar factura
    $('#GenerateInvoice_btn').click(function () {
    
        //clearErrors();
        if (validarservicios()) {
            var records = obtenerLineas();
            var counServices = contarLineas();
            var clientId = $('#ClientId_hidden').val();
            var coin = $('#CoinTypes_DD').val();
            var typefirm = isNaN($('#FirmTypes_DD').val()) ? 'Llave' : $('#FirmTypes_DD').val();
            var clientName = $("#ClientName_text").val();
            var typeDocument = $("#hd_TypeDocument").val();
            var identificationTypes = $("#IdentificationTypes_DD").val();
            var identification = $("#text_identification").val();
            var clientPhoneNumber = $("#text_ClientPhoneNumber").val();
            var clientMobilNumber = $("#text_ClientMobilNumber").val();
            var clientEmail = $("#text_ClientEmail").val();
            var iscontingencia = $("#Check_contingencia").prop('checked');
            var consecutivoContigencia = $("#text_ConsecutiveNumberContingency").val();
            var fechacontigencia = $("#text_DateContingency").val();
            var motivoContigencia = $("#text_ReasonContingency").val();
            if (fechacontigencia == '') {
                fechacontigencia = '1900-01-01';
            }
            var bank = $("#listBank").val();
            var userCard = $("#UserCreditCardText").val();
            var montoFactura = desformatearNumero($('#selectTotalInvoice').val()).replace(",", "");

            var sessionId = $("#sid").val();
            var isPos = $("#IsPos_HD").val();
            var typePrinter = $("#TypePrinter_HD").val();

            $('#GenerateInvoice_btn').attr('disabled', 'disabled');

            //if ((clientId == "") && (typeDocument == "Invoice")) {
            //    abp.notify.error('Debe seleccionar un Cliente');
            //    abp.ui.clearBusy();
            //    $('#GenerateInvoice_btn').removeAttr('disabled');
            //    return false;
            //}


            if (counServices == 0) {
                abp.notify.error('Debe agregar al menos un Producto o Servicio');
                abp.ui.clearBusy();
                $('#GenerateInvoice_btn').removeAttr('disabled');
                return false;
            }

            if (montoFactura > 9999999999999.99) {
                abp.notify.error('El monto de la factura no puede ser mayor a 9,999,999,999,999.99');
                abp.ui.clearBusy();
                $('#GenerateInvoice_btn').removeAttr('disabled');
                return false;
            }

            if ((iscontingencia) && (consecutivoContigencia.trim() == '' || fechacontigencia.trim() == '' || motivoContigencia.trim() == '')) {
                abp.notify.error('Debe ingresar los datos del comprobante de contingencia');
                abp.ui.clearBusy();
                $('#GenerateInvoice_btn').removeAttr('disabled');
                return false;
            }
          

            //if (!$('#ContadoChk').is(':checked') && !$('#CreditoChk').is(':checked')) {
            //    writeError('msgNotaError', 'Debe seleccionar una forma de pago', 'error');
            //    abp.ui.clearBusy();
            //    $('#GenerateInvoice_btn').removeAttr('disabled');
            //    return false;
            //}
            var conditionSaleType = $('#ConditionSaleTypes_DD').val();

            if (conditionSaleType == '1') {
                var validateIsSuccesful = true;
                $("#panelCredito :input[type=text]").each(function () {
                    var inputObject = $(this);
                    if (!inputObject.is(":hidden")) {
                        var input = inputObject.val();
                        var regexValue = $(this).attr("data-regular-expression");
                        if (regexValue != null) {
                            var regex = new RegExp($(this).attr("data-regular-expression"));
                            if (!regex.test(input)) {
                                abp.notify.error("El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.");
                                abp.ui.clearBusy();
                                validateIsSuccesful = false;
                                $('#GenerateInvoice_btn').removeAttr('disabled');
                                return false;
                            }
                        }
                    }
                });

                if (!validateIsSuccesful) {
                    validateIsSuccesful = true;
                    return false;
                }

                if ($('#DayCredit').val() == "") {
                    abp.notify.error('Debe ingresar una cantidad de días de crédito');
                    abp.ui.clearBusy();
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    return false;
                }
                else {
                    if ($('#DayCredit').val() == 0) {
                        abp.notify.error('La cantidad de días de crédito debe ser mayor a 0');
                        abp.ui.clearBusy();
                        $('#GenerateInvoice_btn').removeAttr('disabled');
                        return false;
                    }
                }
            }

            if (conditionSaleType == '0') {
                //____________________________________________________________________________________________________________________________
                var typeCash = $('#selectPaymentMethodCash').val();
                var typeCreditCard = $('#selectPaymentMethodCreditCard').val();
                var typeDeposit = $('#selectPaymentMethodDeposit').val();
                var typeCheck = $('#selectPaymentMethodCheck').val();
                var typePositiveBalance = $('#selectPositiveBalance').val();
                var transCreditCard = "";
                var transDeposit = "";
                var transCheck = "";
                var difPay = "";



                var cash = parseFloat($('#cashText').val() == "" ? 0 : $('#cashText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var creditCard = parseFloat($('#creditCardText').val() == "" ? 0 : $('#creditCardText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var deposit = parseFloat($('#depositText').val() == "" ? 0 : $('#depositText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var check = parseFloat($('#checkText').val() == "" ? 0 : $('#checkText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var positiveBalance = parseFloat($('#positiveBalanceHidden').val() == "" ? 0 : $('#positiveBalanceHidden').val().replace(/[^0-9\.]/g, '')).toFixed(2);

                if ((parseFloat(cash + creditCard + deposit + check) > parseFloat(9999999999999.99))) {                   
                    abp.notify.error("El Monto de las forma de pago no puede ser mayor a 9,999,999,999,999.99");
                    abp.ui.clearBusy();
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    return false;
                }


                if (typePositiveBalance != "4")
                    positiveBalance = 0;

                if (typeCash == "" && typeCreditCard == "" && typeDeposit == "" && typeCheck == "" && typePositiveBalance == "") {
                    abp.notify.error("Debe seleccionar al menos un método de pago");
                    abp.ui.clearBusy();
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    return false;
                }




                if (conditionSaleType == '0') {
                    var validateIsSuccesful = true;
                    $("#panelContado :input[type=text]").each(function () {
                        var inputObject = $(this);
                      
                        if (!inputObject.is(":hidden")) {
                            var input = inputObject.val();
                            var regexValue = $(this).attr("data-regular-expression");
                            if (regexValue != null) {
                                var regex = new RegExp($(this).attr("data-regular-expression"));
                                if (!regex.test(input)) {
                                    abp.notify.error("El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.");
                                    abp.ui.clearBusy();
                                    validateIsSuccesful = false;
                                    $('#GenerateInvoice_btn').removeAttr('disabled');
                                    return false;
                                }
                            }
                        }
                    });

                    if (!validateIsSuccesful) {
                        validateIsSuccesful = true;
                        return false;
                    }



                    if (typeCreditCard == "1") {
                        transCreditCard = $('#nroCreditCardText').val();
                        if (transCreditCard.length <= 0) {
                            abp.notify.error("Debe ingresar un número de transacción");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        } else if (creditCard <= 0) {
                            abp.notify.error("Debe ingresar el monto de transacción");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeDeposit == "3") {
                        transDeposit = $('#nroDepositText').val();
                        if (transDeposit.length <= 0) {
                            abp.notify.error("Debe ingresar un número de depósito / Transferencia");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        } else if (deposit <= 0) {
                            abp.notify.error("Debe ingresar el monto de depósito / Transferencia");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCheck === "2") {
                        transCheck = $('#nroCheckText').val();
                        if (transCheck.length <= 0) {
                            abp.notify.error("Debe ingresar un número de cheque");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        } else if (check <= 0) {
                            abp.notify.error("Debe ingresar el monto de cheque");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCash == "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {

                        var total = parseFloat($('#selectTotalInvoice').val().replace(/[^0-9\.]/g, '')).toFixed(2); //$('#selectTotalInvoice').val()
                        var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                        var dif = (parseFloat(total) - parseFloat(subtotal));
                        if (dif != 0) {
                            abp.notify.error("El monto de los métodos de pago debe ser igual al monto total de la factura");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                        if (dif < 0) {
                            abp.notify.error("El monto de los métodos de pago debe ser menor al monto total de la factura ya que se paga una parte en efectivo");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                        if (dif > 0) {
                            abp.notify.error("El monto de todos los métodos de pago debe ser mayor o igual al monto de la factura.");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }




                    if (typeCash == "0") {
                        var total = parseFloat($('#selectTotalInvoice').val().replace(/[^0-9\.]/g, '')).toFixed(2); //$('#selectTotalInvoice').val()
                        var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                        difPay = number_format(parseFloat(total) - parseFloat(subtotal), 2);
                        var dif = (parseFloat(cash) - parseFloat(difPay));

                        if (dif < 0) {
                            abp.notify.error("El monto en efectivo debe ser mayor o igual al monto total de la factura");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }
                }

            }
            //____________________________________________________________________________________________________________________________

            var typeDiscountGeneral = 1;
            var discountGeneral = 0;//parseFloat($('#DescuentoGrl').val().replace(/[^0-9\.]/g, ''));       
            var discountPercentageOrMount = 0;//$('#porcentajeGrl').val();

            var generalObservation = $('.generalObservation').val();
            abp.ui.setBusy();
            $.ajax({
                url: generateInvoiceList,
                type: "POST",
                cache: false,
                data: JSON.stringify({
                    gridData: records, clientId: clientId, typePaymentCash: typeCash, balanceCash: difPay, typePaymentCreditCard: typeCreditCard, balanceCreditCard: creditCard, transCreditCard: transCreditCard, typePaymentDeposit: typeDeposit,
                    balanceDeposit: deposit, transDeposit: transDeposit, typePaymentCheck: typeCheck, balanceCheck: check, transCheck: transCheck, typePositiveBalance: typePositiveBalance,
                    balancePositiveBalance: positiveBalance, typeDiscount: typeDiscountGeneral, discount: discountGeneral, discountPercentage: discountPercentageOrMount, dayCredit: $('#DayCredit').val(), ConditionSaleType: conditionSaleType,
                    CoinType: coin, firmType: typefirm, ClientName: clientName, TypeDocument: typeDocument, IdentificationTypes: identificationTypes, Identification: identification,
                    ClientPhoneNumber: clientPhoneNumber, ClientMobilNumber: clientMobilNumber, ClientEmail: clientEmail, BankId: bank, UserCard: userCard, GeneralObservation: generalObservation, Sid: sessionId, Iscontingencia: iscontingencia, ConsecutivoContigencia: consecutivoContigencia, Fechacontigencia: fechacontigencia,
                    MotivoContigencia: motivoContigencia                 }),
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    if (data.result.value == "1") {
                        // grid.clear();
                        limpiarLineas();

                        $('#ClientName_text').val("");
                        $('#ClientId_hidden').val("");

                        $("#ConditionSaleTypes_DD option:selected").removeAttr("selected");
                        $('#ConditionSaleTypes_DD').val("0");
                        //$("#ConditionSaleTypes_DD option[value=0]").attr("selected", true);
                        $("#ConditionSaleTypes_DD").chosen("destroy");
                        $('#ConditionSaleTypes_DD').chosen({ max_selected_options: 1, no_results_text: "No se encuentra condición de venta", allow_single_deselect: true, width: "120px" });
                        $('#selectTotalInvoice').val(0);
                        $('#selectPaymentMethodCash').val("");
                        $('#selectPaymentMethodCreditCard').val("");
                        $('#selectPaymentMethodDeposit').val("");
                        $('#selectPaymentMethodCheck').val("");
                        $('#cashText').val("");
                        $('#creditCardText').val("");
                        $('#depositText').val("");
                        $('#checkText').val("");
                        $('#nroCreditCardText').val("");
                        $('#nroDepositText').val("");
                        $('#nroCheckText').val("");
                        $('#deposit').hide();
                        $('#check').hide();
                        $('#creditCard').hide();
                        $('#cash').hide();
                        $('#CashChk').prop('checked', false);
                        $('#CreditCardChk').prop('checked', false);
                        $('#DepositChk').prop('checked', false);
                        $('#CheckChk').prop('checked', false);
                        $('#PositiveBalanceChk').prop('checked', false);
                        $('#selectPositiveBalance').val("");
                        $('#positiveBalanceHidden').val("");
                        $('#positiveBalance').hide();
                        $('#selectDiscountCheckHidden').val(0);
                        $('#balanceDiscountHidden').val(0);
                        $("#UserCreditCardText").val("");
                        $('#listBank').val('');
                        $('.generalObservation').val("");
                        $("#text_ConsecutiveNumberContingency").val('');
                        $("#text_DateContingency").val('');
                        $("#text_ReasonContingency").val('');

                        if (iscontingencia) {
                            $('#Check_contingencia').bootstrapToggle('toggle');
                        }

                        $("#IdentificationTypes_DD").val('');
                        $("#text_identification").val('');
                        $("#text_ClientPhoneNumber").val('');
                        $("#text_ClientMobilNumber").val('');
                        $("#text_ClientEmail").val('');



                        $('#Chkgeneral').attr('checked', false);

                        $('#panelContado').show();
                        $('#panelCredito').hide();
                        $('#DayCredit').val("");
                        abp.notify.success('Comprobante electrónico generado correctamente');
                        $('#GenerateInvoice_btn').removeAttr('disabled');

                        if ($('#Check_Impimir').is(":checked")) {

                            if (isPos == '1') {
                                jsWebClientPrint.print('id=' + data.result.newInvoiceId + '&tenantId=' + data.result.newInvoiceTenantId + '&tipoDocumento=' + data.result.newInvoicetipoDocumento);
                            }
                            else {
                                printicket(data.result.url);
                            }                             
                        }
                    }
                    else if (data.result.value == "-1") {
                        abp.notify.error(data.result.error);

                    }
                    abp.ui.clearBusy();
                }, error: function (err) {
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    abp.notify.error(err.statusText);
                    abp.ui.clearBusy();
                }
            });
        }

    });

    //Crea un nuevo servicio
    $("#anyModalFormService").submit(function (event) {
        //  clearErrors();
        // event.preventDefault();
        abp.ui.setBusy();
        var action = createResultUrl;
        $.post(action, $("#form").serialize(), function (data, status) {


            $("input.newService:checkbox:checked").each(function () {
                $("#anyModalFormService").modal('hide');
                var row = $(this).parents()[1];

                $(row).find("input.newService").prop("checked", false);
                $(row).find('.typeahead_3').val(data.result.name);
                $(row).find("input.cantidad").val("1");
                if (parseFloat(data.result.price) > 0)
                    $(row).find("input.precio").val(data.result.price);
                $(row).find("input.unidadMedida").val(data.result.unitMeasurement);
                $(row).find("input.unidadMedidaOtra").val(data.result.unitMeasurementOthers);
                if (data.result.taxId != null) {

                    $(row).find("select.tipoimpuesto").val(number_format(data.result.tax.rate, 2));
                    $(row).find("select.tipoimpuesto").attr("disabled", "disabled");
                }
                $(row).find("textarea.nota").removeClass("hidden");
                $(row).find("textarea.nota").focus();

                var montoImpuesto = (parseFloat(data.result.Price) * parseFloat(data.result.tax.rate) / 100);
                $(row).find("input.impuesto").val(number_format(montoImpuesto, 2));
                var total = parseFloat(data.result.price) + montoImpuesto;

                calularLinea(row);
            });



            abp.ui.clearBusy();


            // refreshList();//refresh list
        }).fail(function (error, status) {
            abp.notify.error("Error procesando datos. ¡Por favor revise los datos nuevamente!");
            $("#anyModalBodyService").html(error.responseText);
            abp.ui.clearBusy();
        });
        return false;
    });

    //Crea un nuevo producto o servicio
    $("#anyModalFormServiceProduct").submit(function (event) {
        //  clearErrors();
        // event.preventDefault();

        abp.ui.setBusy();
        var action = "";
        switch (productoServicio()) {
            case "Productos":
                action = createProductUrl;
                break;
            case "Services":
                action = createResultUrl;
                break;
        }
        $.post(action, $("#form").serialize(), function (data, status) {

            if (data.result != null) {
            $("input.newService:checkbox:checked").each(function () {
                $("#anyModalFormServiceProduct").modal('hide');
                var row = $(this).parents()[1];
                $(row).find("input.newService").prop("checked", false);
                $(row).find('.typeahead_3').val(data.result.name);
                $(row).find("input.cantidad").val("1");
                    $(row).find("input.idProduct").val(data.result.id);
                if (productoServicio() == "Productos") {
                    if (parseFloat(data.result.retailPrice) > 0)
                        $(row).find("input.precio").val(data.result.retailPrice);
                } else {
                    if (parseFloat(data.result.price) > 0)

                            $(row).find("input.precio").val(number_format2(data.result.price,2));
                }
                $(row).find("select.unidadMedida").val(data.result.unitMeasurement);

                //$(row).find("input.unidadMedida").val(data.result.unitMeasurement);
                //$(row).find("input.unidadMedidaOtra").val(data.result.unitMeasurementOthers);
                if (data.result.taxId != null) {

                    $(row).find("select.tipoimpuesto").val(number_format(data.result.tax.rate, 2));
                    $(row).find("select.tipoimpuesto").attr("disabled", "disabled");
                }
                $(row).find("textarea.nota").removeClass("hidden");
                $(row).find("textarea.nota").focus();

                var montoImpuesto = (parseFloat(data.result.Price) * parseFloat(data.result.tax.rate) / 100);
                $(row).find("input.impuesto").val(number_format(montoImpuesto, 2));
                $(row).find("input.tipo").val(data.result.tipo);
                var total = parseFloat(data.result.price) + montoImpuesto;

                calularLinea(row);
            });
            } else {
                $("#anyModalBodyService").html(data);
            }




            abp.ui.clearBusy();


            // refreshList();//refresh list
        }).fail(function (error, status) {
            abp.notify.error("Error procesando datos. ¡Por favor revise los datos nuevamente!");
            $("#anyModalBodyService").html(error.responseText);
            abp.ui.clearBusy();
        });
        return false;
    });

    //Se aplica el descuento masivo a todad las lineas
    $("#btnAplyDesc").click(function (event) {
        var descuentoGrl = parseFloat(number_format($('#porcentajeGrl').val(),2));
        var message = "No fue posible aplicar el decuento a los siguientes servicios, pues superan el % permitido de descuento:"
        var services = "";
        $("#gridService tbody").children().each(function () {
            var desc = parseFloat($(this).find("input.descuento").val());
            var descuentoTotal = desc + descuentoGrl
            if (descuentoTotal < 100) {
                $(this).find("input.descuento").val(number_format(desc + descuentoGrl, 2));
                calularLinea($(this));
            } else {
                services += " " + $(this).find("input.listaServicio").val() + ",";
            }

        });
        if (services != "")
            abp.message.warn((message + services.substring(0, services.length - 1)), "Descuentos no aplicados");


        $('#porcentajeGrl').val("");
        $("#DescuentoGrl").val("");
        $("#Prc").html("0.00");
    });

    //Se aplica el descuento masivo a todad las lineas en la notas por pronto pago
    $("#btnAplyDescProntoPago").click(function (event) {
        var descuentoGrl = parseFloat($('#porcentajeGrl').val());
        $("#gridService tbody").children().each(function () {
            if (descuentoGrl > 0 && descuentoGrl < 100) {
                var precio = quitarFormato($(this).find("input.precio").val());
                var desc = descuentoGrl * 0.01;
                var descProntoPago = precio * desc;
                $(this).find("input.precio").val(number_format2(getRound(descProntoPago)));
                calularLinea($(this));
                $(this).find("textarea.nota").val("Descuento pronto pago");
                $(".reversarDesc").removeClass('hidden')
            }
            
        });

        $('#porcentajeGrl').val("");
        $("#DescuentoGrl").val("");
        $("#Prc").html("0.00");
    });



    //limpia la columna descuento
    $("#btnlimpiarDes").click(function (event) {


        abp.message.confirm(
            'Eliminar descuentos',
            '¿Seguro desea Eliminar todos los decuentos?',
            function (isConfirmed) {
                if (isConfirmed) {
                    $("#gridService tbody").children().each(function () {
                        $(this).find("input.descuento").val("0.00");
                        calularLinea($(this));

                    });
                }
            }
        );
        $(".confirm").html("Si");
        $(".cancel").html("No");


    });

    //Generar una  nota
    $('#GenerateNote_btn').click(function () {
      
        //clearErrors();
        if (validarservicios()) {
            var records = obtenerLineasNote();
            var counServices = contarLineas();
            var clientId = $('#Client_hidden').val();
            var coin = $('#CoinTypes_DD').val();
            var typefirm = isNaN($('#FirmTypes_DD').val()) ? 'Llave' : $('#FirmTypes_DD').val();
            var typeDocument = $('#TypeDocumentCodigo_hd').val();
            var sessionId = $("#sid").val();
            var isPos = $("#IsPos_HD").val();

            var iscontingencia = $("#Check_contingencia").prop('checked');
            var consecutivoContigencia = $("#text_ConsecutiveNumberContingency").val();
            var fechacontigencia = $("#text_DateContingency").val();
            var motivoContigencia = $("#text_ReasonContingency").val();

            $('#GenerateNote_btn').attr('disabled', 'disabled');

            //if ((clientId == "") && (typeDocument == "1")) {
            //    abp.notify.error('Debe seleccionar un Cliente');
            //    abp.ui.clearBusy();
            //    $('#GenerateInvoice_btn').removeAttr('disabled');
            //    return false;
            //}

            if (counServices == 0) {
                abp.notify.error('Debe agregar al menos un Servicio');
                abp.ui.clearBusy();
                $('#GenerateNote_btn').removeAttr('disabled');
                return false;
            }

            var input = {
                InvoiceId: $('#InvoiceId_hidden').val(),
                NumberInvoiceRef: $('#NumberInvoiceRef_text').val(),
                ClientId: clientId,
                Amount: $('#subtotalNeto_text').val(),
                TaxAmount: $('#TotalImpuesto').val(),
                DiscountAmount: $('#SumDescuentoLinea').val(),
                Total: $('#selectTotalInvoice').val(),
                CodigoMoneda: coin,
                NoteType: $('#TipoNota_dropdown').val(),
                NoteReasons: $('#NoteReasons_DD').val(),
                NoteReasonsOthers: $('#NoteReasons_Others').val(),
                TipoFirma: typefirm,
                DocumentRef: { Id: $('#IdDocument_hidden').val(), ConsecutiveNumber: $('#NumberDocRef_text').val(), TypeDocument: $('#typDoc_text').val() },
                IsContingency: iscontingencia,
                ConsecutiveNumberContingency: consecutivoContigencia,
                ReasonContingency: motivoContigencia,
                DateContingency: fechacontigencia,
                NotesLines: records, Sid: sessionId
            };
            debugger;
            abp.ui.setBusy();
            $.ajax({
                url: generateNote,
                type: "POST",
                cache: false,
                data: JSON.stringify(input),
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                //async: false,
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.url != '') {
                        if ($('#Check_Impimir').is(":checked")) {

                            if (isPos == '1') {
                                jsWebClientPrint.print('id=' + data.result.noteId + '&tenantId=' + data.result.noteTenantId + '&tipoDocumento=' + data.result.noteTypeDocument);
                            }
                            else {
                                printicket(data.result.urlprint);
                            }

                        }
                        window.location.href = data.result.url;
                    }
                    else {
                        abp.notify.error(data.result.error);
                        abp.ui.clearBusy();
                        $('#GenerateNote_btn').removeAttr('disabled');
                    }

                    //if (typeDocument == "4") {
                    //    printicket(data.result.url);
                    //}

                },
                error: function (err) {

                    abp.notify.error(err);
                    abp.ui.clearBusy();
                   

                }
            });
            $('#GenerateNote_btn').removeAttr('disabled');
        }

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

    $(".numeric").on({
        "focus": function (event) {
            $(event.target).select();
        },
        "keyup": function (event) {
            $(event.target).val(function (index, value) {
                return value.replace(/\D/g, "")
                    .replace(/([0-9])([0-9]{2})$/, '$1.$2')
                    .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ",");
            });
        }
    });

    $('input:radio[name=productoServicio]').change(function () {
        if (this.value == 'Servicio') {
            getRequest(createUrl);
        }
        else if (this.value == 'Producto') {
            getRequest(createProduct);
        }
    });




});