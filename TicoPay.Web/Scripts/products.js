﻿(function () {
    $(function () {

        var createUrl = "/Products/Create";
        var editUrl = "/Products/Edit";
        var detailUrl = "/Products/Details";
        var deleteUrl = "/Products/Delete";
        var searchUrl = "/Products/AjaxPage";

        var okFlag = 1;
        var noneFlag = 0;
        var errorFlag = -1;

        //function commaSeparateNumber(val) {
        //    while (/(\d+)(\d{3})/.test(val.toString())) {
        //        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
        //    }
        //    return val;
        //}

        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }
        function clearErrors() { $("#msgErrorAnyModal").html(""); $("#IndexAlerts").html(""); }


        function getRequest(url) {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: url,
                context: document.body,
                success: function (data) {
                    $(".modal-body p.body").html(data);
                    //$(this).addClass("done");
                    $("#anyModalForm").modal("show");
                    //$('#retailpriceText').val(commaSeparateNumber($('#retailpriceText').val()));
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos del Producto!", "error");
                    abp.ui.clearBusy();
                }
            });
        }

        function isEmpty(str) { return (!str || 0 === str.length); }
        $(".closeModal").click(function (e) { $("#anyModalForm").modal("hide"); });


        //--- Butons Events Functions


        this.editFunc = function (btn) {
            clearErrors();
            if (!isEmpty(editUrl)) {
                var id = btn.attr("data-idEntity");
                var url = editUrl + "/" + id;
                getRequest(url);
            }
            return false;
        }

        this.detailFunc = function (btn) {
            clearErrors();
            if (!isEmpty(detailUrl)) {
                var id = btn.attr("data-idEntity");
                var url = detailUrl + "/" + id;
                getRequest(url);
                $('.loading').hide();
            }
            return false;
        }

        this.deleteFunc = function (btn) {
            clearErrors();
            $("#DeleteConfirmation_modal").modal("show");
            $("#ItemToDelete_hidden").val(btn.attr("data-idEntity"));
            return false;
        }

        //Get and refresh list of entities from server
        function refreshList() {
            //abp.ui.setBusy();
            $.ajax({
                url: searchUrl,
                type: "GET",
                cache: false,
                data: { name: $("#SrchNameFilter_hidden").val(), brand: $("#SrchBrandFilter_hidden").val(), page: $("#SrchPage_hidden").val(), tax: $("SrchTaxFilter_hidden").val() },
                success: function (data) {
                    $("#anyListEntity").html(data);
                    //abp.ui.clearBusy();
                }, error: function (xhr, ajaxOptions, thrownError) {
                    writeError("IndexAlerts", "¡Error al consultar los Productos!", "error");
                    // abp.ui.clearBusy();
                }
            });
            return false;
        };

        //send to server and render views, when submit information from edit, create, details
        $("#anyModalForm").submit(function (event) {
            clearErrors();
            abp.ui.setBusy();
            var action = $("#form").attr("action");
            $.post(action, $("#form").serialize(), function (data, status) {
                $("#anyModalBody").html(data);
                abp.ui.clearBusy();
                refreshList();//refresh list
            }).fail(function (error, status) {
                writeError("msgErrorAnyModal", "Error procesando datos. ¡Por favor revise los datos nuevamente!", "error");
                $(".modal-body p.body").html(error.responseText);
                abp.ui.clearBusy();
            });
            return false;
        });

        //send to server and render views, when submit information from search
        $("#searchForm").submit(function (event) {
            console.log('Submit');
            clearErrors();
            abp.ui.setBusy();
            var action = $("#searchForm").attr("action");
            $.post(action, $("#searchForm").serialize(), function (data, status) {
                $("#anyListEntity").html(data);
                abp.ui.clearBusy();
            }).fail(function (error, status) {
                writeError("IndexAlerts", "Error procesando datos. ¡Por favor revise los datos nuevamente!", "error");
                abp.ui.clearBusy();
            });
            return false;
        });

        //Close
        $("a.btnCloseFormSimple").click(function (e) {
            $("#anyModalForm").modal("hide");
            return false;
        });

        //Cellar Create
        $("a.btnCreateForm").click(function (e) {
            clearErrors();
            if (!isEmpty(createUrl)) {
                getRequest(createUrl);
            }
            return false;
        });

        //Cellar Edit
        /*$("#btnEditForm").click(function () {
            editFunc($(this));
        });*/

        //Cellar Details
        //$("#btnDetailForm").click(function () {
        //    detailFunc($(this));

        //});

        ////Delete functions
        //$("#btnDeleteForm").click(function (e) {
        //    deleteFunc($(this));

        //});

        $("#btnModalOkDeleteConfirmation").click(function (e) {
            $("#DeleteConfirmation_modal").modal("hide");
            abp.ui.setBusy();
            if (!isEmpty(deleteUrl)) {
                var id = $("#ItemToDelete_hidden").val();
                $.ajax({
                    url: deleteUrl + "/",
                    type: "DELETE",
                    cache: false,
                    data: { id: id },
                    success: function (data) {
                        debugger;
                        if (data.result === okFlag) {
                            writeError("IndexAlerts", "Producto eliminado correctamente", "success");
                            //$("#tableRowId" + id).remove();
                            $("#SrchPage_hidden").val(1);
                            refreshList();
                            abp.ui.clearBusy();
                        } else if (data.result < noneFlag) {
                            writeError("IndexAlerts", "No se puede Eliminar el Producto. Este producto esta asociado a una factura.", "error");
                            abp.ui.clearBusy();
                        }
                    }, error: function (xhr, ajaxOptions, thrownError) {
                        writeError("IndexAlerts", "¡Error al eliminar el Producto!", "error");
                        abp.ui.clearBusy();
                    }
                });
            } return false;
        });
        $("#ReturnUrlHash").val(location.hash);
        $("#LoginForm input:first-child").focus();


        document.getElementById("Taxes_DD").onload = function () { myFunction() };

        function myFunction() {
            //document.getElementById("demo").innerHTML = "Iframe is loaded.";
            console.log('consultando');
        }
    });
})();