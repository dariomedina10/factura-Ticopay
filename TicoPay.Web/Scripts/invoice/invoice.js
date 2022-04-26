(function () {

    $(function () {

        var showInvoiceList = "/Invoice/SearchInvoices";
        var showClientsList = "/Invoice/ShowClientsList";
        var showRegistersList = "/Invoice/ShowRegistersList";
        var searchUrl = "/Invoice/AjaxPage";
        var applyNote = "/Invoice/ApplyNote";
        var applyReverse = "/Invoice/ApplyReverse";
        var showNotesUrl = "/Invoice/ShowNotesInfo";
        var deleteNote = "/Invoice/DeleteNote";
        var payInvoiceUrl = "/Invoice/PayInvoice";
        var voidInvoiceUrl = "/Invoice/VoidInvoice";
        var AddService = "/Invoice/AddService";
        var CalculateTotal = "/Invoice/CalculateTotals";
        var generateInvoice = "/Invoice/Create";
        var generateInvoiceList = "/Invoice/CreateList";
        var resendInvoice = "/Invoice/Resend";
        var resendNote = "/Invoice/ResendNote";
        var payInvoiceListUrl = "/Invoice/PayInvoiceList";
        var positiveBalanceUrl = "/Invoice/SearchPositiveBalance";
        var AddClientNew = "/Invoice/AddClientNew";
        var createClientNew = "/Invoice/CreateClient";
        var createServiceNew = "/Invoice/CreateService";
        var CalculateTotalNewPrice = "/Invoice/CalculateTotalNewPrice";
        var reverseNote = "/Invoice/ReverseNote";

        $('#btnClearClientsList').click(function (e) {
            $('#ClientName_text').val('');
            $('#ClientId_hidden').val('');

            return false;
        });

        var data, grid;

        data = [
            //{ "ID": 1, "Servicio": "Hristo Stoichkov", "Cantidad": "1", "Precio": "500", "Descuento": "0", "Impuesto": "0", "Total": "0" },
            //{ "ID": 2, "Servicio": "Ronaldo Luís Nazário de Lima", "Cantidad": "2", "Precio": "300", "Descuento": "0", "Impuesto": "0", "Total": "0" },
            //{ "ID": 3, "Servicio": "David Platt", "Cantidad": "3", "Precio": "200", "Descuento": "0", "Impuesto": "0", "Total": "0" }
        ];
        grid = $("#grid").grid({
            dataSource: data,
            dataKey: "ID",
            uiLibrary: "bootstrap",
            notFoundText: "No ha agregado Servicios para facturar",
            columns: [
                { field: "IdService", title: "IdService", hidden: true },
                { field: "ID", title: "#", width: 50, sortable: true },
                { field: "Servicio", width: 250, sortable: true },
                { field: "Cantidad", width: 80, sortable: true },
                { field: "Precio", width: 150, sortable: true },
                { field: "Descuento", width: 110, title: "% Descuento", sortable: true },
                { field: "Impuesto", width: 150, sortable: true },
                { field: "Total", width: 150, sortable: true },
                { field: "TotalDescuento", hidden: true },
                { field: "TotalImpuesto", hidden: true },
                { field: "Edit", title: "", width: 34, type: "icon", icon: "glyphicon-pencil", tooltip: "Edit", events: { "click": Edit } },
                { field: "Delete", title: "", width: 34, type: "icon", icon: "glyphicon-remove", tooltip: "Delete", events: { "click": Remove } }
            ]
        });

        function Remove(e) {
            clearErrors();
            $("#DeleteConfirmation_modal").modal("show");
            $("#ID").val(e.data.id);
            return false;
        }

        $("#btnModalOkDeleteOnDemand").click(function (e) {
            $("#DeleteConfirmation_modal").modal("hide");
            var id = parseInt($("#ID").val()) - 1;
            grid.removeRow(id);
            getCalculateTotalInvoice();
        });

        function getRequest(url) {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: url,
                context: document.body,
                success: function (data) {
                    $(".modal-body p.body").html(data);
                    $("#dialog").modal("show");
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos!", "error");
                    abp.ui.clearBusy();
                }
            });
        }

        function isEmpty(str) { return (!str || 0 === str.length); }
        $(".closeModal").click(function (e) { $("#dialog").modal("hide"); });


        $("a.btnCreateForm").on("click", function () {
            clearErrors();
            if (!isEmpty(AddService)) {
                getRequest(AddService);
            }
            return false;
        });

        $("a.btnDiscountForm").on("click", function () {
            clearErrors();
            $("#DiscountMethods").modal("show");
            if ($('#balanceDiscountHidden').val() == "") {
                $('#alertDescuento').hide();
                $("#DeleteDiscountForm").attr("style", "visibility: hidden")
                resultFormatNumber("" + "0.00").then((val) => { $('#discountAmount').val(val) });
            }
            else {
                $('#alertDescuento').show();
                $("#DeleteDiscountForm").removeAttr("style", "visibility: hidden")
                resultFormatNumber("" + $('#balanceDiscountHidden').val()).then((val) => { $('#discountAmount').val(val) });
            }
            return false;
        });

        $("a.btnDeleteDiscountForm").on("click", function () {
            clearErrors();
            $('#selectDiscountCheckHidden').val("");
            $('#balanceDiscountHidden').val("");
            $("#DiscountMethods").modal("hide");
            $('#PercentajeChk').attr('checked', false);
            $('#Discount_text').text("Monto de Descuento");
            getCalculateTotalInvoice();
            return false;
        });

        $('#PercentajeChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                $('#Discount_text').text("% de Descuento");
                $('#discountAmount').val("0");
            } else {
                $('#Discount_text').text("Monto de Descuento");
                resultFormatNumber("" + "0.00").then((val) => { $('#discountAmount').val(val) });
            }

        });

        $("#btnDiscount").click(function (e) {
            clearErrors();
            abp.ui.setBusy();

            var counServices = grid.count();

            if (counServices == 0) {
                writeError('msgNotaError', 'Debe agregar al menos un Servicio', 'error');
                abp.ui.clearBusy();
                return false;
            }

            if ($('#discountAmount').val() == "") {
                writeError('msgNotaError', 'Debe ingresar un monto o % de descuento', 'error');
                abp.ui.clearBusy();
                return false;
            }

            var validateIsSuccesful = true;
            $("#DiscountMethods :input[type=text]").each(function () {
                var inputObject = $(this);
                if (!inputObject.is(":hidden")) {
                    var input = inputObject.val();
                    var regexValue = $(this).attr("data-regular-expression");
                    if (regexValue != null) {
                        var regex = new RegExp($(this).attr("data-regular-expression"));
                        if (!regex.test(input)) {
                            writeError("msgNotaError", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                            abp.ui.clearBusy();
                            validateIsSuccesful = false;
                            return false;
                        }
                    }
                }
            });

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            var isChecked = $('#PercentajeChk').is(':checked');
            var monto = parseFloat($('#discountAmount').val().replace(/[^0-9\.]/g, ''));


            if (isNaN(monto)) {
                writeError('msgNotaError', 'El formato de monto ó % de descuento es invalido!!', 'error');
                abp.ui.clearBusy();
                return false;
            }


            if (monto < 0 || monto == 0) {
                writeError('msgNotaError', 'El monto ó % de descuento debe ser mayor a 0', 'error');
                abp.ui.clearBusy();
                return false;
            }

            if (isChecked) {
                if (monto > 99) {
                    writeError('msgNotaError', 'El % de descuento debe ser menor a 100%', 'error');
                    abp.ui.clearBusy();
                    return false;
                }
            } else {
                if (monto > parseFloat($('#selectTotalInvoice').val().replace(/[^0-9\.]/g, ''))) {
                    writeError('msgNotaError', 'El monto de descuento debe ser menos al total de la factura', 'error');
                    abp.ui.clearBusy();
                    return false;
                }
            }

            if (isChecked) {
                $('#selectDiscountCheckHidden').val(1);
            } else {
                $('#selectDiscountCheckHidden').val(0);
            }

            $('#balanceDiscountHidden').val($('#discountAmount').val());

            getCalculateTotalInvoice();
            $("#DiscountMethods").modal("hide");
            abp.ui.clearBusy();
            return false;

        });

        $("a.btnCreateClienteNewForm").on("click", function () {
            clearErrors();
            if (!isEmpty(AddClientNew)) {
                getRequest(AddClientNew);
            }
            return false;
        });
    

        function getCalculateTotalLine(Qty, amountDesc, ServiceId, GridId, editing) {
            abp.ui.setBusy();
            $('.modal-body p.body :input[type="button"]').attr('disabled', true);
            $.ajax({
                url: CalculateTotal,
                data: { Qty: Qty, discountpercentage: amountDesc, ServiceId: ServiceId, GridId: GridId },
                success: function (data) {
                    $('.modal-body p.body').html(data);
                    abp.ui.clearBusy();
                    if (editing === true) {
                        $('#dialog .chosen-select').prop('disabled', true).trigger("chosen:updated");
                    }
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos del Servicio!", "error");
                    abp.ui.clearBusy();
                }
            });
        }

        function getCalculateNewPrecioTotalLine(Qty, amountDesc, ServiceId, GridId, editing, price) {
            abp.ui.setBusy();
            $('.modal-body p.body :input[type="button"]').attr('disabled', true);
            $.ajax({
                url: CalculateTotalNewPrice,
                data: { Qty: Qty, discountpercentage: amountDesc, ServiceId: ServiceId, GridId: GridId, price: price },
                success: function (data) {
                    $('.modal-body p.body').html(data);
                    abp.ui.clearBusy();
                    if (editing === true) {
                        $('#dialog .chosen-select').prop('disabled', true).trigger("chosen:updated");
                    }
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos del Servicio!", "error");
                    abp.ui.clearBusy();
                }
            });
        }



        function getCalculateTotalInvoice() {
            var records = grid.getAll(), subtotal = 0, discount = 0, discountTotal = 0, impuestoTotal = 0, discountGeneral = 0, discountPercentageOrMount = 0;

            var subtotalTemp = 0;
            if ($('#selectDiscountCheckHidden').val() == "0") {
                $.each(records, function (key, value) {
                    var precio = parseFloat(value.Precio.replace(/[^0-9\.]/g, ''));
                    var cantidad = parseFloat(value.Cantidad.replace(/[^0-9\.]/g, ''));
                    var descuento = parseFloat(value.Descuento.replace(/[^0-9\.]/g, ''));
                    var descuentoLine = ((descuento + discountPercentageOrMount) - (descuento * discountPercentageOrMount) / 100);
                    discount = ((precio * cantidad) * descuentoLine) / 100;
                    subtotalTemp = subtotalTemp + (precio * cantidad);
                });
                discountPercentageOrMount = (parseFloat($('#balanceDiscountHidden').val()) * 100) / subtotalTemp;
            } else if ($('#selectDiscountCheckHidden').val() == "1") {
                discountPercentageOrMount = parseFloat($('#balanceDiscountHidden').val());
            }

            $('#percentageDiscount').val(discountPercentageOrMount);

            $.each(records, function (key, value) {
                var precio = parseFloat(value.Precio.replace(/[^0-9\.]/g, ''));
                var cantidad = parseFloat(value.Cantidad.replace(/[^0-9\.]/g, ''));
                var descuento = parseFloat(value.Descuento.replace(/[^0-9\.]/g, ''));
                var impuesto = parseFloat(value.Impuesto.replace(/[^0-9\.]/g, ''));
                if ($('#selectDiscountCheckHidden').val() == "0" || $('#selectDiscountCheckHidden').val() == "1") {
                    var descuentoLine = ((descuento + discountPercentageOrMount) - (descuento * discountPercentageOrMount) / 100);
                    discount = ((precio * cantidad) * descuentoLine) / 100;
                } else {
                    discount = ((precio * cantidad) * descuento) / 100;
                }
                subtotal = subtotal + (precio * cantidad);
                discountTotal = discountTotal + discount;

                impuesto = (((precio * cantidad) - discount) * ((impuesto * 100) / (precio * cantidad))) / 100;

                impuestoTotal = impuestoTotal + impuesto;

                grid.updateRow(value.ID, { "IdService": value.IdService, "ID": value.ID, "Servicio": value.Servicio, "Cantidad": value.Cantidad, "Precio": value.Precio, "Descuento": value.Descuento, "Impuesto": value.Impuesto, "Total": value.Total, "TotalDescuento": discount, "TotalImpuesto": impuesto });
            });

            //if ($('#balanceDiscountHidden').val() != "0" && $('#balanceDiscountHidden').val() != "" && $('#balanceDiscountHidden').val() != "0.00") {

            //    if ($('#selectDiscountCheckHidden').val() == "1")
            //        discountGeneral = ((subtotal - discountTotal) * parseFloat($('#balanceDiscountHidden').val())) / 100;
            //    else if ($('#selectDiscountCheckHidden').val() == "0")
            //        discountGeneral = parseFloat($('#balanceDiscountHidden').val().replace(/[^0-9\.]/g, ''));

            //    if (typeof discountGeneral !== "undefined")
            //        discountTotal = discountTotal + discountGeneral;
            //}

            $('#subtotal_text').text(toCurrency(subtotal));
            $('#Descuento_text').text(toCurrency(discountTotal));
            $('#totalMountDiscount').val(discountTotal);
            $('#impuesto_text').text(toCurrency(impuestoTotal));
            $('#total_text').text(toCurrency(subtotal - discountTotal + impuestoTotal));
            $('#cashText').val(toCurrency(subtotal - discountTotal + impuestoTotal));
            $('#selectTotalInvoice').val(subtotal - discountTotal + impuestoTotal);
            if ($('#positiveBalanceHidden').val() == 0 || $('#positiveBalanceHidden').val() > (subtotal - discountTotal + impuestoTotal))
                $('#positiveBalance').hide();
            else
                $('#positiveBalance').show();
        }

        function toCurrency(value, n) {
            var decimalPlaces = 2;
            if (n) {
                decimalPlaces = n;
            }
            if (value || value == 0) {
                return value.toFixed(decimalPlaces).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
            }
            return "";
        }

        function Edit(e) {

            var Qty = e.data.record.Cantidad;
            var amountDesc = e.data.record.Descuento;
            var ServiceId = e.data.record.IdService;
            var Price = e.data.record.Precio;
            var GridId = e.data.id;

            clearErrors();
            getCalculateNewPrecioTotalLine(Qty, amountDesc, ServiceId, GridId, true, Price);
            //getCalculateTotalLine(Qty, amountDesc, ServiceId, GridId, true);

            $("#dialog").modal("show");
        }

        this.SaveClientNew = function () {
            clearErrors();
            abp.ui.setBusy();

            if ($('#Name').val() == "") {
                writeError('msgNotaError', 'Debe seleccionar un Nombre', 'error');
                abp.ui.clearBusy();
                return false;
            }

            var formContainer = $("#addClientNewForm");
            $.ajax({
                url: createClientNew,
                type: "POST",
                cache: false,
                data: formContainer.serialize(),
                success: function (data) {
                    $('#anyModalForm').html(data);
                    $('#anyModalForm').modal('show');
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos!", "error");
                    abp.ui.clearBusy();
                }
            });
        };

        this.Save = function () {

            var validateIsSuccesful = true;
            $("#AddServiceInvoice :input[type=text]").each(function () {
                var inputObject = $(this);
                //if (!inputObject.is(":hidden")) {
                var input = inputObject.val();
                var regexValue = $(this).attr("pattern");
                if (regexValue != null) {
                    var regex = new RegExp($(this).attr("pattern"));
                    if (!regex.test(input)) {
                        writeError("msgNotaError", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                        abp.ui.clearBusy();
                        validateIsSuccesful = false;
                        return false;
                    }
                }
                //}
            });

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            clearErrors();

            if ($('#NameService').val() == "" || $('#idService').val() == "") {
                writeError('msgNotaError', 'Debe seleccionar un Servicio', 'error');
                abp.ui.clearBusy();
                return false;
            }

            if ($('#Cantidad').val() == "" || $('#Cantidad').val() == 0) {
                writeError('msgNotaError', 'Debe ingresar una cantidad mayor a cero(0)', 'error');
                abp.ui.clearBusy();
                return false;
            }

            if ($('#Precio').val() == "") {
                writeError('msgNotaError', 'Debe ingresar un precio para el servicio', 'error');
                abp.ui.clearBusy();
                return false;
            }
            else if (parseFloat($('#Precio').val().replace(',', '')) == 0) {
                writeError('msgNotaError', 'Debe ingresar un precio mayor a cero(0)', 'error');
                abp.ui.clearBusy();
                return false;
            }

            if ($('#Descuento').val() == "") {
                $('#Descuento').val("0");
            } else {
                if ($('#Descuento').val() < 0 || $('#Descuento').val() > 100) {
                    writeError('msgNotaError', 'Debe ingresar un descuento valido', 'error');
                    abp.ui.clearBusy();
                    return false;
                }
            }

            if ($('#Precio').val() != $('#PriceHidden').val()) {
                changePrice($('#Precio').val());
            }

            if ($("#GridId").val() != 0) {
                var id = parseInt($("#GridId").val());
                grid.updateRow(id, { "IdService": $("#idService").val(), "ID": id, "Servicio": $("#NameService").val(), "Cantidad": $("#Cantidad").val(), "Precio": $("#Precio").val(), "Descuento": $("#Descuento").val(), "Impuesto": $("#Impuesto").val(), "Total": $("#Total").val() });
                getCalculateTotalInvoice();
            } else {
                grid.addRow({ "IdService": $("#idService").val(), "ID": grid.count() + 1, "Servicio": $("#NameService").val(), "Cantidad": $("#Cantidad").val(), "Precio": $("#Precio").val(), "Descuento": $("#Descuento").val(), "Impuesto": $("#Impuesto").val(), "Total": $("#Total").val() });
                getCalculateTotalInvoice();
            }
            $("#dialog").modal("hide");
        }

        this.InvoiceGenerate = function () {
            
            clearErrors();
            var records = grid.getAll();
            var counServices = grid.count();
            var clientId = $('#ClientId_hidden').val();

            $('#GenerateInvoice_btn').attr('disabled', 'disabled');

            if (clientId == "") {
                writeError('msgNotaError', 'Debe seleccionar un Cliente', 'error');
                abp.ui.clearBusy();
                $('#GenerateInvoice_btn').removeAttr('disabled');
                return false;
            }

            if (counServices == 0) {
                writeError('msgNotaError', 'Debe agregar al menos un Servicio', 'error');
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
                                writeError("msgNotaError", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
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
                    writeError('msgNotaError', 'Debe ingresar una cantidad de días de crédito', 'error');
                    abp.ui.clearBusy();
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    return false;
                }
                else {
                    if ($('#DayCredit').val() == 0) {
                        writeError('msgNotaError', 'La cantidad de días de crédito debe ser mayor a 0', 'error');
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

                var typeDiscountGeneral = $('#selectDiscountCheckHidden').val();
                var discountGeneral = parseFloat($('#totalMountDiscount').val().replace(/[^0-9\.]/g, ''));
                
                var cash = parseFloat($('#cashText').val() == "" ? 0 : $('#cashText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var creditCard = parseFloat($('#creditCardText').val() == "" ? 0 : $('#creditCardText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var deposit = parseFloat($('#depositText').val() == "" ? 0 : $('#depositText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var check = parseFloat($('#checkText').val() == "" ? 0 : $('#checkText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
                var positiveBalance = parseFloat($('#positiveBalanceHidden').val() == "" ? 0 : $('#positiveBalanceHidden').val().replace(/[^0-9\.]/g, '')).toFixed(2);

                if (typePositiveBalance != "4")
                    positiveBalance = 0;

                if (typeCash == "" && typeCreditCard == "" && typeDeposit == "" && typeCheck == "" && typePositiveBalance == "") {
                    writeError("alertPaymentInvoice", "Debe seleccionar al menos un método de pago", "error");
                    abp.ui.clearBusy();
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    return false;
                }

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
                                    writeError("msgNotaError", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
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
                        writeError('msgNotaError', 'Debe ingresar una cantidad de días de crédito', 'error');
                        abp.ui.clearBusy();
                        $('#GenerateInvoice_btn').removeAttr('disabled');
                        return false;
                    }
                    else {
                        if ($('#DayCredit').val() == 0) {
                            writeError('msgNotaError', 'La cantidad de días de crédito debe ser mayor a 0', 'error');
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }
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
                                    writeError("msgNotaError", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
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

                    if (typePositiveBalance != "4")
                        positiveBalance = 0;

                    if (typeCash == "" && typeCreditCard == "" && typeDeposit == "" && typeCheck == "" && typePositiveBalance == "") {
                        writeError("alertPaymentInvoice", "Debe seleccionar al menos un método de pago", "error");
                        abp.ui.clearBusy();
                        $('#GenerateInvoice_btn').removeAttr('disabled');
                        return false;
                    }

                    if (typeCreditCard == "1") {
                        transCreditCard = $('#nroCreditCardText').val();
                        if (transCreditCard.length <= 0) {
                            writeError("alertPaymentInvoice", "Debe ingresar un número de transacción", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        } else if (creditCard <= 0) {
                            writeError("alertPaymentInvoice", "Debe ingresar el monto de transacción", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeDeposit == "3") {
                        transDeposit = $('#nroDepositText').val();
                        if (transDeposit.length <= 0) {
                            writeError("alertPaymentInvoice", "Debe ingresar un número de depósito / Transferencia", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        } else if (deposit <= 0) {
                            writeError("alertPaymentInvoice", "Debe ingresar el monto de depósito / Transferencia", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCheck === "2") {
                        transCheck = $('#nroCheckText').val();
                        if (transCheck.length <= 0) {
                            writeError("alertPaymentInvoice", "Debe ingresar un número de cheque", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        } else if (check <= 0) {
                            writeError("alertPaymentInvoice", "Debe ingresar el monto de cheque", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCash == "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {

                        var total = $('#selectTotalInvoice').val();
                        var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                        var dif = (parseFloat(total) - parseFloat(subtotal));
                        if (dif != 0) {
                            writeError("alertPaymentInvoice", "El monto de los métodos de pago debe ser igual al monto total de la factura", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCash != "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {

                        var total = $('#selectTotalInvoice').val();
                        var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                        var dif = (parseFloat(total) - parseFloat(subtotal));
                        if (dif < 0) {
                            writeError("alertPaymentInvoice", "El monto de los métodos de pago debe ser menor al monto total de la factura ya que se paga una parte en efectivo", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCash != "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {
                        var total = $('#selectTotalInvoice').val();
                        var subtotal = (parseFloat(cash) + parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                        var dif = (parseFloat(total) - parseFloat(subtotal));
                        if (dif > 0) {
                            writeError("alertPaymentInvoice", "El monto de todos los métodos de pago debe ser mayor o igual al monto de la factura.", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }

                    if (typeCash == "0") {
                        var total = $('#selectTotalInvoice').val();
                        var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                        difPay = (parseFloat(total) - parseFloat(subtotal));
                        var dif = (parseFloat(cash) - parseFloat(difPay));

                        if (dif < 0) {
                            writeError("alertPaymentInvoice", "El monto en efectivo debe ser mayor o igual al monto total de la factura", "error");
                            abp.ui.clearBusy();
                            $('#GenerateInvoice_btn').removeAttr('disabled');
                            return false;
                        }
                    }
                }

            }
            //____________________________________________________________________________________________________________________________

            var typeDiscountGeneral = $('#selectDiscountCheckHidden').val();
            var discountGeneral = parseFloat($('#balanceDiscountHidden').val().replace(/[^0-9\.]/g, ''));
            var discountPercentageOrMount = parseFloat($('#percentageDiscount').val());

            abp.ui.setBusy();
            $.ajax({
                url: generateInvoiceList,
                type: "POST",
                cache: false,
                data: JSON.stringify({
                    gridData: records, clientId: clientId, typePaymentCash: typeCash, balanceCash: difPay, typePaymentCreditCard: typeCreditCard, balanceCreditCard: creditCard, transCreditCard: transCreditCard, typePaymentDeposit: typeDeposit,
                    balanceDeposit: deposit, transDeposit: transDeposit, typePaymentCheck: typeCheck, balanceCheck: check, transCheck: transCheck, typePositiveBalance: typePositiveBalance,
                    balancePositiveBalance: positiveBalance, typeDiscount: typeDiscountGeneral, discount: discountGeneral, discountPercentage: discountPercentageOrMount, dayCredit: $('#DayCredit').val(), ConditionSaleType: conditionSaleType
                }),
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    $('#GenerateInvoice_btn').removeAttr('disabled');
                    if (data.result.value == "1") {
                        grid.clear();
                        $('#ClientName_text').val("");
                        $('#ClientId_hidden').val("");
                        //$('#creditCardText').val("");
                        //$('#FormaPago_dropdown').val("0")
                        $('#subtotal_text').text(0);
                        $('#Descuento_text').text(0);
                        $('#impuesto_text').text(0);
                        $('#total_text').text(0);
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
                        $('#CashChk').attr('checked', false);
                        $('#CreditCardChk').attr('checked', false);
                        $('#DepositChk').attr('checked', false);
                        $('#CheckChk').attr('checked', false);
                        $('#PositiveBalanceChk').removeAttr('checked');
                        $('#selectPositiveBalance').val("");
                        $('#positiveBalanceHidden').val("");
                        $('#positiveBalance').hide();
                        $('#selectDiscountCheckHidden').val(0);
                        $('#balanceDiscountHidden').val(0);
                        $('#PercentajeChk').attr('checked', false);
                        //$('#ContadoChk').attr('checked', false);
                        //$('#CreditoChk').attr('checked', false);
                        $('#panelContado').fadeIn();
                        $('#panelCredito').fadeOut();
                        $('#DayCredit').val("");
                        writeError('IndexAlerts', 'Factura generada correctamente', 'success');
                        $('#totalMountDiscount').val("");
                        $('#percentageDiscount').val("");
                        $('#panelContado').fadeOut();
                        grid.grid('destroy', true, true);
                        grid.grid({
                            dataSource: [],
                            dataKey: "ID",
                            uiLibrary: "bootstrap",
                            notFoundText: "No ha agregado Servicios para facturar",
                            columns: [
                                { field: "IdService", title: "IdService", hidden: true },
                                { field: "ID", title: "#", width: 50, sortable: true },
                                { field: "Servicio", width: 250, sortable: true },
                                { field: "Cantidad", width: 80, sortable: true },
                                { field: "Precio", width: 150, sortable: true },
                                { field: "Descuento", width: 110, title: "% Descuento", sortable: true },
                                { field: "Impuesto", width: 150, sortable: true },
                                { field: "Total", width: 150, sortable: true },
                                { field: "TotalDescuento", hidden: true },
                                { field: "TotalImpuesto", hidden: true },
                                { field: "Edit", title: "", width: 34, type: "icon", icon: "glyphicon-pencil", tooltip: "Edit", events: { "click": Edit } },
                                { field: "Delete", title: "", width: 34, type: "icon", icon: "glyphicon-remove", tooltip: "Delete", events: { "click": Remove } }
                            ]
                        });
                        $('#GenerateInvoice_btn').removeAttr('disabled');
                    }
                    else if (data.result.value == "-1") {
                        writeError('msgNotaError', data.result.error, 'error');
                    }
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError('msgErrorAnyModal', err, 'error');
                    abp.ui.clearBusy();
                }
            });
        }

        $('#btnShowClientsList').click(function (e) {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: showClientsList,
                type: "POST",
                cache: false,
                data: { page: null, q: $("#searchBoxArticulo").val() },
                success: function (data) {
                    $('#anyModalForm').html(data);
                    $('#anyModalForm').modal('show');
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError('msgErrorAnyModal', err, 'error');
                    abp.ui.clearBusy();
                }
            });
        });

        $('#btnShowRegistersList').click(function () {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: showRegistersList,
                type: "POST",
                cache: false,
                data: { page: null, q: $("#searchBoxRegister").val() },
                success: function (data) {
                    $('#anyModalForm').html(data);
                    $('#anyModalForm').modal('show');
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError('msgErrorAnyModal', err, 'error');
                    abp.ui.clearBusy();
                }
            });
        });

        $('#btnPayInvoice').click(function (e) {
            clearErrors();
            abp.ui.setBusy();
            var validateIsSuccesful = true;
            $("#PaymentMethods :input[type=text]").each(function () {
                var inputObject = $(this);
                if (!inputObject.is(":hidden")) {
                    var input = inputObject.val();
                    var regexValue = $(this).attr("data-regular-expression");
                    if (regexValue != null) {
                        var regex = new RegExp($(this).attr("data-regular-expression"));
                        if (!regex.test(input)) {
                            writeError("alertPaymentInvoice", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                            abp.ui.clearBusy();
                            validateIsSuccesful = false;
                            return false;
                        }
                    }
                }
            });

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            var clientId = $('#clientIdHidden').val();
            var invoiceId = $('#selectInvoiceId').val();
            var typeCash = $('#selectPaymentMethodCash').val();
            var typeCreditCard = $('#selectPaymentMethodCreditCard').val();
            var typeDeposit = $('#selectPaymentMethodDeposit').val();
            var typeCheck = $('#selectPaymentMethodCheck').val();
            var typePositiveBalance = $('#selectPositiveBalance').val();

            var tittle = "Transacción";
            var bodyModal = "";
            var transCreditCard = "";
            var transDeposit = "";
            var transCheck = "";
            var difPay = "";
            var cash = parseFloat($('#cashText').val() == "" ? 0 : $('#cashText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
            var creditCard = parseFloat($('#creditCardText').val() == "" ? 0 : $('#creditCardText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
            var deposit = parseFloat($('#depositText').val() == "" ? 0 : $('#depositText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
            var check = parseFloat($('#checkText').val() == "" ? 0 : $('#checkText').val().replace(/[^0-9\.]/g, '')).toFixed(2);
            var positiveBalance = parseFloat($('#positiveBalanceHidden').val() == "" ? 0 : $('#positiveBalanceHidden').val().replace(/[^0-9\.]/g, '')).toFixed(2);
            var bank = $("#listBank").val();
            var userCard = $("#UserCreditCardText").val();
            if ((parseFloat(cash + creditCard + deposit + check) > parseFloat(9999999999999.99)) ){
                writeError("alertPaymentInvoice", "El Monto de las forma de pago no puede ser mayor a 9,999,999,999,999.99 ", "error");
                abp.ui.clearBusy();
                return false;
            }

            if (typePositiveBalance != "4")
                positiveBalance = 0;

            if (typeCash == "" && typeCreditCard == "" && typeDeposit == "" && typeCheck == "" && typePositiveBalance == "") {
                writeError("alertPaymentInvoice", "Debe seleccionar al menos un método de pago", "error");
                abp.ui.clearBusy();
                return false;
            }

            if (typeCash == "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {
                var total = $('#selectTotalInvoice').val();
                var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                var dif = (parseFloat(total) - parseFloat(subtotal)).toFixed(2);
                if (dif != 0) {
                    writeError("alertPaymentInvoice", "El monto de los métodos de pago debe ser igual al monto total de la factura", "error");
                    abp.ui.clearBusy();
                    return false;
                }
            }

            if (typeCash != "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {
                var total = $('#selectTotalInvoice').val();
                var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                var dif = (parseFloat(total) - parseFloat(subtotal)).toFixed(2);
                if (dif < 0) {
                    writeError("alertPaymentInvoice", "El monto de los métodos de pago debe ser menor al monto total de la factura ya que se paga una parte en efectivo", "error");
                    abp.ui.clearBusy();
                    return false;
                }
            }

            if (typeCash != "" && (typeCreditCard != "" || typeDeposit != "" || typeCheck != "" || typePositiveBalance != "")) {
                var total = $('#selectTotalInvoice').val();
                var subtotal = (parseFloat(cash) + parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                var dif = (parseFloat(total) - parseFloat(subtotal)).toFixed(2);
                if (dif > 0) {
                    writeError("alertPaymentInvoice", "El monto de todos los métodos de pago debe ser mayor o igual al monto de la factura.", "error");
                    abp.ui.clearBusy();
                    return false;
                }
            }
            if (typeCash == "0") {

                var total = $('#selectTotalInvoice').val();
                var subtotal = (parseFloat(creditCard) + parseFloat(deposit) + parseFloat(check) + parseFloat(positiveBalance));
                difPay = (parseFloat(total) - parseFloat(subtotal));
                var dif = (parseFloat(cash) - parseFloat(difPay)).toFixed(2);


                if (dif < 0) {
                    writeError("alertPaymentInvoice", "El monto en efectivo debe ser mayor o igual al monto total de la factura", "error");
                    abp.ui.clearBusy();
                    return false;
                }
                //tittle = "Vuelto de la transacción";
                bodyModal = "Su vuelto es de: " + dif;
            }

            if (typeCreditCard == "1") {
                transCreditCard = $('#nroCreditCardText').val();
                if (transCreditCard.length <= 0) {
                    writeError("alertPaymentInvoice", "Debe ingresar un número de transacción", "error");
                    abp.ui.clearBusy();
                    return false;
                } else if (creditCard <= 0) {
                    writeError("alertPaymentInvoice", "Debe ingresar el monto de transacción", "error");
                    abp.ui.clearBusy();
                    return false;
                }


                //tittle = "Transacción";
                bodyModal = "Número de Transacción: " + transCreditCard + " / " + bodyModal;
            }

            if (typeDeposit == "3") {
                transDeposit = $('#nroDepositText').val();
                if (transDeposit.length <= 0) {
                    writeError("alertPaymentInvoice", "Debe ingresar un número de depósito/Transferencia", "error");
                    abp.ui.clearBusy();
                    return false;
                } else if (deposit <= 0) {
                    writeError("alertPaymentInvoice", "Debe ingresar el monto de depósito/Transferencia", "error");
                    abp.ui.clearBusy();
                    return false;
                }

                //tittle = "Cheque";
                bodyModal = "Número de Depósito / Transferencia: " + transDeposit + " / " + bodyModal;
            }

            if (typeCheck === "2") {
                transCheck = $('#nroCheckText').val();
                if (transCheck.length <= 0) {
                    writeError("alertPaymentInvoice", "Debe ingresar un número de cheque", "error");
                    abp.ui.clearBusy();
                    return false;
                } else if (check <= 0) {
                    writeError("alertPaymentInvoice", "Debe ingresar el monto de cheque", "error");
                    abp.ui.clearBusy();
                    return false;
                }
                //tittle = "Deposito";
                bodyModal = "Número de Cheque: " + transCheck + " / " + bodyModal;
            }

            if (typePositiveBalance === "4") {
                bodyModal = "Pago con Saldo a Favor: " + positiveBalance + " / " + bodyModal;
            }

            $.ajax({
                url: payInvoiceListUrl,
                type: "POST",
                cache: false,
                data: {
                    typePaymentCash: typeCash, balanceCash: difPay, typePaymentCreditCard: typeCreditCard, balanceCreditCard: creditCard, transCreditCard: transCreditCard, typePaymentDeposit: typeDeposit,
                    balanceDeposit: deposit, transDeposit: transDeposit, typePaymentCheck: typeCheck, balanceCheck: check, transCheck: transCheck, typePositiveBalance: typePositiveBalance,
                    balancePositiveBalance: positiveBalance, invoiceId: invoiceId, BankId: bank, UserCard: userCard,
                },

                success: function (data) {
                    if (data.result == 1) {
                        updateRequestsListLocal(clientId, invoiceId);
                        $('#PaymentMethods').modal('hide');
                        $('#tittleReturnModal').html(tittle);
                        $('#returnInvoiceSelect').html(bodyModal);
                        $('#return_modal').modal('show');
                        $('#cash').hide();
                        $('#selectPaymentMethodCash').val("");
                        ClearCash();
                        $('#CashChk').removeAttr('checked');
                        $('#creditCard').hide();
                        $('#selectPaymentMethodCreditCard').val("");
                        ClearCreditCard();
                        $('#CreditCardChk').removeAttr('checked');
                        $('#deposit').hide();
                        $('#selectPaymentMethodDeposit').val("");
                        $("#UserCreditCardText").val("");
                        ClearDeposit();
                        $('#DepositChk').removeAttr('checked');
                        $('#check').hide();
                        $('#selectPaymentMethodCheck').val("");
                        $('#listBank').val('');
                        ClearCheck();
                        $('#CheckChk').removeAttr('checked');
                        $('#PositiveBalanceChk').removeAttr('checked');
                        $('#selectPositiveBalance').val("");
                        $('#positiveBalanceHidden').val("");
                        $('#positiveBalance').hide();
                        abp.ui.clearBusy();
                    } else {
                        writeError("alertPaymentInvoice", "Error al pagar la factura, por favor verifique los datos", "error");
                        abp.ui.clearBusy();
                    }

                }, error: function (err) {
                    writeError('msgErrorAnyModal', err, 'error');
                    abp.ui.clearBusy();
                }
            });
        });

        $('#btnModalOkDeleteConfirmation').click(function (e) {
            abp.ui.setBusy();
            clearErrors();
            var clientId = $('#clientIdHidden').val();
            var invoiceId = $('#ItemToDelete_hidden').val();
            $.ajax({
                url: voidInvoiceUrl,
                type: "POST",
                cache: false,
                data: { clientId: clientId, invoiceId: invoiceId },
                success: function (data) {
                    $('#anyListEntity').html(data);
                    $('#AnularConfirmation_modal').modal('hide');
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError('msgErrorAnyModal', err, 'error');
                    abp.ui.clearBusy();
                }
            });
        });


        $('#btnModalOkResendConfirmation').click(function (e) {
            $("#ResendConfirmation_modal").modal("hide");
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: resendInvoice,
                data: { id: $('#btnModalOkResendConfirmation').data('invoice-id') },
                success: function (data) {
                    if (data.success == true) {
                        writeError('IndexAlerts', 'Factura enviada con éxito.', 'success');
                    }
                    abp.ui.clearBusy();
                },
                error: function (err) {
                    writeError('IndexAlerts', 'Error al reenviar la factura.', err);
                    abp.ui.clearBusy();
                }
            });
            return false;
        });       

        this.resendInvoice = function (invoiceId, numero, clientEmail) {
            var enviar = confirm('Se enviará la factura Nro.: ' + numero + ' al correo ' + clientEmail);
            if (enviar == true) {
                clearErrors();
                abp.ui.setBusy();
                $.ajax({
                    url: resendInvoice,
                    data: { id: invoiceId },
                    success: function (data) {
                        if (data.success == true) {
                            writeError('IndexAlerts', 'Factura enviada con éxito.', 'success');
                        }
                        abp.ui.clearBusy();
                    },
                    error: function (err) {
                        writeError('IndexAlerts', 'Error al reenviar la factura.', err);
                        abp.ui.clearBusy();
                    }
                });
                return false;
            }
        }

        $('#btnModalOkResendNoteConfirmation').click(function (e) {
            $("#ResendNoteConfirmation_modal").modal("hide");
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: resendNote,
                data: { id: $('#btnModalOkResendNoteConfirmation').data('note-id') },
                success: function (data) {
                    if (data.success == true) {
                        writeError('IndexAlerts', 'Nota enviada con éxito.', 'success');
                    }
                    abp.ui.clearBusy();
                },
                error: function (err) {
                    writeError('IndexAlerts', 'Error al reenviar la note.', err);
                    abp.ui.clearBusy();
                }
            });
            return false;
        });


        this.resendNote = function (noteId, numero, clientEmail) {
            var enviar = confirm('Se enviará la nota Nro.: ' + numero + ' al correo ' + clientEmail);
            if (enviar == true) {
                clearErrors();
                abp.ui.setBusy();
                $.ajax({
                    url: resendNote,
                    data: { id: noteId },
                    success: function (data) {
                        if (data.success == true) {
                            writeError('IndexAlerts', 'Nota enviada con éxito.', 'success');
                        }
                        abp.ui.clearBusy();
                    },
                    error: function (err) {
                        writeError('IndexAlerts', 'Error al reenviar la Nota.', err);
                        abp.ui.clearBusy();
                    }
                });
                return false;
            }
        }

        $('#CashChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                $('#selectPaymentMethodCash').val(0);
                var isCheckedCreditCard = $('#CreditCardChk').is(':checked');
                var isCheckedDeposit = $('#DepositChk').is(':checked');
                var isCheckedCheck = $('#CheckChk').is(':checked');
                if (isCheckedCreditCard || isCheckedDeposit || isCheckedCheck) {
                    resultFormatNumber("" + "0.00").then((val) => { $('#cashText').val(val) });
                }
                else {
                    resultFormatNumber("" + $('#selectTotalInvoice').val()).then((val) => { $('#cashText').val(val) });
                    //resultFormatNumber("" + $('#total_text').val()).then((val) => { $('#cashText').val(val) });
                    
                }
                $('#cash').show();
            } else {
                $('#cash').hide();
                $('#selectPaymentMethodCash').val("");
            }

        });

        function ClearCash() {
            resultFormatNumber("" + "0.00").then((val) => { $('#cashText').val(val) });
        }

        $('#CreditCardChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                var isCheckedCash = $('#CashChk').is(':checked');
                var isCheckedDeposit = $('#DepositChk').is(':checked');
                var isCheckedCheck = $('#CheckChk').is(':checked');
                if (isCheckedCash || isCheckedDeposit || isCheckedCheck) {
                    resultFormatNumber("" + "0.00").then((val) => { $('#creditCardText').val(val) });
                }
                else {
                    resultFormatNumber("" + $('#selectTotalInvoice').val()).then((val) => { $('#creditCardText').val(val) });
                    //resultFormatNumber("" + $('#total_text').val()).then((val) => { $('#creditCardText').val(val) });
                }
                $('#creditCard').show();
                $('#selectPaymentMethodCreditCard').val(1);
            } else {
                $('#creditCard').hide();
                $('#selectPaymentMethodCreditCard').val("");
                ClearCreditCard();
            }

        });

        function ClearCreditCard() {
            resultFormatNumber("" + "0.00").then((val) => { $('#creditCardText').val(val) });
            $('#nroCreditCardText').val("");
        }

        $('#DepositChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                var isCheckedCash = $('#CashChk').is(':checked');
                var isCheckedCreditCard = $('#CreditCardChk').is(':checked');
                var isCheckedCheck = $('#CheckChk').is(':checked');
                if (isCheckedCash || isCheckedCreditCard || isCheckedCheck) {
                    resultFormatNumber("" + "0.00").then((val) => { $('#depositText').val(val) });
                }
                else {
                    resultFormatNumber("" + $('#selectTotalInvoice').val()).then((val) => { $('#depositText').val(val) });
                    //resultFormatNumber("" + $('#total_text').val()).then((val) => { $('#depositText').val(val) });
                }
                $('#deposit').show();
                $('#selectPaymentMethodDeposit').val(3);
            } else {
                $('#deposit').hide();
                $('#selectPaymentMethodDeposit').val("");
                ClearDeposit();
            }

        });

        function ClearDeposit() {
            $('#depositText').val("");
            $('#nroDepositText').val("");
        }

        $('#CheckChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                var isCheckedCash = $('#CashChk').is(':checked');
                var isCheckedCreditCard = $('#CreditCardChk').is(':checked');
                var isCheckedDeposit = $('#DepositChk').is(':checked');
                if (isCheckedCash || isCheckedCreditCard || isCheckedDeposit) {
                    resultFormatNumber("" + "0.00").then((val) => { $('#checkText').val(val) });
                }
                else {
                    resultFormatNumber("" + $('#selectTotalInvoice').val()).then((val) => { $('#checkText').val(val) });
                    //resultFormatNumber("" + $('#total_text').val()).then((val) => { $('#checkText').val(val) });
                }
                $('#check').show();
                $('#selectPaymentMethodCheck').val(2);
            } else {
                $('#check').hide();
                $('#selectPaymentMethodCheck').val("");
                ClearCheck();
            }

        });

        $('#PositiveBalanceChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                $('#selectPositiveBalance').val(4);
            } else {
                $('#selectPositiveBalance').val("");
                ClearCheck();
            }

        });

        function ClearCheck() {
            resultFormatNumber("" + "0.00").then((val) => { $('#checkText').val(val) });
            $('#nroCheckText').val("");
        }

        //$("#checkText").on({
        //    "focus": function (event) {
        //        $(event.target).select();
        //    },
        //    "keyup": function (event) {
        //        $(event.target).val(function (index, value) {
        //            return value.replace(/\D/g, "")
        //                .replace(/([0-9])([0-9]{2})$/, '$1.$2')
        //                .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ",");
        //        });
        //    }
        //});


        //---- Errors Alerts
        function clearErrors() {
            $('#msgErrorAnyModal').html('');
            $('#IndexAlerts').html('');
            $('#alertPaymentInvoice').html('');
            $('#msgNotaError').html('');
        }


        this.deleteFunc = function (btn) {
            clearErrors();
            $('#DeleteConfirmationMain_modal').modal('show');
            $('#ItemToDeleteMain_hidden').val($(btn).attr("data-idrequest"));
            return false;
        }

        this.ResendFunc = function (invoiceId, numero, clientEmail) {
            clearErrors();
            $("#textConfirmationModal").html('Se enviará la factura Nro.: ' + numero + ' al correo ' + clientEmail);
            $("#ResendConfirmation_modal").modal("show");
            $("#btnModalOkResendConfirmation").data('invoice-id', invoiceId);
            //$("#ItemToDelete_hidden").val(btn.attr("data-idEntity"));
            return false;
        }

        this.ResendNoteFunc = function (noteId, numero, clientEmail) {
            clearErrors();
            $("#textConfirmationNoteModal").html('Se enviará la nota Nro.: ' + numero + ' al correo ' + clientEmail);
            $("#ResendNoteConfirmation_modal").modal("show");
            $("#btnModalOkResendNoteConfirmation").data('note-id', noteId);
            //$("#ItemToDelete_hidden").val(btn.attr("data-idEntity"));
            return false;
        }

        $('#ValueMonto_text').change(function () {

            var validateIsSuccesful = true;

            var inputObject = $(this);
            if (!inputObject.is(":hidden")) {
                var input = inputObject.val();
                var regexValue = $(this).attr("data-regular-expression");
                if (regexValue != null) {
                    var regex = new RegExp($(this).attr("data-regular-expression"));
                    if (!regex.test(input)) {
                        writeError("alertPaymentInvoice", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                        abp.ui.clearBusy();
                        validateIsSuccesful = false;
                        return false;
                    }
                }
            }

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            var amount = parseFloat($(this).val().replace(/[^0-9\.]/g, ''));
            var rate = parseFloat($("#invoiceRateHidden").val().replace(/[^0-9\.]/g, ''));

            //var amount = parseFloat($(this).val());
            //var rate = $("#invoiceRateHidden").val();

            var montotax = parseFloat(amount * rate / 100);
            var totalNota = parseFloat(amount + montotax);

            $('#ValueMonto_text').val(toCurrency(amount));
            $('#MontoTax_text').val(toCurrency(montotax));
            $('#TotalNota_text').val(toCurrency(totalNota));
        });


        this.getval = function (sel) {
            $(':input[id="btnSave"]').prop('disabled', true);

            var validateIsSuccesful = true;
            var inputObject = $(sel);
            var input = inputObject.val();
            var regexValue = $(sel).attr("pattern");
            if (regexValue != null) {
                var regex = new RegExp($(sel).attr("pattern"));
                if (!regex.test(input)) {
                    writeError("msgNotaError", "La " + $(sel).attr("data-name") + " debe ser un numero entero del 1 al 99999, por favor verifique.", "error");
                    abp.ui.clearBusy();
                    validateIsSuccesful = false;
                    return false;
                }
            }

            var Qty = sel.value
            if (Qty < 1) {
                writeError("msgNotaError", "La " + $(sel).attr("data-name") + " debe ser un numero entero del 1 al 99999, por favor verifique.", "error");
                abp.ui.clearBusy();
                validateIsSuccesful = false;
                return false;
            }

            $(':input[id="btnSave"]').prop('disabled', false);
            var amountDesc = $("#Descuento").val();
            var ServiceId = $("#idService").val();
            var GridId = $("#GridId").val();
            var Price = $("#Precio").val();

            clearErrors();
            getCalculateNewPrecioTotalLine(Qty, amountDesc, ServiceId, GridId, true, Price);
        }

        this.changeDesc = function (sel) {
            $(':input[id="btnSave"]').prop('disabled', true);

            var validateIsSuccesful = true;
            var inputObject = $(sel);
            var input = inputObject.val();
            var regexValue = $(sel).attr("pattern");
            if (regexValue != null) {
                var regex = new RegExp($(sel).attr("pattern"));
                if (!regex.test(input)) {
                    writeError("msgNotaError", "La " + $(sel).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                    abp.ui.clearBusy();
                    validateIsSuccesful = false;
                    return false;
                }
            }

            $(':input[id="btnSave"]').prop('disabled', false);
            var amountDesc = sel.value;

            if (amountDesc < 1 || amountDesc > 100) {
                writeError('msgNotaError', 'Debe ingresar un descuento entre 1 y 99', 'error');
                abp.ui.clearBusy();
                return false;
            }

            var Qty = $("#Cantidad").val();
            var ServiceId = $("#idService").val();
            var GridId = $("#GridId").val();
            var Price = $("#Precio").val();

            clearErrors();
            getCalculateNewPrecioTotalLine(Qty, amountDesc, ServiceId, GridId, true, Price)
        }

        this.validateButton = function () {
            $("#btnSave").attr('disabled', 'disabled');
        }

        this.deleteValidateButton = function () {
            $("#btnSave").removeAttr('disabled');
        }

        this.changePrice = function (sel) {
            

            $(':input[id="btnSave"]').prop('disabled', true);

            if ($('#NameService').val() == "" || $('#idService').val() == "") {
                writeError('msgNotaError', 'Debe seleccionar un Servicio', 'error');
                abp.ui.clearBusy();
                return false;
            }

            var validateIsSuccesful = true;
            var inputObject = $(sel);
            if (!inputObject.is(":hidden")) {
                var input = inputObject.val();
                var regexValue = $(sel).attr("pattern");
                if (regexValue != null) {
                    var regex = new RegExp($(sel).attr("pattern"));
                    if (!regex.test(input)) {
                        writeError("msgNotaError", "El " + $(sel).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                        abp.ui.clearBusy();
                        validateIsSuccesful = false;
                        return false;
                    }
                }
            }

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            if ($(sel).val() == "") {
                writeError('msgNotaError', 'Debe ingresar un precio para el servicio', 'error');
                abp.ui.clearBusy();
                return false;
            } else if (parseFloat($(sel).val().replace(/[^0-9\.]/g, '')) == 0) {
                writeError('msgNotaError', 'Debe ingresar un precio mayor a cero(0)', 'error');
                abp.ui.clearBusy();
                return false;
            }

            $(':input[id="btnSave"]').prop('disabled', false);

            var Qty = $("#Cantidad").val();
            var amountDesc = $("#Descuento").val();
            var ServiceId = $("#idService").val();
            var GridId = $("#GridId").val();
            var Price = parseFloat($(sel).val().replace(/[^0-9\.]/g, ''));

            getCalculateNewPrecioTotalLine(Qty, amountDesc, ServiceId, GridId, true, Price)
        }

        this.ApplyNota = function () {
            var amount = 0;
            clearErrors();

            var validateIsSuccesful = true;
            $("#modalNotes :input[type=text]").each(function () {
                var inputObject = $(this);
                if (!inputObject.is(":hidden")) {
                    var input = inputObject.val();
                    var regexValue = $(this).attr("data-regular-expression");
                    if (regexValue != null) {
                        var regex = new RegExp($(this).attr("data-regular-expression"));
                        if (!regex.test(input)) {
                            writeError("alertPaymentInvoice", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                            abp.ui.clearBusy();
                            validateIsSuccesful = false;
                            return false;
                        }
                    }
                }
            });

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            if ($('#ValueMonto_text').val() != "") { amount = parseFloat($('#ValueMonto_text').val().replace(/[^0-9\.]/g, '')); }

            var invoiceId = $('#invoiceIdHidden').val();
            var type = $("#TipoNota_dropdown").val();
            var reason = $("#MotivoNota_dropdown").val();
            var clientId = $('#clientIdHidden').val();
            var Balance = parseFloat($('#invoiceBalanceHidden').val());
            //var montotax = $('#MontoTax_text').val();
            //var totalNota = $('#TotalNota_text').val();
            var montotax = 0;
            if ($('#MontoTax_text').val() != "")
                montotax = parseFloat($('#MontoTax_text').val().replace(/[^0-9\.]/g, ''));

            var totalNota = parseFloat($('#TotalNota_text').val().replace(/[^0-9\.]/g, ''));


            $('#AplicarNota_btn').attr('disabled', 'disabled');
            if (amount <= 0) {
                writeError('msgNotaError', 'El monto debe ser mayor a 0.', 'error');
                $('#AplicarNota_btn').removeAttr('disabled');
                return;
            }
            if ((totalNota > Balance) && (type == 1)) {
                writeError('msgNotaError', 'El monto Total de la Nota de Crédito no puede ser mayor al monto Total de la Factura', 'error');
                $('#AplicarNota_btn').removeAttr('disabled');
                return;
            }
            //console.log(reason);
            //if (description == "") {
            //    writeError('msgNotaError', 'Debe ingresar una descripción', 'error');
            //    $('#AplicarNota_btn').removeAttr('disabled');
            //    return;
            //}
            //console.log("hi");
            abp.ui.setBusy();
            $.ajax({
                type: "POST",
                url: applyNote,
                data: { invoiceId: invoiceId, amount: amount, reason: reason, type: type, clientId: clientId, montotax: montotax, totalNota: totalNota },
                success: function (data) {
                    if (data == "-1") {
                        writeError('msgNotaError', 'Debe ingresar un monto mayor a 0.', 'error');
                    }
                    else if (data == "-2") {
                        writeError('msgNotaError', 'Existen notas pendientes por firma digital', 'error');
                    } 
                    else {
                        $('#modalNotes').modal('hide');
                        $("#anyListEntity").html(data);
                    }
                    $('#AplicarNota_btn').removeAttr('disabled');
                    $('#txtCoin').val('');
                    abp.ui.clearBusy();
                },
                error: function () {
                    writeError('msgNotaError', 'Error al aplicar nota.', 'error');
                    $('#AplicarNota_btn').removeAttr('disabled');
                    abp.ui.clearBusy();
                }
            });
        }

        this.ApplyReverse = function () {
            var amount = 0;
           
            clearErrors();
            if ($('#ReverseValueMonto_text').val() != "") { amount = parseFloat($('#ReverseValueMonto_text').val().replace(/[^0-9\.]/g, '')); }
           

            var invoiceId = $('#invoiceIdHiddenReverse').val();
            var type = $("#ReverseTipoNota_dropdown").val();
            var reason = $("#ReverseMotivoNota_dropdown").val();
            var clientId = $('#clientIdHidden').val();
            var Balance = parseFloat($('#invoiceBalanceHiddenReverse').val());
            //var montotax = $('#ReverseMontoTax_text').val();
            //var totalNota = $('#ReverseTotalNota_text').val();
            var montotax = 0;
            if ($('#ReverseMontoTax_text').val() != "")
                montotax = parseFloat($('#ReverseMontoTax_text').val().replace(/[^0-9\.]/g, ''));

            var totalNota = parseFloat($('#ReverseTotalNota_text').val().replace(/[^0-9\.]/g, ''));


            $('#AplicarReverse_btn').attr('disabled', 'disabled');
            if (amount <= 0) {
                writeError('msgNotaErrorReverse', 'El monto debe ser mayor a 0.', 'error');
                $('#AplicarReverse_btn').removeAttr('disabled');
                return;
            }
            //if ((totalNota > (amount + montotax)) && (type == 1)) {
            //    writeError('msgNotaErrorReverse', 'El monto Total de la Nota de Credito no puede ser mayor al monto Total de la Factura', 'error');
            //    $('#AplicarReverse_btn').removeAttr('disabled');
            //    return;
            //}
            abp.ui.setBusy();
            $.ajax({
                type: "POST",
                url: applyReverse,
                data: { invoiceId: invoiceId, amount: amount, reason: reason, type: type, clientId: clientId, montotax: montotax, totalNota: totalNota },
                success: function (data) {
                    if (data == "-1") {
                        writeError('msgNotaErrorReverse', 'Debe ingresar un monto mayor a 0.', 'error');
                    }
                    else {
                        if (data == "-2") {
                            writeError('msgNotaErrorReverse', 'Existen notas pendientes por firma digital', 'error');
                        }
                        else {
                            $('#modalReverse').modal('hide');
                            $("#anyListEntity").html(data);

                        }
                    }
                    $('#AplicarReverse_btn').removeAttr('disabled');
                    abp.ui.clearBusy();
                },
                error: function () {
                    writeError('msgNotaErrorReverse', 'Error al aplicar nota.', 'error');
                    $('#AplicarReverse_btn').removeAttr('disabled');
                    abp.ui.clearBusy();
                }
            });
        }

        this.deleteNote = function (invoiceId, noteId) {
            clearErrors();
            var clientId = $('#clientIdHidden').val();
            abp.ui.setBusy();
            $.ajax({
                type: "POST",
                url: deleteNote,
                data: { invoiceId: invoiceId, noteId: noteId },
                success: function (data) {
                    if (data === "-1") {
                        writeError('msgNotaError', 'Error al borrar la nota.', 'error');
                    }
                    else if (data === "-2") {
                        writeError('msgNotaError', 'Error al recargar lista de notas.', 'error');
                    }
                    else {
                        clearErrors();
                        $('.modal-body p.bodyNotas').html(data);
                        updateRequestsListLocal(clientId, invoiceId);
                        writeError('msgNotaError', 'Nota borrada exitosamente.', 'success');
                    }
                    abp.ui.clearBusy();
                },
                error: function () {
                    writeError('msgNotaError', 'Error al borrar la nota.', 'error');
                    abp.ui.clearBusy();
                }
            });
        }

        this.showNotes = function (invoiceId, Balance, Rate, ClientId, Coin) {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: showNotesUrl,
                data: { invoiceId: invoiceId },
                success: function (data) {
                    $('.modal-body p.bodyNotas').html(data);
                    $('#txtCoin').val(Coin);
                    $('#invoiceIdHidden').val(invoiceId);
                    $('#clientIdHidden').val(ClientId);
                    $('#invoiceBalanceHidden').val(Balance);
                    $('#invoiceRateHidden').val(parseFloat(Rate.replace(/[^0-9\.]/g, '')));
                    $('#ValueMonto_text').val("");
                    $('#MontoTax_text').val("");
                    $('#TotalNota_text').val("");
                    $('#modalNotes').modal('show');
                    abp.ui.clearBusy();
                },
                error: function (err) {
                    alert("Error");
                    writeError('IndexAlerts', 'Error al obtener las facturas.', err);
                    abp.ui.clearBusy();
                }
            });
            return false;
        }

        this.showReverse = function (invoiceId, Balance, Rate, ClientId, subTotal, impuestoTotal, total, coin) {// no se usa
            clearErrors();
            abp.ui.setBusy();
            $('#invoiceIdHiddenReverse').val(invoiceId);
            $('#ReverseCointext').val(coin);
            $('#clientIdHidden').val(ClientId);
            $('#invoiceBalanceHiddenReverse').val(Balance);
            $('#invoiceRateHiddenReverse').val(parseFloat(Rate.replace(/[^0-9\.]/g, '')));
            $('#ReverseValueMonto_text').val(toCurrency(subTotal));
            $('#ReverseMontoTax_text').val(toCurrency(impuestoTotal));
            $('#ReverseTotalNota_text').val(toCurrency(total));
            $('#modalReverse').modal('show');
            abp.ui.clearBusy();
        }

        this.showPaymentMethods = function (invoiceId, total, ClientId, saldoFavor) {
            
            clearErrors();
            $('#selectInvoiceId').val(invoiceId);
            $('#selectTotalInvoice').val(parseFloat(total).toFixed(2));
            $('#clientIdHidden').val(ClientId);
           
            
            
            var amount = " Monto a pagar ";
            resultFormatNumber("" + total).then((val) => { $('#amountInvoiceSelect').html(amount + val) });
            //$('#amountInvoiceSelect').html(amount);
            $('#alertPaymentInvoice').html('');
            /*debugger*/;
            resultFormatNumber("" + total).then((val) => { $('#cashText').val(val) });
            //$('#cashText').val(total);
            $('#creditCardText').val("");
            $('#depositText').val("");
            $('#checkText').val("");
            resultFormatNumber("" + saldoFavor).then((val) => { $('#positiveBalanceLabel').html(val) });
            //$('#positiveBalanceLabel').html(saldoFavor);
            $('#positiveBalanceHidden').val(saldoFavor);
            $('#PaymentMethods').modal('show');
            if (saldoFavor == 0 || saldoFavor > total)
                $('#positiveBalance').hide();
            else
                $('#positiveBalance').show();

            return false;
        }

        this.selectPaymentMethods = function (value) {
            $('#selectPaymentMethod').val(value);
        }


        this.cancelConfirmation_modal = function (invoiceId) {
            $('#AnularConfirmation_modal').modal('show');
            $('#ItemToDelete_hidden').val(invoiceId);
            return false;
        }

        function updateRequestsListLocal(clientId, invoiceId) {
            //abp.ui.setBusy();
            clearErrors();
            $.ajax({
                url: showInvoiceList,
                type: "POST",
                cache: false,
                data: { clientId: clientId, invoiceId: invoiceId },
                success: function (data) {
                    $("#anyListEntity").html(data);
                    //abp.ui.clearBusy();
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al buscar la Solicitud', 'error'); }
            });
            return false;
        }

        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }

        function getpositiveBalance(clientId, typecoin) {
            if (clientId != '') {
                abp.ui.setBusy();
                $.ajax({
                    url: positiveBalanceUrl,
                    async: false,
                    data: { clientId: clientId, typeCoin: typecoin },
                    success: function (data) {
                        if (data.success == true) {
                            $('#positiveBalanceHidden').val(data.result.code);
                            resultFormatNumber("" + data.result.code).then((val) => { $('#positiveBalanceLabel').html(val) });

                        }
                        abp.ui.clearBusy();
                    },
                    error: function (err) {
                        writeError('IndexAlerts', 'Error al buscar saldo a favor.', err);
                        abp.ui.clearBusy();
                    }
                });
            }
           
        } 

        this.saveClientData = function (clientId, name, code, creditDay, typeIdentification, identification, identificacionExtranjero , phoneNumber, mobilNumber, email) {
            $('#ClientName_text').val(name);
            $('#ClientId_hidden').val(clientId);
            $('#DayCredit').val(creditDay);

          
            $('#IdentificationTypes_DD').val(typeIdentification);

            var ident = typeIdentification == 'NoAsiganda' ? identificacionExtranjero : identification;
            $('#text_identification').val(ident);

            $('#text_ClientPhoneNumber').val(phoneNumber);
            $('#text_ClientMobilNumber').val(mobilNumber);
            $('#text_ClientEmail').val(email);
          
            if (!isEmpty(typecoin)) {

                var saldoFavor = 0;
                if (!isNaN($('#CoinTypes_DD').val())) {
                    var typecoin = $('#CoinTypes_DD').val();
                    abp.ui.setBusy();
                    $.ajax({
                        url: positiveBalanceUrl,
                        data: { clientId: clientId, typeCoin: typecoin },
                        success: function (data) {
                            if (data.success == true) {
                                $('#positiveBalanceHidden').val(data.result.code);
                                resultFormatNumber("" + data.result.code).then((val) => { $('#positiveBalanceLabel').html(val) });
                                //$('#positiveBalanceLabel').html(data.result.code);

                            }
                            abp.ui.clearBusy();
                        },
                        error: function (err) {
                            writeError('IndexAlerts', 'Error al buscar saldo a favor.', err);
                            abp.ui.clearBusy();
                        }
                    });
                }
            }

            return false;
        }

        this.saveRegisterData = function (id, name, code) {
            $('#RegisterId_hidden').val(id);
            $('#RegisterName_text').val(name);
            $('#RegisterCode_hidden').val(code);
            return false;
        }

        //this.resendInvoice = function (invoiceId, numero, clientEmail) {
        //    var enviar = confirm('Se enviara la factura Nro.: ' + numero + ' al correo ' + clientEmail);
        //    if (enviar == true) {
        //        clearErrors();
        //        abp.ui.setBusy();
        //        $.ajax({
        //            url: resendInvoice,
        //            data: { id: invoiceId },
        //            success: function (data) {
        //                if (data.success == true) {
        //                    writeError('IndexAlerts', 'Factura enviada con éxito.', 'success');
        //                }
        //                abp.ui.clearBusy();
        //            },
        //            error: function (err) {
        //                console.log(err);
        //                writeError('IndexAlerts', 'Error al reenviar la factura.', err);
        //                abp.ui.clearBusy();
        //            }
        //        });
        //        return false;
        //    }
        //}

        this.addClient = function (btn) {
            clearErrors();
            $.ajax({
                url: AddClientNew,
                //    data: { id: invoiceId },
                success: function (data) {
                    $("#ContentModalClient").html(data);
                    $("#addNewClient").modal("show");
                    $('#NameClient').val("");
                    $('#clientLastName').val("");
                    $('#IdIdentificacion').val("");
                    $('#EmailClient').val("");
                    $('#typeidentificacion_DD').val("");
                    $('#IdExtranjero').val("");
                    $('#labelLastName').attr('class', 'control-label');
                    abp.ui.clearBusy();
                 },
                error: function (err) {
                    console.log(err);
                    writeError('IndexAlerts', 'Error cragra fomulario cliente', err);
                    abp.ui.clearBusy();
                }
             });

           
            return false;
        }

        $('#IdentificationTypeClient').change(function () {
            if ($('#IdentificationTypeClient').val() != "1") {
                $('#LastNameLabel').attr('class', 'control-label');
            } else {
                $('#LastNameLabel').removeAttr('class');
            }
        });

       

        function isValidEmailAddress(emailAddress) {
            var pattern = new RegExp(/^([\w-\.]+@([\w-]+\.)+[\w-]{2,4}$)/i);
            return pattern.test(emailAddress);
        }

        $("#btnOkAddNewService").click(function (e) {
            clearErrors();
            abp.ui.setBusy();

            var validateIsSuccesful = true;
            $("#serviceNew :input[type=text]").each(function () {
                var inputObject = $(this);
                if (!inputObject.is(":hidden")) {
                    var input = inputObject.val();
                    var regexValue = $(this).attr("data-regular-expression");
                    if (regexValue != null) {
                        var regex = new RegExp($(this).attr("data-regular-expression"));
                        if (!regex.test(input)) {
                            writeError("alertPaymentInvoice", "El " + $(this).attr("data-name") + " tiene un formato incorrecto, por favor verifique.", "error");
                            abp.ui.clearBusy();
                            validateIsSuccesful = false;
                            return false;
                        }
                    }
                }
            });

            if (!validateIsSuccesful) {
                validateIsSuccesful = true;
                return false;
            }

            if ($('#NameServiceNew').val() == "") {
                writeError('msgNotaError', 'Debe ingresar un nombre de servicio', 'error');
                abp.ui.clearBusy();
                return false;
            }

            if ($('#CantService').val() == "") {
                writeError('msgNotaError', 'Debe ingresar una cantidad', 'error');
                abp.ui.clearBusy();
                return false;
            }
            else {
                if ($('#CantService').val() == 0) {
                    writeError('msgNotaError', 'La cantidad debe ser mayor a 0', 'error');
                    abp.ui.clearBusy();
                    return false;
                }
            }
            if ($('#PriceService').val() == "") {
                writeError('msgNotaError', 'Debe ingresar un precio', 'error');
                abp.ui.clearBusy();
                return false;
            }
            else {
                if ($('#PriceService').val() == 0) {
                    writeError('msgNotaError', 'El precio debe ser mayor a 0', 'error');
                    abp.ui.clearBusy();
                    return false;
                }
            }

            $.ajax({
                url: createServiceNew,
                type: "POST",
                cache: false,
                data: {
                    nameService: $('#NameServiceNew').val(), CantService: $('#CantService').val(), priceService: parseFloat($('#PriceService').val().replace(/[^0-9\.]/g, '')),
                    taxService: $('#TaxService').val(), unitMeasurementsService: $('#UnitMeasurementsService').val()
                },
                success: function (data) {
                    if (data.success == true) {
                        var taxGrid = "0.00";
                        if (parseFloat(data.result.impuesto.replace(/[^0-9\.]/g, '')) != 0)
                            var taxGrid = toCurrency(parseFloat(data.result.impuesto.replace(/[^0-9\.]/g, '')));

                        if ($("#GridId").val() != 0 && typeof $("#GridId").val() !== "undefined") {
                            var id = parseInt($("#GridId").val());
                            grid.updateRow(id, { "IdService": data.result.id, "ID": id, "Servicio": data.result.name, "Cantidad": data.result.cantidad, "Precio": toCurrency(parseFloat(data.result.precio.replace(/[^0-9\.]/g, ''))), "Descuento": data.result.descuento, "Impuesto": taxGrid, "Total": toCurrency(parseFloat(data.result.total.replace(/[^0-9\.]/g, ''))) });
                        } else {
                            grid.addRow({ "IdService": data.result.id, "ID": grid.count() + 1, "Servicio": data.result.name, "Cantidad": data.result.cantidad, "Precio": toCurrency(parseFloat(data.result.precio.replace(/[^0-9\.]/g, ''))), "Descuento": data.result.descuento, "Impuesto": taxGrid, "Total": toCurrency(parseFloat(data.result.total.replace(/[^0-9\.]/g, ''))) });
                        }
                        clearNewServices();
                        abp.ui.clearBusy();
                        $('#serviceNew').hide();
                        getCalculateTotalInvoice();
                    }
                    if (data.success == false) {
                        writeError("msgErrorAnyModal", data.result.name, "error");
                        abp.ui.clearBusy();
                    }
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al crear el servicio!", "error");
                    abp.ui.clearBusy();
                }
            });
            return false;
        });

        function clearNewServices() {
            $('#NameServiceNew').val("");
            $('#CantService').val("");
            $('#PriceService').val("");
        }



        $("#btnCancelAddNewService ").click(function (e) {
            clearNewServices();
            $('#serviceNew').hide();
        });
        $("#btnActiveAddNewService ").click(function (e) {
            clearNewServices();
            $('#serviceNew').show();
        });

        //$('#ContadoChk').change(function () {
        //    var isChecked = $(this).is(':checked');
        //    if (isChecked) {
        //        $('#panelContado').fadeIn();
        //        $('#panelCredito').fadeOut();
        //    } else {
        //        $('#panelContado').fadeOut();
        //        $('#panelCredito').fadeOut();
        //    }
        //});

        $('#CreditoChk').change(function () {
            var isChecked = $(this).is(':checked');
            if (isChecked) {
                $('#panelContado').fadeOut();
                $('#panelCredito').fadeIn();
            } else {
                $('#panelContado').fadeOut();
                $('#panelCredito').fadeOut();
            }
        });

        $('#ConditionSaleTypes_DD').change(function () {
            checkConditionSaleType();
        });

        $("#CoinTypes_DD").change(function () {
            var clientId = $('#ClientId_hidden').val();
            var typecoin = $(this).val();
            getpositiveBalance(clientId, typecoin);
            if(clientId != "" && typecoin != "")
                if (parseFloat($('#positiveBalanceHidden').val()) == 0 || parseFloat($('#positiveBalanceHidden').val()) > parseFloat($("#selectTotalInvoice").val()))
                    $('#positiveBalance').hide();
                else
                    $('#positiveBalance').show();
        });

        function checkConditionSaleType() {
            var conditionSaleSelected = $('#ConditionSaleTypes_DD').val();
            switch (conditionSaleSelected) {
                case '0':
                    $('#panelContado').fadeIn();
                    $('#panelCredito').hide();
                    break;
                case '1':
                    $('#panelContado').hide();
                    $('#panelCredito').fadeIn();
                    break;
                default:
            }
        }
    });

    $(document).ready(function () {
        //$('#ConditionSaleTypes_DD').chosen({ max_selected_options: 1, no_results_text: "No se encuentra condición de venta", allow_single_deselect: true, width: "120px" });
        $('#CoinTypes_DD').chosen({ max_selected_options: 1, no_results_text: "No se encuentrala moneda", allow_single_deselect: true, width: "200px" });

       

        //$('#toggle-two').bootstrapToggle({
        //    on: 'Factura',
        //    off: 'Tiquete'
        //});

        $('#toggle-two').change(function () {

            if ($(this).prop('checked')) {
                $("#hd_TypeDocument").val("Invoice");
              
                //$("#ClientName_text").attr('disabled', 'disabled');
                //$("#IdentificationTypes_DD").attr('disabled', 'disabled');
                //$("#text_identification").attr('disabled', 'disabled');
                //$("#text_ClientPhoneNumber").attr('disabled', 'disabled');
                //$("#text_ClientMobilNumber").attr('disabled', 'disabled');
                //$("#text_ClientEmail").attr('disabled', 'disabled');
                $("#Check_Impimir").prop('checked', true);
                $('#Check_Impimir').bootstrapToggle('toggle');
            }

            else {
               
                $("#hd_TypeDocument").val("Ticket");
                //$("#ClientName_text").removeAttr('disabled');
                //$("#IdentificationTypes_DD").removeAttr('disabled');
                //$("#text_identification").removeAttr('disabled');
                //$("#text_ClientPhoneNumber").removeAttr('disabled');
                //$("#text_ClientMobilNumber").removeAttr('disabled');
                //$("#text_ClientEmail").removeAttr('disabled');
                $("#Check_Impimir").prop('checked', false);
                $('#Check_Impimir').bootstrapToggle('toggle');
           
            }
            //location.reload();
            $('#ClientId_hidden').val('');
            $("#ClientName_text").val('');
            $("#IdentificationTypes_DD").val('');
            $("#text_identification").val('');
            $("#text_ClientPhoneNumber").val('');
            $("#text_ClientMobilNumber").val('');
            $("#text_ClientEmail").val('');
        })

        $('#Check_contingencia').change(function () {

            if ($(this).prop('checked')) {
              
                $("#panelContigencia").removeClass("hidden");               
            }
            else {
                $("#panelContigencia").addClass("hidden");                

            }
            //location.reload();

        })

        $("#Branch_DD").on("change", function () {

            var id = $(this).attr('data-cascade-combo');

            var url = $(this).attr('data-cascade-combo-source');

            var paramName = $(this).attr('data-cascade-combo-param-name');

            var data = {};
            data[paramName] = id;

            $.ajax({
                url: url,
                data: {
                    id: $(this).val()
                },
                success: function (data) {
                    $(id).html('');
                    $(id).append('<option value="">Todas</option>');
                    $.each(data.result, function (index, type) {
                        var content = '<option value="' + type.value + '">' + type.text + '</option>';
                        $(id).append(content);
                    });
                }
            });
        });
        
    });

})();
