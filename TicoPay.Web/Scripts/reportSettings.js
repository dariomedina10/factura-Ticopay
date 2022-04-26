(function () {
    $(function () {
        var editUrl = "/ReportSettings/Edit";
        var detailUrl = "/ReportSettings/Details";

        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }

        function clearErrors() {
            $("#msgErrorAnyModal").html(""); $("#IndexAlerts").html("");
        }

        function getRequest(url) {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: url,
                context: document.body,
                success: function (data) {
                    $(".modal-body p.body").html(data);
                    $("#reportSettingsForm").modal("show");
                    abp.ui.clearBusy();
                }, error: function (err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos!", "error");
                    abp.ui.clearBusy();
                }
            });
        }

        function isEmpty(str) {
            return (!str || 0 === str.length);
        }

        this.editFunc = function (btn) {
            clearErrors();
            if (!isEmpty(editUrl)) {
                var id = btn.attr("data-id");
                var url = editUrl + "/" + id;
                getRequest(url);
            }
            return false;
        }

        this.detailFunc = function (btn) {
            clearErrors();
            if (!isEmpty(detailUrl)) {
                var id = btn.attr("data-id");
                var url = detailUrl + "/" + id;
                getRequest(url);
            }
            return false;
        }

        $(".closeModal").click(function (e) {
            $("#reportSettingsForm").modal("hide");
        });

        $("#reportSettingsForm").submit(function (event) {
            event.preventDefault();
            clearErrors();

            var formData = new FormData($("#form").get(0));
            var image = $('input[name="WatermarkFile"]').prop('files')[0];
            formData.append("WatermarkFile", image);

            abp.ui.setBusy();
            $("#form").find("button[type='submit']").prop('disabled', true);
            $.ajax({
                url: $("#form").attr("action"),
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (data) {
                    $("#anyModalBody").html(data);
                    $("#form").find("button[type='submit']").removeProp('disabled')
                    abp.ui.clearBusy();
                },
                error: function (error, status) {
                    writeError("msgErrorAnyModal", "Error procesando datos. ¡Por favor revise los datos nuevamente!", "error");
                    $(".modal-body p.body").html(error.responseText);
                    $("#form").find("button[type='submit']").removeProp('disabled')
                    abp.ui.clearBusy();
                }
            });
            return false;
        });

        $("a.btnCloseFormSimple").click(function (e) {
            $("#reportSettingsForm").modal("hide");
            return false;
        });
    });
})();