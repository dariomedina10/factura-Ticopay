(function () {
    $(function () {

        var createUrl = "/Users/Create";
        var editUrl = "/Users/Edit";
        var searchUrl = "/Users/AjaxPage";
        var changeStatus = "/Users/ChangeStatus";
        var resendConfirmation = "/Users/ResendConfirmation";
        var drawersUrl = "/Users/UserDrawers";

        var okFlag = 1;
        var noneFlag = 0;
        var errorFlag = -1;

      

        function getRequest(url) {
            //clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: url,
                context: document.body,
                success: function (data) {
                    $(".modal-body p.body").html(data);
                    //$(this).addClass("done");
                    $("#anyModalForm").modal("show");
                    abp.ui.clearBusy();
                }, error: function (err) {
                    //writeError("msgErrorAnyModal", "¡Error al consultar los datos el Grupo!", "error");
                    abp.ui.clearBusy();
                }
            });
        }

        

        $("#anyModalForm").submit(function (event) {
            //clearErrors();
            abp.ui.setBusy();
            var action = $("#form").attr("action");
            $.post(action, $("#form").serialize(), function (data, status) {
                $("#anyModalBody").html(data);
                abp.ui.clearBusy();
                refreshList();//refresh list
            }).fail(function (error, status) {
                //writeError("msgErrorAnyModal", "Error procesando datos. ¡Por favor revise los datos nuevamente!", "error");
                $(".modal-body p.body").html(error.responseText);
                abp.ui.clearBusy();
            });
            return false;
        });

        $("#btnCreateForm").click(function (e) {
            clearErrors();
            getRequest(createUrl);
            return false;
        });

        $("#btnModalOkChangeConfirmation").click(function (e) {
            $("#ChangeConfirmation_modal").modal("hide");
            abp.ui.setBusy();
            var id = $("#ItemToChange_hidden").val();
            $.ajax({
                url: changeStatus + "/",
                type: "POST",
                cache: false,
                data: { id: id },
                success: function (data) {
                    if (data.result === okFlag) {
                        writeError("IndexAlerts", "El usuario cambio de estado correctamente", "success");
                        refreshList();
                        abp.ui.clearBusy();
                    } else if (data.result < noneFlag) {
                        writeError("IndexAlerts", "!Error al cambiarle el estado al Usuario!", "error");
                        abp.ui.clearBusy();
                    }
                }, error: function (xhr, ajaxOptions, thrownError) {
                    writeError("IndexAlerts", "!Error al cambiarle el estado al Usuario!", "error");
                    abp.ui.clearBusy();
                }
            });
            return false;
        });

        this.editFunc = function (btn) {
            clearErrors();
            var id = btn.attr("data-idEntity");
            var url = editUrl + "/" + id;
            getRequest(url);
            return false;
        }

        this.drawersFunc = function (btn) {
            clearErrors();
            var id = btn.attr("data-idEntity");
            var url = drawersUrl + "/" + id;
            getRequest(url);
            return false;
        }


        this.changeFunc = function (btn) {
            clearErrors();
            $("#ChangeConfirmation_modal").modal("show");
            $("#ItemToChange_hidden").val(btn.attr("data-idEntity"));
            return false;
        }

        this.resendConfirmation = function (btn) {
            clearErrors();
            abp.ui.setBusy();
            var id = btn.attr("data-idEntity");
            $.ajax({
                url: resendConfirmation + "/",
                type: "POST",
                cache: false,
                data: { id: id },
                success: function (data) {
                    if (data.result === okFlag) {
                        writeError("IndexAlerts", "Se ha enviado el enlace de confirmación", "success");
                        refreshList();
                        abp.ui.clearBusy();
                    } else if (data.result < noneFlag) {
                        writeError("IndexAlerts", "!Error al enviar enlace de confirmacion!", "error");
                        abp.ui.clearBusy();
                    }
                }, error: function (xhr, ajaxOptions, thrownError) {
                    writeError("IndexAlerts", "!Error al enviar enlace de confirmacion!", "error");
                    abp.ui.clearBusy();
                }
            });
            return false;
        }

        function refreshList() {
            //abp.ui.setBusy();
            $.ajax({
                url: searchUrl,
                type: "GET",
                cache: false,
                data: { query: $("#SrchQuery_hidden").val(), page: $("#SrchPage_hidden").val() },
                success: function (data) {
                    $("#anyListEntity").html(data);
                    //abp.ui.clearBusy();
                }, error: function (xhr, ajaxOptions, thrownError) {
                    writeError("IndexAlerts", "¡Error al consultar los Usuarios!", "error");
                    // abp.ui.clearBusy();
                }
            });
            return false;
        };

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


     
    });
})();